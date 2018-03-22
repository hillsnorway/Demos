
USE MASTER;
GO

USE DVDLIB;
GO


--************************************** GetAllDvds ()
--API Methods Get, URL's: /dvds, /dvds/title/{title}, /dvds/year/{realeaseYear}, /dvds/director/{directorName}, /dvds/rating/{rating}
IF EXISTS (
    SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
    WHERE ROUTINE_NAME = 'GetAllDvds'
)
BEGIN
    DROP PROCEDURE GetAllDvds
END
GO

CREATE PROCEDURE GetAllDvds
AS
	SELECT DvdID as dvdId,
		   YearNR as realeaseYear,
		   DirectorName as director,
		   RatingType as rating,
		   DvdTitle as title,
		   DvdNotes as notes
	FROM Dvd
	INNER JOIN [Year] Y on Y.YearID = Dvd.YearID
	INNER JOIN Director D on D.DirectorID = Dvd.DirectorID
	INNER JOIN Rating R on R.RatingID = Dvd.RatingID
	ORDER BY DvdTitle, YearNR
GO	

--************************************** GetDvdByID (@dvdID)
--API Method Get, URL: /dvd/id
IF EXISTS (
    SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
    WHERE ROUTINE_NAME = 'GetDvdByID'
)
BEGIN
    DROP PROCEDURE GetDvdByID
END
GO

CREATE PROCEDURE GetDvdByID(
	@dvdID int
)
AS
	SELECT DvdID as dvdId,
		   YearNR as realeaseYear,
		   DirectorName as director,
		   RatingType as rating,
		   DvdTitle as title,
		   DvdNotes as notes
	FROM Dvd
	INNER JOIN [Year] Y on Y.YearID = Dvd.YearID
	INNER JOIN Director D on D.DirectorID = Dvd.DirectorID
	INNER JOIN Rating R on R.RatingID = Dvd.RatingID
	WHERE Dvd.DvdID = @dvdID
	ORDER BY DvdTitle, YearNR
GO


--************************************** AddDvd (@DvdID output, @DvdTitle, @YearNR, @DirectorName, @RatingType, @DvdNotes)
--API Method Post, URL: /dvd
IF EXISTS (
    SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
    WHERE ROUTINE_NAME = 'AddDvd'
)
BEGIN
    DROP PROCEDURE AddDvd
END
GO

CREATE PROCEDURE AddDvd(
	@DvdID INT OUTPUT,
	@DvdTitle NVARCHAR(50),
	@YearNR CHAR(4),
	@DirectorName NVARCHAR(50),
	@RatingType VARCHAR(5),
	@DvdNotes NVARCHAR(250)
)
AS
	IF (SELECT Count(Y.YearID)
		FROM [Year] Y
		WHERE Y.YearNR = @YearNR) = 0
	BEGIN
		INSERT INTO [Year](YearNR)
		VALUES (@YearNR)
	END

	IF (SELECT Count(D.DirectorID)
		FROM Director D
		WHERE D.DirectorName = @DirectorName) = 0
	BEGIN
		INSERT INTO Director(DirectorName)
		VALUES (@DirectorName)
	END

	IF (SELECT Count(R.RatingID)
		FROM Rating R
		WHERE R.RatingType = @RatingType) = 0
	BEGIN
		INSERT INTO Rating(RatingType, RatingDescr)
		VALUES (@RatingType,'')
	END

	INSERT INTO Dvd(YearID, DirectorID, RatingID, DvdTitle, DvdNotes)
	VALUES
		((SELECT MIN(YearID) FROM [Year] Where YearNR = @YearNR),
		(SELECT MIN(DirectorID) FROM Director WHERE DirectorName = @DirectorName),
		(SELECT MIN(RatingID) FROM Rating WHERE RatingType = @RatingType),
		@DvdTitle,
		@DvdNotes)
		
	SET @DvdID = SCOPE_IDENTITY()
GO


--************************************** EditDvd (@DvdID, @DvdTitle, @YearNR, @DirectorName, @RatingType, @DvdNotes)
--API Method Put, URL: /dvd/id
IF EXISTS (
    SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
    WHERE ROUTINE_NAME = 'EditDvd'
)
BEGIN
    DROP PROCEDURE EditDvd
END
GO

CREATE PROCEDURE EditDvd(
	@DvdID INT,
	@DvdTitle NVARCHAR(50),
	@YearNR CHAR(4),
	@DirectorName NVARCHAR(50),
	@RatingType VARCHAR(5),
	@DvdNotes NVARCHAR(250)
)
AS
	IF (SELECT Count(Y.YearID)
		FROM [Year] Y
		WHERE Y.YearNR = @YearNR) = 0
	BEGIN
		INSERT INTO [Year](YearNR)
		VALUES (@YearNR)
	END

	IF (SELECT Count(D.DirectorID)
		FROM Director D
		WHERE D.DirectorName = @DirectorName) = 0
	BEGIN
		INSERT INTO Director(DirectorName)
		VALUES (@DirectorName)
	END

	IF (SELECT Count(R.RatingID)
		FROM Rating R
		WHERE R.RatingType = @RatingType) = 0
	BEGIN
		INSERT INTO Rating(RatingType, RatingDescr)
		VALUES (@RatingType,'')
	END

	UPDATE Dvd
	SET YearID = (SELECT MIN(YearID) FROM [Year] Where YearNR = @YearNR),
		DirectorID = (SELECT MIN(DirectorID) FROM Director WHERE DirectorName = @DirectorName),
		RatingID = (SELECT MIN(RatingID) FROM Rating WHERE RatingType = @RatingType),
		DvdTitle = @DvdTitle,
		DvdNotes = @DvdNotes
	WHERE DvdID = @DvdID
GO

--************************************** DeleteDvd (@dvdID)
--API Method Delete, URL: /dvd/id
IF EXISTS (
    SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
    WHERE ROUTINE_NAME = 'DeleteDvd'
)
BEGIN
    DROP PROCEDURE DeleteDvd
END
GO

CREATE PROCEDURE DeleteDvd(
	@dvdID int
)
AS

Delete From Dvd
Where Dvd.DvdID = @DvdID

