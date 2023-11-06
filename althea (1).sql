-- phpMyAdmin SQL Dump
-- version 5.1.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1:3306
-- Létrehozás ideje: 2023. Nov 06. 15:41
-- Kiszolgáló verziója: 5.7.36
-- PHP verzió: 7.4.26

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `althea`
--

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `accounts`
--

DROP TABLE IF EXISTS `accounts`;
CREATE TABLE IF NOT EXISTS `accounts` (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `userName` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `email` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `adminLevel` int(11) NOT NULL DEFAULT '0',
  `adminNick` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL DEFAULT '',
  `registerDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `passwordHash` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `passwordSalt` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `serial` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `scId` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `sc` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `characterSlots` tinyint(3) UNSIGNED NOT NULL DEFAULT '2',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `accounts`
--

INSERT INTO `accounts` (`id`, `userName`, `email`, `adminLevel`, `adminNick`, `registerDate`, `passwordHash`, `passwordSalt`, `serial`, `scId`, `sc`, `characterSlots`) VALUES
(1, 'Chy', 'chy@althea.hu', 10, '', '2023-08-31 20:19:07', '7irrMY/4RrueoXI8P+qUWZQbHrvJKbtS+exNEHIx+hd4imPbmfnd+7+MOO/saaU15NcjA0j8F0tPd9hGqTR1zPJCnCuQ5Q==', '5ntrk1MDntd4w8arKiISDul6iiSda6pLdOdyBb4VOnOkLFrbRmcxpDm20g9M/JH1dpx237kjC+WwrXZbo6oxcIS7MOjMxvzy1bKEyjVRbyow4xZfp/gW7MnuLt0oqHN2tGNXZQ==', 'D8903A0447DCA880A7E0057CCBF41C10CE30D8D8D5D27640B0C6102401728A80319C882076A65D10D8E0E54043861920E4526AE0681477E8AA7010047006F0C0', '67714232', 'CHYJIN', 2),
(2, 'mayev', 'megegyvacosacc@gmail.com', 0, '', '2023-09-03 14:36:26', 'SiWEX5Ihqe28wou3c2/UFJJ07sjPm2FZ/vT5Q8qyACBKPDldoG8Ztw+nkEkr1QmHGWFhn/jt4fKBknEYcDucUb3V/t6A+w==', '+q9c7edwuIF1FTIF/jTOQuiqUZpVCD0sbRsbDr3ZZmmlS9K3aPyjPFSBZF/LgmOurFJSGgYwxOqWSDTPc9j94fPmdH/DIDhhVe3wIqmIqbKGwcrOhpqY78xPHiT4r2xOkD74MQ==', 'D8903A045B5858583EEEC4C0D554D1001F6057102FBA81C8BBF018C8DD229C004B5CC1B01096EAD8B0B610BC242A1950A37608A056B6E9E0A83E7A949C96A640', '24763806', 'bati_a_batyus', 2),
(3, 'mandms', 'mandms@hcrp.hu', 0, '', '2023-09-20 13:08:52', 'oxQD+5k67KCwwWJ4tyoq1eMrXkgXhYBV0vY6ciYNgxALVSLsv7ogmmmlBmmDX6w+t9TG8Z1FaTlxwdFUoJizCumzsw9CSg==', 'KB4C7/wq8zkadPPAZfNsfrDxwEiTzplZsVADFsg7zIe78dypJ/1V4RRELv5GXwPcuSBs6pfkcUVOnsuhgRt2w2OiY2pGpdJuVgu/CnAtiEpASmRKjE0ojIPED0HCbss/fpG7gg==', 'DFBE67A8DD7E57487A72B834220A5AA0CFAE37486DCE67D81AC23824C23ABA40BF9E07E8FD1E7768BA12B814626A1AE0AF8ED7888D6E87F85A623804029A7A80', '85991460', 'MandMs-11', 2),
(4, 'morcsog', 'morcsog@vagyok.hu', 0, '', '2023-10-07 19:14:59', 'Yw+y4wDxR2xNYXGJrbNTkswsEG1cXkrGYwk1n/tyzrbn2yt+6UHXFgRUInd9aQPvHyYKssnRo7JG72zdHikLMWi42oQdvw==', 'Mb2ts06L8y2fq78QrBaXbAkfcV6arvQLNkHpybCoHwHPOG8DrQ3vv7ykDQjPh4QGyz9ZOJ1RiUbjgBT53XNAvCV2v3sO9HXHP4FGmfTjroNYzITDYZaXxhw3InFqVqVCul/O/Q==', 'D8903A045BAC82D8BC661CD41A0A4810B9E2F8F8C620B2B070F018C8DD224EC05B3865D0634CE5C0D9BAE3E0AE44D0A072E008A056B6E9F0FEDEB464841AA640', '185981925', 'Botika9696', 2);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `acmds`
--

DROP TABLE IF EXISTS `acmds`;
CREATE TABLE IF NOT EXISTS `acmds` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `command` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `adminLevel` int(11) NOT NULL DEFAULT '7',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `acmds`
--

INSERT INTO `acmds` (`id`, `command`, `adminLevel`) VALUES
(1, 'fly', 7),
(2, 'setdimension', 1),
(3, 'reloadacmds', 2),
(4, 'setadminlevel', 6),
(5, 'setcommandlevel', 7);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `anims`
--

DROP TABLE IF EXISTS `anims`;
CREATE TABLE IF NOT EXISTS `anims` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `command` varchar(100) COLLATE utf8_hungarian_ci NOT NULL,
  `dictionary` varchar(200) COLLATE utf8_hungarian_ci NOT NULL,
  `animation` varchar(200) COLLATE utf8_hungarian_ci NOT NULL,
  `flag` int(11) NOT NULL,
  `category` varchar(50) COLLATE utf8_hungarian_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `anims`
--

INSERT INTO `anims` (`id`, `command`, `dictionary`, `animation`, `flag`, `category`) VALUES
(1, 'ul', '1', '2', 5, 'szép');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `appearances`
--

DROP TABLE IF EXISTS `appearances`;
CREATE TABLE IF NOT EXISTS `appearances` (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `gender` tinyint(1) DEFAULT '1',
  `eyeColor` tinyint(3) UNSIGNED DEFAULT '0',
  `hairStyle` smallint(5) UNSIGNED DEFAULT '0',
  `hairColor` tinyint(3) UNSIGNED DEFAULT '0',
  `hairHighlight` tinyint(3) UNSIGNED DEFAULT '0',
  `parent1face` tinyint(3) UNSIGNED DEFAULT '0',
  `parent2face` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `parent3face` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `parent1skin` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `parent2skin` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `parent3skin` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `faceMix` tinyint(3) UNSIGNED NOT NULL DEFAULT '50',
  `skinMix` tinyint(3) UNSIGNED NOT NULL DEFAULT '50',
  `thirdMix` tinyint(3) UNSIGNED NOT NULL DEFAULT '50',
  `noseWidth` tinyint(4) NOT NULL DEFAULT '0',
  `noseHeight` tinyint(4) NOT NULL DEFAULT '0',
  `noseLength` tinyint(4) NOT NULL DEFAULT '0',
  `noseBridge` tinyint(4) NOT NULL DEFAULT '0',
  `noseTip` tinyint(4) NOT NULL DEFAULT '0',
  `noseBroken` tinyint(4) NOT NULL DEFAULT '0',
  `browHeight` tinyint(4) NOT NULL DEFAULT '0',
  `browWidth` tinyint(4) NOT NULL DEFAULT '0',
  `cheekboneHeight` tinyint(4) NOT NULL DEFAULT '0',
  `cheekboneWidth` tinyint(4) NOT NULL DEFAULT '0',
  `cheekWidth` tinyint(4) NOT NULL DEFAULT '0',
  `eyes` tinyint(4) NOT NULL DEFAULT '0',
  `lips` tinyint(4) NOT NULL DEFAULT '0',
  `jawWidth` tinyint(4) NOT NULL DEFAULT '0',
  `jawHeight` tinyint(4) NOT NULL DEFAULT '0',
  `chinLength` tinyint(4) NOT NULL DEFAULT '0',
  `chinPosition` tinyint(4) NOT NULL DEFAULT '0',
  `chinWidth` tinyint(4) NOT NULL DEFAULT '0',
  `chinShape` tinyint(4) NOT NULL DEFAULT '0',
  `neckWidth` tinyint(4) NOT NULL DEFAULT '0',
  `blemishId` tinyint(3) UNSIGNED NOT NULL DEFAULT '255',
  `blemishOpacity` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `facialhairId` tinyint(3) UNSIGNED NOT NULL DEFAULT '255',
  `facialhairColor` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `facialhairOpacity` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `eyebrowId` tinyint(3) UNSIGNED NOT NULL DEFAULT '255',
  `eyebrowColor` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `eyebrowOpacity` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `ageId` tinyint(3) UNSIGNED NOT NULL DEFAULT '255',
  `ageOpacity` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `makeupId` tinyint(3) UNSIGNED NOT NULL DEFAULT '255',
  `makeupOpacity` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `blushId` tinyint(3) UNSIGNED NOT NULL DEFAULT '255',
  `blushColor` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `blushOpacity` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `complexionId` tinyint(3) UNSIGNED NOT NULL DEFAULT '255',
  `complexionOpacity` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `sundamageId` tinyint(3) UNSIGNED NOT NULL DEFAULT '255',
  `sundamageOpacity` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `lipstickId` tinyint(3) UNSIGNED NOT NULL DEFAULT '255',
  `lipstickColor` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `lipstickOpacity` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `frecklesId` tinyint(3) UNSIGNED NOT NULL DEFAULT '255',
  `frecklesOpacity` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `chesthairId` tinyint(3) UNSIGNED NOT NULL DEFAULT '255',
  `chesthairColor` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `chesthairOpacity` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `bodyblemishId` tinyint(3) UNSIGNED NOT NULL DEFAULT '255',
  `bodyblemishOpacity` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `bodyblemish2Id` tinyint(3) UNSIGNED NOT NULL DEFAULT '255',
  `bodyblemish2Opacity` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `tattoos` json DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `appearances`
--

INSERT INTO `appearances` (`id`, `gender`, `eyeColor`, `hairStyle`, `hairColor`, `hairHighlight`, `parent1face`, `parent2face`, `parent3face`, `parent1skin`, `parent2skin`, `parent3skin`, `faceMix`, `skinMix`, `thirdMix`, `noseWidth`, `noseHeight`, `noseLength`, `noseBridge`, `noseTip`, `noseBroken`, `browHeight`, `browWidth`, `cheekboneHeight`, `cheekboneWidth`, `cheekWidth`, `eyes`, `lips`, `jawWidth`, `jawHeight`, `chinLength`, `chinPosition`, `chinWidth`, `chinShape`, `neckWidth`, `blemishId`, `blemishOpacity`, `facialhairId`, `facialhairColor`, `facialhairOpacity`, `eyebrowId`, `eyebrowColor`, `eyebrowOpacity`, `ageId`, `ageOpacity`, `makeupId`, `makeupOpacity`, `blushId`, `blushColor`, `blushOpacity`, `complexionId`, `complexionOpacity`, `sundamageId`, `sundamageOpacity`, `lipstickId`, `lipstickColor`, `lipstickOpacity`, `frecklesId`, `frecklesOpacity`, `chesthairId`, `chesthairColor`, `chesthairOpacity`, `bodyblemishId`, `bodyblemishOpacity`, `bodyblemish2Id`, `bodyblemish2Opacity`, `tattoos`) VALUES
(1, 0, 3, 84, 12, 14, 37, 27, 21, 29, 21, 12, 57, 60, 27, -62, 37, 84, 68, -10, 0, -48, -30, -65, -49, 29, -32, -100, -45, -67, -7, -8, -53, 0, 0, 255, 0, 10, 29, 0, 1, 56, 100, 255, 0, 5, 60, 2, 8, 74, 3, 0, 255, 0, 1, 7, 52, 0, 0, 255, 0, 0, 8, 0, 255, 0, NULL),
(2, 1, 0, 14, 42, 14, 34, 4, 2, 7, 18, 19, 23, 30, 21, -25, -51, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 0, 11, 2, 100, 18, 42, 100, 0, 0, 255, 100, 0, 0, 0, 3, 100, 0, 0, 0, 0, 0, 0, 0, 255, 31, 100, 0, 0, 0, 0, NULL),
(3, 0, 5, 84, 59, 60, 40, 21, 27, 29, 12, 23, 50, 20, 33, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 0, 0, 2, 57, 100, 0, 0, 5, 66, 2, 7, 100, 0, 0, 0, 0, 3, 7, 67, 0, 42, 255, 0, 0, 2, 100, 0, 0, NULL),
(4, 0, 0, 40, 24, 20, 14, 14, 0, 0, 0, 0, 50, 50, 50, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, NULL),
(5, 1, 5, 30, 61, 0, 12, 2, 30, 2, 14, 24, 61, 47, 28, 81, 43, -27, -20, -20, 0, 48, 20, -28, 42, 2, 14, -100, -55, 40, 26, 25, 41, 0, 0, 0, 0, 0, 61, 18, 21, 61, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 61, 100, 0, 0, 0, 0, NULL),
(6, 1, 0, 11, 13, 14, 1, 2, 6, 8, 17, 12, 50, 50, 50, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, NULL),
(7, 0, 0, 12, 0, 0, 0, 0, 9, 24, 25, 0, 50, 50, 50, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 36, 86, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, NULL),
(8, 1, 0, 40, 1, 0, 4, 15, 11, 14, 0, 0, 50, 9, 50, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, NULL);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `bans`
--

DROP TABLE IF EXISTS `bans`;
CREATE TABLE IF NOT EXISTS `bans` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `playerId` int(10) UNSIGNED NOT NULL,
  `adminId` int(11) NOT NULL,
  `reason` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `timestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `expires` datetime NOT NULL DEFAULT '2050-12-01 00:00:00',
  `deactivated` tinyint(1) NOT NULL DEFAULT '0',
  `deactivatedId` int(11) DEFAULT NULL,
  `deactivatedReason` int(11) DEFAULT NULL,
  `deactivatedTimestamp` int(11) DEFAULT NULL,
  `socialId` int(11) NOT NULL,
  `serial` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `PlayerAccID` (`playerId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `characters`
--

DROP TABLE IF EXISTS `characters`;
CREATE TABLE IF NOT EXISTS `characters` (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `accountId` int(10) UNSIGNED DEFAULT NULL,
  `appearanceId` int(10) UNSIGNED NOT NULL,
  `characterName` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `creationDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dob` date NOT NULL,
  `pob` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `posX` float NOT NULL DEFAULT '-1037',
  `posY` float NOT NULL DEFAULT '-2837',
  `posZ` float NOT NULL DEFAULT '21',
  `rot` float NOT NULL DEFAULT '-30',
  `dimension` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `accountId` (`accountId`),
  KEY `AppearanceID` (`appearanceId`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `characters`
--

INSERT INTO `characters` (`id`, `accountId`, `appearanceId`, `characterName`, `creationDate`, `dob`, `pob`, `posX`, `posY`, `posZ`, `rot`, `dimension`) VALUES
(7, 1, 1, 'Chy Women', '2023-08-31 21:51:19', '2023-09-04', '0', -160.593, -1137.85, 23.7144, 21.1655, 0),
(8, 2, 2, 'Emmett Velaquez', '2023-09-03 14:43:11', '0001-01-01', '0', -160.805, -1133.52, 23.6664, -23.6627, 0),
(10, 2, 4, 'Uncle Mary', '2023-09-04 22:57:04', '2023-09-04', '0', 417.861, -810.597, 28.543, -171.314, 0),
(11, 1, 5, 'Chy Gang', '2023-09-04 22:57:42', '2023-09-04', '0', -43.0844, -1095.11, 26.4224, 144.775, 0),
(12, 3, 6, 'faszos lofasz', '2023-09-20 13:12:16', '2023-09-20', '0', -1037, -2738, 21, -30, 0),
(13, 3, 7, 'szia chy', '2023-09-20 13:13:44', '2023-09-20', '0', 405.117, -813.278, 28.5772, -91.627, 0),
(14, 4, 8, ' ', '2023-10-07 19:16:24', '2023-10-07', '0', 363.018, -832.975, 29.3676, -39.4535, 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `interiors`
--

DROP TABLE IF EXISTS `interiors`;
CREATE TABLE IF NOT EXISTS `interiors` (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` varchar(50) COLLATE utf8_hungarian_ci NOT NULL,
  `postal` smallint(5) UNSIGNED NOT NULL,
  `streetName` varchar(50) COLLATE utf8_hungarian_ci NOT NULL,
  `streetNumber` smallint(5) UNSIGNED NOT NULL,
  `ownerType` int(11) NOT NULL,
  `ownerID` int(11) NOT NULL,
  `entranceX` float NOT NULL,
  `entranceY` float NOT NULL,
  `entranceZ` float NOT NULL,
  `entranceHeading` float NOT NULL,
  `entranceDimension` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `exitX` float NOT NULL,
  `exitY` float NOT NULL,
  `exitZ` float NOT NULL,
  `exitHeading` float NOT NULL,
  `exitDimension` int(10) UNSIGNED NOT NULL,
  `IPL` varchar(100) COLLATE utf8_hungarian_ci DEFAULT NULL,
  `createdBy` varchar(50) COLLATE utf8_hungarian_ci NOT NULL,
  `creationDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `interiors`
--

INSERT INTO `interiors` (`id`, `name`, `postal`, `streetName`, `streetNumber`, `ownerType`, `ownerID`, `entranceX`, `entranceY`, `entranceZ`, `entranceHeading`, `entranceDimension`, `exitX`, `exitY`, `exitZ`, `exitHeading`, `exitDimension`, `IPL`, `createdBy`, `creationDate`) VALUES
(1, 'Amarillo Vista 12.', 0, '', 0, 0, 11, 1210, -1769, 39.92, 0, 0, 172.75, -1004.55, -99, 0, 1, NULL, '', '2023-11-02 00:02:15');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `itemlist`
--

DROP TABLE IF EXISTS `itemlist`;
CREATE TABLE IF NOT EXISTS `itemlist` (
  `itemID` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `itemName` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `itemDescription` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `itemType` int(11) DEFAULT '0',
  `itemWeight` int(10) UNSIGNED NOT NULL DEFAULT '1',
  `itemImage` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `stackable` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`itemID`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `itemlist`
--

INSERT INTO `itemlist` (`itemID`, `itemName`, `itemDescription`, `itemType`, `itemWeight`, `itemImage`, `stackable`) VALUES
(1, 'Kalap', '', 1, 500, 'https://cdn-icons-png.flaticon.com/512/6375/6375825.png', 0),
(2, 'Maszk', '', 1, 500, 'https://i.gyazo.com/9d0471f237c50d646bc110cb6267acaa.png', 0),
(3, 'Nyaklánc', '', 1, 500, 'https://cdn-icons-png.flaticon.com/512/4831/4831818.png', 0),
(4, 'Szemüveg', '', 1, 500, 'https://i.gyazo.com/2cf37f74a6f54d9234ed3f0c8b394149.png', 0),
(5, 'Póló', '', 1, 500, 'https://i.gyazo.com/6c8840caf67a5f11d7ab331a19f5197e.png', 0),
(6, 'Fülbevaló', '', 1, 500, 'https://i.gyazo.com/46dd1062c9eb0f41ba05144a4dcd52fa.png', 0),
(7, 'Nadrág', '', 1, 500, 'https://i.gyazo.com/d899581160510e15138b92220b7b0c5f.png', 0),
(8, 'Karkötő', '', 1, 500, 'https://i.gyazo.com/0279b542f0b43441823ea619e4cb7a47.png', 0),
(9, 'Cipő', '', 1, 500, 'https://i.gyazo.com/357370e3e34824cb542eac1998d1b2d0.png', 0),
(10, 'Óra', '', 1, 500, 'https://i.gyazo.com/214b80dffbd9102877e6cca72a596dd6.png', 0),
(11, 'Táska', '', 1, 500, 'https://i.gyazo.com/933f909710f9ea35550f8f3925152e0f.png', 0),
(12, 'Páncél', '', 1, 500, 'https://i.gyazo.com/09f5aff199b7a7d255cf304d731695a7.png', 0),
(13, 'Kesztyű', 'Ha fázna a kezed', 1, 500, '', 0),
(14, 'Kitűző', 'Decal slot', 1, 500, '', 0),
(15, 'Járműkulcs', 'Száguldás, Porsche, szerelem...', 3, 50, 'https://i.pinimg.com/originals/79/57/bf/7957bf785455733592fa58f8730981b8.png', 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `items`
--

DROP TABLE IF EXISTS `items`;
CREATE TABLE IF NOT EXISTS `items` (
  `DbID` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT,
  `ownerID` int(10) UNSIGNED NOT NULL,
  `ownerType` int(11) NOT NULL,
  `itemID` int(10) UNSIGNED NOT NULL,
  `itemValue` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `itemAmount` int(11) NOT NULL DEFAULT '1',
  `duty` tinyint(1) NOT NULL DEFAULT '0',
  `createdBy` varchar(50) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `creationDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `priority` int(10) UNSIGNED NOT NULL,
  `inUse` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`DbID`),
  KEY `ItemID` (`itemID`),
  KEY `ItemOwnerVehicle` (`ownerID`)
) ENGINE=InnoDB AUTO_INCREMENT=46 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `items`
--

