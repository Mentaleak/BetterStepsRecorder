using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BetterStepsRecorder
{
    /// <summary>
    /// Indicator style for click visualization.
    /// </summary>
    public enum ClickIndicatorStyle
    {
        Arrow,
        Circle,
        Cursor
    }

    /// <summary>
    /// Screenshot mode for click events.
    /// </summary>
    public enum ClickScreenshotMode
    {
        Cropped,
        ActiveWindow,
        ActiveScreen,
        AllScreens
    }

    /// <summary>
    /// Screenshot mode for drag events.
    /// </summary>
    public enum DragScreenshotMode
    {
        Cropped,
        ActiveWindow,
        ActiveScreen,
        AllScreens
    }

    /// <summary>
    /// Screenshot mode for drag events.
    /// </summary>
    public enum FallbackDragScreenshotMode
    {
        Cropped,
        ActiveScreen,
        AllScreens
        // ActiveWindow is skipped since it is the primary mode that triggers fallback when it fails
    }


    /// <summary>
    /// Persisted settings for the recording/capture behaviour.
    /// Saved to %LOCALAPPDATA%\BetterStepsRecorder\bsrsettings.json.
    /// Settings are organized hierarchically to match the Settings UI TreeView.
    /// </summary>
    public partial class BSRSettings
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BetterStepsRecorder",
            "bsrsettings.json");

        private static BSRSettings? _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Default settings instance with all default values.
        /// Example: Default.General.MinimizeOnStartRecording
        /// </summary>
        public static readonly BSRSettings Default = new BSRSettings();

        /// <summary>
        /// Validation constants for settings bounds.
        /// </summary>
        public static class Bounds
        {
            public const int MinCroppedPadding = 50;
            public const int MaxCroppedPadding = 500;
        }

        /// <summary>
        /// Gets the current settings instance (singleton).
        /// </summary>
        public static BSRSettings Current
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= Load();
                    }
                }
                return _instance;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // Top-Level Properties (match TreeView structure)
        // ══════════════════════════════════════════════════════════════════════

        public GeneralSettings General { get; set; } = new GeneralSettings();
        public IndicatorSettings Indicator { get; set; } = new IndicatorSettings();
        public ScreenshotSettings Screenshot { get; set; } = new ScreenshotSettings();
        public ExportSettings ExportOptions { get; set; } = new ExportSettings();

        // ══════════════════════════════════════════════════════════════════════
        // Default Settings Management
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>Resets all settings to their default values by creating fresh instances.</summary>
        public void ResetToDefaults()
        {
            General = new GeneralSettings();
            Indicator = new IndicatorSettings();
            Screenshot = new ScreenshotSettings();
            ExportOptions = new ExportSettings();
        }

        /// <summary>Resets a specific setting to its default value.</summary>
        /// <param name="settingName">The hierarchical path or legacy name of the setting to reset.</param>
        public void ResetToDefault(string settingName)
        {
            // Support both old flat names (for compatibility) and new hierarchical paths
            switch (settingName)
            {
                case "IndicatorColorArgb":
                case "IndicatorColor":
                case "Indicator.Color":
                    Indicator.Color = Default.Indicator.Color;
                    break;
                case "IndicatorStyle":
                case "Indicator.Style":
                    Indicator.Style = Default.Indicator.Style;
                    break;
                case "ClickScreenshotMode":
                case "Screenshot.Click.Mode":
                    Screenshot.Click.Mode = Default.Screenshot.Click.Mode;
                    break;
                case "DragScreenshotMode":
                case "Screenshot.Drag.Mode":
                    Screenshot.Drag.Mode = Default.Screenshot.Drag.Mode;
                    break;
                case "MinimizeOnStartRecording":
                case "General.MinimizeOnStartRecording":
                    General.MinimizeOnStartRecording = Default.General.MinimizeOnStartRecording;
                    break;
                case "DragFallbackMode":
                case "Screenshot.Drag.Fallback.Mode":
                    Screenshot.Drag.Fallback.Mode = Default.Screenshot.Drag.Fallback.Mode;
                    break;
                case "ClickCroppedPadding":
                case "Screenshot.Click.Cropped.Padding":
                    Screenshot.Click.Cropped.Padding = Default.Screenshot.Click.Cropped.Padding;
                    break;
                case "DragCroppedPadding":
                case "Screenshot.Drag.Cropped.Padding":
                    Screenshot.Drag.Cropped.Padding = Default.Screenshot.Drag.Cropped.Padding;
                    break;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // Persistence & Validation
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>Validates and clamps all settings values to their valid ranges.</summary>
        private bool ValidateAndClamp()
        {
            bool wasModified = false;

            // Validate and clamp padding values
            int originalClickPadding = Screenshot.Click.Cropped.Padding;
            int originalDragPadding = Screenshot.Drag.Cropped.Padding;

            Screenshot.Click.Cropped.Padding = Math.Clamp(
                Screenshot.Click.Cropped.Padding, 
                Bounds.MinCroppedPadding, 
                Bounds.MaxCroppedPadding);

            Screenshot.Drag.Cropped.Padding = Math.Clamp(
                Screenshot.Drag.Cropped.Padding, 
                Bounds.MinCroppedPadding, 
                Bounds.MaxCroppedPadding);

            wasModified |= Screenshot.Click.Cropped.Padding != originalClickPadding;
            wasModified |= Screenshot.Drag.Cropped.Padding != originalDragPadding;

            // Validate color (ensure it's not fully transparent and has valid alpha channel)
            var color = System.Drawing.Color.FromArgb(Indicator.Color);
            if (color.A < 128) // If mostly transparent (less than 50% opacity), reset to default
            {
                Indicator.Color = Default.Indicator.Color;
                wasModified = true;
            }

            // Validate enum values
            if (!Enum.IsDefined(typeof(ClickIndicatorStyle), Indicator.Style))
            {
                Indicator.Style = Default.Indicator.Style;
                wasModified = true;
            }

            if (!Enum.IsDefined(typeof(ClickScreenshotMode), Screenshot.Click.Mode))
            {
                Screenshot.Click.Mode = Default.Screenshot.Click.Mode;
                wasModified = true;
            }

            if (!Enum.IsDefined(typeof(DragScreenshotMode), Screenshot.Drag.Mode))
            {
                Screenshot.Drag.Mode = Default.Screenshot.Drag.Mode;
                wasModified = true;
            }

            if (!Enum.IsDefined(typeof(FallbackDragScreenshotMode), Screenshot.Drag.Fallback.Mode))
            {
                Screenshot.Drag.Fallback.Mode = Default.Screenshot.Drag.Fallback.Mode;
                wasModified = true;
            }

            // Note: Boolean values don't need explicit validation as they're value types
            // and JSON deserializer will use default (false) if invalid. The nested object
            // initialization already handles null scenarios by creating new instances with
            // proper default values from class initializers.

            return wasModified;
        }

        /// <summary>Auto-heals and saves settings if any values were invalid or clamped during validation.</summary>
        private void AutoHealIfNeeded()
        {
            if (ValidateAndClamp())
            {
                Save();
            }
        }

        /// <summary>Loads settings from disk.</summary>
        private static BSRSettings Load()
        {
            BSRSettings settings;
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    settings = JsonSerializer.Deserialize<BSRSettings>(json) ?? new BSRSettings();

                    // Ensure nested objects are initialized if deserialization resulted in nulls
                    settings.General ??= new GeneralSettings();
                    settings.Indicator ??= new IndicatorSettings();
                    settings.Screenshot ??= new ScreenshotSettings();
                    settings.Screenshot.Click ??= new ClickSettings();
                    settings.Screenshot.Drag ??= new DragSettings();
                    settings.Screenshot.Click.Cropped ??= new CroppedSettings { Padding = Default.Screenshot.Click.Cropped.Padding };
                    settings.Screenshot.Drag.Cropped ??= new CroppedSettings { Padding = Default.Screenshot.Drag.Cropped.Padding };
                    settings.Screenshot.Drag.Fallback ??= new DragFallbackSettings();
                    settings.ExportOptions ??= new ExportSettings();
                    settings.ExportOptions.Html ??= new HtmlSettings();
                }
                else
                {
                    settings = new BSRSettings();
                }

                // Migrate legacy HtmlExportSettings if they exist

            }
            catch
            {
                settings = new BSRSettings();
            }

            settings.AutoHealIfNeeded();

            return settings;
        }



        /// <summary>Saves settings to disk.</summary>
        public void Save()
        {
            try
            {
                string? directoryPath = Path.GetDirectoryName(SettingsPath);
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch { }
        }

        /// <summary>Exports settings to a specified file path.</summary>
        /// <param name="filePath">The file path to export to.</param>
        /// <returns>True if export succeeded, false otherwise.</returns>
        public bool Export(string filePath)
        {
            try
            {
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Imports settings from a specified file path and updates the singleton.</summary>
        /// <param name="filePath">The file path to import from.</param>
        /// <returns>True if import succeeded, false otherwise.</returns>
        public static bool Import(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return false;

                string json = File.ReadAllText(filePath);
                var imported = JsonSerializer.Deserialize<BSRSettings>(json);
                if (imported == null) return false;

                imported.AutoHealIfNeeded();

                lock (_lock)
                {
                    _instance = imported;
                    _instance.Save();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
