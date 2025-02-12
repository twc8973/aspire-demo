-- SQL Server init script

-- Create the MyDemo database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'MyDemo')
BEGIN
  CREATE DATABASE MyDemo;
END;
GO

USE MyDemo;
GO

-- Create the Todos table
IF OBJECT_ID(N'Todos', N'U') IS NULL
BEGIN
    CREATE TABLE Todos
    (
        Id        INT PRIMARY KEY IDENTITY(1,1) ,
        Title VARCHAR(255) NOT NULL,
        Description  VARCHAR(255) NOT NULL,
        Random     VARCHAR(255) NULL
    );
END;
GO