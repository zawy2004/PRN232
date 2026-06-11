IF DB_ID(N'OdataASPNETCoreDemo') IS NULL
BEGIN
    CREATE DATABASE OdataASPNETCoreDemo;
END
GO

USE OdataASPNETCoreDemo;
GO

IF OBJECT_ID(N'dbo.Gadgets', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Gadgets
    (
        Id           INT            NOT NULL PRIMARY KEY,
        ProductName  VARCHAR(MAX)   NULL,
        Brand        VARCHAR(MAX)   NULL,
        Cost         DECIMAL(18, 0) NOT NULL,
        ImageName    VARCHAR(1024)  NULL,
        Type         VARCHAR(128)   NULL,
        CreatedDate  DATETIME       NULL,
        ModifiedDate DATETIME       NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Gadgets)
BEGIN
    INSERT INTO dbo.Gadgets (Id, ProductName, Brand, Cost, ImageName, Type, CreatedDate, ModifiedDate)
    VALUES
        (1, 'Samsung Galaxy', 'Samsung', 12000, 'samsung.jpeg', 'Mobile', '2021-12-12', '2021-12-12'),
        (2, 'iPhone',         'Apple',   13000, 'iphone.png',   'Mobile', '2021-12-12', '2021-12-12'),
        (3, 'IBM Thinkpad',   'IBM',     34999, 'thinkpad.jpeg','Laptop', '2021-12-12', '2021-12-12'),
        (4, 'HP ProBook',     'HP',      21000, 'probook.png',  'Laptop', '2021-12-12', '2021-12-12'),
        (5, 'Nokia 9222',     'Nokia',   11000, 'nokia.jpeg',   'Mobile', '2021-12-12', '2021-12-12');
END
GO
