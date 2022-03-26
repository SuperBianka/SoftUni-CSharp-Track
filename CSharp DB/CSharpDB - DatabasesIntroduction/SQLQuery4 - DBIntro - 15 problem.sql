CREATE DATABASE [Hotel]

USE [Hotel]

CREATE TABLE [Employees]
(
             [Id] INT PRIMARY KEY IDENTITY,
			 [FirstName] NVARCHAR(30) NOT NULL,
			 [LastName] NVARCHAR(30) NOT NULL,
			 [Title] NVARCHAR(35),
			 [Notes] NVARCHAR(MAX)
)

INSERT INTO [Employees]([FirstName], [LastName], [Title], [Notes]) VALUES
                       ('Nikola', 'Manolov', 'Manager', NULL),
					   ('Emo', 'Nikolov', 'The Big Boss', NULL),
					   ('Pesho', 'Peshev', 'The Cleaner', NULL)

CREATE TABLE [Customers]
(
             [AccountNumber] INT PRIMARY KEY IDENTITY,
			 [FirstName] NVARCHAR(30) NOT NULL,
			 [LastName] NVARCHAR(30) NOT NULL,
			 [PhoneNumber] INT UNIQUE NOT NULL,
			 [EmergencyName] NVARCHAR(30),
			 [EmergencyNumber] INT,
			 [Notes] NVARCHAR(MAX)
)

INSERT INTO [Customers]([FirstName], [LastName], [PhoneNumber], [EmergencyName], [EmergencyNumber], [Notes]) VALUES
                       ('Nikol', 'Nikolova', 02896428, NULL, NULL, NULL),
					   ('Dimo', 'Ivanov', 02168743, NULL, NULL, NULL),
					   ('Aleks', 'Todorov', 02673451, NULL, NULL, NULL)

CREATE TABLE [RoomStatus]
(
             [RoomStatus] NVARCHAR(20) PRIMARY KEY,
			 [Notes] NVARCHAR(MAX)
)

INSERT INTO [RoomStatus]([RoomStatus], [Notes]) VALUES
                        ('Available', NULL),
						('Not Available', NULL),
						('None status', NULL)

CREATE TABLE [RoomTypes]
(
             [RoomType] NVARCHAR(20) PRIMARY KEY,
             [Notes] NVARCHAR(MAX)
)

INSERT INTO [RoomTypes]([RoomType], [Notes]) VALUES
                       ('Apartment', NULL),
                       ('Double room', NULL),
                       ('Single room', NULL)

CREATE TABLE [BedTypes]
(
             [BedType] NVARCHAR(20) PRIMARY KEY,
             [Notes] NVARCHAR(MAX)
)

INSERT INTO [BedTypes]([BedType], [Notes]) VALUES
                      ('King size bed', NULL),
                      ('Queen size bed', NULL),
                      ('Air bed', NULL)

CREATE TABLE [Rooms]
(
             [RoomNumber] INT PRIMARY KEY IDENTITY,
             [RoomType] NVARCHAR(20) FOREIGN KEY REFERENCES [RoomTypes]([RoomType]) NOT NULL,
             [BedType] NVARCHAR(20) FOREIGN KEY REFERENCES [BedTypes]([BedType]) NOT NULL,
             [Rate] DECIMAL(3, 1),
             [RoomStatus] NVARCHAR(20) FOREIGN KEY REFERENCES [RoomStatus]([RoomStatus]) NOT NULL,
             [Notes] NVARCHAR(MAX)
)

INSERT INTO [Rooms]([RoomType], [BedType], [Rate], [RoomStatus], [Notes]) VALUES
                   ('Apartment', 'King size bed', 7.6, 'Status1', NULL),
				   ('Double room', 'Queen size bed', 6.3, 'Status2', NULL),
				   ('Single room', 'Air bed', 7.2, 'Status3', NULL)

CREATE TABLE [Payments]
(
             [Id] INT PRIMARY KEY IDENTITY,
             [EmployeeId] INT FOREIGN KEY REFERENCES [Employees]([Id]) NOT NULL,
             [PaymentDate] DATETIME2 NOT NULL,
             [AccountNumber] INT FOREIGN KEY REFERENCES [Customers]([AccountNumber]) NOT NULL,
             [FirstDateOccupied] DATETIME2 NOT NULL,
             [LastDateOccupied] DATETIME2 NOT NULL,
             [TotalDays] INT NOT NULL,
             [AmountCharged] DECIMAL(10, 2), 
             [TaxRate] DECIMAL(5, 2),
             [TaxAmount] DECIMAL(10, 2),
             [PaymentTotal] DECIMAL(10, 2),
             [Notes] NVARCHAR(MAX)
)

INSERT INTO [Payments]([EmployeeId], [PaymentDate], [AccountNumber], [FirstDateOccupied], [LastDateOccupied],
                       [TotalDays], [AmountCharged], [TaxRate], [TaxAmount], [PaymentTotal], [Notes]) VALUES
                      (1,'2014-05-28', 2, '2014-05-01', '2014-05-21', 6, 580.55, 15.9, 39.8, 636.25, NULL),
					  (2,'2011-08-30', 3, '2011-08-05', '2011-08-25', 4, 436.50, 18.5, 45.4, 500.40, NULL),
					  (3,'2017-11-15', 1, '2017-11-02', '2017-11-12', 7, 610.20, 22.3, 35.9, 668.40, NULL)

CREATE TABLE [Occupancies]
(
             [Id] INT PRIMARY KEY IDENTITY,
             [EmployeeId] INT FOREIGN KEY REFERENCES [Employees]([Id]) NOT NULL,
             [DateOccupied] DATETIME2 NOT NULL,
             [AccountNumber] INT FOREIGN KEY REFERENCES [Customers]([AccountNumber]) NOT NULL,
             [RoomNumber] INT FOREIGN KEY REFERENCES [Rooms]([RoomNumber]) NOT NULL,
             [RateApplied] BIT NOT NULL,
             [PhoneCharge] DECIMAL(5, 2),
             [Notes] NVARCHAR(MAX)
)

INSERT INTO [Occupancies]([EmployeeId], [DateOccupied], [AccountNumber], [RoomNumber],
                          [RateApplied], [PhoneCharge], [Notes]) VALUES
						 (3, '2020-09-17', 2, 1, 0, 19.50, NULL),
						 (1, '2020-09-17', 3, 2, 1, 15.60, NULL),
						 (2, '2020-09-17', 1, 3, 0, 17.20, NULL)