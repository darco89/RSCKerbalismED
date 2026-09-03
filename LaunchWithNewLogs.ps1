$projectPath = $PSScriptRoot
$gamePath = Split-Path $projectPath -Parent | Split-Path -Parent

# ============================================================
# LOG FILES
# ============================================================

$playerLog = Join-Path $env:USERPROFILE "AppData\LocalLow\Squad\Kerbal Space Program\Player.log"
$gameLog = Join-Path $gamePath "KSP.log"
$projectLog = Join-Path $projectPath "KSP.log"

$logFiles = @(
    $playerLog
    $gameLog
    $projectLog
)

# ============================================================
# DELETE EXISTING LOGS
# ============================================================

foreach ($file in $logFiles) {

    if (Test-Path $file) {

        Remove-Item $file -Force

        Write-Host "Deleted: $file" -ForegroundColor Green
    }
    else {

        Write-Host "Not found: $file" -ForegroundColor Yellow
    }
}

# ============================================================
# LAUNCH KSP
# ============================================================

$gameExe = Join-Path $gamePath "KSP_x64.exe"

if (Test-Path $gameExe) {

    Write-Host "Launching KSP..." -ForegroundColor Cyan

    Start-Process `
        -FilePath $gameExe `
        -WorkingDirectory $projectPath `
        -ArgumentList @(
            "-popupwindow"
            "-singleinstance"
        )
}
else {

    Write-Host "KSP executable not found: $gameExe" -ForegroundColor Red
    exit 1
}

# ============================================================
# WAIT FOR LOGS
# ============================================================

Write-Host "Waiting 10 seconds for logs..." -ForegroundColor Cyan
Start-Sleep -Seconds 10

# ============================================================
# OPEN LOGS IN NOTEPAD++
# ============================================================

$notepadShortcut = "C:\Program Files\Notepad++\Notepad++.lnk"

if (Test-Path $notepadShortcut) {

    $shell = New-Object -ComObject WScript.Shell
    $shortcut = $shell.CreateShortcut($notepadShortcut)
    $notepadExe = $shortcut.TargetPath

    if (Test-Path $notepadExe) {

        Write-Host "Opening logs in Notepad++..." -ForegroundColor Cyan

        foreach ($file in $logFiles) {

            if (Test-Path $file) {

                Start-Process `
                    -FilePath $notepadExe `
                    -ArgumentList "`"$file`""
            }
            else {

                Write-Host "Log not found: $file" -ForegroundColor Yellow
            }
        }
    }
    else {

        Write-Host "Notepad++ executable not found: $notepadExe" -ForegroundColor Red
    }
}
else {

    Write-Host "Notepad++ shortcut not found: $notepadShortcut" -ForegroundColor Red
}