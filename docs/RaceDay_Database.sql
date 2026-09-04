-- ============================================================
-- RaceDay Database Schema
-- Created: September 2026
-- Description: Full database schema for the RaceDay event management system
-- ============================================================

-- Create the database
CREATE DATABASE RaceDayDB;
GO

USE RaceDayDB;
GO

-- ============================================================
-- TABLE: Users
-- ============================================================
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50) NOT NULL CHECK (Role IN ('Organiser', 'Participant')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);
GO

-- ============================================================
-- TABLE: Events
-- ============================================================
CREATE TABLE Events (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID INT NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    EventDate DATE NOT NULL,
    Location NVARCHAR(500) NOT NULL,
    RouteInfo NVARCHAR(MAX) NULL,
    MaxParticipants INT NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Active' CHECK (Status IN ('Active', 'Cancelled', 'Completed')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (OrganiserID) REFERENCES Users(UserID)
);
GO

-- ============================================================
-- TABLE: Categories
-- ============================================================
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Distance DECIMAL(10,2) NOT NULL,
    StartTime TIME NOT NULL,
    MaxParticipants INT NOT NULL,
    Fee DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EventID) REFERENCES Events(EventID) ON DELETE CASCADE
);
GO

-- ============================================================
-- TABLE: Enrolments
-- ============================================================
CREATE TABLE Enrolments (
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    EnrolmentDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) DEFAULT 'Active' CHECK (Status IN ('Active', 'Cancelled', 'Completed')),
    PaymentStatus NVARCHAR(50) DEFAULT 'Pending' CHECK (PaymentStatus IN ('Pending', 'Paid', 'Failed', 'Refunded')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ParticipantID) REFERENCES Users(UserID),
    FOREIGN KEY (EventID) REFERENCES Events(EventID),
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    CONSTRAINT UQ_Participant_Event_Enrolment UNIQUE (ParticipantID, EventID)
);
GO

-- ============================================================
-- TABLE: Results
-- ============================================================
CREATE TABLE Results (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    EnrolmentID INT NOT NULL,
    CategoryID INT NOT NULL,
    FinishTime TIME NULL,
    Position INT NULL,
    Status NVARCHAR(50) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Completed', 'Disqualified')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ParticipantID) REFERENCES Users(UserID),
    FOREIGN KEY (EnrolmentID) REFERENCES Enrolments(EnrolmentID),
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    CONSTRAINT UQ_Enrolment_Result UNIQUE (EnrolmentID)
);
GO

-- ============================================================
-- TABLE: Weather
-- ============================================================
CREATE TABLE Weather (
    WeatherID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    ForecastDate DATE NOT NULL,
    Temperature DECIMAL(5,2) NULL,
    Conditions NVARCHAR(100) NULL,
    WindSpeed INT NULL,
    Humidity INT NULL,
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EventID) REFERENCES Events(EventID) ON DELETE CASCADE,
    CONSTRAINT UQ_Event_Weather UNIQUE (EventID)
);
GO

-- ============================================================
-- TABLE: RouteInformation
-- ============================================================
CREATE TABLE RouteInformation (
    RouteID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Distance DECIMAL(10,2) NOT NULL,
    ElevationGain INT NULL,
    Waypoints NVARCHAR(MAX) NULL,
    MapUrl NVARCHAR(500) NULL,
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EventID) REFERENCES Events(EventID) ON DELETE CASCADE,
    CONSTRAINT UQ_Event_Route UNIQUE (EventID)
);
GO

-- ============================================================
-- INDEXES for Performance
-- ============================================================
CREATE INDEX IX_Events_OrganiserID ON Events(OrganiserID);
CREATE INDEX IX_Events_EventDate ON Events(EventDate);
CREATE INDEX IX_Events_Status ON Events(Status);
CREATE INDEX IX_Categories_EventID ON Categories(EventID);
CREATE INDEX IX_Enrolments_ParticipantID ON Enrolments(ParticipantID);
CREATE INDEX IX_Enrolments_EventID ON Enrolments(EventID);
CREATE INDEX IX_Enrolments_CategoryID ON Enrolments(CategoryID);
CREATE INDEX IX_Results_EnrolmentID ON Results(EnrolmentID);
CREATE INDEX IX_Results_ParticipantID ON Results(ParticipantID);
CREATE INDEX IX_Users_Email ON Users(Email);
GO

-- ============================================================
-- INSERT SAMPLE DATA
-- ============================================================

-- Insert 2 Organisers
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role)
VALUES 
('thabo.mokoena@raceday.com', 'hashed_password_1', 'Thabo', 'Mokoena', 'Organiser'),
('sarah.smith@raceday.com', 'hashed_password_2', 'Sarah', 'Smith', 'Organiser');
GO

-- Insert 2 Participants
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role)
VALUES 
('john.participant@gmail.com', 'hashed_password_3', 'John', 'Doe', 'Participant'),
('mary.jane@gmail.com', 'hashed_password_4', 'Mary', 'Jane', 'Participant');
GO

