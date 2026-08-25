/* ═══════════════════════════════════════════════════════════════
   🚗 AUTOMATED GITHUB REPOSITORY SYNC & DEPLOYMENT TOOL
   Pushes/updates project files to GitHub repository via REST API
   ═══════════════════════════════════════════════════════════════ */

const fs = require('fs');
const path = require('path');
const https = require('https');

const REPO_OWNER = process.env.GITHUB_OWNER || 'Azh4aa';
const REPO_NAME = process.env.GITHUB_REPO || 'car-management-system';
const GITHUB_TOKEN = process.env.GITHUB_TOKEN || process.argv[2] || '';

const FILES_TO_SYNC = [
  'server.js',
  'package.json',
  'config.json',
  'Procfile',
  'railway.json',
  'render.yaml',
  'ecosystem.config.js',
  'start-service.bat',
  'start-cloudflare-tunnel.bat',
  '.gitignore',
  'public/index.html',
  'public/app.js',
  'public/styles.css'
];

if (!GITHUB_TOKEN) {
  console.log(`
🔒 GITHUB REPOSITORY SYNC TOOL
======================================================
Repository: https://github.com/${REPO_OWNER}/${REPO_NAME}

To run automated sync, please provide your GitHub Personal Access Token (PAT):

Usage:
  node github-sync.js <YOUR_GITHUB_TOKEN>

Or set environment variable:
  $env:GITHUB_TOKEN="your_token_here"
  node github-sync.js

How to get a free GitHub Token (30 seconds):
  1. Go to https://github.com/settings/tokens
  2. Click "Generate new token (classic)"
  3. Check "repo" scope and click "Generate token".
======================================================
`);
  process.exit(0);
}

function makeApiRequest(apiPath, method, bodyObj) {
  return new Promise((resolve, reject) => {
    const options = {
      hostname: 'api.github.com',
      path: apiPath,
      method: method,
      headers: {
        'User-Agent': 'Car-App-Sync-Engine',
        'Authorization': `token ${GITHUB_TOKEN}`,
        'Accept': 'application/vnd.github.v3+json',
        'Content-Type': 'application/json'
      }
    };

    const req = https.request(options, (res) => {
      let data = '';
      res.on('data', chunk => data += chunk);
      res.on('end', () => {
        try {
          const parsed = JSON.parse(data);
          resolve({ status: res.statusCode, data: parsed });
        } catch (e) {
          resolve({ status: res.statusCode, raw: data });
        }
      });
    });

    req.on('error', reject);
    if (bodyObj) req.write(JSON.stringify(bodyObj));
    req.end();
  });
}

async function syncFiles() {
  console.log(`🚀 Starting sync to https://github.com/${REPO_OWNER}/${REPO_NAME}...\n`);

  for (const relPath of FILES_TO_SYNC) {
    const fullPath = path.join(__dirname, relPath);
    if (!fs.existsSync(fullPath)) continue;

    const fileContent = fs.readFileSync(fullPath);
    const base64Content = fileContent.toString('base64');
    const apiPath = `/repos/${REPO_OWNER}/${REPO_NAME}/contents/${relPath}`;

    try {
      // Check if file already exists to get SHA
      const getRes = await makeApiRequest(apiPath, 'GET');
      const sha = (getRes.status === 200 && getRes.data && getRes.data.sha) ? getRes.data.sha : null;

      const payload = {
        message: `Update ${relPath} to latest build version`,
        content: base64Content,
        branch: 'main'
      };
      if (sha) payload.sha = sha;

      const putRes = await makeApiRequest(apiPath, 'PUT', payload);
      if (putRes.status === 200 || putRes.status === 201) {
        console.log(`  ✅ Successfully updated: ${relPath}`);
      } else {
        console.error(`  ❌ Failed to update ${relPath}: ${putRes.data ? putRes.data.message : putRes.status}`);
      }
    } catch (err) {
      console.error(`  ⚠️ Error updating ${relPath}:`, err.message);
    }
  }

  console.log('\n🎉 Sync execution completed!');
}

syncFiles();
