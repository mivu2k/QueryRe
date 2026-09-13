USE [master];
GO

IF SUSER_ID(N'queryre') IS NULL
BEGIN
    CREATE LOGIN [queryre]
    WITH PASSWORD = 'CHANGE_ME_STRONG_PASSWORD', CHECK_POLICY = ON;
END
ELSE
BEGIN
    ALTER LOGIN [queryre] ENABLE;
    ALTER LOGIN [queryre] WITH PASSWORD = 'CHANGE_ME_STRONG_PASSWORD';
END
GO

USE [Chinook];
GO

IF USER_ID(N'queryre') IS NULL
    CREATE USER [queryre] FOR LOGIN [queryre];
GO

IF DATABASE_PRINCIPAL_ID(N'queryre_readers') IS NULL
    CREATE ROLE [queryre_readers];
GO

ALTER ROLE [queryre_readers] ADD MEMBER [queryre];
GRANT SELECT ON SCHEMA::[dbo] TO [queryre_readers];
DENY INSERT, UPDATE, DELETE, EXECUTE ON SCHEMA::[dbo] TO [queryre_readers];
DENY CREATE TABLE, ALTER ANY SCHEMA, CONTROL TO [queryre_readers];
GO
