CREATE DATABASE [CigarShop]

USE [CigarShop]

 -- 01. DDL
CREATE TABLE Sizes
(
Id INT PRIMARY KEY IDENTITY NOT NULL,
[Length] INT CHECK([Length] BETWEEN 10 AND 25) NOT NULL,
RingRange DECIMAL(18, 2) CHECK((RingRange) BETWEEN 1.5 AND 7.5) NOT NULL
);

CREATE TABLE Tastes
(
Id INT PRIMARY KEY IDENTITY NOT NULL,
TasteType VARCHAR(20) NOT NULL,
TasteStrength VARCHAR(15) NOT NULL,
ImageURL NVARCHAR(100) NOT NULL
);

CREATE TABLE Brands
(
Id INT PRIMARY KEY IDENTITY NOT NULL,
BrandName VARCHAR(30) UNIQUE NOT NULL,
BrandDescription VARCHAR(MAX)
);

CREATE TABLE Cigars
(
Id INT PRIMARY KEY IDENTITY NOT NULL,
CigarName VARCHAR(80) NOT NULL,
BrandId INT FOREIGN KEY REFERENCES Brands(Id) NOT NULL,
TastId INT FOREIGN KEY REFERENCES Tastes(Id) NOT NULL,
SizeId INT FOREIGN KEY REFERENCES Sizes(Id) NOT NULL,
PriceForSingleCigar DECIMAL(18, 2),
ImageURL NVARCHAR(100) NOT NULL
);

CREATE TABLE Addresses
(
Id INT PRIMARY KEY IDENTITY NOT NULL,
Town VARCHAR(30) NOT NULL,
Country NVARCHAR(30) NOT NULL,
Streat NVARCHAR(100) NOT NULL,
ZIP VARCHAR(20) NOT NULL
);

CREATE TABLE Clients
(
Id INT PRIMARY KEY IDENTITY NOT NULL,
FirstName NVARCHAR(30) NOT NULL,
LastName NVARCHAR(30) NOT NULL,
Email NVARCHAR(50) NOT NULL,
AddressId INT FOREIGN KEY REFERENCES Addresses(Id) NOT NULL
);

CREATE TABLE ClientsCigars
(
ClientId INT FOREIGN KEY REFERENCES Clients(Id) NOT NULL,
CigarId INT FOREIGN KEY REFERENCES Cigars(Id) NOT NULL,
PRIMARY KEY(ClientId, CigarId)
)

 -- 02. Insert
INSERT INTO Cigars(CigarName, BrandId, TastId, SizeId, PriceForSingleCigar, ImageURL) VALUES
('COHIBA ROBUSTO',	9,	1,	5,	15.50,	'cohiba-robusto-stick_18.jpg'),
('COHIBA SIGLO I',	9,	1,	10,	410.00,	'cohiba-siglo-i-stick_12.jpg'),
('HOYO DE MONTERREY LE HOYO DU MAIRE',	14,	5,	11,	7.50, 'hoyo-du-maire-stick_17.jpg'),
('HOYO DE MONTERREY LE HOYO DE SAN JUAN',	14,	4,	15,	32.00, 'hoyo-de-san-juan-stick_20.jpg'),
('TRINIDAD COLONIALES', 2,	3,	8,	85.21, 'trinidad-coloniales-stick_30.jpg')

INSERT INTO Addresses(Town, Country, Streat, ZIP) VALUES
('Sofia', 'Bulgaria', '18 Bul. Vasil levski', 1000),
('Athens',	'Greece', '4342 McDonald Avenue', 10435),
('Zagreb',	'Croatia', '4333 Lauren Drive', 10000)

 --03. Update
UPDATE Cigars
   SET PriceForSingleCigar += PriceForSingleCigar * 0.20
 WHERE TastId = 1

UPDATE Brands
   SET BrandDescription = 'New description'
 WHERE BrandDescription IS NULL

 -- 04. Delete
DELETE 
  FROM Clients
 WHERE AddressId IN (7, 8, 10)

DELETE 
  FROM Addresses
 WHERE Country LIKE 'C%'

 -- 05. Cigars by Price
  SELECT CigarName,
         PriceForSingleCigar,
	     ImageURL
    FROM Cigars
