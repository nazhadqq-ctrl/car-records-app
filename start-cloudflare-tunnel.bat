@echo off
title Cloudflare Tunnel - Car Records SQL Server App
color 0b
echo =======================================================
echo   Starting Car Records SQL Server App + Cloudflare
echo =======================================================
echo.
start "Car App Server" /min node server.js
timeout /t 2 /nobreak >nul
echo.
echo Connecting to Cloudflare Global Network...
cloudflared tunnel --url http://127.0.0.1:3002
pause
