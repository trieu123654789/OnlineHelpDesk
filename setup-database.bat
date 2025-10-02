@echo off
echo ========================================
echo   Online Help Desk - Database Setup
echo ========================================
echo.

REM Check if SQL Server LocalDB is installed
echo [1/4] Checking SQL Server LocalDB installation...
sqllocaldb info > nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: SQL Server LocalDB is not installed!
    echo Please install SQL Server Express LocalDB first.
    echo Download from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
    pause
    exit /b 1
)
echo ✓ SQL Server LocalDB is installed

REM Start LocalDB instance
echo.
echo [2/4] Starting LocalDB instance...
sqllocaldb start MSSQLLocalDB > nul 2>&1
if %errorlevel% neq 0 (
    echo Creating new LocalDB instance...
    sqllocaldb create MSSQLLocalDB
    sqllocaldb start MSSQLLocalDB
)
echo ✓ LocalDB instance is running

REM Create database and import schema
echo.
echo [3/4] Creating database and importing schema...
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -Q "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OnlineHelpDesk') CREATE DATABASE OnlineHelpDesk"
if %errorlevel% neq 0 (
    echo ERROR: Failed to create database!
    pause
    exit /b 1
)

sqlcmd -S "(localdb)\MSSQLLocalDB" -E -d "OnlineHelpDesk" -i "online-help-desk.sql"
if %errorlevel% neq 0 (
    echo ERROR: Failed to import database schema!
    echo Make sure online-help-desk.sql file exists in the current directory.
    pause
    exit /b 1
)
echo ✓ Database created and schema imported successfully

REM Update Web.config
echo.
echo [4/4] Updating Web.config connection string...
powershell -Command "(Get-Content 'OnlineHelpDesk2\Web.config') -replace 'data source=localhost;initial catalog=OnlineHelpDesk;user id=sa;password=sqladmin;MultipleActiveResultSets=True;App=EntityFramework', 'Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=OnlineHelpDesk;Integrated Security=True;MultipleActiveResultSets=True' | Set-Content 'OnlineHelpDesk2\Web.config'"
echo ✓ Web.config updated

echo.
echo ========================================
echo   Setup Complete!
echo ========================================
echo.
echo Database has been created successfully!
echo You can now run the application in Visual Studio.
echo.
echo Default login credentials:
echo   Username: admin
echo   Password: hello
echo.
pause
