-- Migration: Create Exercise Tables
-- Date: 2026-05-19
-- Description: Tạo bảng exercise_categories, exercises, exercise_logs

-- Tạo bảng exercise_categories
CREATE TABLE IF NOT EXISTS exercise_categories (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    NameVi VARCHAR(255) NOT NULL,
    NameEn VARCHAR(255) NOT NULL,
    IconUrl VARCHAR(500) NULL,
    CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    UpdatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)
);

-- Tạo bảng exercises
CREATE TABLE IF NOT EXISTS exercises (
    Id CHAR(36) PRIMARY KEY,
    CategoryId INT NOT NULL,
    NameVi VARCHAR(255) NOT NULL,
    NameEn VARCHAR(255) NOT NULL,
    Description TEXT NULL,
    MetValue DECIMAL(5,2) NOT NULL,
    Unit VARCHAR(50) NOT NULL,
    IconUrl VARCHAR(500) NULL,
    CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    UpdatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    FOREIGN KEY (CategoryId) REFERENCES exercise_categories(Id) ON DELETE CASCADE,
    INDEX idx_category (CategoryId)
);

-- Tạo bảng exercise_logs
CREATE TABLE IF NOT EXISTS exercise_logs (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    ExerciseId CHAR(36) NOT NULL,
    LogDate DATE NOT NULL,
    DurationMinutes INT NOT NULL,
    Intensity INT NOT NULL COMMENT '1=Nhẹ, 2=Trung bình, 3=Nặng',
    CaloriesBurned DECIMAL(8,2) NOT NULL,
    Notes TEXT NULL,
    CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    UpdatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    FOREIGN KEY (ExerciseId) REFERENCES exercises(Id) ON DELETE CASCADE,
    INDEX idx_user_date (UserId, LogDate),
    INDEX idx_exercise (ExerciseId),
    INDEX idx_log_date (LogDate)
);
