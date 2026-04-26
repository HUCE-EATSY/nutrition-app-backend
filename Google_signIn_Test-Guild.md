# 📱 Hướng Dẫn Test Tính Năng Google Sign In - Backend

## 🎯 Mục Đích
Hướng dẫn chi tiết, từng bước test tính năng **Đăng Nhập Google** cho backend trước khi deploy lên production.

---

## ✅ Chuẩn Bị Trước Test

### 1. Kiểm Tra Config Google
Mở file `appsettings.Development.json` và kiểm tra đã có Google Client IDs chưa:

```json
"Google": {
  "WebClientId": "1051589648478-2l332kcmrsaaka105lmvdccs2lt2vabk.apps.googleusercontent.com",
  "IosClientId": "1051589648478-jjkkh5et3jlqr5jrm8ejek4h1rjompu5.apps.googleusercontent.com",
  "AndroidClientId": "1051589648478-30226etqd9as8jeca6fr2p5fccdsgk8e.apps.googleusercontent.com"
}
```

✅ Nếu có → OK, tiếp tục
❌ Nếu không → Thêm Google Client IDs

### 2. Kiểm Tra JWT Config
Kiểm tra file `appsettings.Development.json` đã có JWT config:

```json
"Jwt": {
  "Key": "5d9bedbc0d606960aeca90738f98248e9a655d8b5cb57f942e9bedcc368d68d5",
  "Issuer": "WaoHealthApp",
  "Audience": "WaoHealthAppClients"
}
```

✅ Nếu có → OK
❌ Nếu không → Thêm JWT config

### 3. Kiểm Tra Database
```bash
# Khởi động MySQL
# Kiểm tra database tồn tại
mysql -u root -p
```

Chạy SQL để kiểm tra bảng:
```sql
USE wao_health_app;
SHOW TABLES;
-- Kết quả mong đợi:
-- Users
-- UserAuthProviders
-- UserGoals
-- UserProfiles
```

---

## 🚀 Bước 1: Khởi Động Backend

### Mở Terminal/CMD
```bash
cd E:\JetBrains\C#_Folder\nutrition-app-backend
```

### Chạy Backend
```bash
dotnet run
```

### Kết Quả Mong Đợi
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started.
```

✅ Backend đã khởi động thành công trên `http://localhost:5000`

---

## 🔐 Bước 2: Lấy Google ID Token

### Cách 1: Dùng Google OAuth Playground (Nhanh nhất) ⭐

1. Mở trình duyệt
2. Truy cập: https://developers.google.com/oauthplayground
3. Làm theo các bước:
    - Click nút **Authorize** (bên trái)
    - Chọn **Google OAuth 2.0 API v2**
    - Chọn scope: `https://www.googleapis.com/auth/userinfo.profile`
    - Click **Authorize APIs**
    - Đăng nhập bằng tài khoản Google của bạn
    - Chấp nhận quyền
4. Copy **ID Token** từ phần **Response**

### Cách 2: Dùng Postman
1. Mở Postman
2. Tạo request POST đến: `https://oauth2.googleapis.com/token`
3. Thêm body (form-data):
    - `grant_type`: authorization_code
    - `client_id`: Web Client ID
    - `client_secret`: Secret từ Google Console
    - `code`: Authorization code

---

## 📤 Bước 3: Test API Bằng Postman / REST Client

### Tạo Request POST

**URL:**
```
http://localhost:5000/api/auth/google
```

**Headers:**
```
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjEyMzQ1Njc4OTAiLCJ0eXAiOiJKV1QifQ..."
}
```

Thay `eyJhbGciOi...` bằng **Google ID Token** bạn vừa lấy được.

---

## ✨ Test Cases

### Test Case 1: Đăng Nhập Lần Đầu (New User)

**Input:**
```json
{
  "idToken": "YOUR_GOOGLE_ID_TOKEN"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Đăng nhập thành công",
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "email": "your-email@gmail.com",
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "isNewUser": true
  }
}
```

**Kiểm Tra Database:**
```sql
SELECT * FROM Users;
-- Kết quả: Có 1 user mới được tạo

SELECT * FROM UserAuthProviders;
-- Kết quả: Có 1 provider record với Provider = 'google'
```

✅ **Kết quả mong đợi:**
- `isNewUser` = `true`
- Database có user mới
- Access Token được sinh ra

---

### Test Case 2: Đăng Nhập Lần Thứ 2 (Existing User)

**Input (Sử dụng ID Token cũ):**
```json
{
  "idToken": "YOUR_GOOGLE_ID_TOKEN"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Đăng nhập thành công",
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "email": "your-email@gmail.com",
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "isNewUser": false
  }
}
```

