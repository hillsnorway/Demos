
use HOTELSG

--Reset the seed for a table...
--DBCC CHECKIDENT ('Guest', RESEED, 0);
--GO

--RoomType Parent/Child
Select p.RoomTypeName as ParentRoomType, c.RoomTypeName as ChildRoomType, c.MaxOccupants
From RoomType p
inner join RoomType c on p.RoomTypeID = c.ParentRoomTypeID
Where p.ParentRoomTypeID is null
Order By ParentRoomType DESC, c.MaxOccupants;

--AvailableRooms with summary of Amenities
Select r.RoomNumber, rt.isBookable, r.StatusCode, 
STUFF(
(Select ', '+AmenityName
From RoomAmenity ra
Inner Join RoomExtraAmenity rea on rea.RoomAmenityID = ra.RoomAmenityID
Where rea.RoomID = r.RoomID
For XML PATH(''))
,1,2,'') AS RoomExtraAmenities,
STUFF(
(Select ', '+AmenityName
From RoomAmenity ra
Inner Join RoomTypeAmenity rta on rta.RoomAmenityID = ra.RoomAmenityID
Where rta.RoomTypeID = rt.RoomTypeID
For XML PATH(''))
,1,2,'') AS RoomTypeAmenities,
rt.RoomTypeName, rt.MaxOccupants, rt.BedsKingNr, rt.BedsQueenNr, rt.BedsTwinNr
From RoomType rt
Inner Join Room r on r.RoomTypeID = rt.RoomTypeID
Where rt.IsBookable = 1 and r.StatusCode not in ('RP', 'BH') --and rt.RoomTypeID in (9,10);

--RoomRate for given Check-In Date
Select r.RoomId, r.RoomNumber, rt.RoomTypeName, rp.StartDate, rp.EndDate, rtr.RoomRateAmount
From RoomTypeRate rtr
Inner Join RoomType rt on rt.RoomTypeID = rtr.RoomTypeID
Inner Join Room r on r.RoomTypeID = rt.RoomTypeID
Inner Join RatePeriod rp on rp.RatePeriodID = rtr.RatePeriodID
Where r.RoomID between 1 and 32  and '2018-12-25' Between rp.StartDate and rp.EndDate
and rt.IsBookable = 1 and r.StatusCode not in ('RP', 'BH');

--All Rented Rooms By Date & Customer w/ RoomTypeRate lookup (and handling when RatePeriod undefined)
Select r.ReservationID, c.LastName+', '+c.FirstName as CustomerName, Room.RoomNumber, rr.CheckInDate, rr.CheckOutDate, rtr.RoomRateAmount, rr.RateWhenBooked, rr.RateQuoted
From Reservation r
Inner Join Customer c on c.CustomerID = r.CustomerID
Inner Join ReservationRoom rr on rr.ReservationID = r.ReservationID
Inner Join Room on Room.RoomID = rr.RoomID
Inner Join RoomType rt on rt.RoomTypeID = Room.RoomTypeID
Inner Join RoomTypeRate rtr on rtr.RoomTypeID = rt.RoomTypeID
Inner Join RatePeriod rp on rp.RatePeriodID = rtr.RatePeriodID and (rr.CheckinDate between rp.StartDate and rp.EndDate)
Union Select r.ReservationID, c.LastName+', '+c.FirstName as CustomerName, Room.RoomNumber, rr.CheckInDate, rr.CheckOutDate, NULL, rr.RateWhenBooked, rr.RateQuoted
From Reservation r
Inner Join Customer c on c.CustomerID = r.CustomerID
Inner Join ReservationRoom rr on rr.ReservationID = r.ReservationID
Inner Join Room on Room.RoomID = rr.RoomID
Inner Join RoomType rt on rt.RoomTypeID = Room.RoomTypeID
Where rr.CheckinDate not between (select min(rp.StartDate) from RatePeriod rp) and (select max(rp.EndDate) from RatePeriod rp) 
Order By CheckInDate Desc, CheckOutDate

--All Rooms Reserved For A Given Customer OR Given PromoCode
Select r.ReservationID, c.CustomerID, c.LastName+', '+c.FirstName as CustomerName, Room.RoomNumber, rr.CheckInDate, rr.CheckOutDate, r.PromoCode
From Reservation r
Inner Join Customer c on c.CustomerID = r.CustomerID
Inner Join ReservationRoom rr on rr.ReservationID = r.ReservationID
Inner Join Room on Room.RoomID = rr.RoomID
Where r.PromoCode like '%Derby%'
--Where c.CustomerID not in (1,3,5,6)

--List of PromoCodes and the number of times used (also allow specifying a date range)
Select p.PromoCode, Count(r.PromoCode)
From Promo p
Left Join Reservation r on p.PromoCode = r.PromoCode
Left Join ReservationRoom rr on rr.ReservationID = r.ReservationID
--Where rr.CheckInDate Between '2018-5-1' and '2018-5-10'
Group By p.PromoCode

