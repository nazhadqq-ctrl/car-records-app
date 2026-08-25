' ═══════════════════════════════════════════════════════════════
' 🚗 SILENT BACKGROUND SERVICE LAUNCHER FOR WINDOWS
' Launches car-sqlserver-app without displaying any CMD window
' ═══════════════════════════════════════════════════════════════
Set WshShell = CreateObject("WScript.Shell")
WshShell.CurrentDirectory = CreateObject("Scripting.FileSystemObject").GetAbsolutePathName(".")
WshShell.Run "cmd.exe /c node server.js", 0, False
