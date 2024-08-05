using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Better_Steps_Recorder
{

    public class ShellExecuteHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        public const uint SEE_MASK_INVOKEIDLIST = 0x0000000C;

        public static void ShowOpenWithDialog(string filePath)
        {
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
            info.cbSize = Marshal.SizeOf(info);
            info.lpVerb = "openas"; // To show "Open with" dialog
            info.lpFile = filePath;
            info.nShow = 1; // SW_SHOWNORMAL
            info.fMask = SEE_MASK_INVOKEIDLIST;

            ShellExecuteEx(ref info);
        }

        public static Process OpenWithDefaultProgram(string filePath)
        {
            string programPath = @"C:\Program Files\ShareX\ShareX.exe";
            string arguments = $"-ImageEditor {filePath}";

            Process process = Process.Start(new ProcessStartInfo
            {
                FileName = programPath,
                Arguments = arguments,
                UseShellExecute = false
            });
            return process;
        }
    }
}
