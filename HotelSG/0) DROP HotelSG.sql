
USE MASTER;
GO

--************************************** HOTELSG: Drop Old DB & Create New (Empty) DB
IF EXISTS(select * from sys.databases where name = 'HOTELSG') 
BEGIN
	--The following line allows any active connections to be terminated...
	ALTER DATABASE HOTELSG SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE HOTELSG;
	CREATE DATABASE HOTELSG;
END
GO