✅ **Kết quả mong đợi:**
- `isNewUser` = `false`
- `userId` giống lần trước
- Không tạo user mới
- Access Token mới được sinh ra

**Kiểm Tra Database:**
```sql
SELECT COUNT(*) FROM Users;
-- Kết quả: Vẫn là 1 user (không tạo mới)
```

---

### Test Case 3: Token Không Hợp Lệ (Invalid Token)

**Input:**
```json
{
  "idToken": "INVALID_TOKEN_XYZ"
}
```

**Response (401 Unauthorized):**
```json
{
  "success": false,
  "message": "Token Google không hợp lệ hoặc đã hết hạn.",
  "errorCode": "401"
}
```

✅ **Kết quả mong đợi:**
- Status Code: **401**
- Error message rõ ràng
- Không tạo user mới

---

### Test Case 4: Token Hết Hạn (Expired Token)

**Input:**
```json
{
  "idToken": "EXPIRED_TOKEN_FROM_1_HOUR_AGO"
}
```

**Response (401 Unauthorized):**
```json
{
  "success": false,
  "message": "Token Google không hợp lệ hoặc đã hết hạn.",
  "errorCode": "401"
}
```

✅ **Kết quả mong đợi:**
- Status Code: **401**
- Báo lỗi token hết hạn

---

## 🔍 Bước 4: Kiểm Tra Database Chi Tiết

### Mở MySQL Workbench hoặc Terminal

```sql
-- Kết nối database
mysql -u root -p
USE wao_health_app;

-- 1. Xem tất cả users
SELECT * FROM Users;

-- 2. Xem auth providers
SELECT * FROM UserAuthProviders;

-- 3. Xem chi tiết đầy đủ
SELECT 
  u.Id as UserId,
  u.CreatedAt,
  uap.Email,
  uap.Provider,
  uap.ProviderUid,
  uap.VerifiedAt
FROM Users u
JOIN UserAuthProviders uap ON u.Id = uap.UserId
WHERE uap.Provider = 'google';

-- Kết quả mong đợi:
-- | UserId | CreatedAt | Email | Provider | ProviderUid | VerifiedAt |
-- | 550e8400-... | 2026-04-25... | user@gmail.com | google | 1234567890 | 2026-04-25... |
```

---

## 🔐 Bước 5: Kiểm Tra JWT Token

### Giải Mã JWT Token

1. Truy cập: https://jwt.io
2. Copy **Access Token** từ response vào phần **Encoded**
3. Kiểm tra **Decoded** payload

### JWT Token Mẫu
```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "sub": "550e8400-e29b-41d4-a716-446655440000",
    "email": "your-email@gmail.com",
    "jti": "12345678-1234-1234-1234-123456789012",
    "iat": 1703000000,
    "exp": 1703604000,
    "iss": "WaoHealthApp",
    "aud": "WaoHealthAppClients"
  }
}
```

### Ý Nghĩa Các Trường
| Trường | Ý Nghĩa | Mẫu |
|--------|---------|-----|
| `sub` | User ID | 550e8400-e29b-41d4-a716-446655440000 |
| `email` | Email từ Google | your-email@gmail.com |
| `jti` | JWT ID (duy nhất) | 12345678-1234-1234-1234-123456789012 |
| `iat` | Issued At (lúc tạo) | 1703000000 |
| `exp` | Expiration (hết hạn) | 1703604000 |
| `iss` | Issuer | WaoHealthApp |
| `aud` | Audience | WaoHealthAppClients |

✅ **Kiểm Tra:**
- `sub` phải giống `userId` trong response
- `email` phải giống email Google
- `iss` phải là "WaoHealthApp"
- `aud` phải là "WaoHealthAppClients"
- `exp` phải sau `iat` 7 ngày

---

## 🐛 Troubleshooting

### ❌ Lỗi: "Token Google không hợp lệ hoặc đã hết hạn"

**Nguyên nhân có thể:**
1. Token đã hết hạn (thường hết sau 1 giờ)
2. Client ID không khớp
3. Token bị sai

**Giải pháp:**
- Lấy token mới từ Google OAuth Playground
- Kiểm tra Google Client IDs trong `appsettings.Development.json`
- Xem log backend để xem lỗi chi tiết

---

### ❌ Lỗi: "JWT Key is missing"

**Nguyên nhân:**
- Chưa cấu hình JWT trong `appsettings.Development.json`

**Giải pháp:**
```json
{
  "Jwt": {
    "Key": "5d9bedbc0d606960aeca90738f98248e9a655d8b5cb57f942e9bedcc368d68d5",
    "Issuer": "WaoHealthApp",
    "Audience": "WaoHealthAppClients"
  }
}
```

