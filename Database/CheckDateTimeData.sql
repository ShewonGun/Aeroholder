-- Check if dates are stored in the database
USE AeroHolderDB;
GO

-- Check TripRequests table for date/time data
SELECT 
    TripRequestID,
    FullName,
    DepartureAirport,
    ArrivalAirport,
    DepartureDate,
    ReturnDate,
    CASE 
        WHEN DepartureDate IS NULL THEN 'NULL'
        ELSE CONVERT(VARCHAR(30), DepartureDate, 120)
    END AS DepartureDateFormatted,
    CASE 
        WHEN ReturnDate IS NULL THEN 'NULL'
        ELSE CONVERT(VARCHAR(30), ReturnDate, 120)
    END AS ReturnDateFormatted,
    CreatedDate,
    UpdatedDate
FROM TripRequests
ORDER BY CreatedDate DESC;

PRINT '----------------------------------------';
PRINT 'TripRequests checked';
PRINT '----------------------------------------';

-- Check BookingHistory table for date/time data
SELECT 
    BookingID,
    PassengerName,
    DepartureAirport,
    ArrivalAirport,
    DepartureDate,
    ReturnDate,
    CASE 
        WHEN DepartureDate IS NULL THEN 'NULL'
        ELSE CONVERT(VARCHAR(30), DepartureDate, 120)
    END AS DepartureDateFormatted,
    CASE 
        WHEN ReturnDate IS NULL THEN 'NULL'
        ELSE CONVERT(VARCHAR(30), ReturnDate, 120)
    END AS ReturnDateFormatted,
    BookingDate,
    CreatedDate,
    UpdatedDate
FROM BookingHistory
ORDER BY CreatedDate DESC;

PRINT '----------------------------------------';
PRINT 'BookingHistory checked';
PRINT '----------------------------------------';
