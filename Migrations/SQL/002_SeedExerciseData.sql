-- Migration: Seed Exercise Data (30 exercises)
-- Date: 2026-05-19
-- Description: Thêm dữ liệu mẫu cho exercise_categories và exercises (tổng 30 bài tập)

-- Seed Categories
INSERT IGNORE INTO exercise_categories (Id, NameVi, NameEn, IconUrl, CreatedAt, UpdatedAt)
VALUES 
    (1, 'Cardio', 'Cardio', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
    (2, 'Sức mạnh', 'Strength', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
    (3, 'Yoga & Pilates', 'Yoga & Pilates', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
    (4, 'Thể thao', 'Sports', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
    (5, 'Khác', 'Other', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP());

-- Seed Exercises - Cardio (8 bài)
INSERT INTO exercises (Id, CategoryId, NameVi, NameEn, Description, MetValue, Unit, IconUrl, CreatedAt, UpdatedAt)
SELECT * FROM (
    SELECT UUID() AS Id, 1 AS CategoryId, 'Chạy bộ' AS NameVi, 'Running' AS NameEn, 'Chạy bộ tốc độ trung bình' AS Description, 8.0 AS MetValue, 'minutes' AS Unit, NULL AS IconUrl, UTC_TIMESTAMP() AS CreatedAt, UTC_TIMESTAMP() AS UpdatedAt
    UNION ALL SELECT UUID(), 1, 'Đi bộ', 'Walking', 'Đi bộ tốc độ bình thường', 3.5, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 1, 'Đạp xe', 'Cycling', 'Đạp xe tốc độ trung bình', 6.8, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 1, 'Bơi lội', 'Swimming', 'Bơi lội tự do', 7.0, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 1, 'Nhảy dây', 'Jump Rope', 'Nhảy dây', 11.0, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 1, 'Aerobic', 'Aerobic', 'Aerobic cường độ trung bình', 7.0, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 1, 'Leo núi', 'Mountain Climbing', 'Leo núi', 9.5, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 1, 'Lướt sóng', 'Surfing', 'Lướt sóng', 5.0, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
) AS tmp
WHERE NOT EXISTS (
    SELECT 1 FROM exercises WHERE NameVi = tmp.NameVi AND CategoryId = tmp.CategoryId
);

-- Seed Exercises - Sức mạnh (6 bài)
INSERT INTO exercises (Id, CategoryId, NameVi, NameEn, Description, MetValue, Unit, IconUrl, CreatedAt, UpdatedAt)
SELECT * FROM (
    SELECT UUID() AS Id, 2 AS CategoryId, 'Tập tạ' AS NameVi, 'Weight Training' AS NameEn, 'Tập tạ', 5.0 AS MetValue, 'minutes' AS Unit, NULL AS IconUrl, UTC_TIMESTAMP() AS CreatedAt, UTC_TIMESTAMP() AS UpdatedAt
    UNION ALL SELECT UUID(), 2, 'Chống đẩy', 'Push-ups', 'Chống đẩy', 3.8, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 2, 'Gập bụng', 'Sit-ups', 'Gập bụng', 3.8, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 2, 'Plank', 'Plank', 'Plank', 4.0, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 2, 'Squat', 'Squats', 'Squat', 5.5, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 2, 'Kéo xà', 'Pull-ups', 'Kéo xà đơn', 8.0, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
) AS tmp
WHERE NOT EXISTS (
    SELECT 1 FROM exercises WHERE NameVi = tmp.NameVi AND CategoryId = tmp.CategoryId
);

-- Seed Exercises - Yoga & Pilates (3 bài)
INSERT INTO exercises (Id, CategoryId, NameVi, NameEn, Description, MetValue, Unit, IconUrl, CreatedAt, UpdatedAt)
SELECT * FROM (
    SELECT UUID() AS Id, 3 AS CategoryId, 'Yoga' AS NameVi, 'Yoga' AS NameEn, 'Yoga cơ bản' AS Description, 3.0 AS MetValue, 'minutes' AS Unit, NULL AS IconUrl, UTC_TIMESTAMP() AS CreatedAt, UTC_TIMESTAMP() AS UpdatedAt
    UNION ALL SELECT UUID(), 3, 'Pilates', 'Pilates', 'Pilates', 3.0, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 3, 'Giãn cơ', 'Stretching', 'Giãn cơ', 2.3, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
) AS tmp
WHERE NOT EXISTS (
    SELECT 1 FROM exercises WHERE NameVi = tmp.NameVi AND CategoryId = tmp.CategoryId
);

-- Seed Exercises - Thể thao (8 bài)
INSERT INTO exercises (Id, CategoryId, NameVi, NameEn, Description, MetValue, Unit, IconUrl, CreatedAt, UpdatedAt)
SELECT * FROM (
    SELECT UUID() AS Id, 4 AS CategoryId, 'Bóng đá' AS NameVi, 'Soccer' AS NameEn, 'Chơi bóng đá' AS Description, 7.0 AS MetValue, 'minutes' AS Unit, NULL AS IconUrl, UTC_TIMESTAMP() AS CreatedAt, UTC_TIMESTAMP() AS UpdatedAt
    UNION ALL SELECT UUID(), 4, 'Bóng rổ', 'Basketball', 'Chơi bóng rổ', 6.5, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 4, 'Cầu lông', 'Badminton', 'Chơi cầu lông', 5.5, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 4, 'Tennis', 'Tennis', 'Chơi tennis', 7.3, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 4, 'Bóng chuyền', 'Volleyball', 'Chơi bóng chuyền', 4.0, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 4, 'Bóng bàn', 'Table Tennis', 'Chơi bóng bàn', 4.0, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 4, 'Boxing', 'Boxing', 'Đấm bốc', 9.0, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 4, 'Võ thuật', 'Martial Arts', 'Luyện võ thuật', 10.0, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
) AS tmp
WHERE NOT EXISTS (
    SELECT 1 FROM exercises WHERE NameVi = tmp.NameVi AND CategoryId = tmp.CategoryId
);

-- Seed Exercises - Khác (3 bài)
INSERT INTO exercises (Id, CategoryId, NameVi, NameEn, Description, MetValue, Unit, IconUrl, CreatedAt, UpdatedAt)
SELECT * FROM (
    SELECT UUID() AS Id, 5 AS CategoryId, 'Nấu ăn' AS NameVi, 'Cooking' AS NameEn, 'Nấu ăn' AS Description, 2.5 AS MetValue, 'minutes' AS Unit, NULL AS IconUrl, UTC_TIMESTAMP() AS CreatedAt, UTC_TIMESTAMP() AS UpdatedAt
    UNION ALL SELECT UUID(), 5, 'Chơi với trẻ', 'Playing with Kids', 'Chơi với trẻ', 4.0, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
    UNION ALL SELECT UUID(), 5, 'Trượt ván', 'Skateboarding', 'Trượt ván', 6.0, 'minutes', NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()
) AS tmp
WHERE NOT EXISTS (
    SELECT 1 FROM exercises WHERE NameVi = tmp.NameVi AND CategoryId = tmp.CategoryId
);
