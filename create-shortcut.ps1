$WshShell = New-Object -ComObject WScript.Shell
$Desktop = [System.Environment]::GetFolderPath('Desktop')
$Target = (Join-Path $PSScriptRoot 'Start-Desktop-App-Silent.vbs')
$IconPath = (Join-Path $PSScriptRoot 'app.ico')
$ShortcutPath = (Join-Path $Desktop 'تۆماری تاقیگەکان.lnk')

# Remove old shortcut if exists
$OldShortcut = (Join-Path $Desktop 'Car-Management-System.lnk')
if (Test-Path $OldShortcut) { Remove-Item $OldShortcut -Force -ErrorAction SilentlyContinue }

$Shortcut = $WshShell.CreateShortcut($ShortcutPath)
$Shortcut.TargetPath = 'wscript.exe'
$Shortcut.Arguments = "`"$Target`""
$Shortcut.WorkingDirectory = $PSScriptRoot
$Shortcut.IconLocation = "$IconPath,0"
$Shortcut.Description = 'تۆماری تاقیگەکان - سیستەمی پشکنینی ئۆتۆمبێل'
$Shortcut.Save()

Write-Host "✅ Desktop Shortcut successfully created at: $ShortcutPath" -ForegroundColor Green
