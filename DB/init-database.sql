IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'GLMS_Db')
BEGIN
    CREATE DATABASE GLMS_Db;
    PRINT 'GLMS_Db database created';
END
ELSE
BEGIN
    PRINT 'GLMS_Db database already exists... moving on';
END
GO