-- Insert 3 Events
INSERT INTO Events (OrganiserID, Name, Description, EventDate, Location, RouteInfo, MaxParticipants, Status)
VALUES 
(1, 'Cape Town Cycle Tour', 'The worlds largest timed cycle race', '2026-03-08', 'Cape Town, Western Cape', 'Route from Cape Town Stadium to Cape Point and back', 35000, 'Active'),
(2, 'Comrades Marathon', 'The Ultimate Human Race', '2026-06-16', 'Pietermaritzburg to Durban', '90km route from Pietermaritzburg to Durban', 20000, 'Active'),
(1, 'Soweto Marathon', 'Iconic run through the streets of Soweto', '2026-11-01', 'Soweto, Johannesburg', '42.2km route through Soweto', 15000, 'Active');
GO

-- Insert Categories for Cape Town Cycle Tour (EventID = 1)
INSERT INTO Categories (EventID, Name, Distance, StartTime, MaxParticipants, Fee)
VALUES 
(1, 'Elite Men', 109.0, '06:00', 500, 450.00),
(1, 'Elite Women', 109.0, '06:00', 500, 450.00),
(1, 'Amateur Men', 109.0, '06:30', 15000, 350.00),
(1, 'Amateur Women', 109.0, '06:30', 15000, 350.00);
GO

-- Insert Categories for Comrades Marathon (EventID = 2)
INSERT INTO Categories (EventID, Name, Distance, StartTime, MaxParticipants, Fee)
VALUES 
(2, 'Elite Men', 90.0, '05:30', 1000, 800.00),
(2, 'Elite Women', 90.0, '05:30', 1000, 800.00),
(2, 'Amateur Men', 90.0, '06:00', 9000, 600.00),
(2, 'Amateur Women', 90.0, '06:00', 9000, 600.00);
GO

-- Insert Categories for Soweto Marathon (EventID = 3)
INSERT INTO Categories (EventID, Name, Distance, StartTime, MaxParticipants, Fee)
VALUES 
(3, 'Full Marathon Men', 42.2, '06:00', 5000, 400.00),
(3, 'Full Marathon Women', 42.2, '06:00', 5000, 400.00),
(3, 'Half Marathon Men', 21.1, '07:00', 3000, 250.00),
(3, 'Half Marathon Women', 21.1, '07:00', 2000, 250.00);
GO

-- Insert Enrolments
INSERT INTO Enrolments (ParticipantID, EventID, CategoryID, Status, PaymentStatus)
VALUES 
(3, 1, 3, 'Active', 'Paid'),
(4, 1, 4, 'Active', 'Paid'),
(3, 2, 7, 'Active', 'Pending'),
(4, 2, 8, 'Active', 'Pending');
GO

-- Insert Results
INSERT INTO Results (ParticipantID, EnrolmentID, CategoryID, FinishTime, Position, Status)
VALUES 
(3, 1, 3, '03:45:30', 25, 'Completed'),
(4, 2, 4, '04:12:15', 48, 'Completed');
GO

-- Insert Weather
INSERT INTO Weather (EventID, ForecastDate, Temperature, Conditions, WindSpeed, Humidity)
VALUES 
(1, '2026-03-08', 22.5, 'Sunny with light winds', 15, 65),
(2, '2026-06-16', 18.0, 'Partly cloudy', 10, 55),
(3, '2026-11-01', 25.0, 'Sunny and warm', 8, 45);
GO

-- Insert Route Information
INSERT INTO RouteInformation (EventID, Description, Distance, ElevationGain, Waypoints, MapUrl)
VALUES 
(1, 'Scenic route along the Cape Peninsula with stunning ocean views', 109.0, 1200, 'Cape Town Stadium, Hout Bay, Cape Point, Noordhoek, Constantia', 'https://maps.example.com/ctct-route'),
(2, 'Iconic route between Pietermaritzburg and Durban with challenging hills', 90.0, 2400, 'Pietermaritzburg, Inchanga, Cato Ridge, Durban', 'https://maps.example.com/comrades-route'),
(3, 'Flat and fast route through the vibrant streets of Soweto', 42.2, 350, 'Orlando Stadium, Vilakazi Street, Diepkloof, Soweto', 'https://maps.example.com/soweto-route');
GO

-- ============================================================
-- VERIFY DATA
-- ============================================================
SELECT 'Users' as TableName, COUNT(*) as RecordCount FROM Users
UNION ALL
SELECT 'Events', COUNT(*) FROM Events
UNION ALL
SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL
SELECT 'Enrolments', COUNT(*) FROM Enrolments
UNION ALL
SELECT 'Results', COUNT(*) FROM Results
UNION ALL
SELECT 'Weather', COUNT(*) FROM Weather
UNION ALL
SELECT 'RouteInformation', COUNT(*) FROM RouteInformation;
GO