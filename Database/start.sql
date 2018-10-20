IF DB_ID('CheckersDB') IS NOT NULL BEGIN
	PRINT('Создание БД CheckersDB: БД уже существует!');
	PRINT('Пересоздание...');
	USE master;
	DROP DATABASE CheckersDB;
	CREATE DATABASE CheckersDB;
	PRINT('Создание БД CheckersDB: успешно.');
END ELSE BEGIN
	CREATE DATABASE CheckersDB;
	PRINT('Создание БД CheckersDB: успешно.');
END
GO
USE CheckersDB;
GO

CREATE TABLE GameHistory(
	Id       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_GameHistory PRIMARY KEY,
	GameTime DATETIME          NOT NULL,
	Who      NVARCHAR(200)     NOT NULL,
	WithWhom NVARCHAR(200)     NOT NULL,
	IdResult INT NOT NULL CONSTRAINT FK_GameHistory_GameResult
			         FOREIGN KEY REFERENCES GameResult(Id)
			         ON UPDATE CASCADE
                     ON DELETE CASCADE
);
CREATE TABLE GameResult(
	Id    INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_GameResult PRIMARY KEY,
	State NVARCHAR(200)     NOT NULL
);
CREATE TABLE Clients(
	Id       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Clients PRIMARY KEY,
	UserName NVARCHAR(200)     NOT NULL,
	Password NVARCHAR(200)     NOT NULL,
	Email    NVARCHAR(200)     NOT NULL
);
GO

INSERT INTO GameResult(State) VALUES
	('Победа'),
	('Поражение'),
	('Ничья');
GO