
USE MASTER;
GO


--==DROP DB SCRIPT=======================================================================================================================================

--************************************** HOTELSG: Drop Old DB & Create New (Empty) DB
IF EXISTS(select * from sys.databases where name = 'HOTELSG') 
BEGIN
	--The following line allows any active connections to be terminated...
	ALTER DATABASE HOTELSG SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE HOTELSG;
END
GO

CREATE DATABASE HOTELSG;
GO

USE HOTELSG;
GO


--==DDL SCRIPT=======================================================================================================================================

--************************************** RoomStatusCode
CREATE TABLE RoomStatusCode
(
 StatusCode     CHAR(2) NOT NULL ,
 StatusCodeName NVARCHAR(20) NOT NULL ,

 CONSTRAINT PK_RoomStatusCode PRIMARY KEY (StatusCode)
);
GO



--************************************** PromoType
CREATE TABLE PromoType
(
 PromoTypeID INT IDENTITY (1, 1) NOT NULL ,
 PromoType   NVARCHAR(20) NOT NULL ,

 CONSTRAINT PK_PromoType PRIMARY KEY (PromoTypeID)
);
GO



--************************************** PromoDiscountType
CREATE TABLE PromoDiscountType
(
 PromoDiscountFlag BIT NOT NULL ,
 PromoDiscountType NVARCHAR(20) NOT NULL ,

 CONSTRAINT PK_PromoDiscountType PRIMARY KEY (PromoDiscountFlag)
);
GO



--************************************** InvoiceLineItemType
CREATE TABLE InvoiceLineItemType
(
 InvoiceLineItemTypeID INT IDENTITY (1, 1) NOT NULL ,
 ItemTypeName          NVARCHAR(20) NOT NULL ,
 ReferencesKeyName     NVARCHAR(30) NOT NULL ,
 ReferencesTableName   NVARCHAR(30) NOT NULL ,

 CONSTRAINT PK_InvoiceItemType PRIMARY KEY (InvoiceLineItemTypeID)
);
GO



--************************************** Invoice
CREATE TABLE Invoice
(
 InvoiceID            INT IDENTITY (1, 1) NOT NULL ,
 InvoiceNr            CHAR(10) NOT NULL ,
 InvoiceDate          DATE NOT NULL ,
 CustomerID           INT NOT NULL ,
 ReservationID        INT NOT NULL ,
 CustomerName         NVARCHAR(42) NOT NULL ,
 CustomerQuoteRate NUMERIC(4,2) NOT NULL ,
 Tax                  NUMERIC(8,2) NOT NULL ,
 Total                NUMERIC(10,2) NOT NULL ,
 PromoCode            VARCHAR(20) NULL ,
 PaymentMethod        NVARCHAR(10) NULL ,
 PaymentDetail        NVARCHAR(12) NULL ,
 InvoicePaidDate      DATE NULL ,

 CONSTRAINT PK_Invoice PRIMARY KEY (InvoiceID)
);
GO



--************************************** PaymentMethodType
CREATE TABLE PaymentMethodType
(
 PaymentMethodTypeID   INT IDENTITY (1, 1) NOT NULL ,
 PaymentMethodTypeName NVARCHAR(10) NOT NULL ,

 CONSTRAINT PK_PaymentType PRIMARY KEY (PaymentMethodTypeID)
);
GO



--************************************** Service
CREATE TABLE Service
(
 ServiceID     INT IDENTITY (1, 1) NOT NULL ,
 ServiceName   NVARCHAR(20) NOT NULL

 CONSTRAINT PK_Service PRIMARY KEY (ServiceID)
);
GO



--************************************** Customer
CREATE TABLE Customer
(
 CustomerID    INT IDENTITY (1, 1) NOT NULL ,
 FirstName     NVARCHAR(20) NOT NULL ,
 LastName      NVARCHAR(20) NOT NULL ,
 QuotedRate     NUMERIC(4,2) NULL ,
 CustomerSince DATE NULL ,
 Notes         NVARCHAR(500) NULL ,

 CONSTRAINT PK_Customer PRIMARY KEY (CustomerID)
);
GO



--************************************** ContactMethodType
CREATE TABLE ContactMethodType
(
 ContactMethodTypeID   INT IDENTITY (1, 1) NOT NULL ,
 ContactMethodTypeName NVARCHAR(20) NOT NULL ,

 CONSTRAINT PK_PhoneType PRIMARY KEY (ContactMethodTypeID)
);
GO



--************************************** RoomAmenity
CREATE TABLE RoomAmenity
(
 RoomAmenityID INT IDENTITY (1, 1) NOT NULL ,
 AmenityName   NVARCHAR(20) NOT NULL ,

 CONSTRAINT PK_RoomAmenity PRIMARY KEY (RoomAmenityID)
);
GO



--************************************** RoomType
CREATE TABLE RoomType
(
 RoomTypeID INT IDENTITY(1,1) NOT NULL,
 ParentRoomTypeID INT NULL,
 RoomTypeName NVARCHAR(20) NOT NULL,
 MaxOccupants TINYINT NOT NULL,
 IsBookable BIT NOT NULL DEFAULT 0,
 BedsKingNr TINYINT NULL,
 BedsQueenNr TINYINT NULL,
 BedsTwinNr TINYINT NULL,
 BedsRollAwayNr TINYINT NULL,

 CONSTRAINT PK_RoomType PRIMARY KEY (RoomTypeID),
 CONSTRAINT FK_RoomType_RoomType FOREIGN KEY (ParentRoomTypeID)
  REFERENCES RoomType(RoomTypeID)
);
GO



