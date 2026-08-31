@echo off
title Car Management System Launcher
cd /d "%~dp0"

:: Check if Node server is running on port 3002
netstat -ano | findstr :3002 >nul
if %errorlevel% neq 0 (
    wscript.exe "%~dp0start-background-silent.vbs"
    ping 127.0.0.1 -n 3 >nul
)

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
