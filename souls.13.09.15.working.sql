/*
Navicat MySQL Data Transfer

Source Server         : Rotor
Source Server Version : 50625
Source Host           : rotor.hgsd.persoft.lan:3306
Source Database       : souls

Target Server Type    : MYSQL
Target Server Version : 50625
File Encoding         : 65001

Date: 2015-09-13 09:15:08
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for ability
-- ----------------------------
DROP TABLE IF EXISTS `ability`;
CREATE TABLE `ability` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `parameter` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ability
-- ----------------------------
INSERT INTO `ability` VALUES ('1', 'Taunt', '0');
INSERT INTO `ability` VALUES ('2', 'Heal', null);
INSERT INTO `ability` VALUES ('3', 'Summon', null);
INSERT INTO `ability` VALUES ('4', 'Paralyze', null);
INSERT INTO `ability` VALUES ('5', 'Haunt', null);
INSERT INTO `ability` VALUES ('6', 'Harden', null);
INSERT INTO `ability` VALUES ('7', 'Burn', null);

-- ----------------------------
-- Table structure for bot_names
-- ----------------------------
DROP TABLE IF EXISTS `bot_names`;
CREATE TABLE `bot_names` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of bot_names
-- ----------------------------
INSERT INTO `bot_names` VALUES ('1', 'Hansel');
INSERT INTO `bot_names` VALUES ('2', 'Satan');
INSERT INTO `bot_names` VALUES ('3', 'Death');

-- ----------------------------
-- Table structure for card
-- ----------------------------
DROP TABLE IF EXISTS `card`;
CREATE TABLE `card` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `attack` int(11) NOT NULL,
  `health` int(11) NOT NULL,
  `armor` int(11) NOT NULL,
  `cost` int(11) NOT NULL,
  `fk_ability` int(11) DEFAULT NULL,
  `fk_race` int(11) DEFAULT NULL,
  `vendor_price` int(11) DEFAULT NULL,
  `level` int(11) DEFAULT NULL,
  `portrait` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_ability` (`fk_ability`),
  KEY `fk_race` (`fk_race`),
  KEY `fk_ability_2` (`fk_ability`),
  KEY `fk_race_2` (`fk_race`),
  CONSTRAINT `FK2D6D419451402FEA` FOREIGN KEY (`fk_ability`) REFERENCES `ability` (`id`),
  CONSTRAINT `FK2D6D4194571B805A` FOREIGN KEY (`fk_race`) REFERENCES `race` (`id`),
  CONSTRAINT `FK91F019CD2490377B` FOREIGN KEY (`fk_ability`) REFERENCES `ability` (`id`),
  CONSTRAINT `FK91F019CDC99EF538` FOREIGN KEY (`fk_race`) REFERENCES `race` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of card
-- ----------------------------
INSERT INTO `card` VALUES ('1', 'Rhino', '5', '25', '3', '5', '1', '2', '1', '1', '/Content/Images/Card/Portraits/1.png');
INSERT INTO `card` VALUES ('2', 'Zebra', '1', '1', '1', '2', '2', '3', '1', '2', '/Content/Images/Card/Portraits/2.png');
INSERT INTO `card` VALUES ('3', 'Per', '6', '4', '1', '4', '3', '4', '1', '1', '/Content/Images/Card/Portraits/3.png');
INSERT INTO `card` VALUES ('4', 'Christine', '5', '5', '2', '4', '4', '1', '1', '3', '/Content/Images/Card/Portraits/4.png');
INSERT INTO `card` VALUES ('5', 'Anette', '-5', '1', '20', '5', '5', '4', '1', '2', '/Content/Images/Card/Portraits/5.png');
INSERT INTO `card` VALUES ('6', 'Bobby', '2', '-5', '2', '1', '1', '2', '1', '1', '/Content/Images/Card/Portraits/6.png');

-- ----------------------------
-- Table structure for game
-- ----------------------------
DROP TABLE IF EXISTS `game`;
CREATE TABLE `game` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `fk_player1` int(11) NOT NULL,
  `fk_player2` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_player1` (`fk_player1`),
  KEY `faf` (`fk_player2`),
  KEY `fk_player2` (`fk_player2`),
  KEY `fk_player1_2` (`fk_player1`),
  KEY `fk_player2_2` (`fk_player2`),
  CONSTRAINT `FK3BD1C72B6B04444` FOREIGN KEY (`fk_player1`) REFERENCES `player` (`id`),
  CONSTRAINT `FK3BD1C72B6B14444` FOREIGN KEY (`fk_player2`) REFERENCES `player` (`id`),
  CONSTRAINT `FK8AC5CC742A58B73E` FOREIGN KEY (`fk_player2`) REFERENCES `player` (`id`),
  CONSTRAINT `FK8AC5CC743252150F` FOREIGN KEY (`fk_player1`) REFERENCES `player` (`id`),
  CONSTRAINT `faf` FOREIGN KEY (`fk_player2`) REFERENCES `player` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of game
-- ----------------------------
INSERT INTO `game` VALUES ('1', '2', '5');
INSERT INTO `game` VALUES ('2', '5', '1');
INSERT INTO `game` VALUES ('3', '3', '6');
INSERT INTO `game` VALUES ('4', '14', '16');

-- ----------------------------
-- Table structure for game_log
-- ----------------------------
DROP TABLE IF EXISTS `game_log`;
CREATE TABLE `game_log` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `fk_log_type` int(11) DEFAULT NULL,
  `fk_game` int(11) DEFAULT NULL,
  `obj1id` int(11) DEFAULT NULL,
  `obj2id` int(11) DEFAULT NULL,
  `obj1Type` varchar(255) DEFAULT NULL,
  `obj2Type` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_log_type` (`fk_log_type`),
  KEY `fk_game` (`fk_game`),
  KEY `fk_log_type_2` (`fk_log_type`),
  KEY `fk_game_2` (`fk_game`),
  CONSTRAINT `FK4E7CA9B41B352EE2` FOREIGN KEY (`fk_log_type`) REFERENCES `game_log_type` (`id`),
  CONSTRAINT `FK4E7CA9B461CD318D` FOREIGN KEY (`fk_game`) REFERENCES `game` (`id`),
  CONSTRAINT `FKE80842912F627A4E` FOREIGN KEY (`fk_game`) REFERENCES `game` (`id`),
  CONSTRAINT `FKE8084291C6703BC7` FOREIGN KEY (`fk_log_type`) REFERENCES `game_log_type` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=97 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of game_log
-- ----------------------------
INSERT INTO `game_log` VALUES ('1', '1', '1', null, null, null, null);
INSERT INTO `game_log` VALUES ('2', '1', '2', null, null, null, null);
INSERT INTO `game_log` VALUES ('3', '1', '3', null, null, null, null);
INSERT INTO `game_log` VALUES ('4', '2', '2', null, null, null, null);
INSERT INTO `game_log` VALUES ('5', null, null, null, null, null, null);
INSERT INTO `game_log` VALUES ('6', '8', '4', '14', '16', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('7', '9', '4', '14', '6', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('8', '10', '4', '14', '16', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('9', '11', '4', '16', '5', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('10', '9', '4', '16', '6', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('11', '5', '4', '6', '6', 'Card', 'Card');
INSERT INTO `game_log` VALUES ('12', '10', '4', '16', '14', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('13', '11', '4', '14', '5', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('14', '10', '4', '14', '16', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('15', '11', '4', '16', '3', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('16', '10', '4', '16', '14', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('17', '11', '4', '14', '2', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('18', '10', '4', '14', '16', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('19', '11', '4', '16', '1', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('20', '10', '4', '16', '14', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('21', '11', '4', '14', '5', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('22', '9', '4', '14', '2', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('23', '4', '4', '2', '16', 'Card', 'Player');
INSERT INTO `game_log` VALUES ('24', '10', '4', '14', '16', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('25', '11', '4', '16', '2', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('26', '9', '4', '16', '3', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('27', '10', '4', '16', '14', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('28', '11', '4', '14', '2', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('29', '9', '4', '14', '5', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('30', '5', '4', '5', '3', 'Card', 'Card');
INSERT INTO `game_log` VALUES ('31', '10', '4', '14', '16', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('32', '11', '4', '16', '4', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('33', '9', '4', '16', '5', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('34', '4', '4', '5', '14', 'Card', 'Player');
INSERT INTO `game_log` VALUES ('35', '4', '4', '3', '14', 'Card', 'Player');
INSERT INTO `game_log` VALUES ('36', '10', '4', '16', '14', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('37', '11', '4', '14', '2', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('38', '9', '4', '14', '2', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('39', '9', '4', '14', '2', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('40', '5', '4', '2', '3', 'Card', 'Card');
INSERT INTO `game_log` VALUES ('41', '14', '4', '2', '14', 'Card', 'Player');
INSERT INTO `game_log` VALUES ('42', '10', '4', '14', '16', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('43', '11', '4', '16', '2', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('44', '9', '4', '16', '4', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('45', '9', '4', '16', '2', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('46', '4', '4', '4', '14', 'Card', 'Player');
INSERT INTO `game_log` VALUES ('47', '4', '4', '3', '14', 'Card', 'Player');
INSERT INTO `game_log` VALUES ('48', '4', '4', '2', '14', 'Card', 'Player');
INSERT INTO `game_log` VALUES ('49', '10', '4', '16', '14', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('50', '11', '4', '14', '4', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('51', '9', '4', '14', '5', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('52', '4', '4', '5', '16', 'Card', 'Player');
INSERT INTO `game_log` VALUES ('53', '10', '4', '14', '16', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('54', '11', '4', '16', '1', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('55', '9', '4', '16', '1', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('56', '9', '4', '16', '2', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('57', '4', '4', '2', '14', 'Card', 'Player');
INSERT INTO `game_log` VALUES ('58', '10', '4', '16', '14', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('59', '11', '4', '14', '5', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('60', '9', '4', '14', '5', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('61', '5', '4', '5', '1', 'Card', 'Card');
INSERT INTO `game_log` VALUES ('62', '10', '4', '14', '16', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('63', '11', '4', '16', '6', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('64', '9', '4', '16', '1', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('65', '9', '4', '16', '6', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('66', '4', '4', '1', '14', 'Card', 'Player');
INSERT INTO `game_log` VALUES ('67', '10', '4', '16', '14', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('68', '11', '4', '14', '4', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('69', '9', '4', '14', '4', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('70', '5', '4', '4', '1', 'Card', 'Card');
INSERT INTO `game_log` VALUES ('71', '10', '4', '14', '16', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('72', '11', '4', '16', '1', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('73', '9', '4', '16', '1', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('74', '9', '4', '16', '4', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('75', '10', '4', '16', '14', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('76', '11', '4', '14', '5', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('77', '10', '4', '14', '16', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('78', '11', '4', '16', '1', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('79', '9', '4', '16', '1', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('80', '9', '4', '16', '1', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('81', '10', '4', '16', '14', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('82', '11', '4', '14', '5', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('83', '10', '4', '14', '16', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('84', '11', '4', '16', '3', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('85', '10', '4', '16', '14', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('86', '11', '4', '14', '5', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('87', '9', '4', '14', '5', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('88', '9', '4', '14', '5', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('89', '10', '4', '14', '16', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('90', '11', '4', '16', '1', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('91', '5', '4', '1', '5', 'Card', 'Card');
INSERT INTO `game_log` VALUES ('92', '5', '4', '6', '5', 'Card', 'Card');
INSERT INTO `game_log` VALUES ('93', '10', '4', '16', '14', 'Player', 'Player');
INSERT INTO `game_log` VALUES ('94', '11', '4', '14', '4', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('95', '6', '4', '14', '16', 'Player', 'Card');
INSERT INTO `game_log` VALUES ('96', '1', '4', '16', '14', 'Player', 'Player');

-- ----------------------------
-- Table structure for game_log_type
-- ----------------------------
DROP TABLE IF EXISTS `game_log_type`;
CREATE TABLE `game_log_type` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL,
  `text` text,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of game_log_type
-- ----------------------------
INSERT INTO `game_log_type` VALUES ('1', 'WON', 'Player Won', '{0} wins against {1}');
INSERT INTO `game_log_type` VALUES ('2', 'DEFEAT', 'Player defeat', '{0} loses against {1}');
INSERT INTO `game_log_type` VALUES ('3', 'DRAW', 'Draw between players', '{0} plays a draw against {1}');
INSERT INTO `game_log_type` VALUES ('4', 'CARD_ATTACK_PLAYER', 'A card attacks a player (opponent)', '{0} attacks {1} for {2} damage.');
INSERT INTO `game_log_type` VALUES ('5', 'CARD_ATTACK_CARD', 'A card attacks a card ', '{0} attacks {1} for {2} damage.');
INSERT INTO `game_log_type` VALUES ('6', 'PLAYER_ATTACK_PLAYER', 'A player attack the opponent', '{0} attacks {1} for {2} damage.');
INSERT INTO `game_log_type` VALUES ('7', 'PLAYER_ATTACK_CARD', 'A player attack a card', '{0} attacks {1} for {2} damage.');
INSERT INTO `game_log_type` VALUES ('8', 'GAME_CREATED', 'A game was created where X player begins', 'The battle begins, {0} starts the game against {1}');
INSERT INTO `game_log_type` VALUES ('9', 'USE_CARD', 'A player uses a card', '{0} uses {1}');
INSERT INTO `game_log_type` VALUES ('10', 'NEXT_TURN', 'a player initiates next turn', '{0} gives turn to {1}');
INSERT INTO `game_log_type` VALUES ('11', 'NEW_CARD', 'a player draws a new card', '{0} draws a card. The card was {1}');
INSERT INTO `game_log_type` VALUES ('12', 'KICKED', 'a player is kicked', '{0} was kicked from the game by [ADMIN]');
INSERT INTO `game_log_type` VALUES ('13', 'ABILITY_HEAL', 'A card heals X', '{0} heals {1} for {2} hp');
INSERT INTO `game_log_type` VALUES ('14', 'ABILITY_SACRIFICE', 'A card sacrifices itself to X', '{0} sacrifices itself, strengthening {1} for {2}.');

-- ----------------------------
-- Table structure for news
-- ----------------------------
DROP TABLE IF EXISTS `news`;
CREATE TABLE `news` (
  `id` int(11) NOT NULL,
  `title` varchar(255) DEFAULT NULL,
  `text` varchar(255) DEFAULT NULL,
  `author` varchar(255) DEFAULT NULL,
  `date` datetime DEFAULT NULL,
  `enabled` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of news
-- ----------------------------

-- ----------------------------
-- Table structure for player
-- ----------------------------
DROP TABLE IF EXISTS `player`;
CREATE TABLE `player` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `rank` int(11) NOT NULL,
  `created` datetime DEFAULT NULL,
  `fk_type` int(11) DEFAULT NULL,
  `money` int(11) DEFAULT NULL,
  `fk_permission` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`),
  UNIQUE KEY `name_2` (`name`),
  KEY `fk_type` (`fk_type`),
  KEY `fk_permission` (`fk_permission`),
  KEY `fk_type_2` (`fk_type`),
  KEY `fk_permission_2` (`fk_permission`),
  CONSTRAINT `FK6FCEC21B79D6391` FOREIGN KEY (`fk_permission`) REFERENCES `player_permission` (`id`),
  CONSTRAINT `FK6FCEC21B8C0CC74F` FOREIGN KEY (`fk_type`) REFERENCES `player_type` (`id`),
  CONSTRAINT `FK86D0A9E6A67583A0` FOREIGN KEY (`fk_type`) REFERENCES `player_type` (`id`),
  CONSTRAINT `FK86D0A9E6B2A0EE2E` FOREIGN KEY (`fk_permission`) REFERENCES `player_permission` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of player
-- ----------------------------
INSERT INTO `player` VALUES ('1', 'p1', 'b427c3dec9045990213ccc3dbffbdf189e75c7ce18e1a86ed7c4fe9a2db5d627', '1', '2014-03-26 21:09:21', '1', '0', '2');
INSERT INTO `player` VALUES ('2', 'p2', 'b427c3dec9045990213ccc3dbffbdf189e75c7ce18e1a86ed7c4fe9a2db5d627', '2', '2014-03-26 23:04:45', '2', '0', '2');
INSERT INTO `player` VALUES ('3', 'p3', 'b427c3dec9045990213ccc3dbffbdf189e75c7ce18e1a86ed7c4fe9a2db5d627', '3', '2014-03-27 23:04:56', '3', '0', '2');
INSERT INTO `player` VALUES ('4', 'p4', 'b427c3dec9045990213ccc3dbffbdf189e75c7ce18e1a86ed7c4fe9a2db5d627', '4', '2014-03-18 23:05:07', '4', null, '2');
INSERT INTO `player` VALUES ('5', 'p5', 'b427c3dec9045990213ccc3dbffbdf189e75c7ce18e1a86ed7c4fe9a2db5d627', '5', '2014-02-27 23:05:21', '4', null, '2');
INSERT INTO `player` VALUES ('6', 'p6', 'b427c3dec9045990213ccc3dbffbdf189e75c7ce18e1a86ed7c4fe9a2db5d627', '6', '2014-03-27 23:05:37', '3', null, '2');
INSERT INTO `player` VALUES ('7', 'p7', 'b427c3dec9045990213ccc3dbffbdf189e75c7ce18e1a86ed7c4fe9a2db5d627', '7', '2014-03-12 23:06:01', '2', null, '2');
INSERT INTO `player` VALUES ('8', 'p8', 'b427c3dec9045990213ccc3dbffbdf189e75c7ce18e1a86ed7c4fe9a2db5d627', '1', '2014-03-12 23:06:15', '1', null, '2');
INSERT INTO `player` VALUES ('9', 'p9', 'b427c3dec9045990213ccc3dbffbdf189e75c7ce18e1a86ed7c4fe9a2db5d627', '2', '2014-02-08 23:06:26', '2', null, '2');
INSERT INTO `player` VALUES ('10', 'p10', 'b427c3dec9045990213ccc3dbffbdf189e75c7ce18e1a86ed7c4fe9a2db5d627', '3', '2014-03-11 23:06:38', '3', null, '2');
INSERT INTO `player` VALUES ('11', 'p11', 'b427c3dec9045990213ccc3dbffbdf189e75c7ce18e1a86ed7c4fe9a2db5d627', '4', '2014-04-02 23:07:00', '4', null, '2');
INSERT INTO `player` VALUES ('13', 'p12', 'b427c3dec9045990213ccc3dbffbdf189e75c7ce18e1a86ed7c4fe9a2db5d627', '5', '2014-03-20 23:07:15', '2', null, '2');
INSERT INTO `player` VALUES ('14', 'per', 'b427c3dec9045990213ccc3dbffbdf189e75c7ce18e1a86ed7c4fe9a2db5d627', '4', '2014-03-18 23:07:36', '1', '0', '1');
INSERT INTO `player` VALUES ('15', 'keel', 'b427c3dec9045990213ccc3dbffbdf189e75c7ce18e1a86ed7c4fe9a2db5d627', '5', '2014-03-25 23:07:52', '2', null, '1');
INSERT INTO `player` VALUES ('16', 'BOT', 'nopass', '5', '2014-03-25 23:07:52', '3', '1338', '1');

-- ----------------------------
-- Table structure for player_bans
-- ----------------------------
DROP TABLE IF EXISTS `player_bans`;
CREATE TABLE `player_bans` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `until` datetime DEFAULT NULL,
  `fk_user` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_user` (`fk_user`),
  KEY `fk_user_2` (`fk_user`),
  CONSTRAINT `FK296E52DE8FDE4699` FOREIGN KEY (`fk_user`) REFERENCES `player` (`id`),
  CONSTRAINT `FK70B86E621F9813EE` FOREIGN KEY (`fk_user`) REFERENCES `player` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of player_bans
-- ----------------------------

-- ----------------------------
-- Table structure for player_cards
-- ----------------------------
DROP TABLE IF EXISTS `player_cards`;
CREATE TABLE `player_cards` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `obtainedAt` varchar(255) NOT NULL,
  `fk_player` int(11) DEFAULT NULL,
  `fk_card` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_player` (`fk_player`),
  KEY `fk_card` (`fk_card`),
  KEY `fk_player_2` (`fk_player`),
  KEY `fk_card_2` (`fk_card`),
  CONSTRAINT `FK8FE54C2B6113B700` FOREIGN KEY (`fk_player`) REFERENCES `player` (`id`),
  CONSTRAINT `FK8FE54C2BAA2660A0` FOREIGN KEY (`fk_card`) REFERENCES `card` (`id`),
  CONSTRAINT `FKE416AB5D9C6F6967` FOREIGN KEY (`fk_card`) REFERENCES `card` (`id`),
  CONSTRAINT `FKE416AB5DB6A74444` FOREIGN KEY (`fk_player`) REFERENCES `player` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=95 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of player_cards
-- ----------------------------
INSERT INTO `player_cards` VALUES ('1', '201509130832403440', '1', '1');
INSERT INTO `player_cards` VALUES ('2', '201509130832403440', '1', '2');
INSERT INTO `player_cards` VALUES ('3', '201509130832403440', '1', '3');
INSERT INTO `player_cards` VALUES ('4', '201509130832403440', '1', '4');
INSERT INTO `player_cards` VALUES ('5', '201509130832403440', '1', '5');
INSERT INTO `player_cards` VALUES ('6', '201509130832403440', '1', '6');
INSERT INTO `player_cards` VALUES ('11', '201509130832403440', '2', '1');
INSERT INTO `player_cards` VALUES ('12', '201509130832403440', '2', '2');
INSERT INTO `player_cards` VALUES ('13', '201509130832403440', '2', '3');
INSERT INTO `player_cards` VALUES ('14', '201509130832403440', '2', '4');
INSERT INTO `player_cards` VALUES ('15', '201509130832403440', '2', '5');
INSERT INTO `player_cards` VALUES ('16', '201509130832403440', '2', '6');
INSERT INTO `player_cards` VALUES ('17', '201509130832403440', '3', '1');
INSERT INTO `player_cards` VALUES ('18', '201509130832403440', '3', '2');
INSERT INTO `player_cards` VALUES ('19', '201509130832403440', '3', '3');
INSERT INTO `player_cards` VALUES ('20', '201509130832403440', '3', '4');
INSERT INTO `player_cards` VALUES ('21', '201509130832403440', '3', '5');
INSERT INTO `player_cards` VALUES ('22', '201509130832403440', '3', '6');
INSERT INTO `player_cards` VALUES ('23', '201509130832403440', '4', '1');
INSERT INTO `player_cards` VALUES ('24', '201509130832403440', '4', '2');
INSERT INTO `player_cards` VALUES ('25', '201509130832403440', '4', '3');
INSERT INTO `player_cards` VALUES ('26', '201509130832403440', '4', '4');
INSERT INTO `player_cards` VALUES ('27', '201509130832403440', '4', '5');
INSERT INTO `player_cards` VALUES ('28', '201509130832403440', '4', '6');
INSERT INTO `player_cards` VALUES ('29', '201509130832403440', '5', '1');
INSERT INTO `player_cards` VALUES ('30', '201509130832403440', '5', '2');
INSERT INTO `player_cards` VALUES ('31', '201509130832403440', '5', '3');
INSERT INTO `player_cards` VALUES ('32', '201509130832403440', '5', '4');
INSERT INTO `player_cards` VALUES ('33', '201509130832403440', '5', '5');
INSERT INTO `player_cards` VALUES ('34', '201509130832403440', '5', '6');
INSERT INTO `player_cards` VALUES ('35', '201509130832403440', '6', '1');
INSERT INTO `player_cards` VALUES ('36', '201509130832403440', '6', '2');
INSERT INTO `player_cards` VALUES ('37', '201509130832403440', '6', '3');
INSERT INTO `player_cards` VALUES ('38', '201509130832403440', '6', '4');
INSERT INTO `player_cards` VALUES ('39', '201509130832403440', '6', '5');
INSERT INTO `player_cards` VALUES ('40', '201509130832403440', '6', '6');
INSERT INTO `player_cards` VALUES ('41', '201509130832403440', '7', '1');
INSERT INTO `player_cards` VALUES ('42', '201509130832403440', '7', '2');
INSERT INTO `player_cards` VALUES ('43', '201509130832403440', '7', '3');
INSERT INTO `player_cards` VALUES ('44', '201509130832403440', '7', '4');
INSERT INTO `player_cards` VALUES ('45', '201509130832403440', '7', '5');
INSERT INTO `player_cards` VALUES ('46', '201509130832403440', '7', '6');
INSERT INTO `player_cards` VALUES ('47', '201509130832403440', '8', '1');
INSERT INTO `player_cards` VALUES ('48', '201509130832403440', '8', '2');
INSERT INTO `player_cards` VALUES ('49', '201509130832403440', '8', '3');
INSERT INTO `player_cards` VALUES ('50', '201509130832403440', '8', '4');
INSERT INTO `player_cards` VALUES ('51', '201509130832403440', '8', '5');
INSERT INTO `player_cards` VALUES ('52', '201509130832403440', '8', '6');
INSERT INTO `player_cards` VALUES ('53', '201509130832403440', '9', '1');
INSERT INTO `player_cards` VALUES ('54', '201509130832403440', '9', '2');
INSERT INTO `player_cards` VALUES ('55', '201509130832403440', '9', '3');
INSERT INTO `player_cards` VALUES ('56', '201509130832403440', '9', '4');
INSERT INTO `player_cards` VALUES ('57', '201509130832403440', '9', '5');
INSERT INTO `player_cards` VALUES ('58', '201509130832403440', '9', '6');
INSERT INTO `player_cards` VALUES ('59', '201509130832403440', '10', '1');
INSERT INTO `player_cards` VALUES ('60', '201509130832403440', '10', '2');
INSERT INTO `player_cards` VALUES ('61', '201509130832403440', '10', '3');
INSERT INTO `player_cards` VALUES ('62', '201509130832403440', '10', '4');
INSERT INTO `player_cards` VALUES ('63', '201509130832403440', '10', '5');
INSERT INTO `player_cards` VALUES ('64', '201509130832403440', '10', '6');
INSERT INTO `player_cards` VALUES ('65', '201509130832403440', '11', '1');
INSERT INTO `player_cards` VALUES ('66', '201509130832403440', '11', '2');
INSERT INTO `player_cards` VALUES ('67', '201509130832403440', '11', '3');
INSERT INTO `player_cards` VALUES ('68', '201509130832403440', '11', '4');
INSERT INTO `player_cards` VALUES ('69', '201509130832403440', '11', '5');
INSERT INTO `player_cards` VALUES ('70', '201509130832403440', '11', '6');
INSERT INTO `player_cards` VALUES ('71', '201509130832403440', '13', '1');
INSERT INTO `player_cards` VALUES ('72', '201509130832403440', '13', '2');
INSERT INTO `player_cards` VALUES ('73', '201509130832403440', '13', '3');
INSERT INTO `player_cards` VALUES ('74', '201509130832403440', '13', '4');
INSERT INTO `player_cards` VALUES ('75', '201509130832403440', '13', '5');
INSERT INTO `player_cards` VALUES ('76', '201509130832403440', '13', '6');
INSERT INTO `player_cards` VALUES ('77', '201509130832403440', '14', '1');
INSERT INTO `player_cards` VALUES ('78', '201509130832403440', '14', '2');
INSERT INTO `player_cards` VALUES ('79', '201509130832403440', '14', '3');
INSERT INTO `player_cards` VALUES ('80', '201509130832403440', '14', '4');
INSERT INTO `player_cards` VALUES ('81', '201509130832403440', '14', '5');
INSERT INTO `player_cards` VALUES ('82', '201509130832403440', '14', '6');
INSERT INTO `player_cards` VALUES ('83', '201509130832403440', '15', '1');
INSERT INTO `player_cards` VALUES ('84', '201509130832403440', '15', '2');
INSERT INTO `player_cards` VALUES ('85', '201509130832403440', '15', '3');
INSERT INTO `player_cards` VALUES ('86', '201509130832403440', '15', '4');
INSERT INTO `player_cards` VALUES ('87', '201509130832403440', '15', '5');
INSERT INTO `player_cards` VALUES ('88', '201509130832403440', '15', '6');
INSERT INTO `player_cards` VALUES ('89', '201509130832403440', '16', '1');
INSERT INTO `player_cards` VALUES ('90', '201509130832403440', '16', '2');
INSERT INTO `player_cards` VALUES ('91', '201509130832403440', '16', '3');
INSERT INTO `player_cards` VALUES ('92', '201509130832403440', '16', '4');
INSERT INTO `player_cards` VALUES ('93', '201509130832403440', '16', '5');
INSERT INTO `player_cards` VALUES ('94', '201509130832403440', '16', '6');

-- ----------------------------
-- Table structure for player_login
-- ----------------------------
DROP TABLE IF EXISTS `player_login`;
CREATE TABLE `player_login` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `hash` varchar(255) DEFAULT NULL,
  `timestamp` bigint(20) DEFAULT NULL,
  `fk_player_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_player_id` (`fk_player_id`),
  KEY `fk_player_id_2` (`fk_player_id`),
  CONSTRAINT `FK6CB8740BA256EAEA` FOREIGN KEY (`fk_player_id`) REFERENCES `player` (`id`),
  CONSTRAINT `FK7BC74D923EB90CDF` FOREIGN KEY (`fk_player_id`) REFERENCES `player` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of player_login
-- ----------------------------
INSERT INTO `player_login` VALUES ('6', 'b0a2026889dfb4e3825f3acfae52a190afc9883b7022d79a3ffe1b68c02f1f23', '201509121942186573', '1');
INSERT INTO `player_login` VALUES ('7', '4c98a819a20b530c546ac88bea1202f4a4c64bd2f7faccc92454402ebcb41fb1', '201509130902245036', '14');
INSERT INTO `player_login` VALUES ('8', 'BOT', '201509130832403440', '16');

-- ----------------------------
-- Table structure for player_permission
-- ----------------------------
DROP TABLE IF EXISTS `player_permission`;
CREATE TABLE `player_permission` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of player_permission
-- ----------------------------
INSERT INTO `player_permission` VALUES ('1', 'Admin');
INSERT INTO `player_permission` VALUES ('2', 'Player');

-- ----------------------------
-- Table structure for player_type
-- ----------------------------
DROP TABLE IF EXISTS `player_type`;
CREATE TABLE `player_type` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `attack` int(11) NOT NULL,
  `armor` int(11) NOT NULL,
  `health` int(11) NOT NULL,
  `mana` int(11) NOT NULL,
  `fk_ability` int(11) DEFAULT NULL,
  `fk_race` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_ability` (`fk_ability`),
  KEY `fk_race` (`fk_race`),
  KEY `fk_race_2` (`fk_race`),
  KEY `fk_ability_2` (`fk_ability`),
  KEY `fk_race_3` (`fk_race`),
  CONSTRAINT `FK69381FF02490377B` FOREIGN KEY (`fk_ability`) REFERENCES `ability` (`id`),
  CONSTRAINT `FK69381FF0C99EF538` FOREIGN KEY (`fk_race`) REFERENCES `race` (`id`),
  CONSTRAINT `FKE5B91C7051402FEA` FOREIGN KEY (`fk_ability`) REFERENCES `ability` (`id`),
  CONSTRAINT `FKE5B91C70571B805A` FOREIGN KEY (`fk_race`) REFERENCES `race` (`id`),
  CONSTRAINT `fk_race` FOREIGN KEY (`fk_race`) REFERENCES `race` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of player_type
-- ----------------------------
INSERT INTO `player_type` VALUES ('1', 'Rabid Soul', '5', '2', '20', '0', '1', '1');
INSERT INTO `player_type` VALUES ('2', 'Lurker', '2', '4', '35', '1', '2', '2');
INSERT INTO `player_type` VALUES ('3', 'Harrison Faggot', '10', '0', '10', '0', '3', '3');
INSERT INTO `player_type` VALUES ('4', 'Fader', '1', '15', '20', '2', '4', '4');

-- ----------------------------
-- Table structure for race
-- ----------------------------
DROP TABLE IF EXISTS `race`;
CREATE TABLE `race` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) DEFAULT NULL,
  `card_url` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of race
-- ----------------------------
INSERT INTO `race` VALUES ('1', 'Darkness', null);
INSERT INTO `race` VALUES ('2', 'Vampiric', null);
INSERT INTO `race` VALUES ('3', 'Lightbringer', null);
INSERT INTO `race` VALUES ('4', 'Ferocious', null);
