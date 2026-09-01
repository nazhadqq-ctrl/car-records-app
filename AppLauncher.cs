using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace CarAppLauncher
{
    static class Program
    {
        private const string AppUrl = "http://localhost:3002";
        private const string HealthUrl = "http://127.0.0.1:3002/api/setup-status";

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
                Environment.CurrentDirectory = baseDir;

                // 1. Check if server is already running and healthy
                if (!IsServerHealthy())
                {
                    // 2. Kill any hung or dead process holding port 3002
                    KillPortProcess(3002);
                    Thread.Sleep(300);

                    // 3. Locate Node.js binary
                    string nodeExe = FindNodeBinary(baseDir);
                    if (string.IsNullOrEmpty(nodeExe))
                    {
                        MessageBox.Show(
                            "❌ پڕۆگرامی پێویست (Node.js) نەدۆزرایەوە!\n\nتکایە سەرەتا فایلی Setup.exe دابمەزرێنە یان Node.js دابەزێنە.",
                            "تۆماری تاقیگەکان",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }

                    // 4. Start Node.js background server silently
                    string serverJs = Path.Combine(baseDir, "server.js");
                    if (!File.Exists(serverJs))
                    {
                        MessageBox.Show(
                            "❌ فایلی سەرەکی server.js نەدۆزرایەوە!",
                            "تۆماری تاقیگەکان",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }

                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = nodeExe;
                    psi.Arguments = "\"" + serverJs + "\"";
                    psi.WorkingDirectory = baseDir;
                    psi.CreateNoWindow = true;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    psi.UseShellExecute = false;

                    Process.Start(psi);

                    // 5. Wait until server is fully responsive (up to 8 seconds)
                    int attempts = 0;
                    while (attempts < 30)
                    {
                        Thread.Sleep(250);
                        if (IsServerHealthy()) break;
                        attempts++;
                    }
                }

                // 6. Launch Application Window (Edge App mode -> Chrome App mode -> Default Browser)
                LaunchAppWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "هەڵە لە کاتی دەستپێکردنی پڕۆگرامەکە:\n" + ex.Message,
                    "تۆماری تاقیگەکان",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private static bool IsServerHealthy()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(HealthUrl);
                request.Timeout = 600;
                request.ReadWriteTimeout = 600;
                request.Method = "GET";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }

        private static void KillPortProcess(int port)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "cmd.exe";
                psi.Arguments = string.Format("/c for /f \"tokens=5\" %a in ('netstat -aon ^| findstr :{0}') do taskkill /F /PID %a >nul 2>&1", port);
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;

                using (Process p = Process.Start(psi))
                {
                    p.WaitForExit(1000);
                }
            }
            catch { }
        }

        private static string FindNodeBinary(string baseDir)
        {
            string[] candidatePaths = new string[]
            {
                Path.Combine(baseDir, "bin", "node.exe"),
                Path.Combine(baseDir, "node.exe"),
                @"C:\Program Files\nodejs\node.exe",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"nodejs\node.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\node\node.exe")
            };

            foreach (string path in candidatePaths)
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    return path;
                }
            }

            // Check if node is in PATH
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("where", "node");
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                using (Process p = Process.Start(psi))
                {
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit(500);
                    if (p.ExitCode == 0 && !string.IsNullOrEmpty(output))
                    {
                        string firstLine = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                        if (File.Exists(firstLine)) return firstLine;
                        return "node";
                    }
                }
            }
            catch { }

            return null;
        }

        private static void LaunchAppWindow()
        {
            string progFiles86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            string edge86 = Path.Combine(progFiles86, @"Microsoft\Edge\Application\msedge.exe");
            string edge = Path.Combine(progFiles, @"Microsoft\Edge\Application\msedge.exe");
            string chrome = Path.Combine(progFiles, @"Google\Chrome\Application\chrome.exe");
            string chrome86 = Path.Combine(progFiles86, @"Google\Chrome\Application\chrome.exe");

            string browserExe = null;
            if (File.Exists(edge86)) browserExe = edge86;
            else if (File.Exists(edge)) browserExe = edge;
            else if (File.Exists(chrome)) browserExe = chrome;
            else if (File.Exists(chrome86)) browserExe = chrome86;

            if (!string.IsNullOrEmpty(browserExe))
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = browserExe;
                psi.Arguments = string.Format("--app={0} --start-maximized", AppUrl);
                psi.UseShellExecute = false;
                Process.Start(psi);
            }
            else
            {
                Process.Start(AppUrl);
            }
        }
    }
}