INSERT INTO `items` (`DbID`, `ownerID`, `ownerType`, `itemID`, `itemValue`, `itemAmount`, `duty`, `createdBy`, `creationDate`, `priority`, `inUse`) VALUES
(2, 7, 0, 1, '{\"Drawable\":0,\"Texture\":0}', 1, 0, 'dev', '2023-09-26 18:54:52', 2, 0),
(3, 7, 0, 2, '{\"Drawable\":11,\"Texture\":2}', 1, 0, NULL, '2023-09-26 20:29:14', 5, 0),
(4, 7, 0, 3, '{\"Drawable\":6,\"Texture\":3}', 1, 0, NULL, '2023-09-26 21:27:21', 15, 1),
(5, 7, 0, 5, '{\"UndershirtDrawable\":16,\"UndershirtTexture\":2,\"Torso\":5,\"Drawable\":8,\"Texture\":2}', 1, 0, NULL, '2023-10-21 23:26:52', 0, 0),
(6, 7, 0, 7, '{\"Drawable\":27,\"Texture\":0}', 1, 0, NULL, '2023-10-21 23:29:45', 0, 1),
(7, 7, 0, 9, '{\"Drawable\":1,\"Texture\":2}', 1, 0, NULL, '2023-10-21 23:29:59', 0, 1),
(8, 7, 0, 1, '{\"Drawable\":15,\"Texture\":0}', 5, 0, NULL, '2023-10-21 23:51:11', 0, 0),
(14, 11, 0, 5, '{\"UndershirtDrawable\":15,\"UndershirtTexture\":0,\"Torso\":1,\"Drawable\":348,\"Texture\":3}', 1, 0, '11', '2023-10-22 01:40:26', 255, 0),
(15, 11, 0, 7, '{\"Drawable\":52,\"Texture\":2}', 1, 0, '11', '2023-10-22 01:40:41', 255, 1),
(16, 11, 0, 9, '{\"Drawable\":104,\"Texture\":1}', 1, 0, '11', '2023-10-22 01:40:45', 255, 1),
(17, 11, 0, 3, '{\"Drawable\":17,\"Texture\":1}', 1, 0, '11', '2023-10-22 01:40:49', 255, 1),
(18, 11, 0, 5, '{\"UndershirtDrawable\":15,\"UndershirtTexture\":0,\"Torso\":0,\"Drawable\":273,\"Texture\":0}', 1, 0, '11', '2023-10-22 01:45:45', 255, 0),
(19, 11, 0, 5, '{\"UndershirtDrawable\":2,\"UndershirtTexture\":0,\"Torso\":19,\"Drawable\":72,\"Texture\":2}', 1, 0, '11', '2023-10-27 17:48:05', 1000, 0),
(20, 8, 0, 5, '{\"UndershirtDrawable\":15,\"UndershirtTexture\":0,\"Torso\":15,\"Drawable\":72,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:57:05', 1000, 0),
(21, 8, 0, 7, '{\"Drawable\":17,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:57:15', 1000, 0),
(22, 8, 0, 9, '{\"Drawable\":72,\"Texture\":0}', 1, 0, '8', '2023-10-27 17:57:19', 1000, 0),
(23, 8, 0, 11, '{\"Drawable\":82,\"Texture\":6}', 1, 0, '8', '2023-10-27 17:57:23', 1000, 0),
(24, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:57:27', 1000, 0),
(25, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:35', 1000, 0),
(26, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:36', 1000, 0),
(27, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:36', 1000, 0),
(28, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:37', 1000, 0),
(29, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:37', 1000, 0),
(30, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:38', 1000, 0),
(31, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:38', 1000, 0),
(32, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:39', 1000, 0),
(33, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:39', 1000, 0),
(34, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:40', 1000, 0),
(35, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:41', 1000, 1),
(36, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:41', 1000, 0),
(37, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:42', 1000, 0),
(38, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:42', 1000, 0),
(39, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:42', 1000, 0),
(40, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:43', 1000, 0),
(41, 8, 0, 12, '{\"Drawable\":16,\"Texture\":2}', 1, 0, '8', '2023-10-27 17:59:44', 1000, 0),
(42, 11, 0, 15, '3', 1, 0, '11', '2023-10-31 16:48:40', 1000, 0),
(43, 8, 0, 15, '4', 1, 0, '8', '2023-10-31 18:01:21', 1000, 0),
(44, 8, 0, 15, '5', 1, 0, '8', '2023-10-31 23:53:29', 1000, 0),
(45, 7, 0, 15, '6', 1, 0, '7', '2023-11-02 16:32:50', 1000, 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `properties`
--

DROP TABLE IF EXISTS `properties`;
CREATE TABLE IF NOT EXISTS `properties` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(150) COLLATE utf8_hungarian_ci NOT NULL,
  `category` int(11) NOT NULL,
  `posX` float NOT NULL,
  `posY` float NOT NULL,
  `posZ` float NOT NULL,
  `rotation` float NOT NULL DEFAULT '0',
  `IPL` varchar(100) COLLATE utf8_hungarian_ci DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `properties`
--

INSERT INTO `properties` (`id`, `name`, `category`, `posX`, `posY`, `posZ`, `rotation`, `IPL`) VALUES
(1, 'Kis garázs', 1, 172.75, -1004.55, -99, 0, ''),
(2, 'Közepes garázs', 1, 194.35, -1003, -99, 0, ''),
(3, 'Nagy garázs', 1, 231.9, -1002.75, -99, 0, '');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `tokens`
--

DROP TABLE IF EXISTS `tokens`;
CREATE TABLE IF NOT EXISTS `tokens` (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `accountId` int(10) UNSIGNED NOT NULL,
  `token` varchar(255) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `expiration` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `TokenAccID` (`accountId`)
) ENGINE=InnoDB AUTO_INCREMENT=527 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `tokens`
--

INSERT INTO `tokens` (`id`, `accountId`, `token`, `expiration`) VALUES
(43, 1, 'REEOpfWarAui+YClalfsjM7BV0hmWnW8z/h0TCr3AGObn2sbJMKPjHlY2BTyY0GOoyuUImJaL+6AQ+RRD2yBiQ==', '2023-09-15 18:19:49'),
(75, 1, 'v05KZkXEYzeN3AojiIeBfULUaJmXkpsd1ooM9gyKUViBORf9UgJ4t9LKgtQ2xtVudA3kaDgcjIUoDjQ672mDqw==', '2023-09-16 11:10:19'),
(77, 1, 'aGZ6U7IZI5NnzCDC7yPT0Ra6qxbyL6HS3ze79e3NeA3cYd7yVo9uPdqHBIIdQvCdURc2pFU0o2wT9K4hOIcZGw==', '2023-09-16 11:18:32'),
(78, 1, 'jH00jPaBlGOFgdxi0/JQW5ydJKy/GqA6imz47KEG+Pg4wjQOec7s/5la5VfIDLnAGMtKoeJUV4oaCXiivUQIqw==', '2023-09-16 11:40:25'),
(79, 1, '5JlYzQxAvBgt4jfov1DDqFiZuAyy8qyQT2Le2LgZn/hYuliC+7Ugwb8Z1+XUQNXC/wlu5K2c7YyIwOBUoQzJ/A==', '2023-09-16 11:44:36'),
(80, 1, 'x1ZqUOMMplNM7tr0Lahk99GMFkiZkw0E7+4O1f6ufb7mYfhuEvhFzsPmc8CJHSw/k/32E9+AT6lQZMaDMPDOJg==', '2023-09-16 11:51:17'),
(83, 1, '7AdvVdnC6A2k1Nx9AaccOht7WF/1hQMDZtfR2MM0obIhGP6bPpbhKZvi6Iv5Ige2AeEUUVZ84OWb1RXOlUTZng==', '2023-09-16 12:32:56'),
(118, 2, '2zCPepjeUy4OHfkBKKA+7w9S0QCRcq5LsXKXO4SW6RvGKiAjSqiVPiHS7Fz+Stz89O6ovETBe7jpNid1M8vC4A==', '2023-09-17 14:40:31'),
(124, 1, 'h/q91adXZrvWqahw6nTp9c9CF0lRZPBoakvUh0cRFnJETumUx5U6i8t+6y5bjVN5wJLLIMKRRHHjxphfe/jtNw==', '2023-09-18 18:02:07'),
(149, 1, 'mZSubDSoCrNGWmLn1icxqtCR9PVzVqDGqLG5Q814IF+NxODZRQtoROHCFtqSS78cMYcj29KQLVDOiLbpbKko+w==', '2023-09-18 20:21:17'),
(173, 1, 'RtGeP3zvf4oeOKXIdQEncYsq5LXmKGNRzu0eoHINzquCDGGpsdkRCHl3Bzj+q6ujXh44XOt8BSFM85JvB2iXXA==', '2023-09-19 14:41:04'),
(233, 2, '5OFzwxR6hbIezBWLZHfvKvmKw/B6AVolbr9vQddw6ywMIihb7kzh1ljlOASyUTgWiiT75gTQeuiQ0CMlB89l8A==', '2023-09-20 22:41:30'),
(235, 3, 'pDFs/sDCwGfa+/asE91WGKZfOaC5dAFTzVPzH1kvmeJqNMNCCpc5XpMgtueM/cB5bw6bh3yb3vAHmwOCqk5lsA==', '2023-10-04 13:08:58'),
(360, 1, 'kiDM0eT5StBdwpEGSwqMCFA3poVRa2Bw8sS0BEAKYpUI0mIgVNp0zJi1t1/ZuSwxRXf+UdF3tI15sA0Bvk8RSA==', '2023-10-10 20:32:30'),
(389, 1, 'iC9wU8rC+45L0VNDNjmn3y4ZjJ3y65IJhYJ/vedETdxHWAFMCRMjV7evEe6P3U+DQ5Yy/iEtB05tLU+m4Oa1QA==', '2023-10-21 20:07:37'),
(398, 1, 'WHnc8dSC6+rigoR9pfE+Z2a8XF5vDAl38QP0d2GhKpZbQMxddIJWyQOUXsvLofKgjGFaxKTSFQWRjPNAm3imbg==', '2023-10-31 18:03:28'),
(415, 1, 'Yl0Bte/iadAdYaqwpl4LViGHOcCVDYJEjPUDZoqM4fW7sah+N8T/yVrLn3DRhJO0utdslR9agOj0hZozgzeFcQ==', '2023-11-04 01:28:14'),
(423, 1, 'YoJaww/CiyfP8ft+HupqfI0OsFYIHVNSSXy/7XQdyqVPyPTbPh6ihbE3q2wjoTiqEO5HM/+Q54QE7mhhwzCwLw==', '2023-11-04 02:09:53'),
(442, 1, 'DWa7THoSnsjwk2lUjnOJfGEHv7E/QONRL9SnjyxmUU/y3dwY0cNJPp2n+dExx92uOnazXGQmoxsmsFU7cNs6vg==', '2023-11-04 18:18:08'),
(516, 1, '4FNw21bgn1RSr40pY2jXBfl+8Lvkv0uxf7E5cYHgoHExLM0kNOUfjsHKE9pqnWiTgS9fTJwzRX8fEgYhyNqUEA==', '2023-11-06 18:01:59'),
(517, 1, '/uC8kmaMWfZyfh/ikvaZOfzKa6cneCES6RRJjDstuUd3XyAOTuBHkvFYC43FEb/ZrKFVxMSu+9jmmSJVPl0YCw==', '2023-11-10 15:17:43'),
(518, 1, 'wjjIY4h17ZWo0zH3EBm+AmavgiUAm2YKPaOIbz89D/aNVyFkdcSp2PPvJ1H0A7Oe/SEWFWrU/EMvso087uvxmA==', '2023-11-10 17:25:44'),
(519, 1, 'IjuMG7dbGoKXBc1Nu8zDWMBRCQD33A6FV/PveEYhxKehew1O4v+t8xFVHgOCk39QWsZT3mSaZNcTpZIX0x8Kiw==', '2023-11-10 17:30:19'),
(520, 2, 'WHoCxVsjwA0NgkxW1PTHbNMVv2p2TruUuU6j2b/xTXDmIDJilgxhQeT/xITifUBgrFqY6njUZLvuK+tXQ9wfKg==', '2023-11-10 17:36:00'),
(521, 1, 'jFZJOBvqnF0SF30QJ3OXq9Ty54UJoMgSZOJ60j9q7PnpdWOuOp7+cJUBIEIhjf/0Q3chJ0s/aGbxoTFUMGiVgw==', '2023-11-10 17:38:30'),
(522, 1, 'yUk/HnrnHgJrijW0AZ+/NHVbvMJbFbpLtYiOcGpPjddpvHWbCrPpJuI3ovO3t0sJNQbMTcKKUsKmLbEqW5Rp2Q==', '2023-11-10 17:46:04'),
(523, 2, '//GoJ+sn2hQX3U4YOVKj0MlbEIkAg4kJ8w4l2lXBtYeCsS/x0kXklZI+njDHt6iOT54DL9xfLvF5UbWDU7ltDg==', '2023-11-10 17:56:38'),
(524, 2, 'l348Fa+DB8Y1SFwcHQIU0e7+PgE7d1DZt3zKpX7tkfSTQfwGJtrpXijOMRi5VHQr3Aqg3ebfSFVnD+q2E6GVGw==', '2023-11-14 22:37:11'),
(525, 2, 'KGC0kfd7niNC9PT9RoUN3iIyqk/ObN8Q7Qy5iQ7RAui94WuAUA0PG554JU6VVJxgXZJmHGjSgw1m6orM7s/oeg==', '2023-11-15 00:00:42'),
(526, 2, 'XyrCZBPsVz68/6Oawp7YdH8OosGREE8r6ip+5BYyEcYhz3esqrVLfyItCtcgc/fIc9kL9iAgjEO3/XHHi6WeHg==', '2023-11-16 15:51:03');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `vehicles`
--

DROP TABLE IF EXISTS `vehicles`;
CREATE TABLE IF NOT EXISTS `vehicles` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ownerType` tinyint(4) NOT NULL,
  `ownerID` int(11) NOT NULL,
  `model` varchar(30) COLLATE utf8_hungarian_ci NOT NULL,
  `posX` float NOT NULL,
  `posY` float NOT NULL,
  `posZ` float NOT NULL,
  `rotX` float NOT NULL,
  `rotY` float NOT NULL,
  `rotZ` float NOT NULL,
  `red1` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `green1` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `blue1` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `red2` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `green2` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `blue2` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `pearlescent` tinyint(4) NOT NULL DEFAULT '0',
  `locked` tinyint(1) NOT NULL DEFAULT '0',
  `engine` tinyint(1) NOT NULL DEFAULT '0',
  `numberPlateText` varchar(8) COLLATE utf8_hungarian_ci NOT NULL,
  `numberPlateType` tinyint(4) NOT NULL,
  `dimension` int(11) NOT NULL DEFAULT '0',
  `createdBy` varchar(25) COLLATE utf8_hungarian_ci NOT NULL,
  `creationDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `vehicles`
--

INSERT INTO `vehicles` (`id`, `ownerType`, `ownerID`, `model`, `posX`, `posY`, `posZ`, `rotX`, `rotY`, `rotZ`, `red1`, `green1`, `blue1`, `red2`, `green2`, `blue2`, `pearlescent`, `locked`, `engine`, `numberPlateText`, `numberPlateType`, `dimension`, `createdBy`, `creationDate`) VALUES
(1, 0, 11, 'elegy', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 'SZEP', 0, 0, 'Chy', '2023-10-31 16:19:28'),
(2, 0, 11, 'yosemite2', -1395.74, 5101.09, 61.1378, 0, 0, 0, 255, 255, 255, 255, 255, 255, 0, 0, 0, 'VEGLEGES', 0, 0, 'makeveh', '2023-10-31 16:34:17'),
(3, 0, 11, 'tailgater2', -1395.35, 5100.4, 61.1399, 0, 0, 0, 255, 255, 255, 255, 255, 255, 0, 0, 0, 'Chy', 0, 0, 'Chy Gang', '2023-10-31 16:48:40'),
(4, 0, 8, 'growler', -1406.12, 5078.62, 61.1033, 0, 0, 0, 69, 82, 75, 69, 82, 75, 0, 0, 0, 'tesztbat', 0, 0, 'Big Bob', '2023-10-31 18:01:20'),
(5, 0, 8, 'openwheel1', 2435.07, 5650.29, 45.0789, 0, 0, 0, 255, 255, 255, 255, 255, 255, 0, 0, 0, '', 0, 0, 'Big Bob', '2023-10-31 23:53:28'),
(6, 0, 7, 'sentineldm', -81.8346, -1093.38, 26.3948, 0, 0, 0, 116, 181, 223, 116, 181, 223, 70, 0, 0, 'DRIFTER', 0, 0, 'Chy Women', '2023-11-02 16:32:49');

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `bans`
--
ALTER TABLE `bans`
  ADD CONSTRAINT `PlayerAccID` FOREIGN KEY (`playerId`) REFERENCES `accounts` (`id`);

--
-- Megkötések a táblához `characters`
--
ALTER TABLE `characters`
  ADD CONSTRAINT `AccID` FOREIGN KEY (`accountId`) REFERENCES `accounts` (`id`),
  ADD CONSTRAINT `AppearanceID` FOREIGN KEY (`appearanceId`) REFERENCES `appearances` (`id`);

--
-- Megkötések a táblához `items`
--
ALTER TABLE `items`
  ADD CONSTRAINT `ItemID` FOREIGN KEY (`itemID`) REFERENCES `itemlist` (`itemID`);

--
-- Megkötések a táblához `tokens`
--
ALTER TABLE `tokens`
  ADD CONSTRAINT `TokenAccID` FOREIGN KEY (`accountId`) REFERENCES `accounts` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
