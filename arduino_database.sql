-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- PHP Version: 8.2.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

CREATE DATABASE IF NOT EXISTS `db_arduino`;
USE `db_arduino`;

-- --------------------------------------------------------
-- Table structure for table `signin`
-- --------------------------------------------------------
CREATE TABLE `signin` (
  `Name` varchar(255) DEFAULT NULL,
  `UID` varchar(255) DEFAULT NULL,
  `Timestamp` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table `signin`
INSERT INTO `signin` (`Name`, `UID`, `Timestamp`) VALUES
('David B', '43B4C495', '2023-05-31 05:37:34'),
('David B', '43B4C495', '2023-05-31 05:37:38'),
('David B', '43B4C495', '2023-05-31 05:37:42'),
('Tomas G', '63B001A6', '2023-05-31 05:37:46'),
('David B', '43B4C495', '2023-05-31 05:40:30'),
('David B', '43B4C495', '2023-05-31 05:40:33'),
('David B', '43B4C495', '2023-05-31 05:40:38'),
('David B', '43B4C495', '2023-05-31 05:43:45'),
('David B', '43B4C495', '2023-05-31 05:43:52'),
('David B', '43B4C495', '2023-05-31 05:51:49'),
('Tomas G', '63B001A6', '2023-05-31 05:51:55'),
('David B', '43B4C495', '2023-05-31 06:01:08'),
('David B', '43B4C495', '2023-05-31 06:08:23'),
('Tomas G', '63B001A6', '2023-05-31 06:08:33'),
('Unknown', '', '2023-05-31 06:14:36'),
('David B', '43B4C495', '2023-05-31 06:37:44'),
('Tomas G', '63B001A6', '2023-05-31 06:39:17'),
('David B', '43B4C495', '2023-05-31 06:41:25'),
('David B', '43B4C495', '2023-05-31 06:42:38'),
('Tomas G', '63B001A6', '2023-05-31 06:42:46'),
('David B', '43B4C495', '2023-05-31 06:42:59'),
('Tomas G', '63B001A6', '2023-05-31 06:46:53'),
('Unknown', '43B4C495', '2023-05-31 06:53:42'),
('Tomas G', '63B001A6', '2023-05-31 07:01:05'),
('David B', '43B4C495', '2023-05-31 07:01:09'),
('David B', '43B4C495', '2023-05-31 07:01:22'),
('Tomas G', '63B001A6', '2023-05-31 07:01:26');

-- --------------------------------------------------------
-- Table structure for table `signout`
-- --------------------------------------------------------
CREATE TABLE `signout` (
  `Name` varchar(255) DEFAULT NULL,
  `UID` varchar(255) DEFAULT NULL,
  `Timestamp` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table `signout`
INSERT INTO `signout` (`Name`, `UID`, `Timestamp`) VALUES
('David B', '43B4C495', '2023-05-31 05:43:48'),
('David B', '43B4C495', '2023-05-31 05:43:56'),
('David B', '43B4C495', '2023-05-31 05:51:52'),
('Tomas G', '63B001A6', '2023-05-31 05:51:59'),
('David B', '43B4C495', '2023-05-31 06:07:31'),
('David B', '43B4C495', '2023-05-31 06:08:29'),
('Unknown', '', '2023-05-31 06:14:54'),
('David B', '43B4C495', '2023-05-31 06:39:10'),
('Tomas G', '63B001A6', '2023-05-31 06:39:14'),
('Tomas G', '63B001A6', '2023-05-31 06:39:48'),
('David B', '43B4C495', '2023-05-31 06:42:03'),
('Tomas G', '63B001A6', '2023-05-31 06:42:50'),
('David B', '43B4C495', '2023-05-31 06:42:56'),
('David B', '43B4C495', '2023-05-31 06:47:07'),
('Unknown', '63B001A6', '2023-05-31 06:53:37'),
('Unknown', '43B4C495', '2023-05-31 06:53:45'),
('David B', '43B4C495', '2023-05-31 07:01:14'),
('Tomas G', '63B001A6', '2023-05-31 07:01:18');

-- --------------------------------------------------------
-- Table structure for table `students`
-- --------------------------------------------------------
CREATE TABLE `students` (
  `Name` varchar(255) DEFAULT NULL,
  `UID` varchar(255) DEFAULT NULL,
  `Timestamp` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `Attendance` varchar(100) DEFAULT NULL,
  `Present_Count` int(11) DEFAULT NULL,
  `Leave_Count` int(11) DEFAULT NULL,
  `Absent_Count` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table `students`
INSERT INTO `students` (`Name`, `UID`, `Timestamp`, `Attendance`, `Present_Count`, `Leave_Count`, `Absent_Count`) VALUES
('David B', '43B4C495', '2023-05-31 07:01:23', 'Present', 18, 10, 4),
('Tomas G', '63B001A6', '2023-05-31 07:01:27', 'Present', 8, 6, 0),
('Unknown', '', '2023-05-31 06:14:55', 'Leave', 1, 1, 0),
('Tomas K', '81934043', '2023-05-31 07:01:27', 'Present', 0, 0, 0),
('Dominik V', 'A1B2C3D4', '2023-05-31 07:01:27', 'Present', 0, 0, 0);

COMMIT;