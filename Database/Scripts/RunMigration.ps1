# Run Database Migration - Add PersonName Column
# PowerShell Script

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Database Migration Script" -ForegroundColor Cyan
Write-Host "Adding PersonName Column" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Get the script directory
$scriptPath = Join-Path $PSScriptRoot "04_AddPersonNameColumn.sql"

if (-not (Test-Path $scriptPath)) {
    Write-Host "ERROR: Migration script not found at: $scriptPath" -ForegroundColor Red
    exit 1
}

Write-Host "Found migration script: $scriptPath" -ForegroundColor Green
Write-Host ""

# Database connection settings from appsettings.json
$server = "DESKTOP-99QP7PM\SQLEXPRESS"
$database = "TamilHoroscope"
$username = "sa"
$password = "sasa"

Write-Host "Connecting to:" -ForegroundColor Yellow
Write-Host "  Server: $server" -ForegroundColor Gray
Write-Host "  Database: $database" -ForegroundColor Gray
Write-Host "  Authentication: SQL Server (sa)" -ForegroundColor Gray
Write-Host ""

Write-Host "Running migration..." -ForegroundColor Yellow

try {
    # Run using sqlcmd with SQL Server Authentication
    $result = sqlcmd -S $server -d $database -U $username -P $password -i $scriptPath 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "==================================" -ForegroundColor Green
        Write-Host "Migration completed successfully!" -ForegroundColor Green
        Write-Host "==================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "Output:" -ForegroundColor Cyan
        $result | ForEach-Object { Write-Host $_ -ForegroundColor Gray }
        
        Write-Host ""
        Write-Host "Next steps:" -ForegroundColor Yellow
        Write-Host "1. Verify in SSMS that PersonName column exists" -ForegroundColor Gray
        Write-Host "2. Run verification script: .\VerifyPersonNameMigration.ps1" -ForegroundColor Gray
        Write-Host "3. Run the application: dotnet run --project ../../TamilHoroscope.Web" -ForegroundColor Gray
    } else {
        Write-Host ""
        Write-Host "ERROR: Migration failed" -ForegroundColor Red
        Write-Host "Error details:" -ForegroundColor Red
        $result | ForEach-Object { Write-Host $_ -ForegroundColor Red }
        
        Write-Host ""
        Write-Host "Alternative: Run this command manually:" -ForegroundColor Yellow
        Write-Host "sqlcmd -S `"$server`" -d $database -U $username -P $password -i `"$scriptPath`"" -ForegroundColor Cyan
        
        exit 1
    }
} catch {
    Write-Host ""
    Write-Host "ERROR: Failed to execute migration" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    
    Write-Host ""
    Write-Host "Please ensure:" -ForegroundColor Yellow
    Write-Host "1. SQL Server Express is running" -ForegroundColor Gray
    Write-Host "2. You have access to the database with sa credentials" -ForegroundColor Gray
    Write-Host "3. sqlcmd is installed and in PATH" -ForegroundColor Gray
    Write-Host "4. SQL Server allows SQL Server Authentication" -ForegroundColor Gray
    
    Write-Host ""
    Write-Host "Or run the script manually in SSMS" -ForegroundColor Cyan
    
    exit 1
}
