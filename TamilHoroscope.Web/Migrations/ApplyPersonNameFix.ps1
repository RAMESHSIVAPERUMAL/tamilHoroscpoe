# =============================================
# Apply PersonName Column Fix
# Description: Automated script to apply the PersonName column fix
# =============================================

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "PersonName Column Fix - Automated Application" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Navigate to the Web project directory
$webProjectPath = "C:\GitWorkplace\tamilHoroscpoe\TamilHoroscope.Web"
Set-Location $webProjectPath

Write-Host "Step 1: Building the project..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed! Please fix build errors first." -ForegroundColor Red
    exit 1
}

Write-Host "Build successful!" -ForegroundColor Green
Write-Host ""

Write-Host "Step 2: Applying Entity Framework migration..." -ForegroundColor Yellow
dotnet ef database update

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration applied successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host "Fix Applied Successfully!" -ForegroundColor Green
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. Run the application: dotnet run" -ForegroundColor White
    Write-Host "2. Navigate to Horoscope > Generate" -ForegroundColor White
    Write-Host "3. Test generating a horoscope with PersonName field" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host "Migration failed!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Fallback option: Apply SQL script manually" -ForegroundColor Yellow
    Write-Host "1. Open SQL Server Management Studio" -ForegroundColor White
    Write-Host "2. Connect to your database" -ForegroundColor White
    Write-Host "3. Execute: TamilHoroscope.Web\Migrations\AddPersonNameColumn.sql" -ForegroundColor White
    Write-Host ""
    
    $response = Read-Host "Would you like to see the SQL script content? (Y/N)"
    if ($response -eq "Y" -or $response -eq "y") {
        Write-Host ""
        Write-Host "SQL Script:" -ForegroundColor Cyan
        Write-Host "-----------------------------------------------" -ForegroundColor Gray
        Get-Content "$webProjectPath\Migrations\AddPersonNameColumn.sql" | Write-Host -ForegroundColor White
        Write-Host "-----------------------------------------------" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
