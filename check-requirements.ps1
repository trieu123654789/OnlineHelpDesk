# Online Help Desk - System Requirements Checker
# This script checks if all required components are installed

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Online Help Desk - Requirements Check" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$allRequirementsMet = $true

# Check .NET Framework 4.7.2
Write-Host "[1/4] Checking .NET Framework 4.7.2..." -ForegroundColor Yellow
try {
    $netVersion = Get-ItemProperty "HKLM:SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\" -Name Release -ErrorAction Stop
    if ($netVersion.Release -ge 461808) {
        Write-Host "✓ .NET Framework 4.7.2 or higher is installed" -ForegroundColor Green
    } else {
        Write-Host "✗ .NET Framework 4.7.2 is required" -ForegroundColor Red
        Write-Host "  Download from: https://dotnet.microsoft.com/download/dotnet-framework/net472" -ForegroundColor Yellow
        $allRequirementsMet = $false
    }
} catch {
    Write-Host "✗ .NET Framework 4.7.2 is not installed" -ForegroundColor Red
    Write-Host "  Download from: https://dotnet.microsoft.com/download/dotnet-framework/net472" -ForegroundColor Yellow
    $allRequirementsMet = $false
}

# Check SQL Server LocalDB
Write-Host ""
Write-Host "[2/4] Checking SQL Server LocalDB..." -ForegroundColor Yellow
try {
    $localDbInfo = & sqllocaldb info 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ SQL Server LocalDB is installed" -ForegroundColor Green
        
        # Check if MSSQLLocalDB instance exists
        if ($localDbInfo -contains "MSSQLLocalDB") {
            Write-Host "✓ MSSQLLocalDB instance is available" -ForegroundColor Green
        } else {
            Write-Host "! MSSQLLocalDB instance not found, will be created automatically" -ForegroundColor Yellow
        }
    } else {
        throw "LocalDB not found"
    }
} catch {
    Write-Host "✗ SQL Server LocalDB is not installed" -ForegroundColor Red
    Write-Host "  Download SQL Server Express LocalDB from:" -ForegroundColor Yellow
    Write-Host "  https://www.microsoft.com/en-us/sql-server/sql-server-downloads" -ForegroundColor Yellow
    $allRequirementsMet = $false
}

# Check Visual Studio or Build Tools
Write-Host ""
Write-Host "[3/4] Checking Visual Studio..." -ForegroundColor Yellow
$vsInstalled = $false

# Check for Visual Studio 2019/2022
$vsPaths = @(
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\*\Common7\IDE\devenv.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2019\*\Common7\IDE\devenv.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2022\*\Common7\IDE\devenv.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\*\Common7\IDE\devenv.exe"
)

foreach ($path in $vsPaths) {
    if (Test-Path $path) {
        Write-Host "✓ Visual Studio is installed" -ForegroundColor Green
        $vsInstalled = $true
        break
    }
}

if (-not $vsInstalled) {
    Write-Host "✗ Visual Studio is not installed" -ForegroundColor Red
    Write-Host "  Download Visual Studio Community (free) from:" -ForegroundColor Yellow
    Write-Host "  https://visualstudio.microsoft.com/downloads/" -ForegroundColor Yellow
    $allRequirementsMet = $false
}

# Check if project files exist
Write-Host ""
Write-Host "[4/4] Checking project files..." -ForegroundColor Yellow
if (Test-Path "OnlineHelpDesk2.sln") {
    Write-Host "✓ Solution file found" -ForegroundColor Green
} else {
    Write-Host "✗ OnlineHelpDesk2.sln not found in current directory" -ForegroundColor Red
    $allRequirementsMet = $false
}

if (Test-Path "online-help-desk.sql") {
    Write-Host "✓ Database script found" -ForegroundColor Green
} else {
    Write-Host "✗ online-help-desk.sql not found in current directory" -ForegroundColor Red
    $allRequirementsMet = $false
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
if ($allRequirementsMet) {
    Write-Host "  All Requirements Met!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "You can now run the database setup:" -ForegroundColor Green
    Write-Host "1. Double-click 'setup-database.bat' to setup the database automatically" -ForegroundColor White
    Write-Host "2. Open 'OnlineHelpDesk2.sln' in Visual Studio" -ForegroundColor White
    Write-Host "3. Press F5 to run the application" -ForegroundColor White
    Write-Host ""
    Write-Host "Default login: admin / 123456" -ForegroundColor Yellow
} else {
    Write-Host "  Missing Requirements!" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Please install the missing components above before proceeding." -ForegroundColor Yellow
}

Write-Host ""
Read-Host "Press Enter to continue"