--************************************** RoomFloor
CREATE TABLE RoomFloor
(
 RoomFloorID INT IDENTITY (1, 1) NOT NULL ,
 FloorName   NVARCHAR(20) NOT NULL ,
 FloorLevel  TINYINT NOT NULL ,

 CONSTRAINT PK_Floor PRIMARY KEY (RoomFloorID)
);
GO



--************************************** RatePeriod
CREATE TABLE RatePeriod
(
 RatePeriodID INT IDENTITY (1, 1) NOT NULL ,
 RatePeriodDescr		NVARCHAR(20) NOT NULL ,
 StartDate      DATE NOT NULL ,
 EndDate        DATE NULL ,

  CONSTRAINT PK_RatePeriod PRIMARY KEY (RatePeriodID)
);
GO



--************************************** ServiceRate
CREATE TABLE ServiceRate
(
 ServiceRateID     INT IDENTITY (1, 1) NOT NULL ,
 ServiceID        INT NOT NULL ,
 RatePeriodID	  INT NOT NULL ,
 ServiceRateAmount NUMERIC(8,2) NOT NULL

 CONSTRAINT PK_ServiceRate PRIMARY KEY (ServiceRateID),
 CONSTRAINT FK_ServiceRate_Service FOREIGN KEY (ServiceID)
  REFERENCES Service(ServiceID),
 CONSTRAINT FK_ServiceRate_RatePeriod FOREIGN KEY (RatePeriodID)
  REFERENCES RatePeriod(RatePeriodID)
);
GO



--************************************** InvoiceLineItems
CREATE TABLE InvoiceLineItems
(
 InvoiceLineItemID     INT IDENTITY (1, 1) NOT NULL ,
 InvoiceID             INT NOT NULL ,
 ItemAmount            NUMERIC(8,2) NOT NULL ,
 ItemDescr             NVARCHAR(20) NOT NULL ,
 InvoiceLineItemTypeID INT NOT NULL ,

 CONSTRAINT PK_InvoiceLineItems PRIMARY KEY (InvoiceLineItemID),
 CONSTRAINT FK_Invoice_InvoiceLineItems FOREIGN KEY (InvoiceID)
  REFERENCES Invoice(InvoiceID),
 CONSTRAINT FK_InvoiceLineItemType_InvoiceLineItems FOREIGN KEY (InvoiceLineItemTypeID)
  REFERENCES InvoiceLineItemType(InvoiceLineItemTypeID)
);
GO



--************************************** Promo
CREATE TABLE Promo
(
 PromoCode          VARCHAR(20) NOT NULL ,
 PromoTypeID        INT NOT NULL ,
 PromoDiscountFlag  BIT NOT NULL ,
 PromoDiscountValue NUMERIC(8,2) NOT NULL ,
 PromoDescr         NVARCHAR(20) NOT NULL ,
 StartDate          DATE NOT NULL ,
 EndDate            DATE NULL ,

 CONSTRAINT PK_PomotionCode PRIMARY KEY (PromoCode),
 CONSTRAINT FK_Promo_PromoDiscountTypeo FOREIGN KEY (PromoDiscountFlag)
  REFERENCES PromoDiscountType(PromoDiscountFlag),
 CONSTRAINT FK_Promo_PromoType FOREIGN KEY (PromoTypeID)
  REFERENCES PromoType(PromoTypeID)
);
GO



--************************************** CustomerPaymentMethod
CREATE TABLE CustomerPaymentMethod
(
 CustomerPaymentMethodID   INT IDENTITY (1, 1) NOT NULL ,
 CustomerID          INT NOT NULL ,
 PaymentMethodTypeID INT NOT NULL ,
 PaymentDetail       NVARCHAR(12) NOT NULL ,
 PaymentExpiration   DATE NOT NULL ,
 IsPreferredMethod   BIT NOT NULL CONSTRAINT DF_CustomerPaymentMethod_IsPreferredMethod DEFAULT 0 ,

 CONSTRAINT PK_CustomerPayment PRIMARY KEY (CustomerPaymentMethodID),
 CONSTRAINT FK_CustomerPaymentMethod_Customer FOREIGN KEY (CustomerID)
  REFERENCES Customer(CustomerID),
 CONSTRAINT FK_CustomerPaymentMethod_PaymentMethodType FOREIGN KEY (PaymentMethodTypeID)
  REFERENCES PaymentMethodType(PaymentMethodTypeID)
);
GO



--************************************** RoomTypeRate
CREATE TABLE RoomTypeRate
(
 RoomTypeRateID INT IDENTITY (1, 1) NOT NULL ,
 RoomTypeID     INT NOT NULL ,
 RatePeriodID INT NOT NULL ,
 RoomRateAmount     NUMERIC(8,2) NOT NULL ,

 CONSTRAINT PK_RoomTypeRate PRIMARY KEY (RoomTypeRateID),
 CONSTRAINT FK_RoomTypeRate_RoomType FOREIGN KEY (RoomTypeID)
  REFERENCES RoomType(RoomTypeID),
 CONSTRAINT FK_RoomTypeRate_RatePeriod FOREIGN KEY (RatePeriodID)
  REFERENCES RatePeriod(RatePeriodID)
);
GO



--************************************** RoomTypeAmenity
CREATE TABLE RoomTypeAmenity
(
 RoomAmenityID INT IDENTITY (1,1) NOT NULL ,
 RoomTypeID    INT NOT NULL ,

 CONSTRAINT PK_RoomTypeAmenity PRIMARY KEY CLUSTERED (RoomAmenityID, RoomTypeID),
 CONSTRAINT FK_RoomTypeAmenity_RoomAmenity FOREIGN KEY (RoomAmenityID)
  REFERENCES RoomAmenity(RoomAmenityID),
 CONSTRAINT FK_RoomTypeAmenity_RoomType FOREIGN KEY (RoomTypeID)
  REFERENCES RoomType(RoomTypeID)
);
GO



