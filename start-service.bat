@echo off
title Car Inspection App - Windows Service Setup
color 0A
echo ========================================================
echo   CAR INSPECTION SQL SERVER APP - AUTO INSTALLER
echo   Permanent 24/7 Background Service Setup
echo ========================================================
echo.

:: Check for Administrator privileges
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo [ERROR] Please right-click this file and select 'Run as Administrator'.
    echo.
    pause
    exit /b 1
)

echo [1/4] Checking Node.js environment...
node -v >nul 2>&1
if %errorLevel% neq 0 (
    echo [ERROR] Node.js is not installed on this server!
    echo Please download and install Node.js from https://nodejs.org
    echo.
    pause
    exit /b 1
)
echo      Node.js is detected.

echo [2/4] Installing project dependencies (mssql, lucide)...
call npm install --omit=dev

echo [3/4] Opening Port 3002 in Windows Firewall...
netsh advfirewall firewall delete rule name="CarInspectionApp-Port3002" >nul 2>&1
netsh advfirewall firewall add rule name="CarInspectionApp-Port3002" dir=in action=allow protocol=TCP localport=3002 >nul 2>&1
echo      Firewall Port 3002 opened.

echo [4/4] Installing PM2 process manager for 24/7 auto-start...
call npm install -g pm2
call npm install -g pm2-windows-service

echo.
echo Starting Application Service with PM2...
call pm2 delete car-app >nul 2>&1
call pm2 start server.js --name "car-app"
call pm2 save

echo.
echo ========================================================
echo   SUCCESS! The Car Inspection App is running 24/7!
echo.
echo   Local Address on Server: http://localhost:3002
echo   Network Address:         http://62.201.232.190:3002
echo ========================================================
echo.
pause
