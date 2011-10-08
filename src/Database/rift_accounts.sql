/*
Navicat MySQL Data Transfer

Source Server         : Local
Source Server Version : 50512
Source Host           : localhost:3306
Source Database       : rift_accounts

Target Server Type    : MYSQL
Target Server Version : 50512
File Encoding         : 65001

Date: 2011-06-24 01:13:02
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `accounts`
-- ----------------------------
DROP TABLE IF EXISTS `accounts`;
CREATE TABLE `accounts` (
  `Accounts_ID` text,
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Username` varchar(255) DEFAULT NULL,
  `Sha_Password` text,
  `SessionKey` text,
  `Email` text,
  `GmLevel` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Username` (`Username`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of accounts
-- ----------------------------

-- ----------------------------
-- Table structure for `accounts_banned`
-- ----------------------------
DROP TABLE IF EXISTS `accounts_banned`;
CREATE TABLE `accounts_banned` (
  `Accounts_Banned_ID` text,
  `Id` bigint(20) NOT NULL,
  `BanStart` datetime DEFAULT NULL,
  `BanEnd` datetime DEFAULT NULL,
  `Reason` varchar(255) DEFAULT NULL,
  `BannedBy` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of accounts_banned
-- ----------------------------

-- ----------------------------
-- Table structure for `realms`
-- ----------------------------
DROP TABLE IF EXISTS `realms`;
CREATE TABLE `realms` (
  `Realms_ID` text,
  `Online` tinyint(3) unsigned NOT NULL,
  `RealmId` tinyint(3) unsigned NOT NULL,
  `PVP` tinyint(3) unsigned NOT NULL,
  `RP` tinyint(3) unsigned NOT NULL,
  `Language` tinyint(3) unsigned NOT NULL,
  `Recommended` tinyint(3) unsigned NOT NULL,
  `ClientVersion` bigint(20) NOT NULL,
  PRIMARY KEY (`RealmId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of realms
-- ----------------------------