--************************************** Guest
CREATE TABLE Guest
(
 GuestID    INT IDENTITY(1,1) NOT NULL ,
 Age        INT NOT NULL ,
 FirstName  NVARCHAR(20) NULL ,
 LastName   NVARCHAR(20) NULL ,
 CustomerID INT NULL ,

 CONSTRAINT PK_Guest PRIMARY KEY (GuestID),
 CONSTRAINT FK_Guest_Customer FOREIGN KEY (CustomerID)
  REFERENCES Customer(CustomerID)
);
GO



--************************************** CustomerContactMethod
CREATE TABLE CustomerContactMethod
(
 CustomerContactMethodID     INT IDENTITY (1, 1) NOT NULL ,
 CustomerID          INT NOT NULL ,
 ContactMethodTypeID INT NOT NULL ,
 ContactDetail       NVARCHAR(30) NOT NULL ,
 IsPreferredMethod   BIT NOT NULL CONSTRAINT DF_CustomerContactMethod_IsPreferredMethod DEFAULT 0 ,

 CONSTRAINT PK_CustomerPhone PRIMARY KEY (CustomerContactMethodID),
 CONSTRAINT FK_CustomerContactMethod_ContactMethodType FOREIGN KEY (ContactMethodTypeID)
  REFERENCES ContactMethodType(ContactMethodTypeID),
 CONSTRAINT FK_CustomerContactMethod_Customer FOREIGN KEY (CustomerID)
  REFERENCES Customer(CustomerID)
);
GO



--************************************** Room
CREATE TABLE Room
(
 RoomID      INT IDENTITY (1, 1) NOT NULL ,
 RoomFloorID INT NOT NULL ,
 RoomTypeID  INT NOT NULL ,
 StatusCode  CHAR(2) NOT NULL ,
 RoomNumber  NVARCHAR(5) NOT NULL ,

 CONSTRAINT PK_table_6 PRIMARY KEY (RoomID),
 CONSTRAINT FK_Room_RoomFloor FOREIGN KEY (RoomFloorID)
  REFERENCES RoomFloor(RoomFloorID),
 CONSTRAINT FK_Room_RoomType FOREIGN KEY (RoomTypeID)
  REFERENCES RoomType(RoomTypeID),
 CONSTRAINT FK_Room_RoomStatusCode FOREIGN KEY (StatusCode)
  REFERENCES RoomStatusCode(StatusCode)
);
GO



--************************************** RoomExtraAmenity
CREATE TABLE RoomExtraAmenity
(
 RoomAmenityID INT IDENTITY (1,1) NOT NULL ,
 RoomID    INT NOT NULL ,

 CONSTRAINT PK_RoomExtraAmenity PRIMARY KEY CLUSTERED (RoomAmenityID, RoomID),
 CONSTRAINT FK_RoomExtraAmenity_RoomAmenity FOREIGN KEY (RoomAmenityID)
  REFERENCES RoomAmenity(RoomAmenityID),
 CONSTRAINT FK_RoomExtraAmenity_Room FOREIGN KEY (RoomID)
  REFERENCES Room(RoomID)
);
GO



--************************************** Reservation
CREATE TABLE Reservation
(
 ReservationID INT IDENTITY (1, 1) NOT NULL ,
 CustomerID    INT NOT NULL ,
 FirstBooked   DATETIME2(7) NOT NULL ,
 LastUpdated   DATETIME2(7) NOT NULL ,
 Notes         NVARCHAR(500) NULL ,
 DateCancelled DATE NULL ,
 PromoCode     VARCHAR(20) NULL ,

 CONSTRAINT PK_Reservation PRIMARY KEY (ReservationID),
 CONSTRAINT FK_Reservation_Promo FOREIGN KEY (PromoCode)
  REFERENCES Promo(PromoCode),
 CONSTRAINT FK_Reservation_Customer FOREIGN KEY (CustomerID)
  REFERENCES Customer(CustomerID)
);
GO


--************************************** ReservationGuest
CREATE TABLE ReservationGuest
(
 ReservationID INT NOT NULL ,
 GuestID       INT NOT NULL ,

 CONSTRAINT PK_ReservationGuest PRIMARY KEY CLUSTERED (ReservationID, GuestID),
 CONSTRAINT FK_ReservationGuest_Reservation FOREIGN KEY (ReservationID)
  REFERENCES Reservation(ReservationID),
 CONSTRAINT FK_ReservationGuest_Guest FOREIGN KEY (GuestID)
  REFERENCES Guest(GuestID)
);
GO



--************************************** ReservationRoom
CREATE TABLE ReservationRoom
(
 ReservationID  INT NOT NULL ,
 RoomID         INT NOT NULL ,
 RateWhenBooked NUMERIC(8,2) NOT NULL ,
 CheckInDate    DATE NOT NULL ,
 CheckOutDate   DATE NOT NULL ,
 RateQuoted     NUMERIC(8,2) NULL ,
 Notes          NVARCHAR(500) NULL ,

 CONSTRAINT PK_ReservationRoom PRIMARY KEY CLUSTERED (ReservationID, RoomID),
 CONSTRAINT FK_ReservationRoom_Reservation FOREIGN KEY (ReservationID)
  REFERENCES Reservation(ReservationID),
 CONSTRAINT FK_ReservationRoom_Room FOREIGN KEY (RoomID)
  REFERENCES Room(RoomID)
);
GO



