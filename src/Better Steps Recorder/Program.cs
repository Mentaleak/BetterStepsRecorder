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
using static Better_Steps_Recorder.WindowHelper;
using System.Text.Json;
using System.IO.Compression;

namespace Better_Steps_Recorder
{
    internal static class Program
    {
        public static ZipFileHandler zip;
        private static IntPtr _hookID = IntPtr.Zero;
        private static LowLevelMouseProc _proc = HookCallback;
        public static List<RecordEvent> _recordEvents = new List<RecordEvent>();
        private static Form1 _form1Instance;
        public static int EventCounter = 1;
        public static bool IsRecording = false;
        
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            _form1Instance = new Form1();
            _hookID = SetHook(_proc);
            Application.Run(_form1Instance);
            WindowHelper.UnhookWindowsHookEx(_hookID);
        }
        public static void LoadRecordEventsFromFile(string filePath)
        {

            if (File.Exists(filePath))
            {
                try
                {
                    using (ZipArchive archive = ZipFile.OpenRead(filePath))
                    {
                        _recordEvents = new List<RecordEvent>();
                        _form1Instance.Invoke((Action)(() => _form1Instance.ClearListBox()));
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (Path.GetDirectoryName(entry.FullName) == "events" && entry.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                            {
                                using (StreamReader reader = new StreamReader(entry.Open()))
                                {
                                    string jsonContent = reader.ReadToEnd();
                                    var recordEvent = JsonSerializer.Deserialize<RecordEvent>(jsonContent);
                                    // If using Newtonsoft.Json, replace the above line with:
                                    // var recordEvents = JsonConvert.DeserializeObject<List<RecordEvent>>(jsonContent);

                                    if (recordEvent != null)
                                    {
                                        _recordEvents.Add(recordEvent);
                                        _form1Instance.Invoke((Action)(() => _form1Instance.AddRecordEventToListBox(recordEvent)));
                                        EventCounter++;

                                    }
                                }
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    MessageBox.Show($"Invalid JSON format: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"File I/O error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("File does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                        string windowTitle = WindowHelper.GetTopLevelWindowTitle(hwnd);
                        // Get ApplicationName
                        string applicationName=WindowHelper.GetApplicationName(hwnd);

                        // Get UI Elelment coordinates and size
                        WindowHelper.GetWindowRect(hwnd, out WindowHelper.RECT UIrect);
                        int UIWidth = UIrect.Right - UIrect.Left;
                        int UIHeight = UIrect.Bottom - UIrect.Top;

                        // Get window coordinates and size
                       
                        WindowHelper.RECT rect = WindowHelper.GetTopLevelWindowRect(hwnd);
                        int windowWidth = rect.Right - rect.Left;
                        int windowHeight = rect.Bottom - rect.Top;

                        // Get UI element under cursor
                        AutomationElement element = GetElementFromCursor(new System.Windows.Point(cursorPos.X, cursorPos.Y));

                        // Determine click type
                        string clickType = WindowHelper.MouseMessages.WM_LBUTTONDOWN == (WindowHelper.MouseMessages)wParam ? "Left Click" : "Right Click";

                        // Create a record event object and add it to the list
                        RecordEvent recordEvent = new RecordEvent
                        {
                            ID = EventCounter++,
                            WindowTitle = windowTitle,
                            ApplicationName=applicationName,
                            WindowCoordinates = new WindowHelper.RECT { Left = rect.Left, Top = rect.Top, Bottom = rect.Bottom, Right = rect.Right },
                            WindowSize = new WindowHelper.Size { Width = windowWidth, Height = windowHeight },
                            UICoordinates = new WindowHelper.RECT { Left = UIrect.Left, Top = UIrect.Top, Bottom = UIrect.Bottom, Right = UIrect.Right },
                            UISize = new WindowHelper.Size { Width= UIWidth, Height= UIHeight },
                            UIElement = element,
                            ElementName=element.Current.Name,
                            ElementType=element.Current.LocalizedControlType,
                            MouseCoordinates = new WindowHelper.POINT { X = cursorPos.X, Y = cursorPos.Y },
                            EventType = clickType,
                            _StepText = $"In {applicationName}, {clickType} on  {element.Current.LocalizedControlType} {element.Current.Name}"
                        };
                        _recordEvents.Add(recordEvent);

                        // Take screenshot of the window
                        //string screenshotPath = SaveWindowScreenshot(hwnd, recordEvent.ID);
                        string screenshotb64 = SaveScreenRegionScreenshot(rect.Left, rect.Top, windowWidth, windowHeight, recordEvent.ID);
                        if (screenshotb64 != null)
                        {
                            recordEvent.Screenshotb64 = screenshotb64;
                        }

                        // Update ListBox in Form1
                        _form1Instance.Invoke((Action)(() => _form1Instance.AddRecordEventToListBox(recordEvent)));
                        zip.SaveToZip();
                    }
                }
            }
            return WindowHelper.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static AutomationElement? GetElementFromCursor(System.Windows.Point point)
        {
            AutomationElement element = AutomationElement.FromPoint(new System.Windows.Point(point.X, point.Y));
            if (element != null)
            {
                return element;
            }
            return null;
        }

        private static string SaveWindowScreenshot(IntPtr hwnd, int eventId)
        {
            try
            {
                WindowHelper.RECT rect = WindowHelper.GetTopLevelWindowRect(hwnd);
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

        public static string SaveScreenRegionScreenshot(int x, int y, int width, int height, int eventId)
        {
            try
            {
                // Create a bitmap of the specified size
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                // Create graphics object from the bitmap
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    // Copy the specified screen area to the bitmap
                    gfx.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);

                    // Draw an arrow pointing at the cursor
                    DrawArrowAtCursor(gfx, width, height, x, y);
                }

                // Convert the bitmap to a memory stream
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    byte[] imageBytes = ms.ToArray();

                    // Convert byte array to Base64 string
                    string base64String = Convert.ToBase64String(imageBytes);

                    // Dispose of the bitmap
                    bmp.Dispose();

                    // Return the Base64 string
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to capture screenshot: {ex.Message}");
                return null;
            }
        }




        private static void DrawArrowAtCursor(Graphics gfx, int width, int height, int offsetX, int offsetY)
        {
            // Define the arrow properties
            Pen arrowPen = new Pen(Color.Magenta, 5);
            arrowPen.EndCap = System.Drawing.Drawing2D.LineCap.Custom;
            arrowPen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(5, 5); // Bigger arrow head

            // Define the length of the arrow
            int arrowLength = 200;

            // Get the current cursor position
            WindowHelper.POINT cursorPos;
            WindowHelper.GetCursorPos(out cursorPos);

            // Convert the screen coordinates to bitmap coordinates
            int cursorX = cursorPos.X - offsetX;
            int cursorY = cursorPos.Y - offsetY;

            // Determine arrow direction: down if in top half, up if in bottom half
            int endX, endY;
            if (cursorY < height / 2)
            {
                // Cursor is in the top half, arrow points down
                endX = cursorX;
                endY = cursorY + arrowLength;
            }
            else
            {
                // Cursor is in the bottom half, arrow points up
                endX = cursorX;
                endY = cursorY - arrowLength;
            }

            // Draw the arrow
            gfx.DrawLine(arrowPen, endX, endY, cursorX, cursorY);
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
    }
}