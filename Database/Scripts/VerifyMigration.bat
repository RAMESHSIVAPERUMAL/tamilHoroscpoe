@echo off
REM Verify PersonName Migration - Batch File

echo ==================================
echo Verification Script
echo Checking PersonName Column
echo ==================================
echo.

echo Connecting to SQL Server...
echo Server: DESKTOP-99QP7PM\SQLEXPRESS
echo Database: TamilHoroscope
echo.

sqlcmd -S "DESKTOP-99QP7PM\SQLEXPRESS" -d TamilHoroscope -U sa -P sasa -i "%~dp0VerifyPersonNameMigration.sql" -W

echo.
pause
