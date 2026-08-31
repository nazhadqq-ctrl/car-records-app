Set WshShell = CreateObject("WScript.Shell")
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
    If fso.FileExists(currentDir & "\bin\node.exe") Then
        WshShell.Run """" & currentDir & "\bin\node.exe"" server.js", 0, False
    ElseIf fso.FileExists(currentDir & "\node.exe") Then
        WshShell.Run """" & currentDir & "\node.exe"" server.js", 0, False
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
edgePath1 = WshShell.ExpandEnvironmentStrings("%ProgramFiles(x86)%\Microsoft\Edge\Application\msedge.exe")
edgePath2 = WshShell.ExpandEnvironmentStrings("%ProgramFiles%\Microsoft\Edge\Application\msedge.exe")
chromePath1 = WshShell.ExpandEnvironmentStrings("%ProgramFiles%\Google\Chrome\Application\chrome.exe")
chromePath2 = WshShell.ExpandEnvironmentStrings("%ProgramFiles(x86)%\Google\Chrome\Application\chrome.exe")

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
