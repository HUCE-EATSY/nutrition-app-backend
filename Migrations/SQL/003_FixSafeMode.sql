-- Fix: Update all Exercises Status = 1
-- Chạy lệnh này nếu gặp lỗi safe mode

USE wao_health_app;

-- Cách 1: Update từng exercise bằng WHERE Id
UPDATE Exercises SET Status = 1 WHERE Id IS NOT NULL;

-- Hoặc Cách 2: Tắt safe mode tạm thời (chỉ trong session này)
-- SET SQL_SAFE_UPDATES = 0;
-- UPDATE Exercises SET Status = 1;
-- SET SQL_SAFE_UPDATES = 1;
