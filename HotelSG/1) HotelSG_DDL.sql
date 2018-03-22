
USE HOTELSG;
GO

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




