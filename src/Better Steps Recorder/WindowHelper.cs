using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Automation;
using System.Diagnostics.Tracing;
using Microsoft.VisualBasic.ApplicationServices;

namespace Better_Steps_Recorder
{
    public class WindowHelper
    {
        public const int SRCCOPY = 0x00CC0020;
        private const uint GA_ROOT = 2;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X { get; set; }
            public int Y { get; set; }
            public override string ToString()
            {
                // Customize the string representation for display in the ListBox
                return $"({X}, {Y})";
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public override string ToString()
            {
                // Customize the string representation for display in the ListBox
                return $"({Width}, {Height})";
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }

            public override string ToString()
            {
                // Customize the string representation for display in the ListBox
                return $"({Left}, {Top}, {Bottom}, {Right})";
            }
        }

        public const int WH_MOUSE_LL = 14;
        public enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        public static IntPtr GetWindowUnderCursor()
        {
            POINT cursorPos;
            if (GetCursorPos(out cursorPos))
            {
                return WindowFromPoint(cursorPos);
            }
            return IntPtr.Zero;
        }

        public static RECT GetTopLevelWindowRect(IntPtr hWnd)
        {
            // Get the top-level window handle
            IntPtr topLevelHwnd = GetAncestor(hWnd, GA_ROOT);
            RECT rect;
            if (GetWindowRect(topLevelHwnd, out rect))
            {
                return rect;
            }
            throw new InvalidOperationException("Unable to retrieve window rectangle.");
        }


        public static string GetTopLevelWindowTitle(IntPtr hWnd)
        {
            // Get the top-level window handle
            IntPtr rootHwnd = GetAncestor(hWnd, GA_ROOT);
            return GetWindowText(rootHwnd);
        }
        public static string? GetWindowText(IntPtr hWnd)
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            if (GetWindowText(hWnd, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
        public static string? GetApplicationName(IntPtr hWnd)
        {
            uint processId;
            GetWindowThreadProcessId(hWnd, out processId);

            try
            {
                Process process = Process.GetProcessById((int)processId);
                return process.ProcessName;
            }
            catch (ArgumentException)
            {
                // This exception can occur if the process has exited since retrieving the process ID
                return null;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

    }
}