--Summary of Available Rooms for a Date Range (and allow specifying a RoomType)
Select r.RoomID, r.RoomNumber, rt.RoomTypeName
From Room r
Inner Join RoomType rt on rt.RoomTypeID = r.RoomTypeID
Where rt.IsBookable = 1 and r.StatusCode not in ('RP', 'BH')
--and rt.RoomTypeName like '%single%'
and r.RoomID not in
(Select Distinct r.RoomID
From RoomType rt
Inner Join Room r on r.RoomTypeID = rt.RoomTypeID
Inner Join ReservationRoom rr on rr.RoomID = r.RoomID
Where rt.IsBookable = 1 and r.StatusCode not in ('RP', 'BH')
and ((rr.CheckInDate between '2018-5-6' and '2018-5-10') or (rr.CheckOutDate between '2018-5-6' and '2018-5-10')))

--Rooms that do NOT contain particular RoomTypeAmenities
Select r.RoomID, r.RoomNumber, rt.isBookable, r.StatusCode, 
STUFF(
(Select ', '+AmenityName
From RoomAmenity ra
Inner Join RoomExtraAmenity rea on rea.RoomAmenityID = ra.RoomAmenityID
Where rea.RoomID = r.RoomID
For XML PATH(''))
,1,2,'') AS RoomExtraAmenities,
STUFF(
(Select ', '+AmenityName
From RoomAmenity ra
Inner Join RoomTypeAmenity rta on rta.RoomAmenityID = ra.RoomAmenityID
Where rta.RoomTypeID = rt.RoomTypeID
For XML PATH(''))
,1,2,'') AS RoomTypeAmenities,
rt.RoomTypeName, rt.MaxOccupants, rt.BedsKingNr, rt.BedsQueenNr, rt.BedsTwinNr
From RoomType rt
Inner Join Room r on r.RoomTypeID = rt.RoomTypeID
Where r.RoomID not in
(Select Distinct r.RoomID
From Room r
Inner Join RoomType rt on r.RoomTypeID = rt.RoomTypeID
Inner Join RoomTypeAmenity rta on rt.RoomTypeID = rta.RoomTypeID
Inner Join RoomAmenity ra on ra.RoomAmenityID = rta.RoomAmenityID
Where ra.AmenityName in ('Safe', 'Jacuzzi'))
Order By r.RoomID

--Rooms that do NOT contain a particular RoomExtraAmenity
Select r.RoomID, r.RoomNumber, rt.isBookable, r.StatusCode, 
STUFF(
(Select ', '+AmenityName
From RoomAmenity ra
Inner Join RoomExtraAmenity rea on rea.RoomAmenityID = ra.RoomAmenityID
Where rea.RoomID = r.RoomID
For XML PATH(''))
,1,2,'') AS RoomExtraAmenities,
STUFF(
(Select ', '+AmenityName
From RoomAmenity ra
Inner Join RoomTypeAmenity rta on rta.RoomAmenityID = ra.RoomAmenityID
Where rta.RoomTypeID = rt.RoomTypeID
For XML PATH(''))
,1,2,'') AS RoomTypeAmenities,
rt.RoomTypeName, rt.MaxOccupants, rt.BedsKingNr, rt.BedsQueenNr, rt.BedsTwinNr
From RoomType rt
Inner Join Room r on r.RoomTypeID = rt.RoomTypeID
Where r.RoomID not in
(Select Distinct r.RoomID
From Room r
Inner Join RoomType rt on r.RoomTypeID = rt.RoomTypeID
Inner Join RoomExtraAmenity rea on r.RoomID = rea.RoomID
Inner Join RoomAmenity ra on ra.RoomAmenityID = rea.RoomAmenityID
Where ra.AmenityName like 'Balcony')
Order By r.RoomID

--Summary of Room Amenities
Select r.RoomID, r.RoomNumber, rt.isBookable, r.StatusCode, 
STUFF(
(Select ', '+AmenityName
From RoomAmenity ra
Inner Join RoomExtraAmenity rea on rea.RoomAmenityID = ra.RoomAmenityID
Where rea.RoomID = r.RoomID
For XML PATH(''))
,1,2,'') AS RoomExtraAmenities,
STUFF(
(Select ', '+AmenityName
From RoomAmenity ra
Inner Join RoomTypeAmenity rta on rta.RoomAmenityID = ra.RoomAmenityID
Where rta.RoomTypeID = rt.RoomTypeID
For XML PATH(''))
,1,2,'') AS RoomTypeAmenities,
rt.RoomTypeName, rt.MaxOccupants, rt.BedsKingNr, rt.BedsQueenNr, rt.BedsTwinNr
From RoomType rt
Inner Join Room r on r.RoomTypeID = rt.RoomTypeID
Order By r.RoomID
