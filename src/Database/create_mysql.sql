GRANT USAGE ON * . * TO 'siennacore'@'localhost' IDENTIFIED BY 'siennacore' WITH MAX_QUERIES_PER_HOUR 0 MAX_CONNECTIONS_PER_HOUR 0 MAX_UPDATES_PER_HOUR 0 ;
CREATE DATABASE `rift_world` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
CREATE DATABASE `rift_characters` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
CREATE DATABASE `rift_accounts` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
GRANT ALL PRIVILEGES ON `rift_world` . * TO 'siennacore'@'localhost' WITH GRANT OPTION;
GRANT ALL PRIVILEGES ON `rift_characters` . * TO 'siennacore'@'localhost' WITH GRANT OPTION;
GRANT ALL PRIVILEGES ON `rift_accounts` . * TO 'siennacore'@'localhost' WITH GRANT OPTION;
