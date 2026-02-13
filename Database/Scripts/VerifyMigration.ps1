# Verify PersonName Column Migration
# PowerShell Script

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Verification Script" -ForegroundColor Cyan
Write-Host "Checking PersonName Column" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Get the script directory
$scriptPath = Join-Path $PSScriptRoot "VerifyPersonNameMigration.sql"

if (-not (Test-Path $scriptPath)) {
    Write-Host "ERROR: Verification script not found at: $scriptPath" -ForegroundColor Red
    exit 1
}

# Database connection settings from appsettings.json
$server = "DESKTOP-99QP7PM\SQLEXPRESS"
$database = "TamilHoroscope"
$username = "sa"
$password = "sasa"

Write-Host "Connecting to:" -ForegroundColor Yellow
Write-Host "  Server: $server" -ForegroundColor Gray
Write-Host "  Database: $database" -ForegroundColor Gray
Write-Host ""

Write-Host "Running verification..." -ForegroundColor Yellow

try {
    # Run using sqlcmd with SQL Server Authentication
    $result = sqlcmd -S $server -d $database -U $username -P $password -i $scriptPath -W 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "==================================" -ForegroundColor Green
        Write-Host "Verification completed!" -ForegroundColor Green
        Write-Host "==================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "Results:" -ForegroundColor Cyan
        $result | ForEach-Object { Write-Host $_ -ForegroundColor Gray }
    } else {
        Write-Host ""
        Write-Host "ERROR: Verification failed" -ForegroundColor Red
        $result | ForEach-Object { Write-Host $_ -ForegroundColor Red }
        exit 1
    }
} catch {
    Write-Host ""
    Write-Host "ERROR: Failed to execute verification" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
