using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
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
        private static readonly string _ownProcessName = Process.GetCurrentProcess().ProcessName;

        // Drag detection state
        private const int DragThreshold = 10; // pixels before we treat as a drag
        private static bool   _leftButtonDown = false;
        private static bool   _isDragging     = false;
        private static POINT  _dragStartPos;
        private static RECT   _dragStartWinRect;
        private static string? _dragStartWindowTitle;
        private static string? _dragStartAppName;

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

            if (nCode >= 0)
            {
                var msg = (MouseMessages)wParam;

                // ── Left button DOWN: remember start position, no I/O on the hook thread ──
                if (msg == MouseMessages.WM_LBUTTONDOWN)
                {
                    POINT cursorPos;
                    if (GetCursorPos(out cursorPos))
                    {
                        IntPtr hwnd = WindowFromPoint(cursorPos);
                        if (hwnd != IntPtr.Zero)
                        {
                            string? appName = GetApplicationName(hwnd);
                            if (appName != _ownProcessName)
                            {
                                _leftButtonDown       = true;
                                _isDragging           = false;
                                _dragStartPos         = cursorPos;
                                _dragStartWindowTitle = GetTopLevelWindowTitle(hwnd);
                                _dragStartAppName     = appName;
                                _dragStartWinRect     = GetTopLevelWindowRect(hwnd);
                            }
                        }
                    }
                }

                // ── Mouse MOVE: check if the threshold has been crossed while button is held ──
                else if (msg == MouseMessages.WM_MOUSEMOVE && _leftButtonDown && !_isDragging)
                {
                    POINT cursorPos;
                    if (GetCursorPos(out cursorPos))
                    {
                        int dx = cursorPos.X - _dragStartPos.X;
                        int dy = cursorPos.Y - _dragStartPos.Y;
                        if (dx * dx + dy * dy > DragThreshold * DragThreshold)
                            _isDragging = true;
                    }
                }

                // ── Left button UP: commit click or drag ──
                else if (msg == MouseMessages.WM_LBUTTONUP && _leftButtonDown)
                {
                    _leftButtonDown = false;
                    bool wasDragging = _isDragging;
                    _isDragging = false;

                    POINT cursorPos;
                    if (GetCursorPos(out cursorPos))
                    {
                        IntPtr hwnd = WindowFromPoint(cursorPos);
                        if (hwnd != IntPtr.Zero)
                        {
                            if (wasDragging)
                            {
                                // ── Record a drag event ──
                                POINT dragStart     = _dragStartPos;
                                POINT dragEnd       = cursorPos;
                                RECT  winRect       = _dragStartWinRect;
                                string? windowTitle = _dragStartWindowTitle;
                                string? appName     = _dragStartAppName;

                                int winW = winRect.Right  - winRect.Left;
                                int winH = winRect.Bottom - winRect.Top;

                                GetWindowRect(hwnd, out RECT endUIrect);
                                int uiW = endUIrect.Right  - endUIrect.Left;
                                int uiH = endUIrect.Bottom - endUIrect.Top;

                                // Determine if drag spans multiple windows by comparing top-level window rects
                                RECT endWinRect = GetTopLevelWindowRect(hwnd);
                                bool multiWindowDrag = !RectsEqual(winRect, endWinRect);

                                // Use fallback mode if ActiveWindow mode but drag spans multiple windows
                                DragScreenshotMode effectiveMode = DragScreenshotMode;
                                if (effectiveMode == DragScreenshotMode.ActiveWindow && multiWindowDrag)
                                {
                                    effectiveMode = DragFallbackMode;
                                }

                                // Capture region: padded crop, active window, the screen containing the drag end, or all screens
                                int cropLeft, cropTop, cropW, cropH;
                                if (effectiveMode == DragScreenshotMode.AllScreens)
                                {
                                    cropLeft = SystemInformation.VirtualScreen.Left;
                                    cropTop  = SystemInformation.VirtualScreen.Top;
                                    cropW    = SystemInformation.VirtualScreen.Width;
                                    cropH    = SystemInformation.VirtualScreen.Height;
                                }
                                else if (effectiveMode == DragScreenshotMode.ActiveScreen)
                                {
                                    // Use the screen that contains the drag end point
                                    var screen = Screen.FromPoint(new System.Drawing.Point(dragEnd.X, dragEnd.Y));
                                    cropLeft = screen.Bounds.Left;
                                    cropTop  = screen.Bounds.Top;
                                    cropW    = screen.Bounds.Width;
                                    cropH    = screen.Bounds.Height;
                                }
                                else if (effectiveMode == DragScreenshotMode.ActiveWindow)
                                {
                                    // Use the window that was dragged in
                                    cropLeft = winRect.Left;
                                    cropTop  = winRect.Top;
                                    cropW    = winW;
                                    cropH    = winH;
                                }
                                else // Cropped
                                {
                                    const int DragPad = 120;
                                    int cropRight  = Math.Min(SystemInformation.VirtualScreen.Right,  Math.Max(dragStart.X, dragEnd.X) + DragPad);
                                    int cropBottom = Math.Min(SystemInformation.VirtualScreen.Bottom, Math.Max(dragStart.Y, dragEnd.Y) + DragPad);
                                    cropLeft = Math.Max(SystemInformation.VirtualScreen.Left, Math.Min(dragStart.X, dragEnd.X) - DragPad);
                                    cropTop  = Math.Max(SystemInformation.VirtualScreen.Top,  Math.Min(dragStart.Y, dragEnd.Y) - DragPad);
                                    cropW = cropRight  - cropLeft;
                                    cropH = cropBottom - cropTop;
                                }

                                var snapshot = (dragStart, dragEnd, hwnd, windowTitle, appName,
                                                endUIrect, winRect, winW, winH, uiW, uiH,
                                                cropLeft, cropTop, cropW, cropH);

                                ThreadPool.QueueUserWorkItem(_ =>
                                {
                                    var (ds, de, _, wt, app,
                                         uiRect, wr, wW, wH, uW, uH,
                                         cLeft, cTop, cW, cH) = snapshot;

                                    if (app == _ownProcessName) return;

                                    // Give the UI time to settle after the drop before capturing
                                    Thread.Sleep(200);

                                    Bitmap? dragBmp = null;
                                    try
                                    {
                                        dragBmp = new Bitmap(cW, cH, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                        using (Graphics gfx = Graphics.FromImage(dragBmp))
                                            gfx.CopyFromScreen(cLeft, cTop, 0, 0,
                                                new System.Drawing.Size(cW, cH), CopyPixelOperation.SourceCopy);
                                    }
                                    catch
                                    {
                                        dragBmp?.Dispose();
                                        dragBmp = null;
                                    }

                                    // Resolve the UI element at the drag *end* point
                                    AutomationElement? element = GetElementFromPoint(new System.Drawing.Point(de.X, de.Y));
                                    string? elementName = null;
                                    string? elementType = null;
                                    POINT arrowEnd = de;
                                    if (element != null)
                                    {
                                        try { elementName = element.Properties.Name.IsSupported ? element.Name : null; }
                                        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Could not read element Name: {ex.Message}"); }
                                        try { elementType = element.Properties.ControlType.IsSupported ? element.ControlType.ToString() : null; }
                                        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Could not read element ControlType: {ex.Message}"); }
                                    }

                                    string stepText = $"In {app}, Drag from ({ds.X},{ds.Y}) to ({de.X},{de.Y})";
                                    if (!string.IsNullOrEmpty(elementName))
                                        stepText = $"In {app}, Drag to {elementType} {elementName}";

                                    RecordEvent recordEvent;
                                    lock (_recordEventsLock)
                                    {
                                        recordEvent = new RecordEvent
                                        {
                                            WindowTitle       = wt,
                                            ApplicationName   = app,
                                            WindowCoordinates = new RECT { Left = wr.Left, Top = wr.Top, Bottom = wr.Bottom, Right = wr.Right },
                                            WindowSize        = new Size { Width = wW, Height = wH },
                                            UICoordinates     = new RECT { Left = uiRect.Left, Top = uiRect.Top, Bottom = uiRect.Bottom, Right = uiRect.Right },
                                            UISize            = new Size { Width = uW, Height = uH },
                                            UIElement         = null,
                                            ElementName       = elementName,
                                            ElementType       = elementType,
                                            MouseCoordinates  = new POINT { X = de.X, Y = de.Y },
                                            DragStartCoordinates = new POINT { X = ds.X, Y = ds.Y },
                                            DragEndCoordinates   = new POINT { X = de.X, Y = de.Y },
                                            EventType         = "Drag",
                                            _StepText         = stepText,
                                            Step              = _recordEvents.Count + 1
                                        };
                                        _recordEvents.Add(recordEvent);
                                    }

                                    // Annotate and store post-drop screenshot
                                    byte[]? pngBytes = null;
                                    if (dragBmp != null)
                                    {
                                        using (dragBmp)
                                        {
                                            using (Graphics gfx = Graphics.FromImage(dragBmp))
                                                DrawDragArrow(gfx, cW, cH, cLeft, cTop, ds, arrowEnd);
                                            using (var ms = new System.IO.MemoryStream())
                                            {
                                                dragBmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                                pngBytes = ms.ToArray();
                                            }
                                        }
                                    }

                                    if (pngBytes != null)
                                    {
                                        string? spoolPath = SpoolScreenshot(pngBytes, recordEvent.ID);
                                        if (spoolPath != null)
                                            recordEvent.ScreenshotSpoolPath = spoolPath;
                                        else
                                            recordEvent.Screenshotb64 = Convert.ToBase64String(pngBytes);
                                        pngBytes = null;
                                    }

                                    GC.Collect(2, GCCollectionMode.Optimized, blocking: false);

                                    _form1Instance?.BeginInvoke((Action)(() =>
                                    {
                                        _form1Instance.AddRecordEventToListBox(recordEvent);
                                        _form1Instance.activityTimer.Stop();
                                        _form1Instance.activityTimer.Start();
                                    }));
                                });
                            }
                            else
                            {
                                // ── Plain left click — same as the original WM_LBUTTONDOWN path ──
                                string? windowTitle     = GetTopLevelWindowTitle(hwnd);
                                string? applicationName = GetApplicationName(hwnd);
                                string  clickType       = "Left Click";

                                GetWindowRect(hwnd, out RECT UIrect);
                                RECT rect         = GetTopLevelWindowRect(hwnd);
                                int  UIWidth      = UIrect.Right  - UIrect.Left;
                                int  UIHeight     = UIrect.Bottom - UIrect.Top;

                                // Determine screenshot region based on ClickScreenshotMode
                                int captureLeft, captureTop, captureWidth, captureHeight;
                                if (ClickScreenshotMode == ClickScreenshotMode.AllScreens)
                                {
                                    captureLeft = SystemInformation.VirtualScreen.Left;
                                    captureTop = SystemInformation.VirtualScreen.Top;
                                    captureWidth = SystemInformation.VirtualScreen.Width;
                                    captureHeight = SystemInformation.VirtualScreen.Height;
                                }
                                else if (ClickScreenshotMode == ClickScreenshotMode.ActiveScreen)
                                {
                                    var screen = Screen.FromPoint(new System.Drawing.Point(cursorPos.X, cursorPos.Y));
                                    captureLeft = screen.Bounds.Left;
                                    captureTop = screen.Bounds.Top;
                                    captureWidth = screen.Bounds.Width;
                                    captureHeight = screen.Bounds.Height;
                                }
                                else // ActiveWindow
                                {
                                    captureLeft = rect.Left;
                                    captureTop = rect.Top;
                                    captureWidth = rect.Right - rect.Left;
                                    captureHeight = rect.Bottom - rect.Top;
                                }

                                Bitmap? preClickBitmap = null;
                                try
                                {
                                    preClickBitmap = new Bitmap(captureWidth, captureHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                    using (Graphics gfx = Graphics.FromImage(preClickBitmap))
                                        gfx.CopyFromScreen(captureLeft, captureTop, 0, 0,
                                            new System.Drawing.Size(captureWidth, captureHeight),
                                            CopyPixelOperation.SourceCopy);
                                }
                                catch
                                {
                                    preClickBitmap?.Dispose();
                                    preClickBitmap = null;
                                }

                                var snapshot = (cursorPos, hwnd, windowTitle, applicationName, clickType,
                                                UIrect, rect, captureWidth, captureHeight, UIWidth, UIHeight,
                                                captureLeft, captureTop, preClickBitmap);

                                ThreadPool.QueueUserWorkItem(_ =>
                                {
                                    var (cp, _, wt, appName, ct,
                                         uiRect, winRect, winW, winH, uiW, uiH,
                                         capLeft, capTop, preBitmap) = snapshot;

                                    AutomationElement? element = GetElementFromPoint(new System.Drawing.Point(cp.X, cp.Y));
                                    string? elementName = null;
                                    string? elementType = null;
                                    if (element != null)
                                    {
                                        try { elementName = element.Properties.Name.IsSupported ? element.Name : null; }
                                        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Could not read element Name: {ex.Message}"); }
                                        try { elementType = element.Properties.ControlType.IsSupported ? element.ControlType.ToString() : null; }
                                        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Could not read element ControlType: {ex.Message}"); }
                                    }

                                    if (appName == _ownProcessName) { preBitmap?.Dispose(); return; }

                                    RecordEvent recordEvent;
                                    lock (_recordEventsLock)
                                    {
                                        recordEvent = new RecordEvent
                                        {
                                            WindowTitle        = wt,
                                            ApplicationName    = appName,
                                            WindowCoordinates  = new RECT { Left = winRect.Left, Top = winRect.Top, Bottom = winRect.Bottom, Right = winRect.Right },
                                            WindowSize         = new Size { Width = winW, Height = winH },
                                            UICoordinates      = new RECT { Left = uiRect.Left, Top = uiRect.Top, Bottom = uiRect.Bottom, Right = uiRect.Right },
                                            UISize             = new Size { Width = uiW, Height = uiH },
                                            UIElement          = null,
                                            ElementName        = elementName,
                                            ElementType        = elementType,
                                            MouseCoordinates   = new POINT { X = cp.X, Y = cp.Y },
                                            EventType          = ct,
                                            _StepText          = $"In {appName}, {ct} on {elementType} {elementName}",
                                            Step               = _recordEvents.Count + 1
                                        };
                                        _recordEvents.Add(recordEvent);
                                    }

                                    byte[]? pngBytes = null;
                                    if (preBitmap != null)
                                    {
                                        using (preBitmap)
                                        {
                                            using (Graphics gfx = Graphics.FromImage(preBitmap))
                                                DrawArrowAtCursor(gfx, winW, winH, capLeft, capTop, cp);
                                            using (var ms = new System.IO.MemoryStream())
                                            {
                                                preBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                                pngBytes = ms.ToArray();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string? b64 = SaveScreenRegionScreenshot(capLeft, capTop, winW, winH, recordEvent.ID, cp);
                                        if (b64 != null) pngBytes = Convert.FromBase64String(b64);
                                    }

                                    if (pngBytes != null)
                                    {
                                        string? spoolPath = SpoolScreenshot(pngBytes, recordEvent.ID);
                                        if (spoolPath != null)
                                            recordEvent.ScreenshotSpoolPath = spoolPath;
                                        else
                                            recordEvent.Screenshotb64 = Convert.ToBase64String(pngBytes);
                                        pngBytes = null;
                                    }

                                    GC.Collect(2, GCCollectionMode.Optimized, blocking: false);

                                    _form1Instance?.BeginInvoke((Action)(() =>
                                    {
                                        _form1Instance.AddRecordEventToListBox(recordEvent);
                                        _form1Instance.activityTimer.Stop();
                                        _form1Instance.activityTimer.Start();
                                    }));
                                });
                            }
                        }
                    }
                }

                // ── Right click (unchanged) ──
                else if (msg == MouseMessages.WM_RBUTTONUP)
                {
                    POINT cursorPos;
                    if (GetCursorPos(out cursorPos))
                    {
                        IntPtr hwnd = WindowFromPoint(cursorPos);
                        if (hwnd != IntPtr.Zero)
                        {
                            string? windowTitle     = GetTopLevelWindowTitle(hwnd);
                            string? applicationName = GetApplicationName(hwnd);
                            string  clickType       = "Right Click";

                            GetWindowRect(hwnd, out RECT UIrect);
                            RECT rect         = GetTopLevelWindowRect(hwnd);
                            int  windowWidth  = rect.Right  - rect.Left;
                            int  windowHeight = rect.Bottom - rect.Top;
                            int  UIWidth      = UIrect.Right  - UIrect.Left;
                            int  UIHeight     = UIrect.Bottom - UIrect.Top;

                            Bitmap? preClickBitmap = null;
                            try
                            {
                                preClickBitmap = new Bitmap(windowWidth, windowHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                using (Graphics gfx = Graphics.FromImage(preClickBitmap))
                                    gfx.CopyFromScreen(rect.Left, rect.Top, 0, 0,
                                        new System.Drawing.Size(windowWidth, windowHeight),
                                        CopyPixelOperation.SourceCopy);
                            }
                            catch
                            {
                                preClickBitmap?.Dispose();
                                preClickBitmap = null;
                            }

                            var snapshot = (cursorPos, hwnd, windowTitle, applicationName, clickType,
                                            UIrect, rect, windowWidth, windowHeight, UIWidth, UIHeight,
                                            preClickBitmap);

                            ThreadPool.QueueUserWorkItem(_ =>
                            {
                                var (cp, _, wt, appName, ct,
                                     uiRect, winRect, winW, winH, uiW, uiH,
                                     preBitmap) = snapshot;

                                AutomationElement? element = GetElementFromPoint(new System.Drawing.Point(cp.X, cp.Y));
                                string? elementName = null;
                                string? elementType = null;
                                if (element != null)
                                {
                                    try { elementName = element.Properties.Name.IsSupported ? element.Name : null; }
                                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Could not read element Name: {ex.Message}"); }
                                    try { elementType = element.Properties.ControlType.IsSupported ? element.ControlType.ToString() : null; }
                                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Could not read element ControlType: {ex.Message}"); }
                                }

                                if (appName == _ownProcessName) { preBitmap?.Dispose(); return; }

                                RecordEvent recordEvent;
                                lock (_recordEventsLock)
                                {
                                    recordEvent = new RecordEvent
                                    {
                                        WindowTitle        = wt,
                                        ApplicationName    = appName,
                                        WindowCoordinates  = new RECT { Left = winRect.Left, Top = winRect.Top, Bottom = winRect.Bottom, Right = winRect.Right },
                                        WindowSize         = new Size { Width = winW, Height = winH },
                                        UICoordinates      = new RECT { Left = uiRect.Left, Top = uiRect.Top, Bottom = uiRect.Bottom, Right = uiRect.Right },
                                        UISize             = new Size { Width = uiW, Height = uiH },
                                        UIElement          = null,
                                        ElementName        = elementName,
                                        ElementType        = elementType,
                                        MouseCoordinates   = new POINT { X = cp.X, Y = cp.Y },
                                        EventType          = ct,
                                        _StepText          = $"In {appName}, {ct} on {elementType} {elementName}",
                                        Step               = _recordEvents.Count + 1
                                    };
                                    _recordEvents.Add(recordEvent);
                                }

                                byte[]? pngBytes = null;
                                if (preBitmap != null)
                                {
                                    using (preBitmap)
                                    {
                                        using (Graphics gfx = Graphics.FromImage(preBitmap))
                                            DrawArrowAtCursor(gfx, winW, winH, winRect.Left, winRect.Top, cp);
                                        using (var ms = new System.IO.MemoryStream())
                                        {
                                            preBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                            pngBytes = ms.ToArray();
                                        }
                                    }
                                }
                                else
                                {
                                    string? b64 = SaveScreenRegionScreenshot(winRect.Left, winRect.Top, winW, winH, recordEvent.ID, cp);
                                    if (b64 != null) pngBytes = Convert.FromBase64String(b64);
                                }

                                if (pngBytes != null)
                                {
                                    string? spoolPath = SpoolScreenshot(pngBytes, recordEvent.ID);
                                    if (spoolPath != null)
                                        recordEvent.ScreenshotSpoolPath = spoolPath;
                                    else
                                        recordEvent.Screenshotb64 = Convert.ToBase64String(pngBytes);
                                    pngBytes = null;
                                }

                                GC.Collect(2, GCCollectionMode.Optimized, blocking: false);

                                _form1Instance?.BeginInvoke((Action)(() =>
                                {
                                    _form1Instance.AddRecordEventToListBox(recordEvent);
                                    _form1Instance.activityTimer.Stop();
                                    _form1Instance.activityTimer.Start();
                                }));
                            });
                        }
                    }
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        /// <summary>
        /// Helper method to compare two RECT structures for equality
        /// </summary>
        private static bool RectsEqual(RECT r1, RECT r2)
        {
            return r1.Left == r2.Left && r1.Top == r2.Top && 
                   r1.Right == r2.Right && r1.Bottom == r2.Bottom;
        }

        /// <summary>
        /// P/Invoke declaration for the SetWindowsHookEx function
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
    }
}