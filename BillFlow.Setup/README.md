# BillFlow Setup

## Building the Installer

### Prerequisites
1. Install WiX Toolset v4:
```powershell
dotnet tool install --global wix
wix extension add -g WixToolset.UI.wixext
```

2. Build the application in Release mode:
```powershell
dotnet build ..\BillFlow.sln -c Release
```

### Build Installer
Run the build script:
```powershell
.\BuildInstaller.ps1
```

Or manually:
```powershell
# Publish self-contained
dotnet publish ..\BillFlow\BillFlow.csproj -c Release -r win-x64 --self-contained -o .\publish

# Build MSI
wix build -o BillFlow-2.0.0.msi Package.wxs
```

## Output
- `BillFlow-2.0.0.msi` - Windows Installer package
