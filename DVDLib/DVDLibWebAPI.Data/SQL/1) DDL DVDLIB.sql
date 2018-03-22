
USE MASTER;
GO

USE DVDLIB;
GO

--************************************** [Rating]

CREATE TABLE [Rating]
(
 [RatingID]    INT IDENTITY (1, 1) NOT NULL ,
 [RatingType]      VARCHAR(5) NOT NULL ,
 [RatingDescr] NVARCHAR(250) NOT NULL ,

 CONSTRAINT [PK_Rating] PRIMARY KEY CLUSTERED ([RatingID] ASC)
);
GO



--************************************** [Director]

CREATE TABLE [Director]
(
 [DirectorID] INT IDENTITY (1, 1) NOT NULL ,
 [DirectorName]       NVARCHAR(50) NOT NULL ,


 CONSTRAINT [PK_Director] PRIMARY KEY CLUSTERED ([DirectorID] ASC)
);
GO



--************************************** [Year]

CREATE TABLE [Year]
(
 [YearID] INT IDENTITY (1, 1) NOT NULL ,
 [YearNR]   CHAR(4) NOT NULL ,

 CONSTRAINT [PK_ReleaseYear] PRIMARY KEY CLUSTERED ([YearID] ASC)
);
GO



--************************************** [Dvd]

CREATE TABLE [Dvd]
(
 [DvdID]      INT IDENTITY (1, 1) NOT NULL ,
 [YearID]     INT NOT NULL ,
 [DirectorID] INT NOT NULL ,
 [RatingID]   INT NOT NULL ,
 [DvdTitle]      NVARCHAR(50) NOT NULL ,
 [DvdNotes]      NVARCHAR(250) NULL ,

 CONSTRAINT [PK_dvd] PRIMARY KEY CLUSTERED ([DvdID] ASC),
 CONSTRAINT [FK_Dvd_Rating] FOREIGN KEY ([RatingID])
  REFERENCES [Rating]([RatingID]),
 CONSTRAINT [FK_Dvd_Year] FOREIGN KEY ([YearID])
  REFERENCES [Year]([YearID]),
 CONSTRAINT [FK_Dvd_Director] FOREIGN KEY ([DirectorID])
  REFERENCES [Director]([DirectorID])
);
GO