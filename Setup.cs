using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace CarSetup
{
    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    internal class ShellLink {}

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    internal interface IShellLinkW
    {
        void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotkey(out short pwHotkey);
        void SetHotkey(short wHotkey);
        void GetShowCmd(out int piShowCmd);
        void SetShowCmd(int iShowCmd);
        void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
        void Resolve(IntPtr hwnd, int fFlags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    class Program
    {
        // "تۆماری تاقیگەکان"
        private static readonly string AppTitleKurdish = "\u062A\u06C6\u0645\u0627\u0631\u06CC \u062A\u0627\u0642\u06CC\u06AF\u06D5\u06A9\u0627\u0646";
        private static readonly string ShortcutFileName = AppTitleKurdish + ".lnk";

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                string srcDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string destDir = Path.Combine(localAppData, "CarManagementSystem");

                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                // Copy all files using robocopy
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "robocopy.exe";
                psi.Arguments = string.Format("\"{0}\" \"{1}\" /E /IS /IT /NFL /NDL /NJH /NJS /nc /ns /np", srcDir, destDir);
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;

                using (Process p = Process.Start(psi))
                {
                    p.WaitForExit();
                }

                // Target paths
                string targetVbs = Path.Combine(destDir, "Start-Desktop-App-Silent.vbs");
                if (!File.Exists(targetVbs))
                {
                    targetVbs = Path.Combine(destDir, "Start-App-Silent.vbs");
                }
                string iconPath = Path.Combine(destDir, "app.ico");

                // Determine Desktop directory (support OneDrive Desktop if present)
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string onedriveDesktop = Path.Combine(userProfile, "OneDrive", "Desktop");
                string desktopDir = Directory.Exists(onedriveDesktop) ? onedriveDesktop : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string programsDir = Environment.GetFolderPath(Environment.SpecialFolder.Programs);

                string shortcutDesc = AppTitleKurdish + " — دیزاین و پرۆگرامسازی: NAZHAD Q. MAHAMMED";

                // Create Desktop Shortcut with IShellLinkW (Full Unicode Support)
                CreateShortcut(Path.Combine(desktopDir, ShortcutFileName), "wscript.exe", "\"" + targetVbs + "\"", destDir, iconPath, shortcutDesc);

                // Create Start Menu Shortcut
                CreateShortcut(Path.Combine(programsDir, ShortcutFileName), "wscript.exe", "\"" + targetVbs + "\"", destDir, iconPath, shortcutDesc);

                // Launch App
                Process.Start("wscript.exe", "\"" + targetVbs + "\"");

                MessageBox.Show(
                    "\u2705 \u0628\u06D5 \u0633\u06D5\u0631\u06A9\u06D5\u0648\u062A\u0648\u0648\u06CC \u0644\u06D5\u0633\u06D5\u0631 \u0626\u06D5\u0645 \u0626\u0627\u0645\u064E\u06CC\u0631\u06D5 \u062F\u0627\u0645\u06D5\u0632\u0631\u0627!\n\n" +
                    "\u0626\u0627\u06CC\u06A9\u06C6\u0646\u06CC \u067E\u0631\u06C6\u06AF\u0631\u0627\u0645\u06D5\u06A9\u06D5 \u0628\u06D5\u0646\u0627\u0648\u06CC (" + AppTitleKurdish + ") \u062E\u0631\u0627\u06CC\u06D5 \u0633\u06D5\u0631 \u0695\u0648\u0648\u06CC \u0634\u0627\u0634\u06D5 (Desktop).\n\n" +
                    "دیزاین و پرۆگرامسازی: NAZHAD Q. MAHAMMED",
                    AppTitleKurdish,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during setup: " + ex.Message, "Setup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void CreateShortcut(string shortcutPath, string targetPath, string arguments, string workingDir, string iconPath, string description)
        {
            try
            {
                if (File.Exists(shortcutPath))
                {
                    File.Delete(shortcutPath);
                }

                IShellLinkW link = (IShellLinkW)new ShellLink();
                link.SetPath(targetPath);
                link.SetArguments(arguments);
                link.SetWorkingDirectory(workingDir);
                if (File.Exists(iconPath))
                {
                    link.SetIconLocation(iconPath, 0);
                }
                link.SetDescription(description);

                IPersistFile file = (IPersistFile)link;
                file.Save(shortcutPath, false);
            }
            catch { }
        }
    }
}
