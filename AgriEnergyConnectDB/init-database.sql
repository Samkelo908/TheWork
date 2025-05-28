USE master;
GO

-- Create the database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'AgriEnergyConnectDB')
BEGIN
    CREATE DATABASE AgriEnergyConnectDB;
    PRINT 'Database AgriEnergyConnectDB created.';
END
ELSE
BEGIN
    PRINT 'Database AgriEnergyConnectDB already exists.';
END
GO

-- Switch to the AgriEnergyConnectDB database
USE AgriEnergyConnectDB;
GO

-- Create Farmers table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Farmers' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Farmers (
        Id NVARCHAR(450) NOT NULL PRIMARY KEY,
        -- Add any additional farmer fields here
        CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Table dbo.Farmers created.';
END
ELSE
BEGIN
    PRINT 'Table dbo.Farmers already exists.';
END
GO

-- Create Products table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Products (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Name NVARCHAR(255) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        Quantity INT NOT NULL,
        Category NVARCHAR(255) NOT NULL,
        HarvestDate DATETIME2 NOT NULL,
        FarmerId NVARCHAR(450) NOT NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Products_Farmers FOREIGN KEY (FarmerId) REFERENCES dbo.Farmers(Id) ON DELETE CASCADE
    );
    PRINT 'Table dbo.Products created.';
END
ELSE
BEGIN
    PRINT 'Table dbo.Products already exists.';
END
GO

-- Create indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_FarmerId' AND object_id = OBJECT_ID('dbo.Products'))
BEGIN
    CREATE INDEX IX_Products_FarmerId ON dbo.Products(FarmerId);
    PRINT 'Index IX_Products_FarmerId created.';
END
GO

PRINT 'Database initialization complete.';
GO