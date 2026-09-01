$scriptPath = $PSScriptRoot
$gameDataPath = Split-Path $scriptPath -Parent
$instancePath = Split-Path $gameDataPath -Parent

$logFiles = @(
    Join-Path $env:USERPROFILE "AppData\LocalLow\Squad\Kerbal Space Program\Player.log"
    Join-Path $instancePath "KSP.log"
    Join-Path $scriptPath "KSP.log"
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

$gameExe = Join-Path $instancePath "KSP_x64.exe"

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

$notepadShortcut = Join-Path $env:USERPROFILE "Desktop\KSP Logs.lnk"

$logFilesToOpen = @(
    Join-Path $env:USERPROFILE "AppData\LocalLow\Squad\Kerbal Space Program\Player.log"
    Join-Path $instancePath "KSP.log"
)

if (Test-Path $notepadShortcut) {

    $shell = New-Object -ComObject WScript.Shell
    $shortcut = $shell.CreateShortcut($notepadShortcut)
    $notepadExe = $shortcut.TargetPath

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

        Write-Host "Shortcut target not found: $notepadExe" -ForegroundColor Red
    }
}
else {

    Write-Host "Notepad++ shortcut not found: $notepadShortcut" -ForegroundColor Red
}