using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

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
    static void Main(string[] args)
    {
        try
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            string vbsPath = Path.Combine(baseDir, "Start-Desktop-App-Silent.vbs");
            string iconPath = Path.Combine(baseDir, "app.ico");

            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string onedriveDesktop = Path.Combine(userProfile, "OneDrive", "Desktop");
            string desktopDir = Directory.Exists(onedriveDesktop) ? onedriveDesktop : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string shortcutPath = Path.Combine(desktopDir, "تۆماری تاقیگەکان.lnk");

            IShellLinkW link = (IShellLinkW)new ShellLink();
            link.SetPath("wscript.exe");
            link.SetArguments("\"" + vbsPath + "\"");
            link.SetWorkingDirectory(baseDir);
            link.SetIconLocation(iconPath, 0);
            link.SetDescription("تۆماری تاقیگەکان");

            IPersistFile file = (IPersistFile)link;
            file.Save(shortcutPath, false);

            Console.WriteLine("Shortcut created successfully at: " + shortcutPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
