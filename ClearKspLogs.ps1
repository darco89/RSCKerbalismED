$logFiles = @(
    "C:\Users\Diogo\AppData\LocalLow\Squad\Kerbal Space Program\Player.log"
    "K:\Games\KSP_instances\KSP_CustomPatches\KSP.log"
    "K:\Games\KSP_instances\KSP_CustomPatches\GameData\KerbalismRSC\KSP.log"
)

foreach ($file in $logFiles) {
    if (Test-Path $file) {
        Remove-Item $file -Force
        Write-Host "Deleted: $file" -ForegroundColor Green
    }
    else {
        Write-Host "Not found: $file" -ForegroundColor Yellow
    }
}

$gameExe = "K:\Games\KSP_instances\KSP_CustomPatches\KSP_x64.exe"

if (Test-Path $gameExe) {
    Write-Host "Launching KSP..." -ForegroundColor Cyan
    Start-Process $gameExe -ArgumentList "-popupwindow", "-singleinstance"
}
else {
    Write-Host "KSP executable not found: $gameExe" -ForegroundColor Red
}


