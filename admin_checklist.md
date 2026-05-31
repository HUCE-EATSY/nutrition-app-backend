## Phase 5 – Admin: User & Subscription Management

### 1. Goal
Trang bị cho admin các công cụ vận hành tối thiểu cần thiết ngay khi app live — xử lý được các tình huống khẩn cấp như ban user vi phạm, tra cứu subscription khi user report lỗi thanh toán, và kiểm soát Power User slot.

---

### 2. Scope
Bao gồm: quản lý user cơ bản, tra cứu và can thiệp subscription thủ công, kiểm soát Power User slot.

Không bao gồm: analytics, user growth report, bulk action, export CSV — những thứ này hữu ích nhưng không blocking cho launch.

---

### 3. Backend Work

#### API Groups

**Group 1 — User Management**

Các tác vụ cần thiết: tìm kiếm user theo email/phone, xem profile + subscription hiện tại, đổi `status` (active/banned), đổi `role` (free/premium/power_user/admin).

Lưu ý quan trọng:
- Đổi `role` lên `premium` thủ công (ví dụ: tặng premium cho influencer) phải đồng thời tạo row trong `subscriptions` với `store_platform = NULL` để `[RequiresPremium]` middleware hoạt động đúng — không chỉ update `users.role` là đủ.
- Ban user (`status = 2`) không xóa data, không hủy subscription. Middleware auth check `status` trước khi issue token mới — user bị ban sẽ không login được nhưng data giữ nguyên.
- Không cho phép admin tự đổi `role` của chính mình để tránh leo quyền.

**Group 2 — Subscription Lookup & Manual Override**

Các tác vụ cần thiết: xem lịch sử subscription của một user, xem toàn bộ `subscription_events` (webhook log) theo subscription, gia hạn thủ công (extend `current_period_end`), hạ cấp thủ công về Free.

Lưu ý quan trọng:
- Gia hạn thủ công phải tạo row mới trong `subscription_events` với `event_type = 'manual_override'` và `raw_payload` ghi lại admin nào làm, lúc nào — đây là audit trail, không được chỉ update `current_period_end` mà không log.
- Hạ cấp thủ công: set `subscriptions.status = 3` (expired), đồng thời update `users.role = 1` (free). Phải làm trong một transaction.
- Không implement hoàn tiền ở đây — hoàn tiền phải qua App Store/Google Play, backend không có quyền trigger refund.

**Group 3 — Power User Slot Control**

Các tác vụ cần thiết: xem danh sách Power User hiện tại, xem số slot còn lại (max 100), thu hồi Power User của một user.

Lưu ý quan trọng:
- Slot count = `SELECT COUNT(*) FROM subscriptions WHERE plan_id = 3 AND status IN (0,1)` — không cần column riêng, tính động.
- Thu hồi Power User: set `subscriptions.status = 2` (cancelled), update `users.role = 1` (free). Tạo `subscription_events` với `event_type = 'manual_override'` như trên.
- Không cho phép assign Power User nếu slot đã đủ 100.

---

#### DB

Tables tham gia và vai trò:

| Table | Vai trò |
|---|---|
| `users` | Đọc/ghi `status` và `role` — nguồn chính cho user management |
| `user_auth_providers` | Tìm kiếm user theo email hoặc phone số |
| `user_profiles` | Đọc thông tin sinh trắc để hiển thị trong user detail |
| `subscriptions` | Tra cứu, gia hạn, hạ cấp, đếm Power User slot |
| `subscription_plans` | Lookup tên plan khi hiển thị |
| `subscription_events` | Append audit log mọi khi admin can thiệp thủ công |

Index đã có trong schema, không cần migration:

```sql
-- Đã có sẵn, kiểm tra lại trước deploy:
CREATE INDEX idx_auth_email      ON user_auth_providers(email);
CREATE INDEX idx_auth_user       ON user_auth_providers(user_id);
CREATE INDEX idx_sub_user_status ON subscriptions(user_id, status);
CREATE INDEX idx_sub_event       ON subscription_events(subscription_id, received_at);
```

---

### 4. Edge Cases

- Tìm user bằng email nhưng email đó thuộc provider OAuth (không có `hashed_password`) → vẫn tìm được vì `email` được denorm vào `user_auth_providers`.
- Admin gia hạn cho user đang có subscription `status = 3` (expired) → phải tạo subscription row mới thay vì update row cũ, vì row cũ là lịch sử.
- Assign Power User khi slot đã đủ 100 → `409`, trả về số slot hiện tại.
- Admin đổi `role` lên `premium` nhưng quên tạo `subscriptions` row → `[RequiresPremium]` sẽ từ chối dù `role = 2`. Cần wrap cả hai thao tác trong một transaction và enforce ở service layer.
- Ban user đang online (đang giữ valid JWT) → JWT vẫn còn hiệu lực đến khi hết hạn. Nếu cần ban tức thì phải có cơ chế revoke token (ngoài scope phase này — cần thêm `refresh_tokens` blacklist).
- Admin tự đổi role của chính mình → từ chối, trả `403`.
- Thu hồi Power User của user đang trong `trial_ends_at` còn hạn → vẫn thu hồi, không giữ lại trial. Đây là quyết định nghiệp vụ cần confirm với product trước khi implement.

---

### 5. Test Cases

**User Management**
- Tìm user bằng email tồn tại → trả đúng user kèm subscription hiện tại.
- Tìm user bằng email không tồn tại → `404`.
- Ban user → `users.status = 2`, user không login được bằng refresh token mới.
- Đổi role lên premium → `users.role = 2` và tồn tại row `subscriptions` active tương ứng, cả hai trong cùng transaction.
- Admin tự đổi role chính mình → `403`.
- Gọi bất kỳ endpoint `/admin/*` với `role != 9` → `403`.

**Subscription**
- Xem subscription history của user → trả đúng thứ tự `created_at` DESC.
- Gia hạn thủ công → `current_period_end` tăng đúng số ngày, có row mới trong `subscription_events` với `event_type = 'manual_override'`.
- Hạ cấp thủ công → `subscriptions.status = 3` và `users.role = 1` trong cùng một transaction, không có trạng thái trung gian.
- Xem webhook log → trả đúng danh sách `subscription_events` theo `subscription_id`.

**Power User**
- Xem danh sách Power User → đúng số lượng, đúng thông tin.
- Slot còn lại tính đúng = 100 - số subscription `plan_id=3` đang `status IN (0,1)`.
- Assign Power User khi còn slot → thành công.
- Assign Power User khi đủ 100 slot → `409`.
- Thu hồi Power User → `subscriptions.status = 2`, `users.role = 1`, có audit log trong `subscription_events`.

---

### 6. Done Criteria

- [ ] Admin tìm được user bằng email/phone và xem đầy đủ thông tin trong một màn hình.
- [ ] Ban/unban user hoạt động đúng, không mất data.
- [ ] Mọi can thiệp thủ công vào subscription đều có audit trail trong `subscription_events`.
- [ ] Gia hạn và hạ cấp subscription đều atomic — không có trạng thái `role` và `subscriptions.status` lệch nhau.
- [ ] Power User slot không vượt quá 100, được tính động không cần column riêng.
- [ ] Tất cả test cases trên pass.
- [ ] Team vận hành confirm xử lý được các tình huống khẩn cấp thường gặp mà không cần dev can thiệp trực tiếp vào DB.