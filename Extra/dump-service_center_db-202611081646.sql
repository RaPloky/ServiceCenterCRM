-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: localhost    Database: service_center_db
-- ------------------------------------------------------
-- Server version	5.5.5-10.4.32-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `bonus_transactions`
--

DROP TABLE IF EXISTS `bonus_transactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bonus_transactions` (
  `bonus_transaction_id` int(11) NOT NULL AUTO_INCREMENT,
  `customer_id` int(11) NOT NULL,
  `sale_id` int(11) DEFAULT NULL,
  `transaction_type` varchar(50) NOT NULL,
  `amount` decimal(10,2) NOT NULL,
  `description` text DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`bonus_transaction_id`),
  KEY `customer_id` (`customer_id`),
  KEY `sale_id` (`sale_id`),
  CONSTRAINT `bonus_transactions_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`) ON DELETE CASCADE,
  CONSTRAINT `bonus_transactions_ibfk_2` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`sale_id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bonus_transactions`
--

LOCK TABLES `bonus_transactions` WRITE;
/*!40000 ALTER TABLE `bonus_transactions` DISABLE KEYS */;
INSERT INTO `bonus_transactions` VALUES (1,1,1,'EARN',150.00,'Нарахування за покупку','2026-10-26 11:57:42'),(2,4,6,'spend',300.00,'Списання бонусів за покупку №6','2026-10-29 11:29:39'),(3,4,6,'earn',407.00,'Нарахування 1% за покупку №6','2026-10-29 11:29:39'),(4,4,8,'earn',45.00,'Нарахування 1% за покупку №8','2026-10-29 15:11:21'),(5,5,9,'earn',360.00,'Нарахування 1% за покупку №9','2026-10-30 13:26:14'),(6,1,10,'EARN',25.00,'Нарахування за покупку','2026-10-02 09:15:00'),(7,2,11,'SPEND',-20.00,'Списання за покупку','2026-10-04 12:20:00'),(8,2,11,'EARN',50.00,'Нарахування за покупку','2026-10-04 12:20:00'),(9,4,13,'SPEND',-50.00,'Списання за покупку','2026-10-09 13:40:00'),(10,4,13,'EARN',30.00,'Нарахування за покупку','2026-10-09 13:40:00'),(11,12,16,'earn',3.00,'Нарахування 1% за покупку №16','2026-11-03 18:40:00'),(12,12,17,'spend',3.00,'Списання бонусів за покупку №17','2026-11-04 17:26:21'),(13,12,17,'earn',11.97,'Нарахування 1% за покупку №17','2026-11-04 17:26:21'),(14,3,18,'earn',360.00,'Нарахування 1% за покупку №18','2026-11-04 17:35:16'),(15,9,19,'earn',320.00,'Нарахування 1% за покупку №19','2026-11-08 13:46:08'),(16,10,20,'spend',500.00,'Списання бонусів за покупку №20','2026-11-08 14:08:51'),(17,10,20,'earn',317.00,'Нарахування 1% за покупку №20','2026-11-08 14:08:51'),(18,10,21,'spend',317.00,'Списання бонусів за покупку №21','2026-11-08 14:14:31'),(19,10,21,'earn',25.83,'Нарахування 1% за покупку №21','2026-11-08 14:14:31');
/*!40000 ALTER TABLE `bonus_transactions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cash_registers`
--

DROP TABLE IF EXISTS `cash_registers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cash_registers` (
  `cash_register_id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  `location` varchar(150) DEFAULT NULL,
  `status` varchar(50) DEFAULT 'Open',
  PRIMARY KEY (`cash_register_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cash_registers`
--

LOCK TABLES `cash_registers` WRITE;
/*!40000 ALTER TABLE `cash_registers` DISABLE KEYS */;
INSERT INTO `cash_registers` VALUES (1,'Головна каса магазину','Зал №1','Open');
/*!40000 ALTER TABLE `cash_registers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customer_devices`
--

DROP TABLE IF EXISTS `customer_devices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer_devices` (
  `device_id` int(11) NOT NULL AUTO_INCREMENT,
  `customer_id` int(11) NOT NULL,
  `device_type` varchar(100) NOT NULL,
  `brand` varchar(100) NOT NULL,
  `model` varchar(100) NOT NULL,
  `serial_number` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`device_id`),
  KEY `customer_id` (`customer_id`),
  CONSTRAINT `customer_devices_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer_devices`
--

LOCK TABLES `customer_devices` WRITE;
/*!40000 ALTER TABLE `customer_devices` DISABLE KEYS */;
INSERT INTO `customer_devices` VALUES (1,1,'Смартфон','Samsung','Galaxy S22','R52T10XYZ12'),(2,2,'Ноутбук','Asus','TUF Gaming F15','AS987654321'),(3,3,'Смартфон','Apple','iPhone 11','SN9988776655'),(4,4,'Смартфон','Xiaomi','Redmi Note 10','XM123456789'),(5,4,'Смартфон','Samsung','Testorine A2G',NULL),(6,7,'Смартфон','Apple','iPhone 13','SN-IP13-99823'),(7,8,'Ноутбук','Lenovo','IdeaPad 3','SN-LEN-44512'),(8,9,'Планшет','Samsung','Galaxy Tab S7','SN-SAM-77890'),(9,10,'Смартфон','Xiaomi','Redmi Note 10 Pro','SN-XMI-11234'),(10,11,'Ноутбук','Asus','TUF Gaming A15','SN-ASUS-99011'),(11,12,'Смартфон','Samsung','Galaxy S21','SN-SAM-55432'),(12,12,'Ноутбук','Acer','Aspire 5','SN-ACR-33219'),(13,6,'Смартфон','Lenovo','Goypad 3',NULL),(14,7,'Смартфон','XiaOOOmi','1A',NULL),(15,1,'Смартфон','LG','Six',NULL);
/*!40000 ALTER TABLE `customer_devices` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customer_notes`
--

DROP TABLE IF EXISTS `customer_notes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer_notes` (
  `note_id` int(11) NOT NULL AUTO_INCREMENT,
  `customer_id` int(11) NOT NULL,
  `note_text` text NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`note_id`),
  KEY `customer_id` (`customer_id`),
  CONSTRAINT `customer_notes_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer_notes`
--

LOCK TABLES `customer_notes` WRITE;
/*!40000 ALTER TABLE `customer_notes` DISABLE KEYS */;
INSERT INTO `customer_notes` VALUES (1,1,'Просив подзвонити перед видачею','2026-10-26 11:54:08'),(2,2,'Постійний клієнт','2026-10-26 11:54:08'),(3,1,'Постійний клієнт, любить оригінальні запчастини','2026-09-15 07:00:00'),(4,2,'Просить завжди перевіряти сумісність аксесуарів','2026-09-20 08:30:00'),(5,3,'Звертався за гарантійним ремонтом раніше','2026-09-25 11:00:00'),(6,4,'Учасник програми лояльності, високий бонусний баланс','2026-09-28 13:20:00'),(7,5,'Корпоративний клієнт','2026-10-01 06:15:00');
/*!40000 ALTER TABLE `customer_notes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customers`
--

DROP TABLE IF EXISTS `customers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customers` (
  `customer_id` int(11) NOT NULL AUTO_INCREMENT,
  `first_name` varchar(100) NOT NULL,
  `last_name` varchar(100) NOT NULL,
  `middle_name` varchar(100) DEFAULT NULL,
  `phone` varchar(30) NOT NULL,
  `bonus_balance` decimal(10,2) DEFAULT 0.00,
  PRIMARY KEY (`customer_id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customers`
--

LOCK TABLES `customers` WRITE;
/*!40000 ALTER TABLE `customers` DISABLE KEYS */;
INSERT INTO `customers` VALUES (1,'Віктор','Мельник','Петрович','+380671112233',150.00),(2,'Катерина','Бойко','Сергіївна','+380502223344',50.00),(3,'Дмитро','Шевченко','Іванович','+380933334455',360.00),(4,'Ольга','Ковальчук','Миколаївна','+380974445566',452.00),(5,'Максим','Тестовий','Іванович','+380671476082',360.00),(6,'Марія','Кривонос','Сергіївна','0980980982',0.00),(7,'Олександр','Петренко','Іванович','+380501112233',250.00),(8,'Марія','Коваленко','Олегівна','+380672223344',50.00),(9,'Андрій','Шевченко','Миколайович','+380933334455',320.00),(10,'Юлія','Бондар','Вікторівна','+380504445566',25.83),(11,'Дмитро','Мельник','Петрович','+380675556677',120.00),(12,'Олена','Гнатюк','Сергіївна','+380936667788',11.97),(13,'Володимир','Гнасюк','Ігнатович','00000000000',0.00);
/*!40000 ALTER TABLE `customers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `employee_shifts`
--

DROP TABLE IF EXISTS `employee_shifts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `employee_shifts` (
  `shift_id` int(11) NOT NULL AUTO_INCREMENT,
  `employee_id` int(11) NOT NULL,
  `shift_date` date NOT NULL,
  `start_time` time NOT NULL,
  `end_time` time NOT NULL,
  PRIMARY KEY (`shift_id`),
  KEY `employee_id` (`employee_id`),
  CONSTRAINT `employee_shifts_ibfk_1` FOREIGN KEY (`employee_id`) REFERENCES `employees` (`employee_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `employee_shifts`
--

LOCK TABLES `employee_shifts` WRITE;
/*!40000 ALTER TABLE `employee_shifts` DISABLE KEYS */;
INSERT INTO `employee_shifts` VALUES (1,1,'2026-10-26','09:00:00','18:00:00'),(2,2,'2026-10-26','09:00:00','18:00:00'),(3,8,'2026-10-29','09:00:00','18:00:00'),(4,7,'2026-10-01','09:00:00','18:00:00'),(5,8,'2026-10-01','09:00:00','18:00:00'),(6,9,'2026-10-01','09:00:00','18:00:00'),(7,7,'2026-10-02','09:00:00','18:00:00'),(8,8,'2026-10-02','09:00:00','18:00:00'),(9,9,'2026-10-02','09:00:00','18:00:00'),(10,7,'2026-11-05','03:00:00','23:00:00');
/*!40000 ALTER TABLE `employee_shifts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `employees`
--

DROP TABLE IF EXISTS `employees`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `employees` (
  `employee_id` int(11) NOT NULL AUTO_INCREMENT,
  `first_name` varchar(100) NOT NULL,
  `last_name` varchar(100) NOT NULL,
  `middle_name` varchar(100) DEFAULT NULL,
  `phone` varchar(30) NOT NULL,
  `username` varchar(50) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `position_id` int(11) NOT NULL,
  PRIMARY KEY (`employee_id`),
  UNIQUE KEY `username` (`username`),
  KEY `position_id` (`position_id`),
  CONSTRAINT `employees_ibfk_1` FOREIGN KEY (`position_id`) REFERENCES `positions` (`position_id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `employees`
--

LOCK TABLES `employees` WRITE;
/*!40000 ALTER TABLE `employees` DISABLE KEYS */;
INSERT INTO `employees` VALUES (7,'Олександр','Кравчук',NULL,'+380501112233','admin','admin123',1),(8,'Ірина','Либідь',NULL,'+380672223344','seller','seller123',2),(9,'Андрій','Коваль',NULL,'+380933334455','engineer','eng123',3);
/*!40000 ALTER TABLE `employees` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payments`
--

DROP TABLE IF EXISTS `payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payments` (
  `payment_id` int(11) NOT NULL AUTO_INCREMENT,
  `sale_id` int(11) NOT NULL,
  `payment_method` varchar(50) NOT NULL,
  `amount` decimal(10,2) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`payment_id`),
  KEY `sale_id` (`sale_id`),
  CONSTRAINT `payments_ibfk_1` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`sale_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payments`
--

LOCK TABLES `payments` WRITE;
/*!40000 ALTER TABLE `payments` DISABLE KEYS */;
INSERT INTO `payments` VALUES (1,1,'CARD',32000.00,'2026-10-26 11:57:42'),(2,2,'CASH',36000.00,'2026-10-26 11:57:42'),(3,3,'CASH',36000.00,'2026-10-26 13:33:27'),(4,4,'CASH',32000.00,'2026-10-26 13:33:55'),(5,9,'CASH',33000.00,'2026-10-30 13:26:26'),(6,9,'CARD',3000.00,'2026-10-30 13:26:26'),(7,10,'CARD',890.00,'2026-10-02 09:15:00'),(8,11,'CASH',1780.00,'2026-10-04 12:20:00'),(9,12,'CARD',200.00,'2026-10-06 08:00:00'),(10,13,'CASH',900.00,'2026-10-09 13:40:00'),(11,14,'CARD',3800.00,'2026-10-11 10:10:00'),(12,15,'CARD',890.00,'2026-10-13 11:00:00'),(13,16,'CARD',300.00,'2026-11-03 18:40:16'),(14,17,'CASH',1197.00,'2026-11-04 17:26:30'),(15,18,'CASH',36000.00,'2026-11-04 17:35:23'),(16,19,'CASH',32000.00,'2026-11-08 13:46:09'),(17,20,'CASH',31700.00,'2026-11-08 14:08:55'),(18,21,'CASH',2583.00,'2026-11-08 14:14:33');
/*!40000 ALTER TABLE `payments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `positions`
--

DROP TABLE IF EXISTS `positions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `positions` (
  `position_id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(150) NOT NULL,
  `description` text DEFAULT NULL,
  PRIMARY KEY (`position_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `positions`
--

LOCK TABLES `positions` WRITE;
/*!40000 ALTER TABLE `positions` DISABLE KEYS */;
INSERT INTO `positions` VALUES (1,'Адміністратор','Повний доступ до системи'),(2,'Продавець-консультант','Робота з магазином, продажами та клієнтами'),(3,'Інженер з ремонту','Діагностика, ремонти та запчастини');
/*!40000 ALTER TABLE `positions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `problem_types`
--

DROP TABLE IF EXISTS `problem_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `problem_types` (
  `problem_type_id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(150) NOT NULL,
  `category` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`problem_type_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `problem_types`
--

LOCK TABLES `problem_types` WRITE;
/*!40000 ALTER TABLE `problem_types` DISABLE KEYS */;
INSERT INTO `problem_types` VALUES (1,'Пошкодження екрану','Механічні'),(2,'Проблеми з живленням','Апаратні'),(3,'Потрапляння вологи','Механічні');
/*!40000 ALTER TABLE `problem_types` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product_categories`
--

DROP TABLE IF EXISTS `product_categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product_categories` (
  `category_id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(150) NOT NULL,
  `parent_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`category_id`),
  KEY `parent_id` (`parent_id`),
  CONSTRAINT `product_categories_ibfk_1` FOREIGN KEY (`parent_id`) REFERENCES `product_categories` (`category_id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product_categories`
--

LOCK TABLES `product_categories` WRITE;
/*!40000 ALTER TABLE `product_categories` DISABLE KEYS */;
INSERT INTO `product_categories` VALUES (1,'Смартфони',NULL),(2,'Ноутбуки',NULL),(3,'Запчастини',NULL),(4,'Дисплеї',3),(5,'Акумулятори',3);
/*!40000 ALTER TABLE `product_categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products` (
  `product_id` int(11) NOT NULL AUTO_INCREMENT,
  `sku` varchar(100) NOT NULL,
  `name` varchar(255) NOT NULL,
  `category_id` int(11) NOT NULL,
  `sale_price` decimal(10,2) NOT NULL,
  `warranty_months` int(11) DEFAULT 0,
  PRIMARY KEY (`product_id`),
  UNIQUE KEY `sku` (`sku`),
  KEY `category_id` (`category_id`),
  CONSTRAINT `products_ibfk_1` FOREIGN KEY (`category_id`) REFERENCES `product_categories` (`category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` VALUES (1,'PH-SAM-S23','Samsung Galaxy S23 128GB',1,32000.00,12),(2,'PH-APP-14','Apple iPhone 14 128GB',1,36000.00,12),(3,'NB-LEN-LOQ','Lenovo LOQ 15IRX9',2,41000.00,24),(4,'PRT-DIS-S23','Дисплейний модуль для Samsung S23',4,4500.00,3),(5,'PARTS-SCR-IP13','Дисплейний модуль OLED для iPhone 13',3,3500.00,6),(6,'PARTS-BAT-IP13','Акумулятор для iPhone 13 (Original)',3,1200.00,3),(7,'PARTS-PORT-TYPEC','Роз`єм зарядки Type-C універсальний',3,250.00,1),(8,'PARTS-SSD-512','SSD накопичувач M.2 NVMe 512GB',4,1800.00,36),(9,'PARTS-RAM-8GB','Оперативна пам\'ять SODIMM DDR4 8GB',4,950.00,36),(10,'ACC-CHG-65W','Зарядний пристрій GaN 65W Type-C',5,890.00,12),(11,'ACC-CABLE-TC','Кабель USB-C to USB-C 1м',5,200.00,6),(12,'ACC-THERMO','Термопаста MX-4 (4г)',5,300.00,0);
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `repair_attachments`
--

DROP TABLE IF EXISTS `repair_attachments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `repair_attachments` (
  `attachment_id` int(11) NOT NULL AUTO_INCREMENT,
  `repair_id` int(11) NOT NULL,
  `file_path` varchar(255) NOT NULL,
  `description` text DEFAULT NULL,
  `uploaded_by` int(11) DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`attachment_id`),
  KEY `repair_id` (`repair_id`),
  KEY `uploaded_by` (`uploaded_by`),
  CONSTRAINT `repair_attachments_ibfk_1` FOREIGN KEY (`repair_id`) REFERENCES `repairs` (`repair_id`) ON DELETE CASCADE,
  CONSTRAINT `repair_attachments_ibfk_2` FOREIGN KEY (`uploaded_by`) REFERENCES `employees` (`employee_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `repair_attachments`
--

LOCK TABLES `repair_attachments` WRITE;
/*!40000 ALTER TABLE `repair_attachments` DISABLE KEYS */;
INSERT INTO `repair_attachments` VALUES (2,1,'Uploads/59c35652-f6df-41cd-8935-508b33a415ba_photomode_06092026_203324.png','photomode_06092026_203324.png',7,'2026-10-29 14:45:09');
/*!40000 ALTER TABLE `repair_attachments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `repair_parts`
--

DROP TABLE IF EXISTS `repair_parts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `repair_parts` (
  `repair_part_id` int(11) NOT NULL AUTO_INCREMENT,
  `repair_id` int(11) DEFAULT NULL,
  `product_id` int(11) DEFAULT NULL,
  `quantity` int(11) DEFAULT 1,
  `unit_price` decimal(10,2) NOT NULL,
  `supplier_id` int(11) DEFAULT NULL,
  `name` varchar(64) NOT NULL,
  PRIMARY KEY (`repair_part_id`),
  KEY `repair_id` (`repair_id`),
  KEY `product_id` (`product_id`),
  KEY `supplier_id` (`supplier_id`),
  CONSTRAINT `repair_parts_ibfk_1` FOREIGN KEY (`repair_id`) REFERENCES `repairs` (`repair_id`) ON DELETE CASCADE,
  CONSTRAINT `repair_parts_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`),
  CONSTRAINT `repair_parts_ibfk_3` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`supplier_id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `repair_parts`
--

LOCK TABLES `repair_parts` WRITE;
/*!40000 ALTER TABLE `repair_parts` DISABLE KEYS */;
INSERT INTO `repair_parts` VALUES (1,3,4,1,4500.00,1,'Мат. плата'),(2,1,2,1,36000.00,NULL,'Apple iPhone 14 128GB'),(3,1,3,1,41000.00,NULL,'Lenovo LOQ 15IRX9'),(4,NULL,NULL,1,3500.00,1,'Динамік'),(5,8,1,1,3500.00,5,'Дисплейний модуль OLED для iPhone 13'),(6,10,2,1,1200.00,5,'Акумулятор для iPhone 13 (Original)'),(7,9,3,1,250.00,6,'Роз\'єм зарядки Type-C універсальний'),(8,11,8,1,300.00,8,'Термопаста MX-4 (4г)'),(9,12,3,1,250.00,6,'Роз\'єм зарядки Type-C універсальний'),(10,14,11,1,200.00,NULL,'Кабель USB-C to USB-C 1м'),(11,1,10,2,890.00,NULL,'Зарядний пристрій GaN 65W Type-C'),(12,1,4,1,4500.00,NULL,'Дисплейний модуль для Samsung S23');
/*!40000 ALTER TABLE `repair_parts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `repair_problems`
--

DROP TABLE IF EXISTS `repair_problems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `repair_problems` (
  `repair_id` int(11) NOT NULL,
  `problem_type_id` int(11) NOT NULL,
  `description` text DEFAULT NULL,
  `is_primary` tinyint(1) DEFAULT 0,
  PRIMARY KEY (`repair_id`,`problem_type_id`),
  KEY `problem_type_id` (`problem_type_id`),
  CONSTRAINT `repair_problems_ibfk_1` FOREIGN KEY (`repair_id`) REFERENCES `repairs` (`repair_id`) ON DELETE CASCADE,
  CONSTRAINT `repair_problems_ibfk_2` FOREIGN KEY (`problem_type_id`) REFERENCES `problem_types` (`problem_type_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `repair_problems`
--

LOCK TABLES `repair_problems` WRITE;
/*!40000 ALTER TABLE `repair_problems` DISABLE KEYS */;
INSERT INTO `repair_problems` VALUES (1,3,'Окислення на платі',1),(2,2,'Забита система охолодження',1),(3,1,'Тріщини на захисному склі та матриці',1),(6,2,'Розєм розхитаний, контакт відходить',1),(7,3,'Знос акумулятора (здоровня 68%)',1),(8,1,'Забита система охолодження пилом',1),(9,2,'Сліди корозії на платі',1),(10,2,'Ймовірно зліт завантажувача',1);
/*!40000 ALTER TABLE `repair_problems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `repair_queue`
--

DROP TABLE IF EXISTS `repair_queue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `repair_queue` (
  `queue_id` int(11) NOT NULL AUTO_INCREMENT,
  `repair_id` int(11) NOT NULL,
  `queue_type` varchar(100) NOT NULL,
  `priority` int(11) DEFAULT 0,
  `entered_at` timestamp NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`queue_id`),
  KEY `repair_id` (`repair_id`),
  CONSTRAINT `repair_queue_ibfk_1` FOREIGN KEY (`repair_id`) REFERENCES `repairs` (`repair_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `repair_queue`
--

LOCK TABLES `repair_queue` WRITE;
/*!40000 ALTER TABLE `repair_queue` DISABLE KEYS */;
INSERT INTO `repair_queue` VALUES (1,1,'Diagnostics',1,'2026-10-26 11:57:27'),(2,2,'Repair',2,'2026-10-26 11:57:27');
/*!40000 ALTER TABLE `repair_queue` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `repair_status_history`
--

DROP TABLE IF EXISTS `repair_status_history`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `repair_status_history` (
  `history_id` int(11) NOT NULL AUTO_INCREMENT,
  `repair_id` int(11) NOT NULL,
  `old_status_id` int(11) DEFAULT NULL,
  `new_status_id` int(11) NOT NULL,
  `employee_id` int(11) NOT NULL,
  `comment` text DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`history_id`),
  KEY `repair_id` (`repair_id`),
  KEY `old_status_id` (`old_status_id`),
  KEY `new_status_id` (`new_status_id`),
  KEY `employee_id` (`employee_id`),
  CONSTRAINT `repair_status_history_ibfk_1` FOREIGN KEY (`repair_id`) REFERENCES `repairs` (`repair_id`) ON DELETE CASCADE,
  CONSTRAINT `repair_status_history_ibfk_2` FOREIGN KEY (`old_status_id`) REFERENCES `repair_statuses` (`status_id`),
  CONSTRAINT `repair_status_history_ibfk_3` FOREIGN KEY (`new_status_id`) REFERENCES `repair_statuses` (`status_id`),
  CONSTRAINT `repair_status_history_ibfk_4` FOREIGN KEY (`employee_id`) REFERENCES `employees` (`employee_id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `repair_status_history`
--

LOCK TABLES `repair_status_history` WRITE;
/*!40000 ALTER TABLE `repair_status_history` DISABLE KEYS */;
INSERT INTO `repair_status_history` VALUES (1,1,1,1,1,'Створено заявку','2026-10-26 11:57:27'),(2,1,1,2,3,'Передано на діагностику','2026-10-26 11:57:27'),(3,1,3,2,9,'дорого','2026-10-29 12:05:47'),(4,1,2,3,9,'дешевше','2026-10-29 12:06:08'),(5,8,1,2,9,'Початок діагностики','2026-10-01 07:30:00'),(6,8,2,5,9,'Ремонт завершено, все працює','2026-10-02 11:00:00'),(7,9,1,2,9,'Прийнято в діагностику','2026-10-03 09:00:00'),(8,10,1,3,9,'Заміна акумулятора в процесі','2026-10-05 12:00:00'),(9,10,3,5,7,'Видано клієнту','2026-10-06 07:00:00'),(10,11,1,2,9,'Огляд системи охолодження','2026-10-08 07:00:00'),(11,14,1,4,7,'Зміна статусу з ID 1 на ID 4','2026-11-03 17:38:20');
/*!40000 ALTER TABLE `repair_status_history` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `repair_statuses`
--

DROP TABLE IF EXISTS `repair_statuses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `repair_statuses` (
  `status_id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  `description` text DEFAULT NULL,
  `color` varchar(20) DEFAULT NULL,
  `sort_order` int(11) DEFAULT 0,
  `is_final` tinyint(1) DEFAULT 0,
  PRIMARY KEY (`status_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `repair_statuses`
--

LOCK TABLES `repair_statuses` WRITE;
/*!40000 ALTER TABLE `repair_statuses` DISABLE KEYS */;
INSERT INTO `repair_statuses` VALUES (1,'Прийнято в СЦ',NULL,NULL,0,0),(2,'Діагностика',NULL,NULL,0,0),(3,'У ремонті',NULL,NULL,0,0),(4,'Готово до видачі',NULL,NULL,0,0),(5,'Видано клієнту',NULL,NULL,0,0);
/*!40000 ALTER TABLE `repair_statuses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `repairs`
--

DROP TABLE IF EXISTS `repairs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `repairs` (
  `repair_id` int(11) NOT NULL AUTO_INCREMENT,
  `repair_number` varchar(50) NOT NULL,
  `customer_id` int(11) NOT NULL,
  `device_id` int(11) NOT NULL,
  `created_by` int(11) NOT NULL,
  `assigned_employee_id` int(11) DEFAULT NULL,
  `status_id` int(11) NOT NULL,
  `customer_description` text DEFAULT NULL,
  `internal_notes` text DEFAULT NULL,
  `estimated_price` decimal(10,2) DEFAULT NULL,
  `final_price` decimal(10,2) DEFAULT NULL,
  `received_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `estimated_completion_date` datetime DEFAULT NULL,
  `completed_at` datetime DEFAULT NULL,
  `issued_at` datetime DEFAULT NULL,
  PRIMARY KEY (`repair_id`),
  UNIQUE KEY `repair_number` (`repair_number`),
  KEY `customer_id` (`customer_id`),
  KEY `device_id` (`device_id`),
  KEY `created_by` (`created_by`),
  KEY `assigned_employee_id` (`assigned_employee_id`),
  KEY `status_id` (`status_id`),
  CONSTRAINT `repairs_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`),
  CONSTRAINT `repairs_ibfk_2` FOREIGN KEY (`device_id`) REFERENCES `customer_devices` (`device_id`),
  CONSTRAINT `repairs_ibfk_3` FOREIGN KEY (`created_by`) REFERENCES `employees` (`employee_id`),
  CONSTRAINT `repairs_ibfk_4` FOREIGN KEY (`assigned_employee_id`) REFERENCES `employees` (`employee_id`),
  CONSTRAINT `repairs_ibfk_5` FOREIGN KEY (`status_id`) REFERENCES `repair_statuses` (`status_id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `repairs`
--

LOCK TABLES `repairs` WRITE;
/*!40000 ALTER TABLE `repairs` DISABLE KEYS */;
INSERT INTO `repairs` VALUES (1,'REP-0001',1,1,1,8,3,'Не вмикається після падіння у воду','необхідна заміна материнської плати',100000.00,3500.00,'2026-10-26 11:16:47',NULL,NULL,NULL),(2,'REP-0002',2,2,1,3,3,'Сильно перегрівається та шумить під час ігор',NULL,800.00,NULL,'2026-10-26 11:16:47',NULL,NULL,NULL),(3,'REP-0003',3,3,2,3,1,'Розбите скло екрану, сенсор працює',NULL,2200.00,NULL,'2026-10-26 11:16:47',NULL,NULL,NULL),(6,'REP-2405',1,1,7,NULL,1,'Сформовано автоматично з додатку WPF',NULL,1200.00,NULL,'2026-10-26 11:24:05',NULL,NULL,NULL),(7,'REP-261026-3401',1,1,7,NULL,1,'не працює гніздо зарядки',NULL,3500.00,NULL,'2026-10-26 11:34:01',NULL,NULL,NULL),(8,'REP-2026-001',7,6,7,9,5,'Розбився екран після падіння','Дисплей під заміну, рамка ціла',3800.00,3800.00,'2026-10-01 07:00:00',NULL,NULL,NULL),(9,'REP-2026-002',8,7,7,9,3,'Не заряджається','Ймовірно пошкоджений розєм Type-C',600.00,NULL,'2026-10-03 08:30:00',NULL,NULL,NULL),(10,'REP-2026-003',8,8,7,9,5,'Швидко розряджається','Потрібна заміна акумулятора',1400.00,1400.00,'2026-10-05 11:15:00',NULL,NULL,NULL),(11,'REP-2026-004',10,9,7,9,2,'Вимикається в іграх','Потрібна чистка та заміна термопасти',800.00,NULL,'2026-10-08 06:45:00',NULL,NULL,NULL),(12,'REP-2026-005',11,10,7,9,4,'Потрапила волога під корпус','Чистка в ультразвуковій ванні',1200.00,1500.00,'2026-10-10 13:20:00',NULL,NULL,NULL),(13,'REP-2026-006',12,11,7,9,1,'Не вмикається','Первинний огляд',1000.00,NULL,'2026-10-12 09:00:00',NULL,NULL,NULL),(14,'REP-261103-3724',1,1,7,8,4,'Не працює 4G/LTE','',2300.00,2300.00,'2026-11-03 17:37:24',NULL,NULL,NULL),(15,'REP-261104-5613',2,2,7,NULL,1,'Не працює',NULL,35000.00,NULL,'2026-11-04 18:56:13',NULL,NULL,NULL);
/*!40000 ALTER TABLE `repairs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `salary_records`
--

DROP TABLE IF EXISTS `salary_records`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `salary_records` (
  `salary_id` int(11) NOT NULL AUTO_INCREMENT,
  `employee_id` int(11) NOT NULL,
  `period` varchar(20) NOT NULL,
  `salary` decimal(10,2) NOT NULL,
  `status` varchar(50) DEFAULT 'Calculated',
  `paid_at` datetime DEFAULT NULL,
  PRIMARY KEY (`salary_id`),
  KEY `employee_id` (`employee_id`),
  CONSTRAINT `salary_records_ibfk_1` FOREIGN KEY (`employee_id`) REFERENCES `employees` (`employee_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `salary_records`
--

LOCK TABLES `salary_records` WRITE;
/*!40000 ALTER TABLE `salary_records` DISABLE KEYS */;
INSERT INTO `salary_records` VALUES (1,7,'2026-10',15000.00,'Calculated',NULL),(2,8,'Жовтень 2026',25000.00,'Виплачено','2026-10-26 18:01:53'),(3,9,'Жовтень 2026',50000.00,'Нараховано премію','2026-11-03 20:41:37'),(4,7,'Жовтень 2027',9000000.00,'Суперпремія','2026-11-04 19:29:13');
/*!40000 ALTER TABLE `salary_records` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sale_items`
--

DROP TABLE IF EXISTS `sale_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sale_items` (
  `sale_item_id` int(11) NOT NULL AUTO_INCREMENT,
  `sale_id` int(11) NOT NULL,
  `product_id` int(11) NOT NULL,
  `quantity` int(11) DEFAULT 1,
  `unit_price` decimal(10,2) NOT NULL,
  `total` decimal(10,2) NOT NULL,
  PRIMARY KEY (`sale_item_id`),
  KEY `sale_id` (`sale_id`),
  KEY `product_id` (`product_id`),
  CONSTRAINT `sale_items_ibfk_1` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`sale_id`) ON DELETE CASCADE,
  CONSTRAINT `sale_items_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sale_items`
--

LOCK TABLES `sale_items` WRITE;
/*!40000 ALTER TABLE `sale_items` DISABLE KEYS */;
INSERT INTO `sale_items` VALUES (1,1,1,1,32000.00,0.00),(2,3,2,1,36000.00,36000.00),(3,4,1,1,32000.00,32000.00),(4,6,3,1,41000.00,0.00),(5,8,4,1,4500.00,0.00),(6,9,2,1,36000.00,0.00),(7,10,6,1,890.00,890.00),(8,11,4,1,1800.00,1800.00),(9,12,7,1,200.00,200.00),(10,13,5,1,950.00,950.00),(11,14,1,1,3500.00,3500.00),(12,14,8,1,300.00,300.00),(13,15,6,1,890.00,890.00),(14,16,12,1,300.00,0.00),(15,17,6,1,1200.00,0.00),(16,18,2,1,36000.00,0.00),(17,19,1,1,32000.00,0.00),(18,20,1,1,32000.00,32000.00),(19,20,11,1,200.00,200.00),(20,21,9,2,950.00,1900.00),(21,21,7,4,250.00,1000.00);
/*!40000 ALTER TABLE `sale_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sales`
--

DROP TABLE IF EXISTS `sales`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sales` (
  `sale_id` int(11) NOT NULL AUTO_INCREMENT,
  `sale_number` varchar(50) NOT NULL,
  `customer_id` int(11) DEFAULT NULL,
  `employee_id` int(11) NOT NULL,
  `sale_date` timestamp NOT NULL DEFAULT current_timestamp(),
  `bonus_used` decimal(10,2) DEFAULT 0.00,
  `total_amount` decimal(10,2) NOT NULL,
  PRIMARY KEY (`sale_id`),
  UNIQUE KEY `sale_number` (`sale_number`),
  KEY `customer_id` (`customer_id`),
  KEY `employee_id` (`employee_id`),
  CONSTRAINT `sales_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`),
  CONSTRAINT `sales_ibfk_2` FOREIGN KEY (`employee_id`) REFERENCES `employees` (`employee_id`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sales`
--

LOCK TABLES `sales` WRITE;
/*!40000 ALTER TABLE `sales` DISABLE KEYS */;
INSERT INTO `sales` VALUES (1,'',1,7,'2026-10-26 11:41:16',0.00,32000.00),(3,'SALE-20261026153327',2,7,'2026-10-26 13:33:27',0.00,36000.00),(4,'SALE-20261026153355',1,7,'2026-10-26 13:33:55',0.00,32000.00),(6,'POS-20261029132939292',4,8,'2026-10-29 11:29:39',300.00,40700.00),(8,'POS-20261029171121373',4,7,'2026-10-29 15:11:21',0.00,4500.00),(9,'POS-20261030152614009',5,7,'2026-10-30 13:26:14',0.00,36000.00),(10,'SALE-2026-001',1,8,'2026-10-02 09:15:00',0.00,890.00),(11,'SALE-2026-002',2,8,'2026-10-04 12:20:00',20.00,1800.00),(12,'SALE-2026-003',3,8,'2026-10-06 08:00:00',0.00,200.00),(13,'SALE-2026-004',4,8,'2026-10-09 13:40:00',50.00,950.00),(14,'SALE-2026-005',5,8,'2026-10-11 10:10:00',0.00,3800.00),(15,'SALE-2026-006',6,8,'2026-10-13 11:00:00',0.00,890.00),(16,'POS-20261103204000699',12,7,'2026-11-03 18:40:00',0.00,300.00),(17,'POS-20261104192621493',12,7,'2026-11-04 17:26:21',3.00,1197.00),(18,'POS-20261104193516308',3,7,'2026-11-04 17:35:16',0.00,36000.00),(19,'POS-20261108154607937',9,7,'2026-11-08 13:46:07',0.00,32000.00),(20,'POS-20261108160851822',10,7,'2026-11-08 14:08:51',500.00,31700.00),(21,'POS-20261108161431782',10,7,'2026-11-08 14:14:31',317.00,2583.00);
/*!40000 ALTER TABLE `sales` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supplier_products`
--

DROP TABLE IF EXISTS `supplier_products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `supplier_products` (
  `supplier_product_id` int(11) NOT NULL AUTO_INCREMENT,
  `supplier_id` int(11) NOT NULL,
  `product_id` int(11) NOT NULL,
  `purchase_price` decimal(10,2) NOT NULL,
  `delivery_days` int(11) DEFAULT 3,
  `is_available` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`supplier_product_id`),
  KEY `supplier_id` (`supplier_id`),
  KEY `product_id` (`product_id`),
  CONSTRAINT `supplier_products_ibfk_1` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`supplier_id`) ON DELETE CASCADE,
  CONSTRAINT `supplier_products_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier_products`
--

LOCK TABLES `supplier_products` WRITE;
/*!40000 ALTER TABLE `supplier_products` DISABLE KEYS */;
INSERT INTO `supplier_products` VALUES (1,1,1,28000.00,2,1),(2,2,2,31000.00,3,1),(3,5,1,2800.00,2,1),(4,5,2,900.00,2,1),(5,6,3,180.00,3,1),(6,7,4,1400.00,1,1),(7,7,5,750.00,1,1),(8,8,6,650.00,4,1);
/*!40000 ALTER TABLE `supplier_products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `suppliers`
--

DROP TABLE IF EXISTS `suppliers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `suppliers` (
  `supplier_id` int(11) NOT NULL AUTO_INCREMENT,
  `company_name` varchar(150) NOT NULL,
  `contact_person` varchar(150) DEFAULT NULL,
  `phone` varchar(30) DEFAULT NULL,
  `address` text DEFAULT NULL,
  PRIMARY KEY (`supplier_id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `suppliers`
--

LOCK TABLES `suppliers` WRITE;
/*!40000 ALTER TABLE `suppliers` DISABLE KEYS */;
INSERT INTO `suppliers` VALUES (1,'TechParts UA','Максим Петренко','+380441234567','м. Київ, вул. Заводська, 5'),(2,'Global Mobile','Олена Свириденко','+380449876543','м. Львів, пр. Свободи, 12'),(3,'ElectroWorld','Іван Дмитренко','+380487778899','м. Одеса, вул. Дерибасівська, 20'),(4,'TestLaps','Марина Шевчук','+380927259102','Київ'),(5,'ТОВ \"ТехноПром\"','Ігор Василенко','+380441234567','м. Київ, вул. Заводська, 12'),(6,'ДП \"Електроніка-М\"','Ольга Свириденко','+380447654321','м. Львів, вул. Промислова, 5'),(7,'ПП \"КомпСервіс Груп\"','Максим Коваль','+380449876543','м. Одеса, вул. Арнаутська, 45'),(8,'ТОВ \"Мобі-Партс\"','Світлана Бондаренко','+380443332211','м. Харків, пр. Науки, 88');
/*!40000 ALTER TABLE `suppliers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `warehouse_stock`
--

DROP TABLE IF EXISTS `warehouse_stock`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `warehouse_stock` (
  `stock_id` int(11) NOT NULL AUTO_INCREMENT,
  `warehouse_id` int(11) NOT NULL,
  `product_id` int(11) NOT NULL,
  `quantity` int(11) DEFAULT 0,
  `reserved_quantity` int(11) DEFAULT 0,
  `minimum_quantity` int(11) DEFAULT 0,
  PRIMARY KEY (`stock_id`),
  KEY `product_id` (`product_id`),
  CONSTRAINT `warehouse_stock_ibfk_1` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `warehouse_stock`
--

LOCK TABLES `warehouse_stock` WRITE;
/*!40000 ALTER TABLE `warehouse_stock` DISABLE KEYS */;
INSERT INTO `warehouse_stock` VALUES (1,1,1,3,1,2),(2,1,2,0,0,1),(3,1,3,1,1,1),(4,1,4,9,2,3),(6,1,6,7,0,3),(7,1,7,11,2,5),(8,1,8,6,0,2),(9,1,9,8,1,3),(10,1,10,5,0,2),(11,1,11,23,0,5),(12,1,12,12,0,4);
/*!40000 ALTER TABLE `warehouse_stock` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'service_center_db'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-11-08 16:46:45
