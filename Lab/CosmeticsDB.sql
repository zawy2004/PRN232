-- Create Database
CREATE DATABASE CosmeticsDB;
GO

USE CosmeticsDB;
GO

-- Create CosmeticCategory table
CREATE TABLE CosmeticCategory (
    CategoryID NVARCHAR(30) NOT NULL,
    CategoryName NVARCHAR(120),
    UsagePurpose NVARCHAR(250),
    FormulationType NVARCHAR(250),
    CONSTRAINT PK__Cosmetic__19093A2BA2750A62 PRIMARY KEY (CategoryID)
);
GO

-- Create CosmeticInformation table
CREATE TABLE CosmeticInformation (
    CosmeticID NVARCHAR(30) NOT NULL,
    CosmeticName NVARCHAR(160),
    SkinType NVARCHAR(200),
    CosmeticSize NVARCHAR(400),
    DollarPrice DECIMAL(18, 0),
    ExpirationDate NVARCHAR(160),
    CategoryID NVARCHAR(30),
    CONSTRAINT PK__Cosmetic__98ED527E67429B69 PRIMARY KEY (CosmeticID),
    CONSTRAINT FK__CosmeticI__Categ__3C69FB99 FOREIGN KEY (CategoryID)
        REFERENCES CosmeticCategory(CategoryID) ON DELETE CASCADE
);
GO

-- Create SystemAccount table
CREATE TABLE SystemAccount (
    AccountID INT NOT NULL,
    EmailAddress NVARCHAR(100),
    AccountPassword NVARCHAR(100),
    Role INT,
    AccountNote NVARCHAR(240),
    CONSTRAINT PK__SystemAc__349DA58646609DFA PRIMARY KEY (AccountID),
    CONSTRAINT UQ__SystemAc__49A14740597553D5 UNIQUE (EmailAddress)
);
GO

-- Insert sample data for CosmeticCategory
INSERT INTO CosmeticCategory (CategoryID, CategoryName, UsagePurpose, FormulationType) VALUES
('CAT001', 'Skincare', 'Moisturizing and hydrating skin', 'Cream'),
('CAT002', 'Makeup', 'Coverage and enhancement', 'Liquid'),
('CAT003', 'Haircare', 'Nourishing and strengthening hair', 'Gel'),
('CAT004', 'Suncare', 'UV protection', 'Lotion');
GO

-- Insert sample data for CosmeticInformation
INSERT INTO CosmeticInformation (CosmeticID, CosmeticName, SkinType, CosmeticSize, DollarPrice, ExpirationDate, CategoryID) VALUES
('PL100001', 'Hydrating Face Cream', 'Dry', '50ml', 25, '2026-12-31', 'CAT001'),
('PL100002', 'Oil Control Moisturizer', 'Oily', '30ml', 20, '2026-06-30', 'CAT001'),
('PL100003', 'Foundation SPF30', 'All', '30ml', 35, '2025-12-31', 'CAT002'),
('PL100004', 'Red Lipstick', 'All', '3.5g', 15, '2027-01-01', 'CAT002'),
('PL100005', 'Argan Oil Shampoo', 'All', '250ml', 18, '2026-09-30', 'CAT003');
GO

-- Insert sample SystemAccounts
-- Role: Administrator=1, Manager=2, Staff=3, Member=4
INSERT INTO SystemAccount (AccountID, EmailAddress, AccountPassword, Role, AccountNote) VALUES
(1, 'admin@cosmetics.com', 'Admin@123', 1, 'Administrator account'),
(2, 'manager@cosmetics.com', 'Manager@123', 2, 'Manager account'),
(3, 'staff@cosmetics.com', 'Staff@123', 3, 'Staff account'),
(4, 'member@cosmetics.com', 'Member@123', 4, 'Member account');
GO
