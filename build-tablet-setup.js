const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

const rootDir = path.resolve('.');
const distDir = path.join(rootDir, 'dist-package');

console.log('🚀 Building Standalone Windows Tablet Package (Optimized Fast Build)...');

// 1. Ensure dist folder
if (fs.existsSync(distDir)) {
  try {
    fs.rmSync(distDir, { recursive: true, force: true, maxRetries: 3 });
  } catch (e) {
    // If folder is open in explorer, continue and overwrite files
  }
}
fs.mkdirSync(distDir, { recursive: true });
fs.mkdirSync(path.join(distDir, 'bin'), { recursive: true });

// 2. Copy embedded node.exe
const systemNodeExe = 'C:\\Program Files\\nodejs\\node.exe';
if (fs.existsSync(systemNodeExe)) {
  console.log('📦 Copying embedded Node.js binary (so other tablets don\'t need Node installed)...');
  fs.copyFileSync(systemNodeExe, path.join(distDir, 'bin', 'node.exe'));
} else {
  fs.copyFileSync(process.execPath, path.join(distDir, 'bin', 'node.exe'));
}

// 3. Copy required files
console.log('📦 Copying application source files...');
fs.copyFileSync(path.join(rootDir, 'server.js'), path.join(distDir, 'server.js'));
fs.copyFileSync(path.join(rootDir, 'auto-updater.js'), path.join(distDir, 'auto-updater.js'));
fs.copyFileSync(path.join(rootDir, 'version.json'), path.join(distDir, 'version.json'));
fs.copyFileSync(path.join(rootDir, 'github-sync.js'), path.join(distDir, 'github-sync.js'));
fs.copyFileSync(path.join(rootDir, 'config.json'), path.join(distDir, 'config.json'));
fs.copyFileSync(path.join(rootDir, '.env'), path.join(distDir, '.env'));
fs.copyFileSync(path.join(rootDir, 'package.json'), path.join(distDir, 'package.json'));

fs.mkdirSync(path.join(distDir, 'public'), { recursive: true });
fs.mkdirSync(path.join(distDir, 'node_modules'), { recursive: true });

console.log('📦 Copying public web assets...');
execSync(`xcopy /E /I /Y /Q "${path.join(rootDir, 'public')}\\*" "${path.join(distDir, 'public')}\\"`);

console.log('📦 Copying node_modules with fast xcopy...');
execSync(`xcopy /E /I /Y /Q "${path.join(rootDir, 'node_modules')}\\*" "${path.join(distDir, 'node_modules')}\\"`);

// Copy app.ico if present
if (fs.existsSync(path.join(rootDir, 'app.ico'))) {
  fs.copyFileSync(path.join(rootDir, 'app.ico'), path.join(distDir, 'app.ico'));
}

// 4. Create Standalone Launcher Scripts for the target tablet
console.log('⚙️ Creating standalone launcher scripts for Windows tablets...');

// Start-App.bat
const startBat = `@echo off
title تۆماری تاقیگەکان
cd /d "%~dp0"

:: Use embedded node if available, otherwise fallback to system node
if exist "%~dp0bin\\node.exe" (
    set "NODE_BIN=%~dp0bin\\node.exe"
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
if exist "%ProgramFiles(x86)%\\Microsoft\\Edge\\Application\\msedge.exe" (
    start "" "%ProgramFiles(x86)%\\Microsoft\\Edge\\Application\\msedge.exe" --app=http://localhost:3002 --window-size=1300,850
) else if exist "%ProgramFiles%\\Microsoft\\Edge\\Application\\msedge.exe" (
    start "" "%ProgramFiles%\\Microsoft\\Edge\\Application\\msedge.exe" --app=http://localhost:3002 --window-size=1300,850
) else if exist "%ProgramFiles%\\Google\\Chrome\\Application\\chrome.exe" (
    start "" "%ProgramFiles%\\Google\\Chrome\\Application\\chrome.exe" --app=http://localhost:3002 --window-size=1300,850
) else (
    start http://localhost:3002
)
exit
`;
fs.writeFileSync(path.join(distDir, 'Start-App.bat'), startBat, 'utf8');