---

### ❌ Lỗi: "Database Connection Error"

**Nguyên nhân:**
- MySQL không chạy
- Connection string sai

**Giải pháp:**
```bash
# Windows - Kiểm tra MySQL Service
services.msc
# Tìm MySQL80, nhấn Start

# Hoặc terminal
net start MySQL80

# Kiểm tra kết nối
mysql -u root -p
```

---

### ❌ Lỗi: "Cannot connect to http://localhost:5000"

**Nguyên nhân:**
- Backend chưa khởi động
- Port 5000 bị sử dụng

**Giải pháp:**
```bash
# Kiểm tra port 5000 có bị sử dụng
netstat -ano | findstr :5000

# Nếu có process khác dùng port 5000, tìm Process ID rồi kill
taskkill /PID <ProcessId> /F

# Khởi động lại backend
dotnet run
```

---

## 📋 Checklist Hoàn Thành

Đánh dấu các mục khi hoàn thành:

- [ ] ✅ Config Google Client IDs trong `appsettings.Development.json`
- [ ] ✅ Config JWT trong `appsettings.Development.json`
- [ ] ✅ MySQL đang chạy
- [ ] ✅ Database `wao_health_app` tồn tại
- [ ] ✅ Backend chạy trên `http://localhost:5000`
- [ ] ✅ Lấy được Google ID Token từ OAuth Playground
- [ ] ✅ Test Case 1: New User (isNewUser = true)
- [ ] ✅ Test Case 2: Existing User (isNewUser = false)
- [ ] ✅ Test Case 3: Invalid Token (401 error)
- [ ] ✅ Test Case 4: Expired Token (401 error)
- [ ] ✅ Kiểm tra Database có dữ liệu user mới
- [ ] ✅ Giải mã JWT Token trên jwt.io
- [ ] ✅ JWT Token có đúng claims (sub, email, iss, aud)
- [ ] ✅ Không có lỗi trong Backend Console

---

## ⚠️ Quan Trọng Trước Khi Push GitHub

### ❌ KHÔNG push những file này:
```
appsettings.Development.json  (chứa password & secrets)
```

### ✅ Thêm vào `.gitignore`:
```
appsettings.Development.json
appsettings.Production.json
*.user
bin/
obj/
.idea/
.vs/
```

### ✅ Cách push code an toàn:
1. Giữ `appsettings.Development.json` ở local (không push)
2. Push `appsettings.Development.example.json` (template cho team)
3. Hướng dẫn team:
    - Copy `appsettings.Development.example.json` → `appsettings.Development.json`
    - Thay đổi giá trị theo local environment của họ

---

## 📚 Tài Liệu Tham Khảo

- 📖 [Google OAuth 2.0 Documentation](https://developers.google.com/identity/protocols/oauth2)
- 📖 [Google Sign-In for Android](https://developers.google.com/identity/sign-in/android)
- 📖 [Google Sign-In for iOS](https://developers.google.com/identity/sign-in/ios)
- 🔐 [JWT.io - Decode & Verify Token](https://jwt.io)
- 🛠️ [Postman - API Testing Tool](https://www.postman.com)

---

## 💡 Tips & Tricks

### Tip 1: Lưu Token Để Dùng Lại
Token Google hết hạn sau 1 giờ. Hãy lưu token vừa lấy để test:

```bash
# Tạo file token.txt
echo "YOUR_TOKEN_HERE" > token.txt

# Copy token từ file
type token.txt
```

### Tip 2: Dùng Curl Để Test
```bash
curl -X POST http://localhost:5000/api/auth/google \
  -H "Content-Type: application/json" \
  -d "{\"idToken\":\"YOUR_TOKEN_HERE\"}"
```

### Tip 3: Xem Log Backend
Mở tab Console trong IDE khi backend chạy để xem:
- Request/Response
- Database queries
- Lỗi chi tiết

### Tip 4: Reset Database
Nếu muốn xóa hết dữ liệu test:
```bash
# Chạy migration lại từ đầu
dotnet ef database drop
dotnet ef database update
```

---

## 🎉 Kết Luận

Sau khi hoàn thành tất cả test cases và checklist, backend Google Sign In của bạn:
- ✅ Xác thực token từ Google chính xác
- ✅ Lưu user mới vào database
- ✅ Xác định user cũ
- ✅ Sinh JWT token hợp lệ
- ✅ Xử lý lỗi đúng cách
- ✅ Sẵn sàng kết nối với React Native app

**🚀 Sẵn sàng deploy!**
