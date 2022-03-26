CREATE DATABASE [Movies]

USE [Movies]

CREATE TABLE [Directors]
(
             [Id] INT PRIMARY KEY IDENTITY,
			 [DirectorName] NVARCHAR(50) NOT NULL,
			 [Notes] NVARCHAR(MAX) 
)

INSERT INTO [Directors]([DirectorName], [Notes]) VALUES
                       ('Bianka', 'The best'),
                       ('Ivan', 'The Beast'),
                       ('Vessy', 'Amazing'),
                       ('Sia', 'Awesome'),
                       ('John', 'Good')

CREATE TABLE [Genres]
(
             [Id] INT PRIMARY KEY IDENTITY,
			 [GenreName] NVARCHAR(50) NOT NULL,
			 [Notes] NVARCHAR(MAX) 
)

INSERT INTO [Genres]([GenreName], [Notes]) VALUES
                    ('Horror', 'Creepy'),
                    ('Romance', 'Lovely'),
                    ('Comedy', 'Funny'),
                    ('Fantasy', 'Interesting'),
                    ('Drama', 'Emotional')

CREATE TABLE [Categories]
(
             [Id] INT PRIMARY KEY IDENTITY,
			 [CategoryName] NVARCHAR(50) NOT NULL,
			 [Notes] NVARCHAR(MAX) 
)

INSERT INTO [Categories]([CategoryName], [Notes]) VALUES
                    ('Science fiction', NULL),
                    ('Animation', NULL),
                    ('Romantic Comedy', NULL),
                    ('Sports', NULL),
                    ('Historical Fiction', NULL)

CREATE TABLE [Movies]
(
             [Id] INT PRIMARY KEY IDENTITY,
			 [Title] NVARCHAR(50) NOT NULL,
			 [DirectorId] INT FOREIGN KEY REFERENCES [Directors]([Id]) NOT NULL,
			 [CopyrightYear] INT NOT NULL,
			 [Length] INT NOT NULL,
			 [GenreId] INT FOREIGN KEY REFERENCES [Genres]([Id]) NOT NULL,
			 [CategoryId] INT FOREIGN KEY REFERENCES [Categories]([Id]) NOT NULL,
			 [Rating] REAL CHECK([Rating] >= 0 AND [Rating] <= 10) NOT NULL,
			 [Notes] NVARCHAR(MAX)
)

INSERT INTO [Movies]([Title], [DirectorId], [CopyrightYear], [Length], [GenreId], [CategoryId], [Rating], [Notes]) VALUES
                    ('The Danish Girl', 3, 2015, 120, 1, 3, 7.1, 'Memorable movie'),
                    ('My Spy', 4, 2020, 101, 3, 4, 6.4, 'Awesome movie'),
                    ('Just Mercy', 2, 2019, 137, 2, 4, 7.6, 'Deserves to watch'),
                    ('Cruella', 5, 2021, 134, 3, 5, 7.4, 'Good movie'),
                    ('Hachi: A Dogs Tale', 1, 2009, 93, 1, 2, 8.1, 'Deserves to watch')