--************************************** ReservationRoomService
CREATE TABLE ReservationRoomService
(
 ReservationID       INT NOT NULL ,
 RoomID              INT NOT NULL ,
 ServiceID           INT NOT NULL ,
 ServiceDeliveryTime DATETIME2(7) NOT NULL ,
 ServiceDeliveryNote NVARCHAR(200) NULL ,
 ServiceFeeQuoted    NUMERIC(8,2) NULL ,
 GuestID             INT NULL ,
 PromoCode           VARCHAR(20) NULL ,

 CONSTRAINT PK_ReservationService PRIMARY KEY CLUSTERED (ReservationID, RoomID, ServiceID),
 CONSTRAINT FK_ReservationRoomService_ReservationRoom FOREIGN KEY (ReservationID, RoomID)
  REFERENCES ReservationRoom(ReservationID, RoomID),
 CONSTRAINT FK_ReservationRoomService_Service FOREIGN KEY (ServiceID)
  REFERENCES Service(ServiceID),
 CONSTRAINT FK_ReservationRoomService_Promo FOREIGN KEY (PromoCode)
  REFERENCES Promo(PromoCode)
);
GO


--==DML SCRIPT=======================================================================================================================================

--************************************** RoomType
SET IDENTITY_INSERT RoomType ON; 
GO

--************************************** (Parent Rows)
INSERT RoomType (RoomTypeID, ParentRoomTypeID, RoomTypeName, MaxOccupants, IsBookable, BedsKingNr, BedsQueenNr, BedsTwinNr, BedsRollAwayNr)
	VALUES
	(1, NULL, 'Single', 3, 0, NULL, NULL, NULL, NULL),
	(2, NULL, 'Double', 5, 0, NULL, NULL, NULL, NULL);
GO

--************************************** (NonParent Rows)
INSERT RoomType (RoomTypeID, ParentRoomTypeID, RoomTypeName, MaxOccupants, IsBookable, BedsKingNr, BedsQueenNr, BedsTwinNr, BedsRollAwayNr)
	VALUES
	(3, 1, 'Small Single ', 1, 1, NULL, NULL, 1, NULL),
	(4, 2, 'Small Double', 2, 1, NULL, NULL, 2, NULL),
	(5, 1, 'Standard Single', 3, 1, NULL, 1, NULL, 1),
	(6, 2, 'Standard Double', 5, 1, NULL, 2, NULL, 1),
	(7, 1, 'Deluxe Single', 3, 1, 1, NULL, NULL, 1),
	(8, 2, 'Deluxe Double', 5, 1, 2, NULL, NULL, 1),
	(9, 2, 'Standard Loft', 4, 0, NULL, 2, NULL, NULL),
	(10, 2, 'Deluxe Loft', 4, 1, 2, NULL, NULL, NULL);
GO

SET IDENTITY_INSERT RoomType OFF;
GO


--************************************** RoomAmenity
SET IDENTITY_INSERT RoomAmenity ON;
GO

INSERT RoomAmenity (RoomAmenityID, AmenityName)
	VALUES
	(1, 'Minibar'),
	(2, 'Jacuzzi'),
	(3, 'Ocean View'),
	(4, 'City View'),
	(5, 'Wifi'),
	(6, 'Air Conditioning'),
	(7, 'Safe'),
	(8, 'Balcony');
GO

SET IDENTITY_INSERT RoomAmenity OFF;
GO



--************************************** RoomTypeAmenity
SET IDENTITY_INSERT RoomTypeAmenity ON ;
GO

INSERT RoomTypeAmenity (RoomAmenityID, RoomTypeID)
	VALUES
	(1, 3),
	(1, 4),
	(1, 5),
	(1, 6),
	(1, 7),
	(1, 8),
	(1, 9),
	(1, 10),
	(2, 7),
	(2, 8),
	(2, 10),
	(5, 3),
	(5, 4),
	(5, 5),
	(5, 6),
	(5, 7),
	(5, 8),
	(5, 9),
	(5, 10),
	(6, 3),
	(6, 4),
	(6, 5),
	(6, 6),
	(6, 7),
	(6, 8),
	(6, 9),
	(6, 10),
	(7, 7),
	(7, 8),
	(7, 10);
GO

SET IDENTITY_INSERT RoomTypeAmenity OFF;
GO



--************************************** RatePeriod
SET IDENTITY_INSERT RatePeriod ON;
GO

INSERT RatePeriod (RatePeriodID, RatePeriodDescr, StartDate, EndDate)
	VALUES
	(1,'Low Season 2017/18','2017-11-01', '2018-04-12'),
	(2,'Derby 2018','2018-04-13', '2018-05-06'),
	(3,'High Season 2018','2018-05-07', '2018-10-31'),
	(4,'Low Season 2018/19','2018-11-01', '2019-04-11')
GO

SET IDENTITY_INSERT RatePeriod OFF;
GO


--************************************** RoomTypeRate
SET IDENTITY_INSERT RoomTypeRate ON; 
GO

