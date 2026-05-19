-- Migration: Add Status and DisplayOrder columns
-- Date: 2026-05-20
-- Description: Thêm cột Status cho exercises và DisplayOrder cho exercise_categories

-- Thêm cột DisplayOrder vào exercise_categories
ALTER TABLE exercise_categories 
ADD COLUMN DisplayOrder INT NOT NULL DEFAULT 0 AFTER IconUrl;

-- Thêm cột Status vào exercises (1=Active, 0=Inactive)
ALTER TABLE exercises 
ADD COLUMN Status TINYINT NOT NULL DEFAULT 1 AFTER IconUrl;

-- Cập nhật DisplayOrder cho các categories hiện có
UPDATE exercise_categories SET DisplayOrder = 1 WHERE Id = 1; -- Cardio
UPDATE exercise_categories SET DisplayOrder = 2 WHERE Id = 2; -- Sức mạnh
UPDATE exercise_categories SET DisplayOrder = 3 WHERE Id = 3; -- Yoga & Pilates
UPDATE exercise_categories SET DisplayOrder = 4 WHERE Id = 4; -- Thể thao
UPDATE exercise_categories SET DisplayOrder = 5 WHERE Id = 5; -- Khác

-- Cập nhật Status cho tất cả exercises hiện có (set = 1 = Active)
UPDATE exercises SET Status = 1;
