# BillFlow Deployment Guide

## System Requirements

- Windows 10 (Version 1903+) or Windows 11
- .NET 8.0 Desktop Runtime (included in self-contained build)
- 100 MB free disk space

## Installation Methods

### Method 1: MSI Installer (Recommended)

1. Download `BillFlow-2.0.0.msi`
2. Double-click to run the installer
3. Follow the installation wizard
4. BillFlow will be available in Start Menu

### Method 2: Portable Deployment

1. Extract `BillFlow-2.0.0.zip` to desired location
2. Run `BillFlow.exe`
3. Database and settings will be stored in `%LOCALAPPDATA%\BillFlow\`

## Building from Source

### Prerequisites
- Visual Studio 2022 or later
- .NET 8.0 SDK
- WiX Toolset v4 (for installer)

### Build Steps

```powershell
# Build the solution
dotnet build BillFlow.sln -c Release

# Run the application
dotnet run --project BillFlow\BillFlow.csproj

# Build installer (requires WiX)
cd BillFlow.Setup
.\BuildInstaller.ps1
```

## WiX Installer Setup

Install WiX Toolset v4:
```powershell
dotnet tool install --global wix
wix extension add -g WixToolset.UI.wixext
```

Build the MSI:
```powershell
cd BillFlow.Setup
wix build -o BillFlow-2.0.0.msi Package.wxs
```

## File Locations

| Purpose | Location |
|---------|----------|
| Application | `C:\Program Files\BillFlow\` |
| Database | `%LOCALAPPDATA%\BillFlow\billflow.db` |
| Settings | `%APPDATA%\BillFlow\` |
| Logs | `%APPDATA%\BillFlow\Logs\` |
| Backups | `%APPDATA%\BillFlow\Backups\` |

## Uninstallation

### MSI Uninstall
1. Windows Settings > Apps > BillFlow > Uninstall
2. Or run: `msiexec /x BillFlow-2.0.0.msi`

### Portable Uninstall
1. Delete application folder
2. Delete `%APPDATA%\BillFlow\` for complete removal

## Version History

- **v2.0.0** (2026-04-11) - Production Release
  - Complete WPF desktop application
  - Customer management with auto-generated IDs
  - Digital Khata (ledger) with credit tracking
  - Work orders with SqFt calculations
  - Daily schedule view
  - PDF invoice generation
  - SQLite offline database
  - Material Design UI with glassmorphism

## Support

For technical support or issues, please contact the development team.
