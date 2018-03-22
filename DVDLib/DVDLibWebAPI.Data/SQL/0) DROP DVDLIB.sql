
USE MASTER;
GO

--************************************** HOTELSG: Drop Old DB & Create New (Empty) DB
IF EXISTS(select * from sys.databases where name = 'DVDLIB') 
BEGIN
	--The following line allows any active connections to be terminated...
	ALTER DATABASE DVDLIB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE DVDLIB;
END
CREATE DATABASE DVDLIB;
GO
