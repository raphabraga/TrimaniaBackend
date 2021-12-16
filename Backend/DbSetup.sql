CREATE DATABASE IF NOT EXISTS trimaniadb;
CREATE USER IF NOT EXISTS 'trilogo'@'%' IDENTIFIED BY '1234';
GRANT ALL PRIVILEGES ON trimaniadb.* TO 'trilogo'@'%';
USE trimaniadb;
SET GLOBAL log_bin_trust_function_creators = 1;
CREATE TABLE IF NOT EXISTS `Addresses` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Number` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Street` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Neighborhood` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `City` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `State` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
CREATE TABLE IF NOT EXISTS `Users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Login` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Password` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Cpf` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Birthday` datetime(6) NOT NULL,
  `CreationDate` datetime(6) NOT NULL,
  `AddressId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Users_Cpf` (`Cpf`),
  KEY `IX_Users_AddressId` (`AddressId`),
  CONSTRAINT `FK_Users_Addresses_AddressId` FOREIGN KEY (`AddressId`) REFERENCES `Addresses` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
