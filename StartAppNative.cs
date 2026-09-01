using System;
using System.IO;
using System.Diagnostics;

namespace StartAppNative
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
                string batFile = Path.Combine(baseDir, "Start-App.bat");
                if (!File.Exists(batFile))
                {
                    batFile = Path.Combine(baseDir, "Start-Desktop-App.bat");
                }

                if (File.Exists(batFile))
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "cmd.exe";
                    psi.Arguments = string.Format("/c \"\"{0}\"\"", batFile);
                    psi.WorkingDirectory = baseDir;
                    psi.WindowStyle = ProcessWindowStyle.Minimized;
                    psi.UseShellExecute = false;

                    Process.Start(psi);
                }
            }
            catch { }
        }
    }
}
