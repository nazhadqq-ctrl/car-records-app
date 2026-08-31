@echo off
title Create Desktop Shortcut
cd /d "%~dp0"

set "SCRIPT_DIR=%~dp0"
set "TARGET_VBS=%SCRIPT_DIR%Start-Desktop-App-Silent.vbs"
set "SHORTCUT_NAME=تۆمارکردنی زانیاری ئۆتۆمبێل.lnk"

powershell -NoProfile -ExecutionPolicy Bypass -Command "$WshShell = New-Object -ComObject WScript.Shell; $Desktop = [System.Environment]::GetFolderPath('Desktop'); $Shortcut = $WshShell.CreateShortcut(\"$Desktop\%SHORTCUT_NAME%\"); $Shortcut.TargetPath = 'wscript.exe'; $Shortcut.Arguments = '\"%TARGET_VBS%\"'; $Shortcut.WorkingDirectory = '%SCRIPT_DIR%'; $Shortcut.Description = 'تۆمارکردنی زانیاری ئۆتۆمبێل — دیزاین و پرۆگرامسازی: NAZHAD Q. MAHAMMED'; $Shortcut.Save(); Write-Host '✅ شۆرتکەتی بەرنامەکە بە سەرکەوتوویی لەسەر دێسکتۆپ دروستکرا!' -ForegroundColor Green"

pause