INSERT RoomTypeRate (RoomTypeRateID, RoomTypeID, RatePeriodID, RoomRateAmount)
	VALUES
	(1, 3, 1, '50.00'),
	(2, 4, 1, '80.00'),
	(3, 5, 1, '75.00'),
	(4, 6, 1, '95.00'),
	(5, 7, 1, '90.00'),
	(6, 8, 1, '110.00'),
	(7, 3, 2, '80.00'),
	(8, 4, 2, '110.00'),
	(9, 5, 2, '105.00'),
	(10, 6, 2, '125.00'),
	(11, 7, 2, '120.00'),
	(12, 8, 2, '140.00'),
	(13, 3, 3, '70.00'),
	(14, 4, 3, '100.00'),
	(15, 5, 3, '95.00'),
	(16, 6, 3, '115.00'),
	(17, 7, 3, '110.00'),
	(18, 8, 3, '130.00'),
	(19, 3, 4, '55.00'),
	(20, 4, 4, '85.00'),
	(21, 5, 4, '80.00'),
	(22, 6, 4, '100.00'),
	(23, 7, 4, '95.00'),
	(24, 8, 4, '115.00'),
	(25, 9, 1, '130.00'),
	(26, 9, 2, '160.00'),
	(27, 9, 3, '150.00'),
	(28, 9, 4, '135.00'),
	(29, 10, 1, '150.00'),
	(30, 10, 2, '180.00'),
	(31, 10, 3, '170.00'),
	(32, 10, 4, '155.00');
GO

SET IDENTITY_INSERT RoomTypeRate OFF;
GO


--************************************** Service
SET IDENTITY_INSERT Service ON;
GO

INSERT Service (ServiceID, ServiceName)
	VALUES
	(1, 'Room Service'),
	(2, 'Roll-Away Bed'),
	(3, 'Movie Rental'),
	(4, 'Room Charge'),
	(5, '30 Min. Massage'),
	(6, '60 Min. Massage'),
	(7, 'Laundry');
GO

SET IDENTITY_INSERT Service OFF;
GO


--************************************** ServiceRate
SET IDENTITY_INSERT ServiceRate ON; 
GO

INSERT ServiceRate (ServiceRateID, ServiceID, RatePeriodID, ServiceRateAmount)
	VALUES
	(1, 1, 1, '2.00'),
	(2, 2, 1, '15.00'),
	(3, 3, 1, '5.00'),
	(4, 4, 1, '0.20'),
	(5, 1, 2, '2.00'),
	(6, 2, 2, '15.00'),
	(7, 3, 2, '5.00'),
	(8, 4, 2, '0.20'),
	(9, 1, 3, '2.00'),
	(10, 2, 3, '15.00'),
	(11, 3, 3, '5.00'),
	(12, 4, 3, '0.20'),
	(13, 1, 4, '2.00'),
	(14, 2, 4, '15.00'),
	(15, 3, 4, '5.00'),
	(16, 4, 4, '0.20'),
	(17, 5, 1, '40.00'),
	(18, 5, 2, '50.00'),
	(19, 5, 3, '45.00'),
	(20, 5, 4, '42.50'),
	(21, 6, 1, '80.00'),
	(22, 6, 2, '100.00'),
	(23, 6, 3, '90.00'),
	(24, 6, 4, '85.00'),
	(25, 7, 1, '10.00'),
	(26, 7, 2, '15.00'),
	(27, 7, 3, '12.00'),
	(28, 7, 4, '11.00');
GO

SET IDENTITY_INSERT ServiceRate OFF; 
GO



--************************************** RoomFloor
SET IDENTITY_INSERT RoomFloor ON;
GO

INSERT RoomFloor (RoomFloorID, FloorName, FloorLevel)
	VALUES
	(1, '1 West', 1),
	(2, '1 East', 1),
	(3, '2 West', 2),
	(4, '2 East', 2),
	(5, '3 Loft', 3);
GO

SET IDENTITY_INSERT RoomFloor OFF;
GO



--************************************** RoomStatusCode
INSERT RoomStatusCode (StatusCode, StatusCodeName)
	VALUES
	('OC', 'Occupied'),
	('VA', 'Vacant'),
	('PR', 'Prepping Room'),
	('BH', 'Block Hold'),
	('RP', 'Repair Pending');
GO



--************************************** Room
SET IDENTITY_INSERT Room ON ;
GO

INSERT Room (RoomID, RoomFloorID, RoomTypeID, StatusCode, RoomNumber)
	VALUES
	(1, 1, 3, 'RP', '1W1'),
	(2, 1, 4, 'VA', '1W2'),
	(3, 1, 5, 'VA', '1W3'),
	(4, 1, 6, 'VA', '1W4'),
	(5, 1, 3, 'VA', '1W5'),
	(6, 1, 4, 'VA', '1W6'),
	(7, 2, 3, 'VA', '1E1'),
	(8, 2, 4, 'VA', '1E2'),
	(9, 2, 5, 'VA', '1E3'),
	(10, 2, 6, 'VA', '1E4'),
	(11, 2, 3, 'VA', '1E5'),
	(12, 2, 4, 'VA', '1E6'),
	(13, 3, 5, 'VA', '2W1'),
	(14, 3, 6, 'VA', '2W2'),
	(15, 3, 7, 'VA', '2W3'),
	(16, 3, 7, 'VA', '2W4'),
	(17, 3, 8, 'VA', '2W5'),
	(18, 4, 5, 'VA', '2E1'),
	(19, 4, 6, 'VA', '2E2'),
	(20, 4, 7, 'VA', '2E3'),
	(21, 4, 7, 'VA', '2E4'),
	(22, 4, 8, 'VA', '2E5'),
	(23, 5, 9, 'VA', '3L1'),
	(24, 5, 9, 'VA', '3L2'),
	(25, 5, 9, 'VA', '3L3'),
	(26, 5, 9, 'VA', '3L4'),
	(27, 5, 9, 'VA', '3L5'),
	(28, 5, 10, 'VA', '3L6'),
	(29, 5, 10, 'VA', '3L7'),
	(30, 5, 10, 'BH', '3L8'),
	(31, 5, 10, 'BH', '3L9'),
	(32, 5, 10, 'BH', '3L10')
