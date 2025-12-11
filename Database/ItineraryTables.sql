-- =============================================
-- AeroHolder Itinerary Tables
-- Add these tables to your AeroHolderDB database
-- =============================================

USE AeroHolderDB;
GO

-- =============================================
-- Trip Requests Table
-- Stores trip request information from shareholders
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TripRequests')
BEGIN
    CREATE TABLE TripRequests (
        TripRequestID INT IDENTITY(1,1) PRIMARY KEY,
        ShareholderID INT NOT NULL,
        FullName NVARCHAR(200) NOT NULL,
        TicketType NVARCHAR(50) NOT NULL,
        Relationship NVARCHAR(50) NULL,
        Remarks NVARCHAR(500) NULL,
        TicketIssue INT NULL,
        Entitlement NVARCHAR(10) NULL,
        -- Manage Request Fields
        TicketNo NVARCHAR(50) NULL,
        PassportNumber NVARCHAR(50) NULL,
        DepartureAirport NVARCHAR(10) NULL,
        DepartureCity NVARCHAR(100) NULL,
        ArrivalAirport NVARCHAR(10) NULL,
        ArrivalCity NVARCHAR(100) NULL,
        DepartureDate DATETIME NULL,
        ReturnDate DATETIME NULL,
        Status NVARCHAR(20) DEFAULT 'Pending', -- Pending, Approved, Issued, Cancelled
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME DEFAULT GETDATE(),
        CreatedBy NVARCHAR(100) NULL,
        UpdatedBy NVARCHAR(100) NULL,
        FOREIGN KEY (ShareholderID) REFERENCES Shareholders(ShareholderID) ON DELETE CASCADE
    );
    PRINT 'TripRequests table created successfully';
END
ELSE
BEGIN
    PRINT 'TripRequests table already exists';
END
GO

-- =============================================
-- Booking History Table
-- Stores all booking transactions and history
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BookingHistory')
BEGIN
    CREATE TABLE BookingHistory (
        BookingID INT IDENTITY(1,1) PRIMARY KEY,
        ShareholderID INT NOT NULL,
        TripRequestID INT NULL,
        TicketNo NVARCHAR(50) NULL,
        PassengerName NVARCHAR(200) NOT NULL,
        TicketType NVARCHAR(50) NOT NULL,
        DepartureAirport NVARCHAR(10) NULL,
        ArrivalAirport NVARCHAR(10) NULL,
        DepartureDate DATETIME NULL,
        ReturnDate DATETIME NULL,
        BookingDate DATETIME NULL,
        Status NVARCHAR(20) DEFAULT 'Issued', -- Issued, Cancelled, Completed
        UpdatedBy NVARCHAR(200) NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (ShareholderID) REFERENCES Shareholders(ShareholderID) ON DELETE CASCADE,
        FOREIGN KEY (TripRequestID) REFERENCES TripRequests(TripRequestID) ON DELETE NO ACTION
    );
    PRINT 'BookingHistory table created successfully';
END
ELSE
BEGIN
    PRINT 'BookingHistory table already exists';
END
GO

-- =============================================
-- Add Indexes for Better Performance
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TripRequests_ShareholderID')
BEGIN
    CREATE INDEX IX_TripRequests_ShareholderID ON TripRequests(ShareholderID);
    PRINT 'Index IX_TripRequests_ShareholderID created';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_BookingHistory_ShareholderID')
BEGIN
    CREATE INDEX IX_BookingHistory_ShareholderID ON BookingHistory(ShareholderID);
    PRINT 'Index IX_BookingHistory_ShareholderID created';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_BookingHistory_TripRequestID')
BEGIN
    CREATE INDEX IX_BookingHistory_TripRequestID ON BookingHistory(TripRequestID);
    PRINT 'Index IX_BookingHistory_TripRequestID created';
END
GO

-- =============================================
-- Insert Sample Data (Optional - for testing)
-- =============================================
PRINT '=========================================';
PRINT 'Itinerary tables setup completed!';
PRINT 'Tables created: TripRequests, BookingHistory';
PRINT '=========================================';
GO
