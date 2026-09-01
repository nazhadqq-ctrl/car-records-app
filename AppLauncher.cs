using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace CarAppLauncher
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
                string batPath = Path.Combine(baseDir, "Start-Desktop-App.bat");
                if (!File.Exists(batPath))
                {
                    batPath = Path.Combine(baseDir, "Start-App.bat");
                }

                if (!File.Exists(batPath))
                {
                    MessageBox.Show(
                        "❌ فایلی دەستپێکردنی سیستەم نەدۆزرایەوە!",
                        "تۆماری تاقیگەکان",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "cmd.exe";
                psi.Arguments = string.Format("/c \"\"{0}\"\"", batPath);
                psi.WorkingDirectory = baseDir;
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "هەڵە لە کاتی دەستپێکردنی بەرنامەکە:\n" + ex.Message,
                    "تۆماری تاقیگەکان",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