// Start-App-Silent.vbs
const startVbs = `Set WshShell = CreateObject("WScript.Shell")
Set fso = CreateObject("Scripting.FileSystemObject")
currentDir = fso.GetParentFolderName(WScript.ScriptFullName)
WshShell.CurrentDirectory = currentDir

' Function to test if server on port 3002 is responding
Function IsServerActive()
    On Error Resume Next
    Dim http
    Set http = CreateObject("MSXML2.ServerXMLHTTP.6.0")
    http.setTimeouts 200, 200, 200, 200
    http.open "GET", "http://localhost:3002/api/setup-status", False
    http.send
    If Err.Number = 0 And (http.Status = 200 Or http.Status = 304) Then
        IsServerActive = True
    Else
        IsServerActive = False
    End If
    Set http = Nothing
    On Error GoTo 0
End Function

' Start node server silently in background with ZERO CMD window
If Not IsServerActive() Then
    If fso.FileExists(currentDir & "\\bin\\node.exe") Then
        WshShell.Run """" & currentDir & "\\bin\\node.exe"" server.js", 0, False
    ElseIf fso.FileExists(currentDir & "\\node.exe") Then
        WshShell.Run """" & currentDir & "\\node.exe"" server.js", 0, False
    Else
        WshShell.Run "node server.js", 0, False
    End If
    For i = 1 To 25
        WScript.Sleep 200
        If IsServerActive() Then Exit For
    Next
End If

' Launch App Window without any CMD window
Dim edgePath1, edgePath2, chromePath1, chromePath2
edgePath1 = WshShell.ExpandEnvironmentStrings("%ProgramFiles(x86)%\\Microsoft\\Edge\\Application\\msedge.exe")
edgePath2 = WshShell.ExpandEnvironmentStrings("%ProgramFiles%\\Microsoft\\Edge\\Application\\msedge.exe")
chromePath1 = WshShell.ExpandEnvironmentStrings("%ProgramFiles%\\Google\\Chrome\\Application\\chrome.exe")
chromePath2 = WshShell.ExpandEnvironmentStrings("%ProgramFiles(x86)%\\Google\\Chrome\\Application\\chrome.exe")

If fso.FileExists(edgePath1) Then
    WshShell.Run """" & edgePath1 & """ --app=http://localhost:3002 --start-maximized", 1, False
ElseIf fso.FileExists(edgePath2) Then
    WshShell.Run """" & edgePath2 & """ --app=http://localhost:3002 --start-maximized", 1, False
ElseIf fso.FileExists(chromePath1) Then
    WshShell.Run """" & chromePath1 & """ --app=http://localhost:3002 --start-maximized", 1, False
ElseIf fso.FileExists(chromePath2) Then
    WshShell.Run """" & chromePath2 & """ --app=http://localhost:3002 --start-maximized", 1, False
Else
    WshShell.Run "http://localhost:3002", 1, False
End If
`;
fs.writeFileSync(path.join(distDir, 'Start-App-Silent.vbs'), startVbs, 'utf8');

// Setup.vbs
const setupVbs = `' 🚗 تۆماری تاقیگەکان - Standalone Desktop Installer (VBS)
Option Explicit

Dim fso, wshShell, currentDir, localAppData, installDir, desktopPath, programsPath
Dim targetVbs, iconPath, shortcut, menuShortcut

Set fso = CreateObject("Scripting.FileSystemObject")
Set wshShell = CreateObject("WScript.Shell")

currentDir = fso.GetParentFolderName(WScript.ScriptFullName)
localAppData = wshShell.ExpandEnvironmentStrings("%LOCALAPPDATA%")
installDir = localAppData & "\\CarManagementSystem"

If Not fso.FolderExists(installDir) Then
    fso.CreateFolder(installDir)
End If

wshShell.Run "robocopy """ & currentDir & """ """ & installDir & """ /E /IS /IT /NFL /NDL /NJH /NJS /nc /ns /np", 0, True

targetVbs = installDir & "\\Start-App-Silent.vbs"
iconPath = installDir & "\\app.ico"
desktopPath = wshShell.SpecialFolders("Desktop")
programsPath = wshShell.SpecialFolders("Programs")

Set shortcut = wshShell.CreateShortcut(desktopPath & "\\تۆماری تاقیگەکان.lnk")
shortcut.TargetPath = "wscript.exe"
shortcut.Arguments = """" & targetVbs & """"
shortcut.WorkingDirectory = installDir
shortcut.IconLocation = iconPath & ",0"
shortcut.Description = "تۆماری تاقیگەکان — دیزاین و پرۆگرامسازی: NAZHAD Q. MAHAMMED"
shortcut.Save

Set menuShortcut = wshShell.CreateShortcut(programsPath & "\\تۆماری تاقیگەکان.lnk")
menuShortcut.TargetPath = "wscript.exe"
menuShortcut.Arguments = """" & targetVbs & """"
menuShortcut.WorkingDirectory = installDir
menuShortcut.IconLocation = iconPath & ",0"
menuShortcut.Description = "تۆماری تاقیگەکان — دیزاین و پرۆگرامسازی: NAZHAD Q. MAHAMMED"
menuShortcut.Save

wshShell.CurrentDirectory = installDir
wshShell.Run """" & targetVbs & """", 0, False

MsgBox "✅ بە سەرکەوتوویی دامەزرا!" & vbCrLf & vbCrLf & _
       "ئایکۆنی پرۆگرامەکە بەناوی (تۆماری تاقیگەکان) خرایە سەر ڕووی شاشە (Desktop) و لیستی پرۆگرامەکان." & vbCrLf & _
       "دیزاین و پرۆگرامسازی: NAZHAD Q. MAHAMMED" & vbCrLf & _
       "پرۆگرامەکە ئێستا کرایەوە.", vbInformation, "تۆماری تاقیگەکان"
`;
fs.writeFileSync(path.join(distDir, 'Setup.vbs'), setupVbs, 'utf8');

