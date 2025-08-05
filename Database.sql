-- ============================
-- 1. DROP ALL TABLES IF EXIST
-- ============================
DROP TABLE IF EXISTS TblScheduleUser;
DROP TABLE IF EXISTS TblAttendance;
DROP TABLE IF EXISTS TblUser;
DROP TABLE IF EXISTS TblCamera;
DROP TABLE IF EXISTS TblSchedule;
DROP TABLE IF EXISTS TblSubject;
DROP TABLE IF EXISTS TblClass;

-- ============================
-- 2. CREATE TABLES
-- ============================

-- User Table
CREATE TABLE TblUser (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255),
    Email NVARCHAR(255),
    Password NVARCHAR(255),
    Role NVARCHAR(50),
    ClassId INT
);

-- Class Table
CREATE TABLE TblClass (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255)
);

-- Subject Table
CREATE TABLE TblSubject (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255),
    Description NVARCHAR(500)
);

-- Schedule Table
CREATE TABLE TblSchedule (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ClassId INT,
    SubjectId INT,
    Date DATE,
    Time TIME,
    Room NVARCHAR(255),
    FOREIGN KEY (ClassId) REFERENCES TblClass(Id),
    FOREIGN KEY (SubjectId) REFERENCES TblSubject(Id)
);

-- Camera Table
CREATE TABLE TblCamera (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Room NVARCHAR(255),
    IPAddress NVARCHAR(50)
);

-- Attendance Table
CREATE TABLE TblAttendance (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT,
    ScheduleId INT,
    Status NVARCHAR(50),
    TimeRecorded DATETIME,
    FOREIGN KEY (UserId) REFERENCES TblUser(Id),
    FOREIGN KEY (ScheduleId) REFERENCES TblSchedule(Id)
);

-- Schedule-User Relationship Table
CREATE TABLE TblScheduleUser (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ScheduleId INT,
    UserId INT,
    FOREIGN KEY (ScheduleId) REFERENCES TblSchedule(Id),
    FOREIGN KEY (UserId) REFERENCES TblUser(Id)
);

-- ============================
-- 3. STORED PROCEDURES
-- ============================

-- Add a new schedule
CREATE PROCEDURE AddSchedule
    @ClassId INT,
    @SubjectId INT,
    @Date DATE,
    @Time TIME,
    @Room NVARCHAR(255)
AS
BEGIN
    INSERT INTO TblSchedule (ClassId, SubjectId, Date, Time, Room)
    VALUES (@ClassId, @SubjectId, @Date, @Time, @Room)
END
GO

-- Add new student
CREATE PROCEDURE AddStudent
    @Name NVARCHAR(255),
    @Email NVARCHAR(255),
    @Password NVARCHAR(255),
    @ClassId INT
AS
BEGIN
    INSERT INTO TblUser (Name, Email, Password, Role, ClassId)
    VALUES (@Name, @Email, @Password, 'student', @ClassId)
END
GO

-- Add new teacher
CREATE PROCEDURE AddTeacher
    @Name NVARCHAR(255),
    @Email NVARCHAR(255),
    @Password NVARCHAR(255)
AS
BEGIN
    INSERT INTO TblUser (Name, Email, Password, Role)
    VALUES (@Name, @Email, @Password, 'teacher')
END
GO

-- Add new subject
CREATE PROCEDURE AddSubject
    @Name NVARCHAR(255),
    @Description NVARCHAR(500)
AS
BEGIN
    INSERT INTO TblSubject (Name, Description)
    VALUES (@Name, @Description)
END
GO

-- Get attendance by schedule
CREATE PROCEDURE GetAttendanceBySchedule
    @ScheduleId INT
AS
BEGIN
    SELECT u.Name, a.Status, a.TimeRecorded
    FROM TblAttendance a
    JOIN TblUser u ON a.UserId = u.Id
    WHERE a.ScheduleId = @ScheduleId
END
GO

-- Update attendance for 1 student
CREATE PROCEDURE UpdateAttendanceForStudent
    @UserId INT,
    @ScheduleId INT,
    @Status NVARCHAR(50),
    @TimeRecorded DATETIME
AS
BEGIN
    IF EXISTS (SELECT 1 FROM TblAttendance WHERE UserId = @UserId AND ScheduleId = @ScheduleId)
    BEGIN
        UPDATE TblAttendance
        SET Status = @Status, TimeRecorded = @TimeRecorded
        WHERE UserId = @UserId AND ScheduleId = @ScheduleId
    END
    ELSE
    BEGIN
        INSERT INTO TblAttendance (UserId, ScheduleId, Status, TimeRecorded)
        VALUES (@UserId, @ScheduleId, @Status, @TimeRecorded)
    END
END
GO

-- Update attendance for multiple students
CREATE PROCEDURE UpdateAttendanceForStudentsByList
    @ScheduleId INT,
    @StudentsAttendance NVARCHAR(MAX)
AS
BEGIN
    BEGIN TRY
        -- Chuỗi JSON sẽ ở dạng: [{"UserId":1,"Status":"Present","TimeRecorded":"2024-08-05T09:00:00"},...]
        DECLARE @json NVARCHAR(MAX) = @StudentsAttendance;
        SELECT * INTO #TempAttendance
        FROM OPENJSON(@json)
        WITH (
            UserId INT,
            Status NVARCHAR(50),
            TimeRecorded DATETIME
        );

        MERGE TblAttendance AS target
        USING #TempAttendance AS source
        ON target.UserId = source.UserId AND target.ScheduleId = @ScheduleId
        WHEN MATCHED THEN
            UPDATE SET target.Status = source.Status, target.TimeRecorded = source.TimeRecorded
        WHEN NOT MATCHED THEN
            INSERT (UserId, ScheduleId, Status, TimeRecorded)
            VALUES (source.UserId, @ScheduleId, source.Status, source.TimeRecorded);

        DROP TABLE #TempAttendance;
    END TRY
    BEGIN CATCH
        PRINT ERROR_MESSAGE();
    END CATCH
END
GO

-- Check if user already attended
CREATE PROCEDURE CheckIfUserAttended
    @UserId INT,
    @ScheduleId INT
AS
BEGIN
    SELECT * FROM TblAttendance
    WHERE UserId = @UserId AND ScheduleId = @ScheduleId
END
GO

-- Get schedule by date range for class
CREATE PROCEDURE GetSchedulesForClassInRange
    @ClassId INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SELECT s.Id, s.Date, s.Time, s.Room, subj.Name AS SubjectName
    FROM TblSchedule s
    JOIN TblSubject subj ON s.SubjectId = subj.Id
    WHERE s.ClassId = @ClassId AND s.Date BETWEEN @StartDate AND @EndDate
END
GO

-- Get all classes
CREATE PROCEDURE GetAllClasses
AS
BEGIN
    SELECT * FROM TblClass
END
GO

-- Get all subjects
CREATE PROCEDURE GetAllSubjects
AS
BEGIN
    SELECT * FROM TblSubject
END
GO

-- Add a new class
CREATE PROCEDURE AddClass
    @Name NVARCHAR(255)
AS
BEGIN
    INSERT INTO TblClass (Name)
    VALUES (@Name)
END
GO

-- Get students by class
CREATE PROCEDURE GetStudentsByClass
    @ClassId INT
AS
BEGIN
    SELECT * FROM TblUser
    WHERE Role = 'student' AND ClassId = @ClassId
END
GO
