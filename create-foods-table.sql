-- Create foods table
CREATE TABLE IF NOT EXISTS `foods` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) NOT NULL,
    `Category` varchar(100) NULL,
    `Calories` float NOT NULL,
    `Protein` float NOT NULL,
    `Carbs` float NOT NULL,
    `Fat` float NOT NULL,
    `ServingSize` float NOT NULL,
    `ImageUrl` varchar(500) NULL,
    `Description` varchar(1000) NULL,
    `CreatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    PRIMARY KEY (`Id`),
    INDEX `idx_food_name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