ORDER BY PriceForSingleCigar ASC, CigarName DESC

 -- 06. Cigars by Taste
    SELECT c.Id,
           c.CigarName,
		   c.PriceForSingleCigar,
		   t.TasteType,
		   t.TasteStrength        
      FROM Cigars AS c
INNER JOIN Tastes AS t
        ON c.TastId = t.Id
     WHERE TasteType IN ('Earthy', 'Woody')
  ORDER BY PriceForSingleCigar DESC

 -- 07. Clients without Cigars
   SELECT Id,
          CONCAT(FirstName, ' ', LastName) 
	   AS ClientName,
	      Email
     FROM Clients cl
LEFT JOIN ClientsCigars AS cc
       ON cl.Id = cc.ClientId
    WHERE cc.CigarId IS NULL
 ORDER BY ClientName ASC

 -- 08. First 5 Cigars
    SELECT TOP(5) c.CigarName,
                  c.PriceForSingleCigar,
		          c.ImageURL
      FROM Cigars AS c
INNER JOIN Sizes AS s
        ON c.SizeId = s.Id
     WHERE [Length] >= 12
       AND (c.CigarName LIKE '%ci%'
        OR c.PriceForSingleCigar > 50)
       AND s.RingRange > 2.55
  ORDER BY c.CigarName ASC, c.PriceForSingleCigar DESC

 -- 09. Clients with ZIP Codes
    SELECT CONCAT(FirstName, ' ', LastName) 
	    AS FullName,
           a.Country,
		   a.ZIP,
		   CONCAT('$', MAX(cg.PriceForSingleCigar)) 
		AS CigarPrice
      FROM Clients AS c
INNER JOIN Addresses AS a
        ON c.AddressId =  a.Id
INNER JOIN ClientsCigars AS cc
        ON c.Id = cc.ClientId
INNER JOIN Cigars AS cg
        ON cc.CigarId = cg.Id
     WHERE a.ZIP NOT LIKE '%[A-Za-z]%'
  GROUP BY c.FirstName, c.LastName, a.Country, a.ZIP
  ORDER BY FullName ASC

 -- 10. Cigars by Size
    SELECT c.LastName,
           AVG(s.[Length]) 
	    AS CiagrLength,
	       CEILING(AVG(s.RingRange)) 
	    AS CiagrRingRange
      FROM Clients AS c
INNER JOIN ClientsCigars AS cc
        ON c.Id = cc.ClientId
INNER JOIN Cigars AS cg
        ON cc.CigarId = cg.Id
INNER JOIN Sizes AS s
        ON cg.SizeId = s.Id
     WHERE cc.CigarId IS NOT NULL
  GROUP BY c.LastName
  ORDER BY AVG(s.[Length]) DESC

  -- 11. Client with Cigars
GO

CREATE FUNCTION udf_ClientWithCigars(@name NVARCHAR(30))
RETURNS INT
AS
BEGIN
    DECLARE @totalCigarsCount INT = 
	        (SELECT COUNT(cc.CigarId) 
			   FROM Clients AS c
			   JOIN ClientsCigars AS cc
			     ON c.Id = cc.ClientId
			  WHERE c.FirstName = @name)
    
	RETURN @totalCigarsCount
END
GO

SELECT dbo.udf_ClientWithCigars('Betty')

 -- 12. Search for Cigar with Specific Taste
GO

CREATE PROC usp_SearchByTaste(@taste VARCHAR(20))
AS
   SELECT cg.CigarName,
          CONCAT('$', cg.PriceForSingleCigar) AS Price,
		  t.TasteType,
		  b.BrandName,
		  CONCAT(s.[Length], ' ', 'cm') AS CigarLength,
		  CONCAT(s.RingRange, ' ', 'cm') AS CigarRingRange
	 FROM Cigars AS cg
     JOIN Tastes AS t
	   ON cg.TastId = t.Id
	 JOIN Brands AS b
	   ON cg.BrandId = b.Id
	 JOIN Sizes AS s
	   ON cg.SizeId = s.Id
	WHERE t.TasteType = @taste
 ORDER BY CigarLength ASC, CigarRingRange DESC

GO

EXEC usp_SearchByTaste 'Woody'
 








