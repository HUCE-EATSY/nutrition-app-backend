# Database Migrations

## Cách chạy migrations

### Lần đầu setup database:

```bash
# 1. Tạo bảng
mysql -u root -p wao_health_app < 001_CreateExerciseTables.sql

# 2. Seed data cơ bản
mysql -u root -p wao_health_app < 002_SeedExerciseData.sql

# 3. Thêm cột Status và DisplayOrder
mysql -u root -p wao_health_app < 003_AddStatusAndDisplayOrderColumns.sql

# 4. Bổ sung thêm exercises
mysql -u root -p wao_health_app < 004_AddMoreExercises.sql
```

### Hoặc chạy trong MySQL Workbench:

1. Mở MySQL Workbench
2. Kết nối đến database `wao_health_app`
3. File → Open SQL Script
4. Chọn file `001_CreateExerciseTables.sql` → Execute
5. Chọn file `002_SeedExerciseData.sql` → Execute
6. Chọn file `003_AddStatusAndDisplayOrderColumns.sql` → Execute
7. Chọn file `004_AddMoreExercises.sql` → Execute

### Kiểm tra:

```sql
-- Kiểm tra số lượng
SELECT 'Categories' AS TableName, COUNT(*) AS Count FROM ExerciseCategories
UNION ALL
SELECT 'Exercises', COUNT(*) FROM Exercises;

-- Xem chi tiết
SELECT 
    ec.NameVi AS Category,
    COUNT(e.Id) AS ExerciseCount
FROM ExerciseCategories ec
LEFT JOIN Exercises e ON e.CategoryId = ec.Id
GROUP BY ec.Id, ec.NameVi
ORDER BY ec.Id;
```

## Danh sách migrations

| File | Description | Date |
|------|-------------|------|
| 001_CreateExerciseTables.sql | Tạo bảng ExerciseCategories, Exercises, ExerciseLogs | 2026-05-19 |
| 002_SeedExerciseData.sql | Seed dữ liệu mẫu (5 categories, 23 exercises) | 2026-05-19 |
| 003_AddStatusAndDisplayOrderColumns.sql | Thêm cột Status và DisplayOrder | 2026-05-20 |
| 004_AddMoreExercises.sql | Bổ sung thêm ~40 exercises (tổng ~63 exercises) | 2026-05-20 |

## Lưu ý

- Các file migration này **idempotent** (chạy nhiều lần không bị lỗi)
- Sử dụng `CREATE TABLE IF NOT EXISTS` và `INSERT IGNORE`
- Mọi người trong team chỉ cần pull code và chạy migrations
