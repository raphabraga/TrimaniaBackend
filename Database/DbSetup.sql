DROP DATABASE IF EXISTS trimaniadb;
CREATE DATABASE IF NOT EXISTS trimaniadb;
CREATE USER IF NOT EXISTS 'trilogo'@'%' IDENTIFIED BY '1234';
GRANT ALL PRIVILEGES ON trimaniadb.* TO 'trilogo'@'%';
USE trimaniadb;
SET GLOBAL log_bin_trust_function_creators = 1;
-- CREATE TABLE IF NOT EXISTS `Addresses` (
--   `Id` int NOT NULL AUTO_INCREMENT,
--   `Number` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
--   `Street` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
--   `Neighborhood` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
--   `City` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
--   `State` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
--   PRIMARY KEY (`Id`)
-- ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
-- ALTER TABLE `Addresses` AUTO_INCREMENT=1;
-- CREATE TABLE IF NOT EXISTS `Users` (
--   `Id` int NOT NULL AUTO_INCREMENT,
--   `Login` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
--   `Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
--   `Role` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
--   `Password` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
--   `Cpf` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
--   `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
--   `Birthday` datetime(6) DEFAULT NULL,
--   `CreationDate` datetime(6) DEFAULT NULL,
--   `AddressId` int DEFAULT NULL,
--   PRIMARY KEY (`Id`),
--   UNIQUE KEY `IX_Users_Login` (`Login`),
--   KEY `IX_Users_AddressId` (`AddressId`),
--   CONSTRAINT `FK_Users_Addresses_AddressId` FOREIGN KEY (`AddressId`) REFERENCES `Addresses` (`Id`)
-- ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
-- ALTER TABLE `Users` AUTO_INCREMENT=1;
-- CREATE TABLE IF NOT EXISTS `Products` (
--   `Id` int NOT NULL AUTO_INCREMENT,
--   `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
--   `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
--   `Price` decimal(65,30) NOT NULL,
--   `StockQuantity` int NOT NULL,
--   PRIMARY KEY (`Id`),
--   UNIQUE KEY `IX_Products_Name` (`Name`)
-- ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
-- ALTER TABLE `Products` AUTO_INCREMENT=1;
-- CREATE TABLE IF NOT EXISTS `Orders` (
--   `Id` int NOT NULL AUTO_INCREMENT,
--   `ClientId` int DEFAULT NULL,
--   `TotalValue` decimal(65,30) NOT NULL,
--   `CreationDate` datetime(6) DEFAULT NULL,
--   `CancellationDate` datetime(6) DEFAULT NULL,
--   `FinishingDate` datetime(6) DEFAULT NULL,
--   `Status` int NOT NULL,
--   PRIMARY KEY (`Id`),
--   KEY `IX_Orders_ClientId` (`ClientId`),
--   CONSTRAINT `FK_Orders_Users_ClientId` FOREIGN KEY (`ClientId`) REFERENCES `Users` (`Id`)
-- ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
-- ALTER TABLE `Orders` AUTO_INCREMENT=1;
-- CREATE TABLE IF NOT EXISTS `Items` (
--   `Id` int NOT NULL AUTO_INCREMENT,
--   `ProductId` int DEFAULT NULL,
--   `Price` decimal(65,30) NOT NULL,
--   `Quantity` int NOT NULL,
--   `OrderId` int DEFAULT NULL,
--   PRIMARY KEY (`Id`),
--   KEY `IX_Items_OrderId` (`OrderId`),
--   KEY `IX_Items_ProductId` (`ProductId`),
--   CONSTRAINT `FK_Items_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`),
--   CONSTRAINT `FK_Items_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`)
-- ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
-- ALTER TABLE `Items` AUTO_INCREMENT=1;