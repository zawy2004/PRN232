/*
Additive seed script for the application layer. Run AFTER FUNewsManagement.sql.
- Inserts a reserved Admin row (AccountID = 0) so FK columns on NewsArticle have a
  valid owner when the config-based Admin account (see appsettings.json) acts on news.
  Real admin login is still validated against appsettings.json, not this row's password.
- Re-hashes the seed AccountPassword values from plaintext to PBKDF2-SHA256
  (format: iterations.base64(salt).base64(hash)), matching PasswordHasher in
  FUNewsManagement.BusinessLogic, so no plaintext password is stored.
- Fixes the original seed data's Category rows, which each set ParentCategoryID equal
  to their own CategoryID (a self-reference bug that breaks tree building) -> NULL,
  making them root categories. Adds a couple of demo child categories so the
  multi-level dropdown menu has real parent/child data to render.
*/
USE [FUNewsManagement]
GO

UPDATE [dbo].[Category] SET [ParentCategoryID] = NULL WHERE [CategoryID] = [ParentCategoryID]
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Category] WHERE [CategoryName] = N'Technology')
BEGIN
    INSERT INTO [dbo].[Category] ([CategoryName], [CategoryDesciption], [ParentCategoryID], [IsActive])
    VALUES (N'Technology', N'Technology-related news under Academic news.', 1, 1)

    DECLARE @TechId SMALLINT = (SELECT CategoryID FROM [dbo].[Category] WHERE [CategoryName] = N'Technology')

    INSERT INTO [dbo].[Category] ([CategoryName], [CategoryDesciption], [ParentCategoryID], [IsActive])
    VALUES
        (N'.NET', N'.NET platform news.', @TechId, 1),
        (N'Java', N'Java platform news.', @TechId, 1)
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[SystemAccount] WHERE [AccountID] = 0)
BEGIN
    INSERT INTO [dbo].[SystemAccount] ([AccountID], [AccountName], [AccountEmail], [AccountRole], [AccountPassword])
    VALUES (0, N'Admin', N'admin@FUNewsManagementSystem.org', 3, N'100000.P7muhk+q63Mj+bPS2xtSig==.BKCEi8+BfSaDsFDC5xtEBw==')
END
GO

-- Seed staff/lecturer accounts (1-5) all used plaintext password '@1'
UPDATE [dbo].[SystemAccount]
SET [AccountPassword] = N'100000.7qM0HQZzIQ2YA9lboC4RhQ==.ZBF6FLd9Ti6Dvg3nztpYLA=='
WHERE [AccountID] BETWEEN 1 AND 5
GO
