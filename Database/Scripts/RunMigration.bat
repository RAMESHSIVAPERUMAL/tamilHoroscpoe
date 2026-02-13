@echo off
REM Run PersonName Migration - Batch File
REM This is easier to run than PowerShell (no execution policy issues)

echo ==================================
echo Database Migration Script
echo Adding PersonName Column
echo ==================================
echo.

echo Connecting to SQL Server...
echo Server: DESKTOP-99QP7PM\SQLEXPRESS
echo Database: TamilHoroscope
echo Authentication: SQL Server (sa)
echo.

echo Running migration...
sqlcmd -S "DESKTOP-99QP7PM\SQLEXPRESS" -d TamilHoroscope -U sa -P sasa -i "%~dp004_AddPersonNameColumn.sql"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ==================================
    echo Migration completed successfully!
    echo ==================================
    echo.
    echo Next steps:
    echo 1. Run VerifyMigration.bat to verify
    echo 2. Start the application
) else (
    echo.
    echo ERROR: Migration failed!
    echo Please check the error messages above.
)

echo.
pause