GO

SET IDENTITY_INSERT Room OFF;
GO



--************************************** RoomExtraAmenity
SET IDENTITY_INSERT RoomExtraAmenity ON ;
GO

INSERT RoomExtraAmenity (RoomAmenityID, RoomID)
	VALUES
	(3, 1),
	(3, 2),
	(3, 3),
	(3, 4),
	(3, 5),
	(3, 6),
	(4, 7),
	(4, 8),
	(4, 9),
	(4, 10),
	(4, 11),
	(4, 12),
	(3, 13),
	(3, 14),
	(3, 15),
	(3, 16),
	(3, 17),
	(4, 18),
	(4, 19),
	(4, 20),
	(4, 21),
	(4, 22),
	(4, 23),
	(4, 24),
	(4, 25),
	(4, 26),
	(4, 27),
	(3, 28),
	(3, 29),
	(3, 30),
	(3, 31),
	(3, 32),
	(8, 23),
	(8, 24),
	(8, 25),
	(8, 26),
	(8, 27),
	(8, 28),
	(8, 29),
	(8, 30),
	(8, 31),
	(8, 32);
GO

SET IDENTITY_INSERT RoomExtraAmenity OFF;
GO



--************************************** PromoType
SET IDENTITY_INSERT PromoType ON;
GO

INSERT PromoType (PromoTypeID, PromoType)
	VALUES
	(1, 'Reservation Promo'),
	(2, 'Service Promo');
GO

SET IDENTITY_INSERT PromoType OFF;
GO



--************************************** PromoDiscountType
INSERT PromoDiscountType (PromoDiscountFlag, PromoDiscountType)
	VALUES
	(0, 'Percent Amount'),
	(1, 'Flat Amount');
GO



--************************************** PromoType
INSERT Promo (PromoCode, PromoTypeID, PromoDiscountFlag, PromoDiscountValue, PromoDescr, StartDate, EndDate)
	VALUES
	('LoftyDerby30', 1, 1, '-30', '$30 off loft rooms', '2018-05-01', '2018-05-06'),
	('DerbyDaze10', 2, 0, '-.1', '10% off all services', '2018-05-01', '2018-05-06'),
	('Spring25Fever', 1, 1, '-.25', '25% discount', '2018-04-01', '2018-04-30'),
	('Big25Fall', 1, 1, '-25', '$25 off all rooms', '2018-09-01', '2018-09-30');
GO



--************************************** ContactMethodType
SET IDENTITY_INSERT ContactMethodType ON;
GO

INSERT ContactMethodType (ContactMethodTypeID, ContactMethodTypeName)
	VALUES
	(1, 'Home Phone'),
	(2, 'Mobile Phone'),
	(3, 'Work Phone'),
	(4, 'Home Phone'),
	(5, 'Work Phone');
GO

SET IDENTITY_INSERT ContactMethodType OFF;
GO



--************************************** PaymentMethodType
SET IDENTITY_INSERT PaymentMethodType ON;
GO

INSERT PaymentMethodType (PaymentMethodTypeID, PaymentMethodTypeName)
	VALUES
	(1, 'Visa'),
	(2, 'MasterCard'),
	(3, 'Amex');
	GO

SET IDENTITY_INSERT PaymentMethodType OFF;
GO



--************************************** Customer
SET IDENTITY_INSERT Customer ON;
GO

INSERT Customer (CustomerID, FirstName, LastName, QuotedRate, CustomerSince, Notes)
	VALUES
	(1, 'Joe', 'Cool', NULL, '2000-01-01', NULL),
	(2, 'Dude', 'Awesome', -0.1, '2000-01-02', NULL),
	(3, 'Jim', 'Smith', NULL, '2000-01-03', NULL),
	(4, 'Jane', 'Doe', NULL, '2000-01-04', NULL),
	(5, 'Ola', 'Nordmann', NULL, '2016-01-01', NULL),
	(6, 'Sverre', 'Svenske', NULL, '2017-01-01', NULL),
	(7, 'Markus', 'Dane', NULL, '2018-01-01', NULL),
	(8, 'Johnny', 'Rotten', .5, NULL, 'This guy ALWAYS TRASHES some rooms!'),
	(9, 'David', 'Bowie', -0.5, '1969-01-01', 'Our OLDEST, BESTEST customer!!!'),
	(10, 'Mark', 'Knopfler', -0.15, NULL, NULL),
	(11, 'David-Lee', 'Roth', -0.1, NULL, NULL),
	(12, 'Jesus','Kristus',-1, '0001-01-01','We NEVER charge Jesus!');
GO

SET IDENTITY_INSERT Customer OFF;
GO



--************************************** CustomerContactMethod
SET IDENTITY_INSERT CustomerContactMethod ON;
GO

INSERT CustomerContactMethod (CustomerContactMethodID, CustomerID, ContactMethodTypeID, ContactDetail, IsPreferredMethod)
	VALUES
	(1, 1, 1, '503-555-1212', 0);
GO

SET IDENTITY_INSERT CustomerContactMethod OFF;
GO



--************************************** CustomerPaymentMethod
SET IDENTITY_INSERT CustomerPaymentMethod ON;
GO

INSERT CustomerPaymentMethod (CustomerPaymentMethodID, CustomerID, PaymentMethodTypeID, PaymentDetail, PaymentExpiration, IsPreferredMethod)
	VALUES
	(1, 1, 1, '123456789012', '2021-12-01', 1);
GO

SET IDENTITY_INSERT CustomerPaymentMethod OFF;
GO


