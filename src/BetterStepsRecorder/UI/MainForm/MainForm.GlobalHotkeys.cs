using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BetterStepsRecorder.UI;
using BetterStepsRecorder.UI.Settings;
using static BetterStepsRecorder.WindowHelper;

namespace BetterStepsRecorder
{
    public partial class MainForm
    {
        // Windows API for global hotkeys
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Hotkey IDs
        private const int HOTKEY_START_RECORDING = 1;
        private const int HOTKEY_PAUSE_RECORDING = 2;
        private const int HOTKEY_WINDOW_SNAP = 3;
        private const int HOTKEY_SCREEN_SNAP = 4;
        private const int HOTKEY_ALL_SCREENS_SNAP = 5;

        // Windows message for hotkey
        private const int WM_HOTKEY = 0x0312;

        // Modifier key constants
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_NOREPEAT = 0x4000;

        private bool _hotkeysRegistered = false;

        /// <summary>
        /// Registers all global hotkeys based on user settings
        /// </summary>
        public void RegisterGlobalHotkeys()
        {
            if (!BSRSettings.Current.KeyBinds.EnableGlobalHotkeys)
                return;

            UnregisterGlobalHotkeys(); // Unregister any existing hotkeys first

            var keyBinds = BSRSettings.Current.KeyBinds;

            // Register Start/Pause Recording hotkey
            RegisterHotkeyFromString(HOTKEY_START_RECORDING, keyBinds.StartRecording);

            // Only register pause separately if it's different from start
            if (keyBinds.PauseRecording != keyBinds.StartRecording)
            {
                RegisterHotkeyFromString(HOTKEY_PAUSE_RECORDING, keyBinds.PauseRecording);
            }

            // Register Window Snap hotkey
            RegisterHotkeyFromString(HOTKEY_WINDOW_SNAP, keyBinds.WindowSnap);

            // Register Screen Snap hotkey
            RegisterHotkeyFromString(HOTKEY_SCREEN_SNAP, keyBinds.ScreenSnap);

            // Register All Screens Snap hotkey
            RegisterHotkeyFromString(HOTKEY_ALL_SCREENS_SNAP, keyBinds.AllScreensSnap);

            _hotkeysRegistered = true;
        }

        /// <summary>
        /// Unregisters all global hotkeys
        /// </summary>
        public void UnregisterGlobalHotkeys()
        {
            if (!_hotkeysRegistered)
                return;

            UnregisterHotKey(Handle, HOTKEY_START_RECORDING);
            UnregisterHotKey(Handle, HOTKEY_PAUSE_RECORDING);
            UnregisterHotKey(Handle, HOTKEY_WINDOW_SNAP);
            UnregisterHotKey(Handle, HOTKEY_SCREEN_SNAP);
            UnregisterHotKey(Handle, HOTKEY_ALL_SCREENS_SNAP);

            _hotkeysRegistered = false;
        }

        /// <summary>
        /// Registers a hotkey from a string like "Ctrl+Alt+R"
        /// </summary>
        private bool RegisterHotkeyFromString(int hotkeyId, string keyString)
        {
            if (string.IsNullOrWhiteSpace(keyString) || keyString.Equals("None", StringComparison.OrdinalIgnoreCase))
                return false;

            Keys keys = KeyBindHelper.StringToKeys(keyString);
            if (keys == Keys.None)
                return false;

            // Extract modifiers and key code
            uint modifiers = MOD_NOREPEAT; // Prevent repeat when held down
            Keys keyCode = keys & Keys.KeyCode;

            if ((keys & Keys.Control) == Keys.Control)
                modifiers |= MOD_CONTROL;
            if ((keys & Keys.Shift) == Keys.Shift)
                modifiers |= MOD_SHIFT;
            if ((keys & Keys.Alt) == Keys.Alt)
                modifiers |= MOD_ALT;

            bool success = RegisterHotKey(Handle, hotkeyId, modifiers, (uint)keyCode);

            if (!success)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to register hotkey {hotkeyId}: {keyString}");
            }

            return success;
        }

