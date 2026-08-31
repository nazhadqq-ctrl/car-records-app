Set WshShell = CreateObject("WScript.Shell")
Set fso = CreateObject("Scripting.FileSystemObject")
currentDir = fso.GetParentFolderName(WScript.ScriptFullName)
WshShell.CurrentDirectory = currentDir

nodeExe = "node"
If fso.FileExists("C:\Program Files\nodejs\node.exe") Then
    nodeExe = """C:\Program Files\nodejs\node.exe"""
ElseIf fso.FileExists(currentDir & "\bin\node.exe") Then
    nodeExe = """" & currentDir & "\bin\node.exe"""
End If

WshShell.Run nodeExe & " """ & currentDir & "\server.js""", 0, False
