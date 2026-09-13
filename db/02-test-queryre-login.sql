-- In SSMS, connect with SQL Server Authentication:
-- Login: queryre
-- Database: Chinook

-- This must work.
SELECT TOP (5) CustomerId, FirstName, LastName, Country
FROM dbo.Customer;
GO

-- This must fail with a permission error.
DELETE FROM dbo.Customer WHERE CustomerId = -1;
GO
