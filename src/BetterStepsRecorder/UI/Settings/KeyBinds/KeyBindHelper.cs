using System;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    /// <summary>
    /// Helper class for parsing and formatting key combinations.
    /// </summary>
    public static class KeyBindHelper
    {
        /// <summary>
        /// Converts a Keys value to a display string.
        /// </summary>
        public static string KeysToString(Keys keys)
        {
            if (keys == Keys.None)
                return "None";

            var parts = new System.Collections.Generic.List<string>();

            // Check modifiers
            if ((keys & Keys.Control) == Keys.Control)
                parts.Add("Ctrl");
            if ((keys & Keys.Shift) == Keys.Shift)
                parts.Add("Shift");
            if ((keys & Keys.Alt) == Keys.Alt)
                parts.Add("Alt");

            // Get the key without modifiers
            Keys keyCode = keys & Keys.KeyCode;
            if (keyCode != Keys.None && keyCode != Keys.ControlKey && keyCode != Keys.ShiftKey && keyCode != Keys.Menu)
            {
                parts.Add(GetKeyDisplayName(keyCode));
            }

            return parts.Count > 0 ? string.Join("+", parts) : "None";
        }

        /// <summary>
        /// Parses a key combination string to a Keys value.
        /// </summary>
        public static Keys StringToKeys(string keyString)
        {
            if (string.IsNullOrWhiteSpace(keyString) || keyString.Equals("None", StringComparison.OrdinalIgnoreCase))
                return Keys.None;

            Keys result = Keys.None;
            string[] parts = keyString.Split('+');

            foreach (string part in parts)
            {
                string trimmed = part.Trim();
                switch (trimmed.ToLowerInvariant())
                {
                    case "ctrl":
                    case "control":
                        result |= Keys.Control;
                        break;
                    case "shift":
                        result |= Keys.Shift;
                        break;
                    case "alt":
                        result |= Keys.Alt;
                        break;
                    default:
                        // Try to parse the key
                        if (Enum.TryParse<Keys>(trimmed, true, out Keys key))
                        {
                            result |= key;
                        }
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a display-friendly name for a key.
        /// </summary>
        private static string GetKeyDisplayName(Keys key)
        {
            return key switch
            {
                Keys.D0 => "0",
                Keys.D1 => "1",
                Keys.D2 => "2",
                Keys.D3 => "3",
                Keys.D4 => "4",
                Keys.D5 => "5",
                Keys.D6 => "6",
                Keys.D7 => "7",
                Keys.D8 => "8",
                Keys.D9 => "9",
                Keys.OemMinus => "-",
                Keys.Oemplus => "+",
                Keys.OemOpenBrackets => "[",
                Keys.OemCloseBrackets => "]",
                Keys.OemPipe => "\\",
                Keys.OemSemicolon => ";",
                Keys.OemQuotes => "'",
                Keys.Oemcomma => ",",
                Keys.OemPeriod => ".",
                Keys.OemQuestion => "/",
                Keys.Oemtilde => "`",
                _ => key.ToString()
            };
        }

        /// <summary>
        /// Validates that a key combination is suitable for a global hotkey.
        /// Returns true if valid, false if invalid.
        /// </summary>
        public static bool ValidateKeyBind(Keys keys, out string? errorMessage)
        {
            errorMessage = null;

            if (keys == Keys.None)
            {
                errorMessage = "No key combination set.";
                return false;
            }

            // Get the key without modifiers
            Keys keyCode = keys & Keys.KeyCode;
            Keys modifiers = keys & Keys.Modifiers;

            // Must have at least one modifier for global hotkeys
            if (modifiers == Keys.None)
            {
                errorMessage = "Global hotkeys must include at least one modifier (Ctrl, Shift, or Alt).";
                return false;
            }

            // Must have a non-modifier key
            if (keyCode == Keys.None || keyCode == Keys.ControlKey || keyCode == Keys.ShiftKey || keyCode == Keys.Menu)
            {
                errorMessage = "Please press a key along with the modifier(s).";
                return false;
            }

            // Check for reserved system combinations
            if (IsReservedCombination(keys))
            {
                errorMessage = "This key combination is reserved by the system.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if a key combination is reserved by the system.
        /// </summary>
        private static bool IsReservedCombination(Keys keys)
        {
            // Common reserved combinations
            return keys switch
            {
                Keys.Control | Keys.Alt | Keys.Delete => true,  // Ctrl+Alt+Delete
                Keys.Alt | Keys.F4 => true,                      // Alt+F4
                Keys.Alt | Keys.Tab => true,                     // Alt+Tab
                Keys.Control | Keys.Escape => true,              // Ctrl+Escape
                Keys.Control | Keys.C => true,                   // Ctrl+C (Copy)
                Keys.Control | Keys.V => true,                   // Ctrl+V (Paste)
                Keys.Control | Keys.X => true,                   // Ctrl+X (Cut)
                Keys.Control | Keys.Z => true,                   // Ctrl+Z (Undo)
                Keys.Control | Keys.Y => true,                   // Ctrl+Y (Redo)
                Keys.Control | Keys.A => true,                   // Ctrl+A (Select All)
                Keys.Control | Keys.S => true,                   // Ctrl+S (Save)
                Keys.Control | Keys.O => true,                   // Ctrl+O (Open)
                Keys.Control | Keys.N => true,                   // Ctrl+N (New)
                Keys.Control | Keys.P => true,                   // Ctrl+P (Print)
                Keys.Control | Keys.F => true,                   // Ctrl+F (Find)
                Keys.Control | Keys.W => true,                   // Ctrl+W (Close Tab)
                Keys.Control | Keys.Shift | Keys.S => true,      // Ctrl+Shift+S (Save As)
                Keys.Control | Keys.Shift | Keys.W => true,      // Ctrl+Shift+W (Close Window/All Tabs)
                Keys.Control | Keys.Shift | Keys.N => true,      // Ctrl+Shift+N (New Window/Incognito)
                Keys.Control | Keys.Shift | Keys.T => true,      // Ctrl+Shift+T (Reopen Tab)
                Keys.Control | Keys.Shift | Keys.Escape => true, // Ctrl+Shift+Escape (Task Manager)
                _ => false
            };
        }

        /// <summary>
        /// Gets a list of suggested non-conflicting key combinations.
        /// </summary>
        public static string[] GetSuggestedKeyBinds()
        {
            return new[]
            {
                "Ctrl+Shift+R",
                "Ctrl+Shift+W",
                "Ctrl+Shift+S",
                "Ctrl+Shift+A",
                "Ctrl+Shift+P",
                "Ctrl+Alt+R",
                "Ctrl+Alt+W",
                "Ctrl+Alt+S",
                "Ctrl+Alt+A",
                "F9",
                "F10",
                "F11",
                "Ctrl+F9",
                "Ctrl+F10",
                "Ctrl+F11",
                "Shift+F9",
                "Shift+F10",
                "Shift+F11"
            };
        }
    }
}
