using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Automation;

namespace Better_Steps_Recorder
{
    internal static class Program
    {
        private static IntPtr _hookID = IntPtr.Zero;
        private static LowLevelMouseProc _proc = HookCallback;
        public static List<RecordEvent> _recordEvents = new List<RecordEvent>();
        private static Form1 _form1Instance;
        private static int EventCounter = 1;
        public static bool IsRecording = false;

        static void Main()
        {
            ApplicationConfiguration.Initialize();
            _form1Instance = new Form1();
            _hookID = SetHook(_proc);
            Application.Run(_form1Instance);
            WindowHelper.UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WindowHelper.WH_MOUSE_LL, proc, WindowHelper.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (!IsRecording)
                return WindowHelper.CallNextHookEx(_hookID, nCode, wParam, lParam);

            if (nCode >= 0 && (WindowHelper.MouseMessages.WM_LBUTTONDOWN == (WindowHelper.MouseMessages)wParam || WindowHelper.MouseMessages.WM_RBUTTONDOWN == (WindowHelper.MouseMessages)wParam))
            {
                WindowHelper.POINT cursorPos;
                if (WindowHelper.GetCursorPos(out cursorPos))
                {
                    IntPtr hwnd = WindowHelper.WindowFromPoint(cursorPos);
                    if (hwnd != IntPtr.Zero)
                    {
                        // Get window title
                        string windowTitle = WindowHelper.GetWindowText(hwnd);

                        // Get window coordinates and size
                        WindowHelper.GetWindowRect(hwnd, out WindowHelper.RECT rect);
                        int windowWidth = rect.Right - rect.Left;
                        int windowHeight = rect.Bottom - rect.Top;

                        // Get UI element under cursor
                        string elementInfo = GetElementFromCursor(new System.Windows.Point(cursorPos.X, cursorPos.Y));

                        // Determine click type
                        string clickType = WindowHelper.MouseMessages.WM_LBUTTONDOWN == (WindowHelper.MouseMessages)wParam ? "Left Click" : "Right Click";

                        // Create a record event object and add it to the list
                        RecordEvent recordEvent = new RecordEvent
                        {
                            ID = EventCounter++,
                            WindowTitle = windowTitle,
                            WindowCoordinates = (rect.Left, rect.Top, rect.Right, rect.Bottom),
                            WindowSize = (windowWidth, windowHeight),
                            UIElement = elementInfo,
                            MouseCoordinates = (cursorPos.X, cursorPos.Y),
                            EventType = clickType
                        };
                        _recordEvents.Add(recordEvent);

                        // Take screenshot of the window
                        string screenshotPath = SaveWindowScreenshot(hwnd, recordEvent.ID);
                        if (screenshotPath != null)
                        {
                            recordEvent.ScreenshotPath = screenshotPath;
                        }

                        // Update ListBox in Form1
                        _form1Instance.Invoke((Action)(() => _form1Instance.AddRecordEventToListBox(recordEvent)));

                        // Output the details
                        Debug.WriteLine($"ID: {recordEvent.ID}");
                        Debug.WriteLine($"Window Title: {windowTitle}");
                        Debug.WriteLine($"Window Coordinates: Left={rect.Left}, Top={rect.Top}, Right={rect.Right}, Bottom={rect.Bottom}");
                        Debug.WriteLine($"Window Size: Width={windowWidth}, Height={windowHeight}");
                        Debug.WriteLine($"UI Element: {elementInfo}");
                        Debug.WriteLine($"Mouse Coordinates: X={cursorPos.X}, Y={cursorPos.Y}");
                        Debug.WriteLine($"Click Type: {clickType}");
                        Debug.WriteLine($"Screenshot Path: {screenshotPath}");
                    }
                }
            }
            return WindowHelper.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static string GetElementFromCursor(System.Windows.Point point)
        {
            AutomationElement element = AutomationElement.FromPoint(new System.Windows.Point(point.X, point.Y));
            if (element != null)
            {
                string elementType = element.Current.LocalizedControlType;
                string elementName = element.Current.Name;
                return $"{elementType}: {elementName}";
            }
            return "No UI element found";
        }

        private static string SaveWindowScreenshot(IntPtr hwnd, int eventId)
        {
            try
            {
                WindowHelper.GetWindowRect(hwnd, out WindowHelper.RECT rect);
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                Graphics gfxBmp = Graphics.FromImage(bmp);
                IntPtr hdcBitmap = gfxBmp.GetHdc();
                IntPtr hdcWindow = WindowHelper.GetWindowDC(hwnd);

                WindowHelper.BitBlt(hdcBitmap, 0, 0, width, height, hdcWindow, 0, 0, WindowHelper.SRCCOPY);
                gfxBmp.ReleaseHdc(hdcBitmap);
                gfxBmp.Dispose();
                WindowHelper.ReleaseDC(hwnd, hdcWindow);

                string screenshotPath = $"screenshot_{eventId}.png";
                bmp.Save(screenshotPath, ImageFormat.Png);
                bmp.Dispose();

                return screenshotPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to capture screenshot: {ex.Message}");
                return null;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
    }
}