-- =====================================================
-- 🌱 COMPLETE SEED DATA — MODULE 1 + MODULE 2
-- Employee Management System (EmpMS)
-- =====================================================
-- Run this in SQL Server Management Studio (SSMS)
-- This script CLEARS all existing data and inserts fresh test data
-- =====================================================

USE EmpMS;
GO

-- =====================================================
-- 🗑️ CLEAR ALL EXISTING DATA + RESET IDs TO 0
-- =====================================================
-- Step 1: Disable ALL foreign key constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';
GO

-- Step 2: Delete all data from all tables (order doesn't matter now)
DELETE FROM RolePrivileges;
DELETE FROM UserRoles;
DELETE FROM Employees;
DELETE FROM Users;
DELETE FROM Privileges;
DELETE FROM Roles;
DELETE FROM Designations;
DELETE FROM Departments;
GO

-- Step 3: Reset identity seeds to 0 (so next insert starts from 1)
DBCC CHECKIDENT ('Roles', RESEED, 0);
DBCC CHECKIDENT ('Privileges', RESEED, 0);
DBCC CHECKIDENT ('RolePrivileges', RESEED, 0);
DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('UserRoles', RESEED, 0);
DBCC CHECKIDENT ('Departments', RESEED, 0);
DBCC CHECKIDENT ('Designations', RESEED, 0);
DBCC CHECKIDENT ('Employees', RESEED, 0);
GO

-- Step 4: Re-enable ALL foreign key constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
GO

-- =====================================================
-- ╔══════════════════════════════════════════════════╗
-- ║           MODULE 1: AUTH & RBAC                  ║
-- ╚══════════════════════════════════════════════════╝
-- =====================================================

-- =====================================================
-- 1. ROLES (4 roles)
-- =====================================================
SET IDENTITY_INSERT Roles ON;

INSERT INTO Roles (Id, RoleName, Description)
VALUES
(1, 'Admin',    'Full system administrator with all privileges'),
(2, 'HR',       'Human Resources — manages employees and recruitment'),
(3, 'Manager',  'Department Manager — manages team and approvals'),
(4, 'Employee', 'Regular employee — limited access');

SET IDENTITY_INSERT Roles OFF;
GO

-- =====================================================
-- 2. PRIVILEGES (12 privileges)
-- =====================================================
SET IDENTITY_INSERT Privileges ON;

INSERT INTO Privileges (Id, PrivilegeName, Description)
VALUES
-- Employee Management
(1,  'employee.view',        'View employee list and details'),
(2,  'employee.create',      'Create new employee'),
(3,  'employee.update',      'Update employee details'),
(4,  'employee.delete',      'Soft delete employee'),
(5,  'employee.search',      'Search and filter employees'),
-- Role Management
(6,  'role.view',            'View roles'),
(7,  'role.manage',          'Create, update, delete roles'),
-- Privilege Management
(8,  'privilege.view',       'View privileges'),
(9,  'privilege.manage',     'Assign/remove privileges'),
-- User Management
(10, 'user.view',            'View user accounts'),
(11, 'user.manage',          'Manage user accounts'),
-- Self
(12, 'profile.own',          'View and update own profile');

SET IDENTITY_INSERT Privileges OFF;
GO

-- =====================================================
-- 3. ROLE-PRIVILEGES (which role gets which privileges)
-- =====================================================
SET IDENTITY_INSERT RolePrivileges ON;

INSERT INTO RolePrivileges (Id, RoleId, PrivilegeId)
VALUES
-- Admin (RoleId=1) → gets ALL 12 privileges
(1,  1, 1),  (2,  1, 2),  (3,  1, 3),  (4,  1, 4),
(5,  1, 5),  (6,  1, 6),  (7,  1, 7),  (8,  1, 8),
(9,  1, 9),  (10, 1, 10), (11, 1, 11), (12, 1, 12),

-- HR (RoleId=2) → employee CRUD + search + view roles + own profile
(13, 2, 1),  -- employee.view
(14, 2, 2),  -- employee.create
(15, 2, 3),  -- employee.update
(16, 2, 4),  -- employee.delete
(17, 2, 5),  -- employee.search
(18, 2, 6),  -- role.view
(19, 2, 12), -- profile.own

-- Manager (RoleId=3) → view employees + search + own profile
(20, 3, 1),  -- employee.view
(21, 3, 5),  -- employee.search
(22, 3, 12), -- profile.own

-- Employee (RoleId=4) → own profile only
(23, 4, 12); -- profile.own

SET IDENTITY_INSERT RolePrivileges OFF;
GO

-- =====================================================
-- 4. USERS (8 users — password: "Test@123" for all)
-- =====================================================
-- BCrypt hash of "Test@123":
-- $2a$11$TBoF5oRes5Aamx8czoCV0OhThqDgYPkJgac5QwBE9v7XNYTE1Yy4S

SET IDENTITY_INSERT Users ON;

INSERT INTO Users (Id, Username, Email, PasswordHash, IsActive, CreatedAt)
VALUES
(1,  'admin',         'admin@empms.com',          '$2a$11$TBoF5oRes5Aamx8czoCV0OhThqDgYPkJgac5QwBE9v7XNYTE1Yy4S', 1, GETDATE()),
(2,  'hr_sunita',     'sunita.rao@empms.com',     '$2a$11$TBoF5oRes5Aamx8czoCV0OhThqDgYPkJgac5QwBE9v7XNYTE1Yy4S', 1, GETDATE()),
(3,  'mgr_amit',      'amit.verma@empms.com',     '$2a$11$TBoF5oRes5Aamx8czoCV0OhThqDgYPkJgac5QwBE9v7XNYTE1Yy4S', 1, GETDATE()),
(4,  'mgr_vikram',    'vikram.singh@empms.com',   '$2a$11$TBoF5oRes5Aamx8czoCV0OhThqDgYPkJgac5QwBE9v7XNYTE1Yy4S', 1, GETDATE()),
(5,  'emp_kartik',    'kartik.zala@empms.com',    '$2a$11$TBoF5oRes5Aamx8czoCV0OhThqDgYPkJgac5QwBE9v7XNYTE1Yy4S', 1, GETDATE()),
(6,  'emp_rahul',     'rahul.mishra@empms.com',   '$2a$11$TBoF5oRes5Aamx8czoCV0OhThqDgYPkJgac5QwBE9v7XNYTE1Yy4S', 1, GETDATE()),
(7,  'emp_rohan',     'rohan.saxena@empms.com',   '$2a$11$TBoF5oRes5Aamx8czoCV0OhThqDgYPkJgac5QwBE9v7XNYTE1Yy4S', 1, GETDATE()),
(8,  'inactive_user', 'inactive@empms.com',       '$2a$11$TBoF5oRes5Aamx8czoCV0OhThqDgYPkJgac5QwBE9v7XNYTE1Yy4S', 0, GETDATE());

SET IDENTITY_INSERT Users OFF;
GO

-- =====================================================
-- 5. USER-ROLES (assign roles to users)
-- =====================================================
SET IDENTITY_INSERT UserRoles ON;

INSERT INTO UserRoles (Id, UserId, RoleId)
VALUES
(1, 1, 1),  -- admin       → Admin
(2, 2, 2),  -- hr_sunita   → HR
(3, 3, 3),  -- mgr_amit    → Manager
(4, 4, 3),  -- mgr_vikram  → Manager
(5, 5, 4),  -- emp_kartik  → Employee
(6, 6, 4),  -- emp_rahul   → Employee
(7, 7, 4),  -- emp_rohan   → Employee
(8, 1, 2);  -- admin       → HR (admin also has HR role — multi-role test!)

SET IDENTITY_INSERT UserRoles OFF;
GO


-- =====================================================
-- ╔══════════════════════════════════════════════════╗
-- ║           MODULE 2: EMPLOYEE MANAGEMENT          ║
-- ╚══════════════════════════════════════════════════╝
-- =====================================================

-- =====================================================
-- 6. DEPARTMENTS (6 departments)
-- =====================================================
SET IDENTITY_INSERT Departments ON;

INSERT INTO Departments (Id, DepartmentName, Description, IsActive, CreatedAt)
VALUES
(1, 'IT',           'Information Technology Department', 1, GETDATE()),
(2, 'HR',           'Human Resources Department',       1, GETDATE()),
(3, 'Finance',      'Finance & Accounting Department',  1, GETDATE()),
(4, 'Marketing',    'Marketing & Sales Department',     1, GETDATE()),
(5, 'Operations',   'Operations & Logistics Department',1, GETDATE()),
(6, 'Admin',        'Administration Department',        1, GETDATE());

SET IDENTITY_INSERT Departments OFF;
GO

-- =====================================================
-- 7. DESIGNATIONS (8 designations)
-- =====================================================
SET IDENTITY_INSERT Designations ON;

INSERT INTO Designations (Id, DesignationName, Description, IsActive, CreatedAt)
VALUES
(1, 'CEO',              'Chief Executive Officer',           1, GETDATE()),
(2, 'CTO',              'Chief Technology Officer',          1, GETDATE()),
(3, 'Manager',          'Department Manager',                1, GETDATE()),
(4, 'Team Lead',        'Team Lead / Senior Developer',      1, GETDATE()),
(5, 'Senior Developer', 'Senior Software Developer',         1, GETDATE()),
(6, 'Junior Developer', 'Junior Software Developer',         1, GETDATE()),
(7, 'HR Executive',     'Human Resources Executive',         1, GETDATE()),
(8, 'Analyst',          'Business / Data Analyst',           1, GETDATE());

SET IDENTITY_INSERT Designations OFF;
GO

-- =====================================================
-- 8. EMPLOYEES (50 employees)
-- =====================================================
SET IDENTITY_INSERT Employees ON;

-- ─── TOP LEVEL: CEO & CTO (no manager) ────────────────
INSERT INTO Employees (Id, FirstName, LastName, Email, Phone, DOB, Gender, Address, JoinDate, DepartmentId, DesignationId, ManagerId, Salary, IsActive, CreatedAt)
VALUES
(1,  'Rajesh',    'Sharma',     'rajesh.sharma@empms.com',    '9876543210', '1975-03-15', 'Male',   '101, MG Road, Mumbai',         '2020-01-01', 6, 1, NULL, 250000.00, 1, GETDATE()),
(2,  'Priya',     'Mehta',      'priya.mehta@empms.com',      '9876543211', '1980-07-22', 'Female', '202, Park Street, Delhi',       '2020-01-15', 1, 2, 1,    200000.00, 1, GETDATE());

-- ─── MANAGERS (report to CEO/CTO) ─────────────────────
INSERT INTO Employees (Id, FirstName, LastName, Email, Phone, DOB, Gender, Address, JoinDate, DepartmentId, DesignationId, ManagerId, Salary, IsActive, CreatedAt)
VALUES
(3,  'Amit',      'Verma',      'amit.verma@empms.com',       '9876543212', '1985-01-10', 'Male',   '303, Bandra West, Mumbai',     '2020-03-01', 1, 3, 2,    120000.00, 1, GETDATE()),
(4,  'Sunita',    'Rao',        'sunita.rao@empms.com',       '9876543213', '1983-09-18', 'Female', '404, Koramangala, Bangalore',   '2020-02-15', 2, 3, 1,    115000.00, 1, GETDATE()),
(5,  'Vikram',    'Singh',      'vikram.singh@empms.com',     '9876543214', '1982-05-25', 'Male',   '505, Salt Lake, Kolkata',       '2020-04-01', 3, 3, 1,    118000.00, 1, GETDATE()),
(6,  'Neha',      'Gupta',      'neha.gupta@empms.com',       '9876543215', '1986-11-30', 'Female', '606, Hitech City, Hyderabad',   '2020-05-10', 4, 3, 1,    112000.00, 1, GETDATE()),
(7,  'Arjun',     'Patel',      'arjun.patel@empms.com',      '9876543216', '1984-08-14', 'Male',   '707, SG Highway, Ahmedabad',    '2020-06-01', 5, 3, 1,    110000.00, 1, GETDATE());

-- ─── TEAM LEADS (report to managers) ──────────────────
INSERT INTO Employees (Id, FirstName, LastName, Email, Phone, DOB, Gender, Address, JoinDate, DepartmentId, DesignationId, ManagerId, Salary, IsActive, CreatedAt)
VALUES
(8,  'Kartik',    'Zala',       'kartik.zala@empms.com',      '9876543217', '1992-04-12', 'Male',   '808, Navrangpura, Ahmedabad',   '2021-01-10', 1, 4, 3,    85000.00, 1, GETDATE()),
(9,  'Ravi',      'Kumar',      'ravi.kumar@empms.com',       '9876543218', '1990-06-20', 'Male',   '909, Andheri East, Mumbai',     '2021-02-01', 1, 4, 3,    82000.00, 1, GETDATE()),
(10, 'Anjali',    'Desai',      'anjali.desai@empms.com',     '9876543219', '1991-12-05', 'Female', '1010, Whitefield, Bangalore',   '2021-03-15', 2, 4, 4,    80000.00, 1, GETDATE()),
(11, 'Manish',    'Joshi',      'manish.joshi@empms.com',     '9876543220', '1989-02-28', 'Male',   '1111, Viman Nagar, Pune',       '2021-04-01', 3, 4, 5,    83000.00, 1, GETDATE()),
(12, 'Deepika',   'Nair',       'deepika.nair@empms.com',     '9876543221', '1993-10-17', 'Female', '1212, Infopark, Kochi',         '2021-05-10', 4, 4, 6,    78000.00, 1, GETDATE());

-- ─── SENIOR DEVELOPERS ────────────────────────────────
INSERT INTO Employees (Id, FirstName, LastName, Email, Phone, DOB, Gender, Address, JoinDate, DepartmentId, DesignationId, ManagerId, Salary, IsActive, CreatedAt)
VALUES
(13, 'Rahul',     'Mishra',     'rahul.mishra@empms.com',     '9876543222', '1993-07-08', 'Male',   '13A, Sector 62, Noida',         '2022-01-05', 1, 5, 8,    65000.00, 1, GETDATE()),
(14, 'Pooja',     'Iyer',       'pooja.iyer@empms.com',       '9876543223', '1994-03-14', 'Female', '14B, OMR Road, Chennai',        '2022-01-20', 1, 5, 8,    63000.00, 1, GETDATE()),
(15, 'Sanjay',    'Tiwari',     'sanjay.tiwari@empms.com',    '9876543224', '1992-09-22', 'Male',   '15C, Hinjewadi, Pune',          '2022-02-10', 1, 5, 9,    64000.00, 1, GETDATE()),
(16, 'Meera',     'Reddy',      'meera.reddy@empms.com',      '9876543225', '1995-01-30', 'Female', '16D, Gachibowli, Hyderabad',    '2022-03-01', 1, 5, 9,    62000.00, 1, GETDATE()),
(17, 'Aditya',    'Chopra',     'aditya.chopra@empms.com',    '9876543226', '1991-06-11', 'Male',   '17E, Connaught Place, Delhi',   '2022-03-15', 3, 5, 11,   66000.00, 1, GETDATE()),
(18, 'Kavita',    'Bhat',       'kavita.bhat@empms.com',      '9876543227', '1993-08-19', 'Female', '18F, Jayanagar, Bangalore',     '2022-04-01', 4, 5, 12,   61000.00, 1, GETDATE());

-- ─── JUNIOR DEVELOPERS ────────────────────────────────
INSERT INTO Employees (Id, FirstName, LastName, Email, Phone, DOB, Gender, Address, JoinDate, DepartmentId, DesignationId, ManagerId, Salary, IsActive, CreatedAt)
VALUES
(19, 'Rohan',     'Saxena',     'rohan.saxena@empms.com',     '9876543228', '1997-04-05', 'Male',   '19G, Electronic City, Bangalore','2023-01-10', 1, 6, 8,    40000.00, 1, GETDATE()),
(20, 'Nisha',     'Agarwal',    'nisha.agarwal@empms.com',    '9876543229', '1998-11-12', 'Female', '20H, Powai, Mumbai',            '2023-01-15', 1, 6, 8,    38000.00, 1, GETDATE()),
(21, 'Akash',     'Pandey',     'akash.pandey@empms.com',     '9876543230', '1999-02-20', 'Male',   '21I, Sector 18, Noida',         '2023-02-01', 1, 6, 9,    39000.00, 1, GETDATE()),
(22, 'Shruti',    'Kapoor',     'shruti.kapoor@empms.com',    '9876543231', '1998-05-16', 'Female', '22J, Malviya Nagar, Jaipur',    '2023-02-15', 1, 6, 9,    37000.00, 1, GETDATE()),
(23, 'Varun',     'Malik',      'varun.malik@empms.com',      '9876543232', '1997-08-09', 'Male',   '23K, Lajpat Nagar, Delhi',      '2023-03-01', 1, 6, 8,    41000.00, 1, GETDATE()),
(24, 'Divya',     'Srinivasan', 'divya.srini@empms.com',      '9876543233', '1999-12-25', 'Female', '24L, T Nagar, Chennai',         '2023-03-10', 1, 6, 9,    36000.00, 1, GETDATE()),
(25, 'Nikhil',    'Bansal',     'nikhil.bansal@empms.com',    '9876543234', '1998-07-03', 'Male',   '25M, Rajouri Garden, Delhi',    '2023-04-01', 1, 6, 8,    40000.00, 1, GETDATE()),
(26, 'Ananya',    'Das',        'ananya.das@empms.com',       '9876543235', '2000-01-18', 'Female', '26N, New Town, Kolkata',         '2023-04-15', 1, 6, 9,    35000.00, 1, GETDATE());

-- ─── HR TEAM ──────────────────────────────────────────
INSERT INTO Employees (Id, FirstName, LastName, Email, Phone, DOB, Gender, Address, JoinDate, DepartmentId, DesignationId, ManagerId, Salary, IsActive, CreatedAt)
VALUES
(27, 'Swati',     'Kulkarni',   'swati.kulkarni@empms.com',   '9876543236', '1994-06-14', 'Female', '27O, FC Road, Pune',            '2022-05-01', 2, 7, 4,    52000.00, 1, GETDATE()),
(28, 'Gaurav',    'Thakur',     'gaurav.thakur@empms.com',    '9876543237', '1993-03-22', 'Male',   '28P, Civil Lines, Lucknow',     '2022-06-01', 2, 7, 10,   50000.00, 1, GETDATE()),
(29, 'Ritu',      'Bhardwaj',   'ritu.bhardwaj@empms.com',    '9876543238', '1995-09-08', 'Female', '29Q, Chandigarh Sector 17',     '2022-07-15', 2, 7, 10,   48000.00, 1, GETDATE()),
(30, 'Tarun',     'Chandra',    'tarun.chandra@empms.com',    '9876543239', '1996-01-11', 'Male',   '30R, Gomti Nagar, Lucknow',     '2023-01-01', 2, 7, 10,   45000.00, 1, GETDATE());

-- ─── FINANCE TEAM ─────────────────────────────────────
INSERT INTO Employees (Id, FirstName, LastName, Email, Phone, DOB, Gender, Address, JoinDate, DepartmentId, DesignationId, ManagerId, Salary, IsActive, CreatedAt)
VALUES
(31, 'Pallavi',   'Jain',       'pallavi.jain@empms.com',     '9876543240', '1992-04-19', 'Female', '31S, Vastrapur, Ahmedabad',     '2022-02-01', 3, 8, 5,    58000.00, 1, GETDATE()),
(32, 'Suresh',    'Menon',      'suresh.menon@empms.com',     '9876543241', '1990-10-30', 'Male',   '32T, MG Road, Kochi',           '2022-03-10', 3, 8, 11,   56000.00, 1, GETDATE()),
(33, 'Aparna',    'Ghosh',      'aparna.ghosh@empms.com',     '9876543242', '1994-07-27', 'Female', '33U, Salt Lake, Kolkata',        '2022-05-20', 3, 8, 11,   54000.00, 1, GETDATE()),
(34, 'Dinesh',    'Rawat',      'dinesh.rawat@empms.com',     '9876543243', '1991-12-03', 'Male',   '34V, Rajpur Road, Dehradun',    '2023-02-01', 3, 8, 11,   52000.00, 1, GETDATE()),
(35, 'Sneha',     'Pillai',     'sneha.pillai@empms.com',     '9876543244', '1996-06-16', 'Female', '35W, Trivandrum Central',       '2023-06-01', 3, 8, 11,   49000.00, 1, GETDATE());

-- ─── MARKETING TEAM ───────────────────────────────────
INSERT INTO Employees (Id, FirstName, LastName, Email, Phone, DOB, Gender, Address, JoinDate, DepartmentId, DesignationId, ManagerId, Salary, IsActive, CreatedAt)
VALUES
(36, 'Mohit',     'Sehgal',     'mohit.sehgal@empms.com',    '9876543245', '1993-05-21', 'Male',   '36X, Hauz Khas, Delhi',         '2022-04-01', 4, 8, 6,    55000.00, 1, GETDATE()),
(37, 'Komal',     'Sethi',      'komal.sethi@empms.com',     '9876543246', '1995-02-14', 'Female', '37Y, Banjara Hills, Hyderabad', '2022-08-01', 4, 8, 12,   53000.00, 1, GETDATE()),
(38, 'Harsh',     'Trivedi',    'harsh.trivedi@empms.com',   '9876543247', '1994-11-08', 'Male',   '38Z, Satellite, Ahmedabad',     '2022-09-15', 4, 8, 12,   51000.00, 1, GETDATE()),
(39, 'Ishita',    'Mukherjee',  'ishita.mukh@empms.com',     '9876543248', '1997-03-29', 'Female', '39AA, Park Circus, Kolkata',     '2023-03-01', 4, 8, 12,   47000.00, 1, GETDATE()),
(40, 'Abhishek',  'Chauhan',    'abhishek.ch@empms.com',     '9876543249', '1996-08-17', 'Male',   '40BB, Aundh, Pune',             '2023-05-01', 4, 6, 12,   42000.00, 1, GETDATE());

-- ─── OPERATIONS TEAM ──────────────────────────────────
INSERT INTO Employees (Id, FirstName, LastName, Email, Phone, DOB, Gender, Address, JoinDate, DepartmentId, DesignationId, ManagerId, Salary, IsActive, CreatedAt)
VALUES
(41, 'Vivek',     'Dubey',      'vivek.dubey@empms.com',     '9876543250', '1991-01-25', 'Male',   '41CC, Ashok Nagar, Bhopal',     '2022-01-15', 5, 8, 7,    57000.00, 1, GETDATE()),
(42, 'Shweta',    'Mishra',     'shweta.mishra@empms.com',   '9876543251', '1994-04-13', 'Female', '42DD, Dharampeth, Nagpur',       '2022-06-10', 5, 8, 7,    53000.00, 1, GETDATE()),
(43, 'Rajat',     'Bhatt',      'rajat.bhatt@empms.com',     '9876543252', '1993-09-07', 'Male',   '43EE, Vaishali Nagar, Jaipur',  '2022-10-01', 5, 8, 7,    51000.00, 1, GETDATE()),
(44, 'Poonam',    'Yadav',      'poonam.yadav@empms.com',    '9876543253', '1996-12-19', 'Female', '44FF, Indira Nagar, Lucknow',   '2023-01-20', 5, 6, 7,    43000.00, 1, GETDATE()),
(45, 'Karan',     'Oberoi',     'karan.oberoi@empms.com',    '9876543254', '1997-07-04', 'Male',   '45GG, Model Town, Ludhiana',    '2023-04-01', 5, 6, 7,    40000.00, 1, GETDATE());

-- ─── ADMIN TEAM ───────────────────────────────────────
INSERT INTO Employees (Id, FirstName, LastName, Email, Phone, DOB, Gender, Address, JoinDate, DepartmentId, DesignationId, ManagerId, Salary, IsActive, CreatedAt)
VALUES
(46, 'Lakshmi',   'Narayan',    'lakshmi.narayan@empms.com', '9876543255', '1992-10-02', 'Female', '46HH, Mylapore, Chennai',       '2021-11-01', 6, 8, 1,    55000.00, 1, GETDATE()),
(47, 'Hemant',    'Saxena',     'hemant.saxena@empms.com',   '9876543256', '1990-05-15', 'Male',   '47II, Lal Bagh, Bangalore',     '2021-12-10', 6, 8, 1,    53000.00, 1, GETDATE());

-- ─── INACTIVE EMPLOYEES (soft deleted — for testing) ──
INSERT INTO Employees (Id, FirstName, LastName, Email, Phone, DOB, Gender, Address, JoinDate, DepartmentId, DesignationId, ManagerId, Salary, IsActive, CreatedAt)
VALUES
(48, 'Ramesh',    'Patil',      'ramesh.patil@empms.com',    '9876543257', '1988-03-11', 'Male',   '48JJ, Shivaji Nagar, Pune',     '2021-06-01', 1, 5, 3,    60000.00, 0, GETDATE()),
(49, 'Geeta',     'Sharma',     'geeta.sharma@empms.com',    '9876543258', '1995-08-23', 'Female', '49KK, Sector 22, Chandigarh',   '2022-01-01', 2, 7, 4,    45000.00, 0, GETDATE()),
(50, 'Vijay',     'Rathore',    'vijay.rathore@empms.com',   '9876543259', '1993-06-29', 'Male',   '50LL, C-Scheme, Jaipur',        '2022-09-01', 3, 8, 5,    48000.00, 0, GETDATE());

SET IDENTITY_INSERT Employees OFF;
GO


-- =====================================================
-- ✅ VERIFY ALL DATA
-- =====================================================

PRINT '========================================';
PRINT '  MODULE 1: AUTH & RBAC';
PRINT '========================================';

SELECT 'Roles' AS TableName, COUNT(*) AS TotalRows FROM Roles
UNION ALL SELECT 'Privileges', COUNT(*) FROM Privileges
UNION ALL SELECT 'RolePrivileges', COUNT(*) FROM RolePrivileges
UNION ALL SELECT 'Users (Active)', COUNT(*) FROM Users WHERE IsActive = 1
UNION ALL SELECT 'Users (Inactive)', COUNT(*) FROM Users WHERE IsActive = 0
UNION ALL SELECT 'UserRoles', COUNT(*) FROM UserRoles;

PRINT '';
PRINT '========================================';
PRINT '  MODULE 2: EMPLOYEE MANAGEMENT';
PRINT '========================================';

SELECT 'Departments' AS TableName, COUNT(*) AS TotalRows FROM Departments
UNION ALL SELECT 'Designations', COUNT(*) FROM Designations
UNION ALL SELECT 'Employees (Active)', COUNT(*) FROM Employees WHERE IsActive = 1
UNION ALL SELECT 'Employees (Inactive)', COUNT(*) FROM Employees WHERE IsActive = 0
UNION ALL SELECT 'Employees (Total)', COUNT(*) FROM Employees;

-- Show role-privilege matrix
PRINT '';
PRINT '========================================';
PRINT '  ROLE-PRIVILEGE MATRIX';
PRINT '========================================';

SELECT r.RoleName, p.PrivilegeName
FROM RolePrivileges rp
JOIN Roles r ON rp.RoleId = r.Id
JOIN Privileges p ON rp.PrivilegeId = p.Id
ORDER BY r.RoleName, p.PrivilegeName;

-- Show user-role assignments
PRINT '';
PRINT '========================================';
PRINT '  USER-ROLE ASSIGNMENTS';
PRINT '========================================';

SELECT u.Username, u.Email, r.RoleName, u.IsActive
FROM UserRoles ur
JOIN Users u ON ur.UserId = u.Id
JOIN Roles r ON ur.RoleId = r.Id
ORDER BY u.Username;
GO
