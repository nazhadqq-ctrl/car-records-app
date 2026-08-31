const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');

const userProfile = process.env.USERPROFILE || '';
const onedriveDesktop = path.join(userProfile, 'OneDrive', 'Desktop');
const standardDesktop = path.join(userProfile, 'Desktop');

const targets = [];
if (fs.existsSync(onedriveDesktop)) targets.push(onedriveDesktop);
if (fs.existsSync(standardDesktop)) targets.push(standardDesktop);

const vbsPath = path.resolve('Start-Desktop-App-Silent.vbs');
const workDir = path.resolve('.');
const iconPath = path.resolve('app.ico');

targets.forEach(desktopDir => {
  // Remove old shortcut if exists
  const oldLnk = path.join(desktopDir, 'Car-Management-System.lnk');
  if (fs.existsSync(oldLnk)) {
    try { fs.unlinkSync(oldLnk); } catch(e) {}
  }

  const lnkPath = path.join(desktopDir, 'تۆماری تاقیگەکان.lnk');
  const ps = `
    $w = New-Object -ComObject WScript.Shell;
    $s = $w.CreateShortcut([System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String('${Buffer.from(lnkPath, 'utf8').toString('base64')}')));
    $s.TargetPath = 'wscript.exe';
    $s.Arguments = '"${vbsPath}"';
    $s.WorkingDirectory = '${workDir}';
    $s.IconLocation = '${iconPath},0';
    $s.Description = 'تۆماری تاقیگەکان';
    $s.Save();
  `;
  execSync('powershell -NoProfile -Command "' + ps.replace(/\n/g, ' ') + '"');
});

console.log('✅ Desktop shortcut created with custom icon: تۆماری تاقیگەکان');
