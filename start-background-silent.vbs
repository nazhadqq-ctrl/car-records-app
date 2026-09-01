Set WshShell = CreateObject("WScript.Shell")
Set fso = CreateObject("Scripting.FileSystemObject")
currentDir = fso.GetParentFolderName(WScript.ScriptFullName)
WshShell.CurrentDirectory = currentDir

' Check node paths
nodeExe = "node"
Dim localApp, prog86
localApp = WshShell.ExpandEnvironmentStrings("%LOCALAPPDATA%")
prog86 = WshShell.ExpandEnvironmentStrings("%ProgramFiles(x86)%")

If fso.FileExists(currentDir & "\bin\node.exe") Then
    nodeExe = """" & currentDir & "\bin\node.exe"""
ElseIf fso.FileExists(currentDir & "\node.exe") Then
    nodeExe = """" & currentDir & "\node.exe"""
ElseIf fso.FileExists("C:\Program Files\nodejs\node.exe") Then
    nodeExe = """C:\Program Files\nodejs\node.exe"""
ElseIf fso.FileExists(prog86 & "\nodejs\node.exe") Then
    nodeExe = """" & prog86 & "\nodejs\node.exe"""
ElseIf fso.FileExists(localApp & "\Programs\node\node.exe") Then
    nodeExe = """" & localApp & "\Programs\node\node.exe"""
End If

WshShell.Run nodeExe & " """ & currentDir & "\server.js""", 0, False
