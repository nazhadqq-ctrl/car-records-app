const { app, BrowserWindow, Menu, shell } = require('electron');
const path = require('path');
const http = require('http');

// Automatically start the backend server inside the Electron process
let serverProcess = null;
try {
  // Start server.js directly
  require('./server.js');
} catch (err) {
  console.error('Failed to require server.js:', err);
}

let mainWindow = null;
const SERVER_PORT = process.env.PORT || 3002;
const SERVER_URL = `http://localhost:${SERVER_PORT}`;

function waitForServer(callback, maxAttempts = 30) {
  let attempts = 0;
  const interval = setInterval(() => {
    attempts++;
    http.get(SERVER_URL, (res) => {
      clearInterval(interval);
      callback(true);
    }).on('error', (err) => {
      if (attempts >= maxAttempts) {
        clearInterval(interval);
        callback(false);
      }
    });
  }, 300);
}

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 1300,
    height: 850,
    minWidth: 950,
    minHeight: 650,
    title: 'تۆمارکردنی زانیاری و وێنەی ئۆتۆمبێل — Car Management System',
    backgroundColor: '#060910',
    icon: path.join(__dirname, 'public', 'favicon.ico'),
    webPreferences: {
      nodeIntegration: false,
      contextIsolation: true,
      webSecurity: false
    }
  });

  // Remove default menu bar for clean modern desktop look
  Menu.setApplicationMenu(null);

  // Wait for local server to be ready, then load URL
  waitForServer((ready) => {
    if (ready && mainWindow) {
      mainWindow.loadURL(SERVER_URL);
    } else if (mainWindow) {
      mainWindow.loadURL(SERVER_URL);
    }
  });

  mainWindow.webContents.setWindowOpenHandler(({ url }) => {
    shell.openExternal(url);
    return { action: 'deny' };
  });

  mainWindow.on('closed', () => {
    mainWindow = null;
  });
}

app.whenReady().then(() => {
  createWindow();

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});