--************************************** Guest
SET IDENTITY_INSERT [dbo].[Guest] ON 
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (1, 70, N'David', N'Bowie', 9)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (2, 62, N'Johnny', N'Rotten', 8)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (3, 22, N'JJ', N'Lydon', NULL)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (4, 49, N'Andrew', N'Bolton', NULL)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (5, 59, N'Markus', N'Dane', 7)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (6, 34, N'Jim', N'Smith', 3)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (7, 26, N'Jane', N'Doe', 4)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (8, 25, N'Jonathan', N'Doe', NULL)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (9, 37, N'Joe', N'Cool', 1)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (10, 35, N'Jo-Anne', N'Cool', NULL)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (11, 3, N'child', NULL, NULL)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (12, 5, N'child', NULL, NULL)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (13, 39, N'Dude', N'Awesome', 2)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (14, 49, N'Ola', N'Nordmann', 5)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (15, 44, N'Oletta', N'Bestevenn', NULL)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (16, 22, N'Sturla', N'Nordmann', NULL)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (17, 20, N'Siri', N'Skrevs', NULL)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (18, 47, N'Sverre', N'Svenske', 6)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (19, 47, N'Carolla', N'Svenske', NULL)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (20, 68, N'Mark', N'Knopfler', 10)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (21, 66, N'David-Lee', N'Roth', 11)
GO
INSERT [dbo].[Guest] ([GuestID], [Age], [FirstName], [LastName], [CustomerID]) VALUES (22, 2018, N'Jesus', N'Kristus', 12)
GO
SET IDENTITY_INSERT [dbo].[Guest] OFF
GO