        /// <summary>
        /// Process Windows messages to handle global hotkeys
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int hotkeyId = m.WParam.ToInt32();
                HandleHotkey(hotkeyId);
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Handles a triggered hotkey
        /// </summary>
        private void HandleHotkey(int hotkeyId)
        {
            switch (hotkeyId)
            {
                case HOTKEY_START_RECORDING:
                case HOTKEY_PAUSE_RECORDING:
                    // Toggle recording - only if recording is enabled
                    if (ToolStripMenuItem_Recording.Enabled)
                    {
                        ToolStripMenuItem_Recording_Click(this, EventArgs.Empty);
                    }
                    break;

                case HOTKEY_WINDOW_SNAP:
                    CaptureActiveWindow();
                    break;

                case HOTKEY_SCREEN_SNAP:
                    CaptureActiveScreen();
                    break;

                case HOTKEY_ALL_SCREENS_SNAP:
                    CaptureAllScreens();
                    break;
            }
        }

        /// <summary>
        /// Captures a screenshot of the active window and adds it as a new step
        /// </summary>
        private void CaptureActiveWindow()
        {
            try
            {
                // Get the foreground window
                IntPtr foregroundWindow = GetForegroundWindow();
                if (foregroundWindow == IntPtr.Zero)
                {
                    StatusManager.ShowMessage("No active window found", isError: true);
                    return;
                }

                // Get window rectangle
                RECT windowRect;
                if (!GetWindowRect(foregroundWindow, out windowRect))
                {
                    StatusManager.ShowMessage("Failed to get window bounds", isError: true);
                    return;
                }

                int width = windowRect.Right - windowRect.Left;
                int height = windowRect.Bottom - windowRect.Top;

                if (width <= 0 || height <= 0)
                {
                    StatusManager.ShowMessage("Invalid window dimensions", isError: true);
                    return;
                }

                // Get window title
                string? windowTitle = GetWindowText(foregroundWindow);

                // Capture the screenshot
                string? screenshotBase64 = CaptureRegionToBase64(windowRect.Left, windowRect.Top, width, height);
                if (string.IsNullOrEmpty(screenshotBase64))
                {
                    StatusManager.ShowMessage("Failed to capture screenshot", isError: true);
                    return;
                }

                // Create a new RecordEvent
                var recordEvent = new RecordEvent
                {
                    Step = Listbox_Events.Items.Count + 1,
                    EventType = "Manual Snapshot (Window)",
                    WindowTitle = windowTitle ?? "Unknown Window",
                    ApplicationName = GetApplicationName(foregroundWindow),
                    WindowCoordinates = windowRect,
                    WindowSize = new WindowHelper.Size { Width = width, Height = height },
                    _StepText = $"Window snapshot: {windowTitle ?? "Unknown Window"}",
                    BaseScreenshotb64 = screenshotBase64,
                    Screenshotb64 = screenshotBase64
                };

                // Add to the list
                Program._recordEvents.Add(recordEvent);
                AddRecordEventToListBox(recordEvent);

                // Select the new item
                Listbox_Events.SelectedIndex = Listbox_Events.Items.Count - 1;

                StatusManager.ShowSuccess("Window snapshot captured");

                // Trigger auto-save
                activityTimer.Stop();
                activityTimer.Start();
            }
            catch (Exception ex)
            {
                StatusManager.ShowMessage($"Failed to capture window: {ex.Message}", isError: true);
                System.Diagnostics.Debug.WriteLine($"Window capture error: {ex}");
            }
        }

