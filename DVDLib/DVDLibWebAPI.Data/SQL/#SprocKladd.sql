USE MASTER;
GO

USE DVDLIB;
GO

Exec GetAllDvds
GO

Exec GetDvdByID 1
GO

Declare @DvdID int
Exec AddDvd @DvdID output, @DvdTitle = 'Modern Stoneage Family', @YearNR = '2022', @DirectorName = 'Fred Flintstone', @RatingType = 'X', @DvdNotes = '"Right Out Of Bedrock", its a modern stoneage family!'
Select @DvdID
Exec GetDvdByID @DvdID
GO

Exec EditDvd @DvdID = 4, @DvdTitle = 'zModern Stoneage Family', @YearNR = '2039', @DirectorName = 'zFred Flintstone', @RatingType = 'Z', @DvdNotes = '"zRight Out Of Bedrockz", its a modern stoneage family!'
Exec GetDvdByID 4
GO

Exec DeleteDvd @DvdID = 4
Exec GetDvdByID 4
GO