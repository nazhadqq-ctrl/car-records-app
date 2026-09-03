/**
 * 🚗 CAR MANAGEMENT SYSTEM - STANDALONE CONFIGURATION FILE
 * ═══════════════════════════════════════════════════════════════
 * You can edit this file to change the default server IP and port,
 * or configure Master Admin Passwords.
 * 
 * دەتوانیت ئەم فایله دەستکاری بکەیت بۆ گۆڕینی ئای‌پی سێرڤەر و وشەی نهێنی ئەدمین
 * ═══════════════════════════════════════════════════════════════
 */

window.APP_CONFIG = {
  // Default server address (IP:Port or http://domain)
  // ئای‌پی سێرڤەری سەرەکی کە سیستمەکەی لەسەر بەگەڕخراوە
  serverUrl: "http://192.168.1.100:3002",

  // Master Admin Passwords accepted locally (works even when server is offline/disconnected)
  // وشەکانی نهێنی ئەدمین کە بە بێ سێرڤەر و بە ئۆفلاین کار دەکەن
  adminMasterPasswords: [
    "Na2652014Va",
    "ChangeMeInDotEnv123",
    "admin"
  ],

  // App Metadata
  appName: "تۆمارکردنی زانیاری ئۆتۆمبێل",
  appVersion: "1.3.2"
};
