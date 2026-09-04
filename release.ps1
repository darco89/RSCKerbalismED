param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

# ============================================================
# RSCKerbalismED RELEASE SCRIPT
# ============================================================
# Assumes both the script and cs-project are in GameData.
# Takes steps to avoid .dll duplication in GameData.

$ProjectName = "RSCKerbalismED"

$ProjectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$ModRoot = $ProjectRoot
$ConfigRoot = Join-Path $ProjectRoot "Config"

# Leave release files outside of GameData folder
$BuildRoot = Join-Path $ProjectRoot "..\..\RSCKE_Builds"
$ReleaseRoot = Join-Path $BuildRoot "releases"

# Identify project to release
$ProjectFile = Join-Path $ProjectRoot "$ProjectName.csproj"

# DLL to release is looked up from where the script is called
$DllName = "$ProjectName.dll"
$DllPath = Join-Path $ProjectRoot "Plugins\$DllName"

# Release Info
$ReleaseName = "$ProjectName-$Version"
$PackageRoot = Join-Path $ReleaseRoot $ReleaseName
$PackageGameData = Join-Path $PackageRoot "GameData\$ProjectName"
$PackagePlugins = Join-Path $PackageGameData "Plugins"
$PackageDllPath = Join-Path $PackagePlugins $DllName
$ZipPath = Join-Path $ReleaseRoot "$ReleaseName.zip"

Write-Host ""
Write-Host "============================================================"
Write-Host " $ProjectName RELEASE"
Write-Host "============================================================"
Write-Host ""
Write-Host "Version: $Version"
Write-Host ""

# ------------------------------------------------------------
# Validate version
# ------------------------------------------------------------

if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    throw "Invalid version '$Version'. Expected format: x.y.z (for example: 0.1.0)"
}

# ------------------------------------------------------------
# Validate required paths
# ------------------------------------------------------------

if (-not (Test-Path $ProjectFile)) {
    throw "Project file not found: $ProjectFile"
}

if (-not (Test-Path $ModRoot)) {
    throw "Mod directory not found: $ModRoot"
}

if (-not (Test-Path $ConfigRoot)) {
    throw "Config directory not found: $ConfigRoot"
}

# ------------------------------------------------------------
# Clean previous package
# ------------------------------------------------------------

if (Test-Path $PackageRoot) {
    Remove-Item $PackageRoot -Recurse -Force
}

if (Test-Path $ZipPath) {
    Remove-Item $ZipPath -Force
}

New-Item -ItemType Directory -Path $PackageGameData -Force | Out-Null

# ------------------------------------------------------------
# Build
# ------------------------------------------------------------

Write-Host "Building $ProjectName..."
Write-Host ""

dotnet build $ProjectFile --configuration Release

if ($LASTEXITCODE -ne 0) {
    throw "Build failed."
}

Write-Host ""
Write-Host "Build successful."
Write-Host ""

# ------------------------------------------------------------
# Validate DLL
# ------------------------------------------------------------

if (-not (Test-Path $DllPath)) {
    throw "Built DLL not found: $DllPath"
}

# ------------------------------------------------------------
# Copy DLL
# ------------------------------------------------------------

New-Item -ItemType Directory -Path $PackagePlugins -Force | Out-Null

Copy-Item `
    -Path $DllPath `
    -Destination $PackageDllPath `
    -Force

# ------------------------------------------------------------
# Copy CFG files
# ------------------------------------------------------------

Get-ChildItem `
    -Path $ConfigRoot `
    -Filter "*.cfg" `
    -File |
    Copy-Item -Destination $PackageGameData -Force

# ------------------------------------------------------------
# Create ZIP
# ------------------------------------------------------------

Write-Host "Creating release package..."
Write-Host ""

Compress-Archive `
    -Path (Join-Path $PackageRoot "*") `
    -DestinationPath $ZipPath `
    -CompressionLevel Optimal

# ------------------------------------------------------------
# Report
# ------------------------------------------------------------

Write-Host ""
Write-Host "============================================================"
Write-Host " RELEASE CREATED"
Write-Host "============================================================"
Write-Host ""
Write-Host "Package:"
Write-Host "  $ZipPath"
Write-Host ""
Write-Host "Contents:"
Write-Host "  $PackageDllPath"

Get-ChildItem `
    -Path $PackageGameData `
    -Filter "*.cfg" `
    -File |
    ForEach-Object {
        Write-Host "  $($_.FullName)"
    }

Write-Host ""
Write-Host "============================================================"
Write-Host ""