--************************************** Reservation
SET IDENTITY_INSERT [dbo].[Reservation] ON 
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (1, 9, CAST(N'2015-12-01T00:00:00.0000000' AS DateTime2), CAST(N'2016-02-01T00:00:00.0000000' AS DateTime2), N'Last Stay with us :-(', NULL, NULL)
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (2, 9, CAST(N'2016-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2016-02-01T00:00:00.0000000' AS DateTime2), N'Cancelled (deceased)', CAST(N'2016-02-01' AS Date), NULL)
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (3, 8, CAST(N'2017-11-01T00:00:00.0000000' AS DateTime2), CAST(N'2017-12-31T00:00:00.0000000' AS DateTime2), N'ALWAYS trashes rooms', NULL, NULL)
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (4, 7, CAST(N'2018-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2018-01-15T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (5, 3, CAST(N'2018-04-01T00:00:00.0000000' AS DateTime2), CAST(N'2018-04-15T00:00:00.0000000' AS DateTime2), NULL, NULL, N'Spring25Fever')
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (6, 4, CAST(N'2018-04-02T00:00:00.0000000' AS DateTime2), CAST(N'2018-04-10T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (7, 1, CAST(N'2018-04-01T00:00:00.0000000' AS DateTime2), CAST(N'2018-04-09T00:00:00.0000000' AS DateTime2), NULL, NULL, N'Spring25Fever')
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (8, 2, CAST(N'2018-04-05T00:00:00.0000000' AS DateTime2), CAST(N'2018-04-09T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (9, 5, CAST(N'2018-04-01T00:00:00.0000000' AS DateTime2), CAST(N'2018-05-10T00:00:00.0000000' AS DateTime2), NULL, NULL, N'LoftyDerby30')
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (10, 6, CAST(N'2018-04-01T00:00:00.0000000' AS DateTime2), CAST(N'2018-05-08T00:00:00.0000000' AS DateTime2), NULL, NULL, N'DerbyDaze10')
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (11, 7, CAST(N'2018-04-01T00:00:00.0000000' AS DateTime2), CAST(N'2018-05-08T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (12, 10, CAST(N'2018-05-10T00:00:00.0000000' AS DateTime2), CAST(N'2018-05-25T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (13, 11, CAST(N'2018-05-10T00:00:00.0000000' AS DateTime2), CAST(N'2018-05-22T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (14, 4, CAST(N'2018-06-01T00:00:00.0000000' AS DateTime2), CAST(N'2018-06-15T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (15, 3, CAST(N'2018-06-10T00:00:00.0000000' AS DateTime2), CAST(N'2018-06-19T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (16, 7, CAST(N'2018-07-01T00:00:00.0000000' AS DateTime2), CAST(N'2018-07-21T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (17, 1, CAST(N'2018-08-09T00:00:00.0000000' AS DateTime2), CAST(N'2018-08-28T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (18, 2, CAST(N'2018-11-03T00:00:00.0000000' AS DateTime2), CAST(N'2018-11-29T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (19, 4, CAST(N'2018-12-01T00:00:00.0000000' AS DateTime2), CAST(N'2018-12-31T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[Reservation] ([ReservationID], [CustomerID], [FirstBooked], [LastUpdated], [Notes], [DateCancelled], [PromoCode]) VALUES (20, 12, CAST(N'2018-12-25T00:00:00.0000000' AS DateTime2), CAST(N'2018-12-26T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Reservation] OFF
GO


--************************************** ReservationGuest
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (1, 1)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (2, 1)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (3, 2)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (3, 3)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (3, 4)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (4, 5)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (5, 6)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (6, 7)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (6, 8)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (7, 9)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (7, 10)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (7, 11)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (7, 12)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (8, 13)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (9, 14)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (9, 15)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (9, 16)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (9, 17)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (10, 18)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (10, 19)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (11, 5)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (12, 20)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (13, 21)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (14, 7)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (14, 8)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (15, 6)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (16, 5)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (17, 9)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (17, 10)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (17, 11)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (17, 12)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (18, 13)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (19, 7)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (19, 8)
GO
INSERT [dbo].[ReservationGuest] ([ReservationID], [GuestID]) VALUES (20, 22)
GO


--************************************** ReservationRoom
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (1, 31, CAST(120.00 AS Numeric(8, 2)), CAST(N'2016-01-01' AS Date), CAST(N'2016-01-08' AS Date), CAST(60.00 AS Numeric(8, 2)), N'Bowie 1/2 price!')
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (1, 32, CAST(120.00 AS Numeric(8, 2)), CAST(N'2016-01-01' AS Date), CAST(N'2016-01-08' AS Date), CAST(60.00 AS Numeric(8, 2)), N'Bowie 1/2 price!')
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (3, 30, CAST(150.00 AS Numeric(8, 2)), CAST(N'2017-12-05' AS Date), CAST(N'2017-12-06' AS Date), NULL, N'Manager: NOT TRASHED')
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (3, 31, CAST(150.00 AS Numeric(8, 2)), CAST(N'2017-12-05' AS Date), CAST(N'2017-12-09' AS Date), CAST(350.00 AS Numeric(8, 2)), N'Manager: +$200/day - TRASHED')
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (3, 32, CAST(150.00 AS Numeric(8, 2)), CAST(N'2017-12-05' AS Date), CAST(N'2017-12-09' AS Date), CAST(350.00 AS Numeric(8, 2)), N'Manager: +$200/day - TRASHED')
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (4, 16, CAST(90.00 AS Numeric(8, 2)), CAST(N'2018-01-01' AS Date), CAST(N'2018-01-30' AS Date), CAST(75.00 AS Numeric(8, 2)), N'Manager: -$15/day')
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (5, 18, CAST(75.00 AS Numeric(8, 2)), CAST(N'2018-04-01' AS Date), CAST(N'2018-04-15' AS Date), NULL, NULL)
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (6, 22, CAST(110.00 AS Numeric(8, 2)), CAST(N'2018-04-05' AS Date), CAST(N'2018-04-10' AS Date), NULL, NULL)
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (7, 15, CAST(90.00 AS Numeric(8, 2)), CAST(N'2018-04-04' AS Date), CAST(N'2018-04-09' AS Date), NULL, NULL)
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (8, 17, CAST(110.00 AS Numeric(8, 2)), CAST(N'2018-04-05' AS Date), CAST(N'2018-04-09' AS Date), CAST(100.00 AS Numeric(8, 2)), N'Manager: 10% Discount')
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (9, 28, CAST(180.00 AS Numeric(8, 2)), CAST(N'2018-05-01' AS Date), CAST(N'2018-05-10' AS Date), NULL, NULL)
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (9, 29, CAST(180.00 AS Numeric(8, 2)), CAST(N'2018-05-01' AS Date), CAST(N'2018-05-07' AS Date), NULL, NULL)
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (10, 19, CAST(125.00 AS Numeric(8, 2)), CAST(N'2018-05-01' AS Date), CAST(N'2018-05-08' AS Date), NULL, NULL)
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (11, 16, CAST(120.00 AS Numeric(8, 2)), CAST(N'2018-05-01' AS Date), CAST(N'2018-05-08' AS Date), NULL, NULL)
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (12, 28, CAST(170.00 AS Numeric(8, 2)), CAST(N'2018-05-20' AS Date), CAST(N'2018-05-25' AS Date), CAST(147.83 AS Numeric(8, 2)), N'Manager: 15% Discount')
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (13, 29, CAST(170.00 AS Numeric(8, 2)), CAST(N'2018-05-24' AS Date), CAST(N'2018-05-25' AS Date), CAST(154.55 AS Numeric(8, 2)), N'Manager: 10% Discount')
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (14, 22, CAST(130.00 AS Numeric(8, 2)), CAST(N'2018-06-05' AS Date), CAST(N'2018-06-10' AS Date), NULL, NULL)
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (15, 18, CAST(95.00 AS Numeric(8, 2)), CAST(N'2018-06-10' AS Date), CAST(N'2018-06-19' AS Date), NULL, NULL)
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (16, 21, CAST(110.00 AS Numeric(8, 2)), CAST(N'2018-06-07' AS Date), CAST(N'2018-06-21' AS Date), CAST(100.00 AS Numeric(8, 2)), N'Manager: -$10/day')
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (17, 28, CAST(170.00 AS Numeric(8, 2)), CAST(N'2018-09-20' AS Date), CAST(N'2018-09-28' AS Date), NULL, NULL)
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (17, 29, CAST(170.00 AS Numeric(8, 2)), CAST(N'2018-09-20' AS Date), CAST(N'2018-09-28' AS Date), NULL, NULL)
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (18, 17, CAST(115.00 AS Numeric(8, 2)), CAST(N'2018-11-23' AS Date), CAST(N'2018-11-29' AS Date), CAST(100.00 AS Numeric(8, 2)), N'Manager: 10% Discount')
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (19, 22, CAST(115.00 AS Numeric(8, 2)), CAST(N'2018-12-24' AS Date), CAST(N'2018-12-31' AS Date), NULL, NULL)
GO
INSERT [dbo].[ReservationRoom] ([ReservationID], [RoomID], [RateWhenBooked], [CheckInDate], [CheckOutDate], [RateQuoted], [Notes]) VALUES (20, 22, CAST(115.00 AS Numeric(8, 2)), CAST(N'2018-12-24' AS Date), CAST(N'2018-12-25' AS Date), CAST(0.00 AS Numeric(8, 2)), N'Manager: 100% Discount')
GO
