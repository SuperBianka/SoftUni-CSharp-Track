CREATE DATABASE [CarRental]

USE [CarRental]

CREATE TABLE [Categories]
(
             [Id] INT PRIMARY KEY IDENTITY,
			 [CategoryName] NVARCHAR(35) NOT NULL,
			 [DailyRate] DECIMAL(5, 2) NOT NULL,
			 [WeeklyRate] DECIMAL(5, 2) NOT NULL,
			 [MonthlyRate] DECIMAL(5, 2) NOT NULL,
			 [WeekendRate] DECIMAL(5, 2) NOT NULL
)

INSERT INTO [Categories]([CategoryName], [DailyRate], [WeeklyRate], [MonthlyRate], [WeekendRate]) VALUES
                        ('Category1', 25.00, 60.00, 85.00, 35.00),
						('Category2', 20.00, 55.00, 80.00, 30.00),
						('Category3', 35.00, 70.00, 95.00, 45.00)

CREATE TABLE [Cars]
(
             [Id] INT PRIMARY KEY IDENTITY,
			 [PlateNumber] NVARCHAR(15) UNIQUE NOT NULL,
			 [Manufacturer] NVARCHAR(25) NOT NULL,
			 [Model] NVARCHAR(25) NOT NULL,
			 [CarYear] INT NOT NULL,
			 [CategoryId] INT FOREIGN KEY REFERENCES [Categories]([Id]) NOT NULL,
			 [Doors] INT NOT NULL,
			 [Picture] VARBINARY(MAX),
			 [Condition] NVARCHAR(250),
			 [Available] BIT NOT NULL,
)

INSERT INTO [Cars]([PlateNumber], [Manufacturer], [Model], [CarYear], [CategoryId], [Doors], [Picture], [Condition],  [Available]) VALUES
                  ('E 1712 TK', 'Nissan', 'Rogue', 2015, 3, 5, NULL, 'Good', 1),
				  ('PÂ 6903 PM', 'Toyota', 'Rav4', 2018, 2, 4, NULL, 'Very Good', 1),
				  ('ÑÂ 7777 ÕÊ', 'Tesla', 'S', 2013, 1, 5, NULL, 'Excellent', 0)

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
             [Id] INT PRIMARY KEY IDENTITY,
			 [DriverLicenceNumber] INT UNIQUE NOT NULL,
			 [FullName] NVARCHAR(55) NOT NULL,
			 [Address] NVARCHAR(95),
			 [City] NVARCHAR(25) NOT NULL,
			 [ZIPCode] INT,
			 [Notes] NVARCHAR(MAX)
)

INSERT INTO [Customers]([DriverLicenceNumber], [FullName], [Address], [City], [ZIPCode], [Notes]) VALUES
                       ('369752', 'Simo Simov', NULL, 'Plovdiv', 4000, NULL),
					   ('241963', 'Oleg Ognyanov', NULL, 'Sofia', 1000, NULL),
					   ('516874', 'Misho Notev', NULL, 'Varna', 9000, NULL)

CREATE TABLE [RentalOrders]
(
             [Id] INT PRIMARY KEY IDENTITY,
			 [EmployeeId] INT FOREIGN KEY REFERENCES [Employees]([Id]) NOT NULL,
			 [CustomerId] INT FOREIGN KEY REFERENCES [Customers]([Id]) NOT NULL,
			 [CarId] INT FOREIGN KEY REFERENCES [Cars]([Id]) NOT NULL,
			 [TankLevel] INT,
			 [KilometrageStart] INT NOT NULL,
			 [KilometrageEnd] INT NOT NULL,
			 [TotalKilometrage] INT NOT NULL,
			 [StartDate] DATETIME2 NOT NULL,
			 [EndDate] DATETIME2 NOT NULL,
			 [TotalDays] INT NOT NULL,
			 [RateApplied] BIT NOT NULL,
			 [TaxRate] DECIMAL(5, 2),
			 [OrderStatus] NVARCHAR(50),
			 [Notes] NVARCHAR(MAX)
)

INSERT INTO [RentalOrders]([EmployeeId], [CustomerId], [CarId], [TankLevel], [KilometrageStart], [KilometrageEnd], 
			[TotalKilometrage], [StartDate], [EndDate], [TotalDays], [RateApplied], [TaxRate], [OrderStatus], [Notes]) VALUES
			              (1, 2, 3, 60, 0, 340, 13550, '2012-05-09', '2012-05-19', 7, 1, NULL, NULL, NULL),
						  (2, 3, 1, 50, 0, 260, 22341, '2011-12-17', '2011-12-31', 6, 0, NULL, NULL, NULL),
						  (3, 1, 2, 70, 0, 290, 61895, '2017-03-10', '2017-03-22', 4, 1, NULL, NULL, NULL)