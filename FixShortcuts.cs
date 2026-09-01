using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

[ComImport, Guid("00021401-0000-0000-C000-000000000046")]
internal class ShellLink {}

[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214F9-0000-0000-C000-000000000046")]
internal interface IShellLinkW {
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

class FixShortcuts {
    static void Main() {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string installDir = Path.Combine(appData, "CarManagementSystem");
        string targetExe = Path.Combine(installDir, "CarManagement.exe");
        string iconPath = Path.Combine(installDir, "app.ico");

        string userProf = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string[] desktopDirs = new string[] {
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Path.Combine(userProf, "OneDrive", "Desktop")
        };

        string[] names = new string[] { "\u062A\u06C6\u0645\u0627\u0631\u06CC \u062A\u0627\u0642\u06CC\u06AF\u06D5\u06A9\u0627\u0646.lnk", "Car Management System.lnk" };

        foreach (string d in desktopDirs) {
            if (!Directory.Exists(d)) continue;
            foreach (string name in names) {
                string scPath = Path.Combine(d, name);
                try {
                    if (File.Exists(scPath)) File.Delete(scPath);
                    IShellLinkW link = (IShellLinkW)new ShellLink();
                    link.SetPath(targetExe);
                    link.SetArguments("");
                    link.SetWorkingDirectory(installDir);
                    if (File.Exists(iconPath)) link.SetIconLocation(iconPath, 0);
                    link.SetDescription("\u062A\u06C6\u0645\u0627\u0631\u06CC \u062A\u0627\u0642\u06CC\u06AF\u06D5\u06A9\u0627\u0646 — دیزاین و پرۆگرامسازی: NAZHAD Q. MAHAMMED");
                    ((IPersistFile)link).Save(scPath, false);
                    Console.WriteLine("Updated shortcut: " + scPath);
                } catch(Exception ex) {
                    Console.WriteLine("Error on " + scPath + ": " + ex.Message);
                }
            }
        }
    }
}
