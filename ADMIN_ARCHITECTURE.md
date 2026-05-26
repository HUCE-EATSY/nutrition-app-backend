# 🏛️ Admin Architecture - Cấu Trúc & Best Practices

## 📌 Tổng Quan

Hệ thống Admin được thiết kế theo **Composite Pattern + Facade** để quản lý nhiều services mà không bị phức tạp.

---

## 🗂️ Cấu Trúc Folder Admin

```
📁 Services/Admin/
├── 📁 Core/
│   ├── IAdminCompositeService.cs          ← Main coordinator interface
│   └── AdminCompositeService.cs           ← Tập hợp tất cả sub-services
│
├── 📁 UserManagement/
│   ├── IAdminUserService.cs               ← Interface: quản lý users
│   └── AdminUserService.cs                ← Implementation
│
├── 📁 FoodManagement/
│   ├── IAdminFoodService.cs               ← Interface: duyệt/xóa foods
│   └── AdminFoodService.cs                ← Implementation
│
├── 📁 AnalyticsService/
│   ├── IAdminAnalyticsService.cs          ← Interface: dashboard stats
│   └── AdminAnalyticsService.cs           ← Implementation
│
└── 📁 LogsManagement/
    ├── IAdminLogsService.cs               ← Interface: quản lý logs
    └── AdminLogsService.cs                ← Implementation
```

---

## 🎯 Nguyên Tắc Chia Services

### **1. Separation by Domain (Chia theo chức năng)**

| Service | Trách nhiệm |
|---------|------------|
| **UserManagement** | Xóa user, toggle status, reset password, danh sách users |
| **FoodManagement** | Duyệt foods, reject foods, xóa foods, tìm kiếm |
| **Analytics** | Dashboard stats, user analytics, food analytics |
| **LogsManagement** | Xem food logs, weight logs, xóa logs |

### **2. Composite Service (Tập hợp)**

- `IAdminCompositeService` là **coordinator** chính
- Chứa references đến tất cả sub-services: `Users`, `Foods`, `Analytics`, `Logs`
- Controller chỉ inject 1 service duy nhất: `IAdminCompositeService`
- Lợi ích: Thêm service mới chỉ cần sửa composite, controller không đổi

### **3. Single Responsibility (Mỗi service 1 trách nhiệm)**

```
UserManagement Service:
  ✓ GetAllUsers()
  ✓ GetUserDetail()
  ✓ DeleteUser()
  ✓ ToggleUserStatus()
  ✗ KHÔNG quản lý Foods
  ✗ KHÔNG tính Analytics

FoodManagement Service:
  ✓ GetPendingFoods()
  ✓ ApproveFood()
  ✓ RejectFood()
  ✗ KHÔNG quản lý Users
  ✗ KHÔNG tính Analytics
```

---

## 🔌 Cách Hoạt Động

### **Injection Flow**

```
1. Program.cs:
   ✓ Register UserManagement → IAdminUserService
   ✓ Register FoodManagement → IAdminFoodService
   ✓ Register Analytics → IAdminAnalyticsService
   ✓ Register Logs → IAdminLogsService
   ✓ Register Composite → IAdminCompositeService
     (Composite nhận tất cả 4 services trên)

2. Controller:
   ✓ Inject IAdminCompositeService
   ✓ Dùng: _admin.Users.GetAllUsers()
   ✓ Dùng: _admin.Foods.GetPendingFoods()
   ✓ Dùng: _admin.Analytics.GetDashboard()
   ✓ Dùng: _admin.Logs.GetFoodLogs()

3. Kết quả:
   ✓ Đơn giản, clean
   ✓ Dễ thêm service mới
   ✓ Không phức tạp
```

---

## 📐 Hình Ảnh Kiến Trúc

```
┌─────────────────────────────────┐
│   AdminController               │
│  (Inject: IAdminCompositeService)
└────────────────┬────────────────┘
                 │
                 ▼
    ┌────────────────────────────┐
    │ IAdminCompositeService     │
    │ Facade/Coordinator         │
    └──┬──────┬──────┬──────────┘
       │      │      │      │
    ┌──▼┐  ┌─▼─┐  ┌──▼┐  ┌─▼──┐
    │ U │  │ F │  │ A │  │ L  │
    │ S │  │ S │  │ S │  │ S  │
    │ M │  │ M │  │ S │  │ M  │
    └──┘  └───┘  └──┘  └────┘
    
    USM = UserManagement
    FSM = FoodManagement
    ASS = Analytics
    LSM = LogsManagement
```

---

## 🔒 Authorization

### **[RequireAdmin] Attribute**

```csharp
[ApiController]
[Route("api/admin")]
[RequireAdmin]  ← ✓ Check JWT token có role = Admin
public class AdminController : ControllerBase
{
    // Tất cả endpoints ở đây đều protected
}
```

**Cách hoạt động:**
1. Client gửi request kèm JWT token
2. `[RequireAdmin]` filter kiểm tra JWT có chứa `role: Admin`
3. Nếu không phải admin → HTTP 403 Forbidden
4. Nếu phải admin → cho qua

---

## 📊 Ví Dụ Quy Trình

### **Khi Admin Duyệt Một Món Ăn:**

