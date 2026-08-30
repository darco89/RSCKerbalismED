$logFiles = @(
    "C:\Users\Diogo\AppData\LocalLow\Squad\Kerbal Space Program\Player.log"
    "K:\Games\KSP_instances\KSP_CustomPatches\KSP.log"
    "K:\Games\KSP_instances\KSP_CustomPatches\GameData\RSCKerbalismED\KSP.log"
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

    Start-Process -FilePath $gameExe -ArgumentList @(
        "-popupwindow"
        "-singleinstance"
    )
}
else {

    Write-Host "KSP executable not found: $gameExe" -ForegroundColor Red
}

Write-Host "Waiting 10 seconds for logs..." -ForegroundColor Cyan
Start-Sleep -Seconds 10

$notepadExe = "C:\Arco\Programas\Notepad++\notepad++.exe"

$logFilesToOpen = @(
    "C:\Users\Diogo\AppData\LocalLow\Squad\Kerbal Space Program\Player.log"
    "K:\Games\KSP_instances\KSP_CustomPatches\KSP.log"
)

if (Test-Path $notepadExe) {

    Write-Host "Opening logs in Notepad++..." -ForegroundColor Cyan

    foreach ($file in $logFilesToOpen) {

        if (Test-Path $file) {

            Start-Process -FilePath $notepadExe -ArgumentList "`"$file`""
        }
        else {

            Write-Host "Log not found: $file" -ForegroundColor Yellow
        }
    }
}
else {

    Write-Host "Notepad++ executable not found: $notepadExe" -ForegroundColor Red
}