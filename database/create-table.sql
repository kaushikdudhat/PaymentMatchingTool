-- ============================================================
-- Payments Matching Tool - Database Setup
-- Run this script against your SQL Server database
-- ============================================================

-- Create database (optional - skip if DB already exists)
-- CREATE DATABASE PaymentsMatchingDb;
-- GO
-- USE PaymentsMatchingDb;
-- GO

-- Drop table if re-running
IF OBJECT_ID('dbo.MatchResults', 'U') IS NOT NULL
    DROP TABLE dbo.MatchResults;
GO

CREATE TABLE dbo.MatchResults
(
    Id             INT            IDENTITY(1,1) NOT NULL,
    OrderId        NVARCHAR(50)   NOT NULL,
    Currency       NVARCHAR(10)   NOT NULL,
    SystemAmount   DECIMAL(18,2)  NULL,
    ProviderAmount DECIMAL(18,2)  NULL,
    Status         NVARCHAR(50)   NOT NULL,
    IsResolved     BIT            NOT NULL CONSTRAINT DF_MatchResults_IsResolved DEFAULT (0),
    ResolutionSide NVARCHAR(20)   NULL,
    CreatedDate    DATETIME       NOT NULL,
    UpdatedDate    DATETIME       NULL,

    CONSTRAINT PK_MatchResults PRIMARY KEY CLUSTERED (Id ASC)
);
GO

-- Index to speed up status filtering
CREATE NONCLUSTERED INDEX IX_MatchResults_Status
    ON dbo.MatchResults (Status)
    INCLUDE (IsResolved);
GO

-- Index to speed up resolved filtering
CREATE NONCLUSTERED INDEX IX_MatchResults_IsResolved
    ON dbo.MatchResults (IsResolved);
GO

PRINT 'MatchResults table created successfully.';
GO
