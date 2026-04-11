# BillFlow MSI Installer Build Script
# Run this script to build the WiX installer

param(
    [string]$Configuration = "Release",
    [string]$Version = "2.0.0"
)

$ErrorActionPreference = "Stop"

Write-Host "Building BillFlow Installer v$Version..." -ForegroundColor Green

# Step 1: Build the application
Write-Host "Step 1: Building BillFlow application..." -ForegroundColor Yellow
dotnet build ..\BillFlow.sln -c $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Error "Application build failed!"
    exit 1
}

# Step 2: Publish self-contained
Write-Host "Step 2: Publishing self-contained application..." -ForegroundColor Yellow
dotnet publish ..\BillFlow\BillFlow.csproj -c $Configuration -r win-x64 --self-contained true -o .\publish
if ($LASTEXITCODE -ne 0) {
    Write-Error "Application publish failed!"
    exit 1
}

# Step 3: Build WiX installer
Write-Host "Step 3: Building WiX installer..." -ForegroundColor Yellow
if (Get-Command wix -ErrorAction SilentlyContinue) {
    wix build -o .\BillFlow-$Version.msi Package.wxs
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Installer build failed!"
        exit 1
    }
} else {
    Write-Warning "WiX Toolset not found. Please install WiX Toolset v4:"
    Write-Warning "  dotnet tool install --global wix"
    Write-Warning "  wix extension add -g WixToolset.UI.wixext"
    exit 1
}

Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "Installer: .\BillFlow-$Version.msi" -ForegroundColor Cyan
