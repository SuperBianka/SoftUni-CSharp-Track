CREATE DATABASE [Minions]

USE [Minions]

CREATE TABLE [Minions]
(
             [Id] INT PRIMARY KEY NOT NULL,
             [Name] NVARCHAR(50) NOT NULL,
             [Age] INT
)

CREATE TABLE [Towns]
(
             [Id] INT PRIMARY KEY NOT NULL,
             [Name] NVARCHAR(50) NOT NULL
)

ALTER TABLE [Minions]
        ADD [TownId] INT 

ALTER TABLE [Minions]
ADD CONSTRAINT [FK_MinionsTownId] FOREIGN KEY ([TownId]) REFERENCES [Towns]([Id])

INSERT INTO [Towns]([Id], [Name]) VALUES
                   (1, 'Sofia'),
                   (2, 'Plovdiv'),
                   (3, 'Varna')

INSERT INTO [Minions]([Id], [Name], [Age], [TownId]) VALUES
                     (1, 'Kevin', 22, 1),
                     (2, 'Bob', 15, 3),
					 (3, 'Steward', NULL, 2)

--DELETE FROM [Minions]
TRUNCATE TABLE [Minions]

DROP TABLE [Minions]
DROP TABLE [Towns]

CREATE TABLE [People]
(
             [Id] INT PRIMARY KEY IDENTITY,
			 [Name] NVARCHAR(200) NOT NULL,
			 [Picture] VARBINARY(MAX),
			 [Height] DECIMAL(3, 2),
			 [Weight] DECIMAL(5, 2),
			 [Gender] CHAR(1) NOT NULL,
			 [Birthdate] DATE NOT NULL,
			 [Biography] NVARCHAR(MAX)
)

INSERT INTO [People]([Name], [Picture], [Height], [Weight], [Gender], [Birthdate], [Biography]) VALUES
                    ('Bianka Grozdarska', NULL, 1.65, 55.80, 'f', '1995-12-17', 'Special'),
					('Vesi Maxeva', NULL, 1.73, 68.50, 'f', '1969-08-30', 'Special'),
					('Ivan Ivanov', NULL, 1.80, 73.40, 'm', '1992-07-29', 'Special'),
					('Sia Nikolaeva', NULL, 1.65, 54.70, 'f', '1995-12-17', 'Special'),
					('Ariana Queen', NULL, 1.60, 35.90, 'f', '2008-01-09', 'Special')

CREATE TABLE [Users]
(
             [Id] BIGINT PRIMARY KEY IDENTITY,
			 [Username] VARCHAR(30) UNIQUE NOT NULL,
			 [Password] VARCHAR(26) NOT NULL,
			 [ProfilePicture] VARBINARY(MAX),
			 CHECK (DATALENGTH([ProfilePicture]) <= 900000),
			 [LastLoginTime] DATETIME2,
			 [IsDeleted] BIT NOT NULL
)

INSERT INTO [Users]([Username], [Password], [ProfilePicture], [LastLoginTime], [IsDeleted]) Values
                   ('Bibi17', '012BG', NULL, '2021-07-04', 0),
				   ('Niks03', '369Nx', NULL, '2019-12-17', 1),
				   ('Vivi77', '999VV', NULL, '2020-11-29', 0),
				   ('Sony6', '6969ok', NULL, '2018-01-27', 1),
				   ('Molly96', '35FGj', NULL, '2021-08-30', 0)

    ALTER TABLE [Users]
DROP CONSTRAINT [PK__Users__3214EC07EEB35BCD]

   ALTER TABLE [Users]
ADD CONSTRAINT [PK_UsersCompositeIdUsername] PRIMARY KEY ([Id], [Username])

   ALTER TABLE [Users]
ADD CONSTRAINT [CHK_PassLength] CHECK (DATALENGTH([Password]) >= 5)

   ALTER TABLE [Users]
ADD CONSTRAINT [df_LastLoginTime] DEFAULT SYSDATETIME() FOR [LastLoginTime]

    ALTER TABLE [Users]
DROP CONSTRAINT [PK_UsersCompositeIdUsername]
    ALTER TABLE [Users]
 ADD CONSTRAINT [PK_UsersId] PRIMARY KEY ([Id])
    ALTER TABLE [Users]
 ADD CONSTRAINT [UC_Username] UNIQUE ([Username])
    ALTER TABLE [Users]
 ADD CONSTRAINT [CHK_UsernameLength] CHECK (DATALENGTH([Username]) >= 3)


 


 
