CREATE DATABASE IF NOT EXISTS trimaniadb;
CREATE USER IF NOT EXISTS 'trilogo'@'%' IDENTIFIED BY '1234';
GRANT ALL PRIVILEGES ON trimaniadb.* TO 'trilogo'@'%';
USE trimaniadb;
SET GLOBAL log_bin_trust_function_creators = 1;
CREATE TABLE IF NOT EXISTS `Addresses` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Number` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Street` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Neighborhood` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `City` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `State` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
CREATE TABLE IF NOT EXISTS `Users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Login` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Password` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Cpf` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Birthday` datetime(6) NOT NULL,
  `CreationDate` datetime(6) NOT NULL,
  `AddressId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Users_Login` (`Login`),
  KEY `IX_Users_AddressId` (`AddressId`),
  CONSTRAINT `FK_Users_Addresses_AddressId` FOREIGN KEY (`AddressId`) REFERENCES `Addresses` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
CREATE TABLE IF NOT EXISTS `Orders` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ClientId` int DEFAULT NULL,
  `TotalValue` decimal(65,30) NOT NULL,
  `CreationDate` datetime(6) NOT NULL,
  `CancelDate` datetime(6) NOT NULL,
  `FinishedDate` datetime(6) NOT NULL,
  `Status` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Orders_ClientId` (`ClientId`),
  CONSTRAINT `FK_Orders_Users_ClientId` FOREIGN KEY (`ClientId`) REFERENCES `Users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
CREATE TABLE IF NOT EXISTS `Products` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Price` decimal(65,30) NOT NULL,
  `Quantity` int NOT NULL,
  `OrderId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Products_OrderId` (`OrderId`),
  CONSTRAINT `FK_Products_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;