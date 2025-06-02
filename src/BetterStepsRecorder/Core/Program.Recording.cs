using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FlaUI.Core.AutomationElements;
using static BetterStepsRecorder.WindowHelper;
using Size = BetterStepsRecorder.WindowHelper.Size;

namespace BetterStepsRecorder
{
    internal static partial class Program
    {
        private static IntPtr _hookID = IntPtr.Zero;
        private static LowLevelMouseProc _proc = HookCallback;
        public static bool IsRecording = false;

        /// <summary>
        /// Sets up the mouse hook to start recording user interactions
        /// </summary>
        public static void HookMouseOperations()
        {
            _hookID = SetHook(_proc);
            IsRecording = true;
        }
        
        /// <summary>
        /// Removes the mouse hook to stop recording user interactions
        /// </summary>
        public static void UnHookMouseOperations()
        {
            UnhookWindowsHookEx(_hookID);
            IsRecording = false;
        }

        /// <summary>
        /// Sets up the Windows hook for capturing mouse events
        /// </summary>
        /// <param name="proc">The callback procedure for the hook</param>
        /// <returns>A handle to the hook</returns>
        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule? curModule = curProcess.MainModule)
            {
                if (curModule != null)
                {
                    return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
                else
                {
                    // Handle the case where MainModule is null
                    throw new InvalidOperationException("The process does not have a main module.");
                }
            }
        }

        /// <summary>
        /// Delegate for the low-level mouse hook callback
        /// </summary>
        /// <param name="nCode">The hook code</param>
        /// <param name="wParam">The message identifier</param>
        /// <param name="lParam">A pointer to the message data</param>
        /// <returns>The result of the hook processing</returns>
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Callback function for processing mouse events
        /// </summary>
        /// <param name="nCode">The hook code</param>
        /// <param name="wParam">The message identifier</param>
        /// <param name="lParam">A pointer to the message data</param>
        /// <returns>The result of the hook processing</returns>
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (!IsRecording)
                return CallNextHookEx(_hookID, nCode, wParam, lParam);

            if (nCode >= 0 && (MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam || MouseMessages.WM_RBUTTONDOWN == (MouseMessages)wParam))
            {
                POINT cursorPos;
                if (GetCursorPos(out cursorPos))
                {
                    IntPtr hwnd = WindowFromPoint(cursorPos);
                    if (hwnd != IntPtr.Zero)
                    {
                        // Get window title
                        string? windowTitle = GetTopLevelWindowTitle(hwnd);
                        // Get ApplicationName
                        string? applicationName = GetApplicationName(hwnd);

                        // Get UI Element coordinates and size
                        GetWindowRect(hwnd, out RECT UIrect);
                        int UIWidth = UIrect.Right - UIrect.Left;
                        int UIHeight = UIrect.Bottom - UIrect.Top;

                        // Get window coordinates and size
                        RECT rect = GetTopLevelWindowRect(hwnd);
                        int windowWidth = rect.Right - rect.Left;
                        int windowHeight = rect.Bottom - rect.Top;

                        // Get UI element under cursor using FlaUI
                        AutomationElement? element = GetElementFromPoint(new System.Drawing.Point(cursorPos.X, cursorPos.Y));
                        string? elementName = null;
                        string? elementType = null;
                        
                        if (element != null)
                        {
                            elementName = element.Name;
                            elementType = element.ControlType.ToString();
                        }
                        
                        // Determine click type
                        string clickType = MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam ? "Left Click" : "Right Click";

                        //Skip record if its to pause recording
                        if (elementName != "Pause Recording" && applicationName != "Better Steps Recorder")
                        {
                            // Create a record event object and add it to the list
                            RecordEvent recordEvent = new RecordEvent
                            {
                                WindowTitle = windowTitle,
                                ApplicationName = applicationName,
                                WindowCoordinates = new RECT { Left = rect.Left, Top = rect.Top, Bottom = rect.Bottom, Right = rect.Right },
                                WindowSize = new Size { Width = windowWidth, Height = windowHeight },
                                UICoordinates = new RECT { Left = UIrect.Left, Top = UIrect.Top, Bottom = UIrect.Bottom, Right = UIrect.Right },
                                UISize = new Size { Width = UIWidth, Height = UIHeight },
                                UIElement = element,
                                ElementName = elementName,
                                ElementType = elementType,
                                MouseCoordinates = new POINT { X = cursorPos.X, Y = cursorPos.Y },
                                EventType = clickType,
                                _StepText = $"In {applicationName}, {clickType} on {elementType} {elementName}",
                                Step = _recordEvents.Count + 1
                            };
                            _recordEvents.Add(recordEvent);

                            // Take screenshot of the window
                            string? screenshotb64 = SaveScreenRegionScreenshot(rect.Left, rect.Top, windowWidth, windowHeight, recordEvent.ID);
                            if (screenshotb64 != null)
                            {
                                recordEvent.Screenshotb64 = screenshotb64;
                            }

                            // Update ListBox in Form1
                            _form1Instance?.Invoke((Action)(() => _form1Instance.AddRecordEventToListBox(recordEvent)));
                            _form1Instance?.Invoke((Action)(() => _form1Instance.activityTimer.Stop()));
                            _form1Instance?.Invoke((Action)(() => _form1Instance.activityTimer.Start()));
                        }
                    }
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        /// <summary>
        /// P/Invoke declaration for the SetWindowsHookEx function
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
    }
}