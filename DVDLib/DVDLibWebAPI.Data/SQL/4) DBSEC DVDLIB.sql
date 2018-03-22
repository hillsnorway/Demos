
USE MASTER;
GO

--DB SECURITY
--Server Login: DvdLibraryApp
--Password: testing123
--DVDLIB DB User Account for: DvdLibraryApp
--Grant Execute, Select, Insert, Update & Delete to DvdLibraryApp


USE MASTER;
GO

--Create new Login
IF EXISTS (select loginname from master.dbo.syslogins where name = 'DvdLibraryApp' and dbname = 'MASTER')
BEGIN
    DROP LOGIN DvdLibraryApp
END
CREATE LOGIN DvdLibraryApp WITH PASSWORD = 'testing123'
GO

USE DVDLIB;
GO

--Create new DB User
CREATE USER DvdLibraryApp FOR LOGIN DvdLibraryApp
GO

--Create a new DB Role
CREATE ROLE db_executor
 
--Grant EXECUTE to the Role
GRANT EXECUTE TO db_executor
 
--Add DB User to Role
ALTER ROLE db_executor ADD MEMBER DvdLibraryApp


--Alternative method, Grant explicitly on each SPROC
/* GRANT EXECUTE ON GetAllDvds TO DvdLibraryApp
GRANT EXECUTE ON GetDvdByID TO DvdLibraryApp
GRANT EXECUTE ON AddDvd TO DvdLibraryApp
GRANT EXECUTE ON EditDvd TO DvdLibraryApp
GRANT EXECUTE ON DeleteDvd TO DvdLibraryApp
GO */



