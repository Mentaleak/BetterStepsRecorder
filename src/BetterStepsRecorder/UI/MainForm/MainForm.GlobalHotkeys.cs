using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BetterStepsRecorder.UI;
using BetterStepsRecorder.UI.Settings;

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
                    // Capture active window (placeholder - implement when snap feature exists)
                    StatusManager.ShowMessage("Window Snap hotkey pressed (not yet implemented)");
                    break;

                case HOTKEY_SCREEN_SNAP:
                    // Capture active screen (placeholder - implement when snap feature exists)
                    StatusManager.ShowMessage("Screen Snap hotkey pressed (not yet implemented)");
                    break;

                case HOTKEY_ALL_SCREENS_SNAP:
                    // Capture all screens (placeholder - implement when snap feature exists)
                    StatusManager.ShowMessage("All Screens Snap hotkey pressed (not yet implemented)");
                    break;
            }
        }

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
