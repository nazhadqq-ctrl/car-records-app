@echo off
title Car Management System Launcher
cd /d "%~dp0"

:: Find and kill any existing process holding port 3002 to ensure a clean start
for /f "tokens=5" %%a in ('netstat -aon ^| findstr :3002') do (
    if not "%%a"=="0" taskkill /F /PID %%a >nul 2>&1
)

:: Wait 1 second to ensure port is freed
ping 127.0.0.1 -n 2 >nul

:: Start the background Node server
wscript.exe "%~dp0start-background-silent.vbs"
ping 127.0.0.1 -n 4 >nul

:: Launch dedicated desktop window
if exist "%ProgramFiles(x86)%\Microsoft\Edge\Application\msedge.exe" (
    start "" "%ProgramFiles(x86)%\Microsoft\Edge\Application\msedge.exe" --app=http://localhost:3002 --start-maximized
) else if exist "%ProgramFiles%\Microsoft\Edge\Application\msedge.exe" (
    start "" "%ProgramFiles%\Microsoft\Edge\Application\msedge.exe" --app=http://localhost:3002 --start-maximized
) else if exist "%ProgramFiles%\Google\Chrome\Application\chrome.exe" (
    start "" "%ProgramFiles%\Google\Chrome\Application\chrome.exe" --app=http://localhost:3002 --start-maximized
) else if exist "%ProgramFiles(x86)%\Google\Chrome\Application\chrome.exe" (
    start "" "%ProgramFiles(x86)%\Google\Chrome\Application\chrome.exe" --app=http://localhost:3002 --start-maximized
) else (
    start http://localhost:3002
)

exit