        /// <summary>
        /// Captures a screenshot of the screen containing the cursor and adds it as a new step
        /// </summary>
        private void CaptureActiveScreen()
        {
            try
            {
                // Get cursor position to determine which screen to capture
                POINT cursorPos;
                GetCursorPos(out cursorPos);

                // Find the screen containing the cursor
                Screen activeScreen = Screen.FromPoint(new Point(cursorPos.X, cursorPos.Y));
                Rectangle bounds = activeScreen.Bounds;

                // Capture the screenshot
                string? screenshotBase64 = CaptureRegionToBase64(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                if (string.IsNullOrEmpty(screenshotBase64))
                {
                    StatusManager.ShowMessage("Failed to capture screenshot", isError: true);
                    return;
                }

                // Create a new RecordEvent
                var recordEvent = new RecordEvent
                {
                    Step = Listbox_Events.Items.Count + 1,
                    EventType = "Manual Snapshot (Screen)",
                    WindowTitle = $"Screen: {activeScreen.DeviceName}",
                    WindowCoordinates = new RECT 
                    { 
                        Left = bounds.X, 
                        Top = bounds.Y, 
                        Right = bounds.Right, 
                        Bottom = bounds.Bottom 
                    },
                    WindowSize = new WindowHelper.Size { Width = bounds.Width, Height = bounds.Height },
                    _StepText = $"Screen snapshot: {activeScreen.DeviceName}",
                    BaseScreenshotb64 = screenshotBase64,
                    Screenshotb64 = screenshotBase64
                };

                // Add to the list
                Program._recordEvents.Add(recordEvent);
                AddRecordEventToListBox(recordEvent);

                // Select the new item
                Listbox_Events.SelectedIndex = Listbox_Events.Items.Count - 1;

                StatusManager.ShowSuccess("Screen snapshot captured");

                // Trigger auto-save
                activityTimer.Stop();
                activityTimer.Start();
            }
            catch (Exception ex)
            {
                StatusManager.ShowMessage($"Failed to capture screen: {ex.Message}", isError: true);
                System.Diagnostics.Debug.WriteLine($"Screen capture error: {ex}");
            }
        }

        /// <summary>
        /// Captures a screenshot of all screens (virtual screen) and adds it as a new step
        /// </summary>
        private void CaptureAllScreens()
        {
            try
            {
                // Get the virtual screen bounds (encompasses all monitors)
                Rectangle virtualScreen = SystemInformation.VirtualScreen;

                // Capture the screenshot
                string? screenshotBase64 = CaptureRegionToBase64(
                    virtualScreen.X, virtualScreen.Y, 
                    virtualScreen.Width, virtualScreen.Height);

                if (string.IsNullOrEmpty(screenshotBase64))
                {
                    StatusManager.ShowMessage("Failed to capture screenshot", isError: true);
                    return;
                }

                // Create a new RecordEvent
                var recordEvent = new RecordEvent
                {
                    Step = Listbox_Events.Items.Count + 1,
                    EventType = "Manual Snapshot (All Screens)",
                    WindowTitle = $"All Screens ({Screen.AllScreens.Length} monitors)",
                    WindowCoordinates = new RECT 
                    { 
                        Left = virtualScreen.X, 
                        Top = virtualScreen.Y, 
                        Right = virtualScreen.Right, 
                        Bottom = virtualScreen.Bottom 
                    },
                    WindowSize = new WindowHelper.Size { Width = virtualScreen.Width, Height = virtualScreen.Height },
                    _StepText = $"All screens snapshot ({Screen.AllScreens.Length} monitors)",
                    BaseScreenshotb64 = screenshotBase64,
                    Screenshotb64 = screenshotBase64
                };

                // Add to the list
                Program._recordEvents.Add(recordEvent);
                AddRecordEventToListBox(recordEvent);

                // Select the new item
                Listbox_Events.SelectedIndex = Listbox_Events.Items.Count - 1;

                StatusManager.ShowSuccess("All screens snapshot captured");

                // Trigger auto-save
                activityTimer.Stop();
                activityTimer.Start();
            }
            catch (Exception ex)
            {
                StatusManager.ShowMessage($"Failed to capture all screens: {ex.Message}", isError: true);
                System.Diagnostics.Debug.WriteLine($"All screens capture error: {ex}");
            }
        }

        /// <summary>
        /// Captures a screen region and returns it as a Base64 string
        /// </summary>
        private static string? CaptureRegionToBase64(int x, int y, int width, int height)
        {
            try
            {
                using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                using var graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);

                using var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to capture region: {ex.Message}");
                return null;
            }
        }

        // Additional P/Invoke for getting foreground window
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Refreshes hotkey registrations (call after settings change)
        /// </summary>
        public void RefreshGlobalHotkeys()
        {
            UnregisterGlobalHotkeys();
            RegisterGlobalHotkeys();
        }
    }
}
