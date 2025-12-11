-- =============================================
-- QUICK DATABASE TEST SCRIPT
-- Run this to verify everything is set up correctly
-- =============================================

USE AeroHolderDB;
GO

PRINT '===================================';
PRINT 'ITINERARY DATABASE DIAGNOSTIC TEST';
PRINT '===================================';
PRINT '';

-- Test 1: Check Database
PRINT '1. Current Database: ' + DB_NAME();
PRINT '';

-- Test 2: Check if tables exist
PRINT '2. Checking Tables...';
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('TripRequests', 'BookingHistory', 'Shareholders');

DECLARE @TripRequestsExists INT = 0;
DECLARE @BookingHistoryExists INT = 0;
DECLARE @ShareholdersExists INT = 0;

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TripRequests')
    SET @TripRequestsExists = 1;
    
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'BookingHistory')
    SET @BookingHistoryExists = 1;
    
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Shareholders')
    SET @ShareholdersExists = 1;

PRINT '';
PRINT '   TripRequests: ' + CASE WHEN @TripRequestsExists = 1 THEN 'EXISTS ?' ELSE 'MISSING ?' END;
PRINT '   BookingHistory: ' + CASE WHEN @BookingHistoryExists = 1 THEN 'EXISTS ?' ELSE 'MISSING ?' END;
PRINT '   Shareholders: ' + CASE WHEN @ShareholdersExists = 1 THEN 'EXISTS ?' ELSE 'MISSING ?' END;
PRINT '';

-- Test 3: Check record counts
IF @TripRequestsExists = 1 AND @BookingHistoryExists = 1 AND @ShareholdersExists = 1
BEGIN
    PRINT '3. Checking Record Counts...';
    
    DECLARE @ShareholderCount INT = (SELECT COUNT(*) FROM Shareholders);
    DECLARE @TripRequestCount INT = (SELECT COUNT(*) FROM TripRequests);
    DECLARE @BookingCount INT = (SELECT COUNT(*) FROM BookingHistory);
    
    PRINT '   Shareholders: ' + CAST(@ShareholderCount AS VARCHAR(10));
    PRINT '   Trip Requests: ' + CAST(@TripRequestCount AS VARCHAR(10));
    PRINT '   Bookings: ' + CAST(@BookingCount AS VARCHAR(10));
    PRINT '';
    
    -- Test 4: Show sample data
    IF @ShareholderCount > 0
    BEGIN
        PRINT '4. Sample Shareholder (for testing):';
        SELECT TOP 1 
            ShareholderID,
            FolioID,
            FullName
        FROM Shareholders;
        PRINT '';
    END
    ELSE
    BEGIN
        PRINT '4. ? WARNING: No shareholders found! Add shareholders first.';
        PRINT '';
    END
    
    -- Test 5: Show recent trip requests
    IF @TripRequestCount > 0
    BEGIN
        PRINT '5. Recent Trip Requests:';
        SELECT TOP 5
            TripRequestID,
            FullName,
            TicketType,
            Status,
            CreatedDate
        FROM TripRequests
        ORDER BY CreatedDate DESC;
        PRINT '';
    END
    ELSE
    BEGIN
        PRINT '5. ? No trip requests yet. This is normal if you haven''t tested the form.';
        PRINT '';
    END
    
    -- Test 6: Show recent bookings
    IF @BookingCount > 0
    BEGIN
        PRINT '6. Recent Bookings:';
        SELECT TOP 5
            BookingID,
            PassengerName,
            DepartureAirport + ' ? ' + ArrivalAirport AS Route,
            Status,
            CreatedDate
        FROM BookingHistory
        ORDER BY CreatedDate DESC;
        PRINT '';
    END
    ELSE
    BEGIN
        PRINT '6. ? No bookings yet. This is normal if you haven''t completed the form workflow.';
        PRINT '';
    END
END
ELSE
BEGIN
    PRINT '';
    PRINT '? ERROR: Required tables are missing!';
    PRINT '';
    PRINT 'ACTION REQUIRED:';
    PRINT '1. Open Database\ItineraryTables.sql';
    PRINT '2. Execute the entire script';
    PRINT '3. Run this diagnostic test again';
    PRINT '';
END

-- Test 7: Test Insert (Optional - only if you want to manually test)
/*
PRINT '7. Testing Manual Insert...';
DECLARE @TestShareholderID INT = (SELECT TOP 1 ShareholderID FROM Shareholders);

IF @TestShareholderID IS NOT NULL
BEGIN
    -- Insert test trip request
    INSERT INTO TripRequests (ShareholderID, FullName, TicketType, Relationship, Remarks, TicketIssue, Entitlement, Status, CreatedBy, CreatedDate, UpdatedDate)
    VALUES (@TestShareholderID, 'SQL Test Passenger', 'Dependent', 'Test', 'Manual SQL Insert Test', 1, '01', 'Pending', 'SQL Test', GETDATE(), GETDATE());
    
    DECLARE @NewTripID INT = SCOPE_IDENTITY();
    PRINT '   ? Test Trip Request Created with ID: ' + CAST(@NewTripID AS VARCHAR(10));
    
    -- Insert test booking
    INSERT INTO BookingHistory (ShareholderID, TripRequestID, TicketNo, PassengerName, TicketType, DepartureAirport, ArrivalAirport, DepartureDate, ReturnDate, BookingDate, Status, UpdatedBy, CreatedDate, UpdatedDate)
    VALUES (@TestShareholderID, @NewTripID, 'TEST123456', 'SQL Test Passenger', 'Dependent', 'CMB', 'DXB', GETDATE()+30, GETDATE()+40, GETDATE(), 'Issued', 'SQL Test', GETDATE(), GETDATE());
    
    DECLARE @NewBookingID INT = SCOPE_IDENTITY();
    PRINT '   ? Test Booking Created with ID: ' + CAST(@NewBookingID AS VARCHAR(10));
    PRINT '';
    PRINT '   To delete test data, run:';
    PRINT '   DELETE FROM BookingHistory WHERE BookingID = ' + CAST(@NewBookingID AS VARCHAR(10));
    PRINT '   DELETE FROM TripRequests WHERE TripRequestID = ' + CAST(@NewTripID AS VARCHAR(10));
END
ELSE
BEGIN
    PRINT '   ? Cannot test insert - no shareholders found';
END
PRINT '';
*/

-- Summary
PRINT '===================================';
PRINT 'DIAGNOSTIC COMPLETE';
PRINT '===================================';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. If tables are missing ? Run Database\ItineraryTables.sql';
PRINT '2. If no shareholders ? Add shareholders through the application';
PRINT '3. If no trip requests ? Test the itinerary modal form';
PRINT '4. Check browser console (F12) for JavaScript errors';
PRINT '5. Visit /ItineraryDebug/TestDatabase in browser for backend test';
PRINT '';

GO
