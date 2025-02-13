-- Create the database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'Aspired')
BEGiN
    CREATE DATABASE Aspired
END

GO

USE Aspired
GO

-- Create the Products table
IF OBJECT_ID(N'Products', N'U') IS NULL
BEGIN
    CREATE TABLE Products (
        [ProductId] INT PRIMARY KEY IDENTITY(1,1),
        [Name] VARCHAR(100) NOT NULL,
        [Description] VARCHAR(255) NOT NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE()
    )
END

GO

-- Add some seed data
INSERT INTO Products (Name, Description, CreatedAt) VALUES ('Widget 2000', 'The best widget of the millenium.  A must have for widget collectors!', GETDATE())
INSERT INTO Products (Name, Description, CreatedAt) VALUES ('The Gadget', 'Gadgets are the best, get yours today!', GETDATE())