// Compile Setup.exe with embedded icon
console.log('🔨 Compiling native Windows Setup.exe with embedded icon...');
const cscPath = 'C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\csc.exe';
if (fs.existsSync(cscPath) && fs.existsSync(path.join(rootDir, 'Setup.cs'))) {
  const iconFlag = fs.existsSync(path.join(rootDir, 'app.ico')) ? `/win32icon:"${path.join(rootDir, 'app.ico')}"` : '';
  execSync(`"${cscPath}" /target:winexe ${iconFlag} /out:"${path.join(distDir, 'Setup.exe')}" /r:System.Windows.Forms.dll "${path.join(rootDir, 'Setup.cs')}"`);
}

// Readme for Tablet Users
const readmeTxt = `تۆماری تاقیگەکان — سیستەمی تۆمارکردن و پشکنینی ئۆتۆمبێل
=======================================================
دیزاین و پرۆگرامسازی: NAZHAD Q. MAHAMMED
هەموو مافەکانی پارێزراوە © 2026 NAZHAD Q. MAHAMMED
=======================================================
ڕێنمایی دامەزراندن لەسەر تابلێتی ویندۆز و کۆمپیوتەری تر:

١. ئەم فۆڵدەرە (یان فایلی ZIP) کۆپی بکە بۆ سەر تابلێتەکەت لە ڕێگەی فلاش (USB) یان هێڵی ناوخۆ.
٢. کلیک لەسەر فایلی (Setup.exe) یان (Setup.vbs) بکە.
٣. بە شێوەی خۆکار پرۆگرامەکە دادەمەزرێت و ئایکۆنی (تۆماری تاقیگەکان) لەسەر Desktop دروست دەکات.
٤. هیچ پێویست ناکات Node.js یان هیچ شتێکی تر لەسەر تابلێتەکە دابەزێنیت، چونکە هەموو پێداویستییەکان لەناو ئەم پاکێجەدا ئامادەکراون!
=======================================================
`;
fs.writeFileSync(path.join(distDir, 'README-TABLET.txt'), readmeTxt, 'utf8');

console.log('📦 Compressing into standalone ZIP package...');
const zipFile = path.join(rootDir, 'Car-Management-Tablet-Setup.zip');
if (fs.existsSync(zipFile)) {
  try { fs.unlinkSync(zipFile); } catch(e) {}
}

try {
  // Use tar.exe from within distDir to ensure paths don't have leading './' (which breaks Windows Explorer)
  execSync(`powershell -NoProfile -Command "Push-Location '${distDir}'; tar.exe -a -c -f '${zipFile}' *; Pop-Location"`, { stdio: 'inherit' });
} catch(err) {
  console.error('Error creating ZIP with tar:', err.message);
  try {
    execSync(`powershell -NoProfile -Command "Compress-Archive -Path '${distDir}\\*' -DestinationPath '${zipFile}' -Force"`, { stdio: 'inherit' });
  } catch(psErr) {
    console.log('⚠️ Note: Direct folder dist-package is ready and can be copied directly.');
  }
}

console.log('🎉 STANDALONE TABLET SETUP PACKAGE CREATED SUCCESSFULLY!');
console.log('📁 Output Folder:', distDir);
console.log('📦 Output ZIP File:', zipFile);

