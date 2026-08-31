@echo off
title تۆماری تاقیگەکان
cd /d "%~dp0"

:: Use embedded node if available, otherwise fallback to system node
if exist "%~dp0bin\node.exe" (
    set "NODE_BIN=%~dp0bin\node.exe"
) else (
    set "NODE_BIN=node"
)

:: Check if port 3002 is running
netstat -ano | findstr :3002 >nul
if %errorlevel% neq 0 (
    start /min "" "%NODE_BIN%" server.js
    ping 127.0.0.1 -n 3 >nul
)

:: Launch dedicated App Window (Edge / Chrome)
if exist "%ProgramFiles(x86)%\Microsoft\Edge\Application\msedge.exe" (
    start "" "%ProgramFiles(x86)%\Microsoft\Edge\Application\msedge.exe" --app=http://localhost:3002 --window-size=1300,850
) else if exist "%ProgramFiles%\Microsoft\Edge\Application\msedge.exe" (
    start "" "%ProgramFiles%\Microsoft\Edge\Application\msedge.exe" --app=http://localhost:3002 --window-size=1300,850
) else if exist "%ProgramFiles%\Google\Chrome\Application\chrome.exe" (
    start "" "%ProgramFiles%\Google\Chrome\Application\chrome.exe" --app=http://localhost:3002 --window-size=1300,850
) else (
    start http://localhost:3002
)
exit