```
1. Client gửi: POST /api/admin/foods/{id}/approve

2. Controller nhận:
   ✓ Check [RequireAdmin] → Verified
   ✓ Gọi: _admin.Foods.ApproveFoodAsync(id)

3. FoodManagement Service xử lý:
   ✓ Tìm food item bằng id
   ✓ Cập nhật status = 1 (Approved)
   ✓ Save vào DB
   ✓ Return: true/false

4. Controller response:
   ✓ Success → HTTP 200 OK
   ✓ Not found → HTTP 404
```

### **Khi Admin Xem Dashboard:**

```
1. Client gửi: GET /api/admin/dashboard

2. Controller nhận:
   ✓ Check [RequireAdmin] → Verified
   ✓ Gọi: _admin.Analytics.GetDashboardAsync()

3. Analytics Service tính toán:
   ✓ COUNT(Users)
   ✓ COUNT(Foods)
   ✓ COUNT(FoodLogs)
   ✓ COUNT(PendingFoods)
   ✓ Return: AdminDashboardDto

4. Controller response:
   ✓ HTTP 200 + Dashboard data
```

---

## 🧩 Mở Rộng (Adding New Features)

### **Nếu cần thêm "Manage Exercises" cho Admin:**

```
Bước 1: Tạo folder
   Services/Admin/ExerciseManagement/

Bước 2: Tạo Interface & Implementation
   IAdminExerciseService.cs
   AdminExerciseService.cs

Bước 3: Thêm vào Composite
   public IAdminExerciseService Exercises { get; }

Bước 4: Register trong Program.cs
   builder.Services.AddScoped<IAdminExerciseService, AdminExerciseService>();

Bước 5: Sử dụng trong Controller
   _admin.Exercises.GetAllExercises()
   _admin.Exercises.DeleteExercise()
   ...

✓ XONG! Không cần modify controller logic
✓ Hoàn toàn modular
```

---

## ✅ Best Practices Áp Dụng

### **1. DRY (Don't Repeat Yourself)**
- ✓ Mỗi service method chỉ handle 1 việc
- ✓ Không duplicate logic
- ✓ Reuse sub-services trong sub-services nếu cần

### **2. Separation of Concerns**
- ✓ Controller chỉ handle HTTP (request/response)
- ✓ Service handle business logic
- ✓ Repository/DbContext handle data access

### **3. Single Responsibility Principle (SRP)**
- ✓ `AdminUserService` → chỉ quản lý users
- ✓ `AdminFoodService` → chỉ quản lý foods
- ✓ Không pha lẫn logic

### **4. Composition Over Inheritance**
- ✓ Dùng composite service thay vì thừa kế
- ✓ Dễ test, dễ maintain hơn

### **5. Dependency Injection**
- ✓ Tất cả services đều inject từ constructor
- ✓ Không new instance trực tiếp
- ✓ Dễ mock cho testing

---

## 🧪 Testing (Ví Dụ Tư Duy)

```
Unit Test AdminFoodService.ApproveFoodAsync():
  1. Mock WaoDbContext
  2. Tạo fake FoodItem (status=0)
  3. Call ApproveFood(foodId)
  4. Assert status = 1
  5. Assert SaveAsync called

Unit Test AdminController.ApproveFood():
  1. Mock IAdminFoodService
  2. Setup: Foods.ApproveFoodAsync() → true
  3. Call controller.ApproveFood(id)
  4. Assert response = 200 OK

✓ Mỗi layer test riêng
✓ Không phụ thuộc vào layer khác
```

---

## 🚀 Tóm Tắt Quy Trình

| Bước | Chi tiết | Ưu điểm |
|------|----------|--------|
| **1. Chia by Domain** | UserMgmt, FoodMgmt, Analytics, Logs | Clear responsibility |
| **2. Composite Service** | Tập hợp tất cả sub-services | 1 injection point |
| **3. Controller** | Gọi via `_admin.{Service}.Method()` | Clean, readable |
| **4. [RequireAdmin]** | Check JWT role = Admin | Centralized auth |
| **5. DTOs** | Separate Admin DTOs | Type-safe responses |
| **6. Register** | Trong Program.cs 1 lần | Dependency injection |

---

## 📝 Checklist Khi Implement

- [ ] Tạo Services/Admin/Core với IAdminCompositeService
- [ ] Tạo Services/Admin/{Domain}/ cho mỗi admin feature
- [ ] Tạo [RequireAdmin] attribute
- [ ] Tạo Admin DTOs trong DTOs/Admin/
- [ ] Register tất cả services trong Program.cs
- [ ] Tạo AdminController
- [ ] Thêm AutoMapper profiles cho Admin DTOs
- [ ] Update README với admin endpoints

---

## 📚 Kết Luận

**Cách chia này:**
- ✅ **Không phức tạp** - Mỗi service focus 1 việc
- ✅ **Dễ maintain** - Tìm & sửa code nhanh
- ✅ **Scalable** - Thêm feature mới dễ dàng
- ✅ **Testable** - Mock từng service
- ✅ **Professional** - Follow industry standards

**Thay vì:**
```
❌ 1 AdminService khổng lồ (quản lý User, Food, Analytics, Logs)
❌ Vô số if/else conditions
❌ Khó maintain & test
❌ Khó thêm feature mới
```

**Dùng:**
```
✅ 4-5 services nhỏ (UserMgmt, FoodMgmt, Analytics, Logs)
✅ 1 Composite coordinator
✅ 1 clean Controller
✅ Dễ maintain & test
✅ Dễ scale
```
