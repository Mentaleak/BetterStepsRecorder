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

        /// <summary>Resets all settings to their default values.</summary>
        public void ResetToDefaults()
        {
            General.MinimizeOnStartRecording = Defaults.MinimizeOnStartRecording;

            Indicator.Color = Defaults.IndicatorColorArgb;
            Indicator.Style = Defaults.IndicatorStyle;

            Screenshot.Click.Mode = Defaults.ClickScreenshotMode;
            Screenshot.Click.Cropped.Padding = Defaults.ClickCroppedPadding;

            Screenshot.Drag.Mode = Defaults.DragScreenshotMode;
            Screenshot.Drag.Cropped.Padding = Defaults.DragCroppedPadding;
            Screenshot.Drag.Fallback.Mode = Defaults.DragFallbackMode;

            ExportOptions.Html.ShowSummary = Defaults.HtmlShowSummary;
            ExportOptions.Html.ShowGeneratedDate = Defaults.HtmlShowGeneratedDate;
            ExportOptions.Html.ShowStepTimestamps = Defaults.HtmlShowStepTimestamps;
            ExportOptions.Html.ShowAction = Defaults.HtmlShowAction;
            ExportOptions.Html.ShowApplication = Defaults.HtmlShowApplication;
            ExportOptions.Html.ShowWindow = Defaults.HtmlShowWindow;
            ExportOptions.Html.ShowElement = Defaults.HtmlShowElement;
            ExportOptions.Html.ShowElementType = Defaults.HtmlShowElementType;
            ExportOptions.Html.ShowMousePosition = Defaults.HtmlShowMousePosition;
        }

        /// <summary>Resets a specific setting to its default value.</summary>
        /// <param name="settingName">The hierarchical path to the setting (e.g., "General.MinimizeOnStartRecording").</param>
        public void ResetToDefault(string settingName)
        {
            // Support both old flat names (for compatibility) and new hierarchical paths
            switch (settingName)
            {
                // Legacy flat names
                case "IndicatorColorArgb":
                case "IndicatorColor":
                case "Indicator.Color":
                    Indicator.Color = Defaults.IndicatorColorArgb;
                    break;
                case "IndicatorStyle":
                case "Indicator.Style":
                    Indicator.Style = Defaults.IndicatorStyle;
                    break;
                case "ClickScreenshotMode":
                case "Screenshot.Click.Mode":
                    Screenshot.Click.Mode = Defaults.ClickScreenshotMode;
                    break;
                case "DragScreenshotMode":
                case "Screenshot.Drag.Mode":
                    Screenshot.Drag.Mode = Defaults.DragScreenshotMode;
                    break;
                case "MinimizeOnStartRecording":
                case "General.MinimizeOnStartRecording":
                    General.MinimizeOnStartRecording = Defaults.MinimizeOnStartRecording;
                    break;
                case "DragFallbackMode":
                case "Screenshot.Drag.Fallback.Mode":
                    Screenshot.Drag.Fallback.Mode = Defaults.DragFallbackMode;
                    break;
                case "ClickCroppedPadding":
                case "Screenshot.Click.Cropped.Padding":
                    Screenshot.Click.Cropped.Padding = Defaults.ClickCroppedPadding;
                    break;
                case "DragCroppedPadding":
                case "Screenshot.Drag.Cropped.Padding":
                    Screenshot.Drag.Cropped.Padding = Defaults.DragCroppedPadding;
                    break;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // Persistence & Validation
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>Validates and clamps all settings values to their valid ranges.</summary>
        private void ValidateAndClamp()
        {
            // Clamp padding values
            Screenshot.Click.Cropped.Padding = Math.Clamp(
                Screenshot.Click.Cropped.Padding, 
                Bounds.MinCroppedPadding, 
                Bounds.MaxCroppedPadding);
            
            Screenshot.Drag.Cropped.Padding = Math.Clamp(
                Screenshot.Drag.Cropped.Padding, 
                Bounds.MinCroppedPadding, 
                Bounds.MaxCroppedPadding);
        }

        /// <summary>Auto-heals and saves settings if any values were clamped during validation.</summary>
        private void AutoHealIfNeeded()
        {
            int originalClick = Screenshot.Click.Cropped.Padding;
            int originalDrag = Screenshot.Drag.Cropped.Padding;

            ValidateAndClamp();

            // If clamping occurred, save the healed values back to disk
            if (Screenshot.Click.Cropped.Padding != originalClick || 
                Screenshot.Drag.Cropped.Padding != originalDrag)
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
                    settings.Screenshot.Click.Cropped ??= new CroppedSettings { Padding = Defaults.ClickCroppedPadding };
                    settings.Screenshot.Drag.Cropped ??= new CroppedSettings { Padding = Defaults.DragCroppedPadding };
                    settings.Screenshot.Drag.Fallback ??= new DragFallbackSettings();
                    settings.ExportOptions ??= new ExportSettings();
                    settings.ExportOptions.Html ??= new HtmlSettings();
                }
                else
                {
                    settings = new BSRSettings();
                }

                // Migrate legacy HtmlExportSettings if they exist
                settings.MigrateLegacyHtmlExportSettings();
            }
            catch
            {
                settings = new BSRSettings();
            }

            settings.AutoHealIfNeeded();

            return settings;
        }

        /// <summary>Migrates settings from legacy htmlexport.json if it exists and removes the old file.</summary>
        private void MigrateLegacyHtmlExportSettings()
        {
            string legacyPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BetterStepsRecorder",
                "htmlexport.json");

            try
            {
                if (File.Exists(legacyPath))
                {
                    string json = File.ReadAllText(legacyPath);
                    var legacy = JsonSerializer.Deserialize<HtmlSettings>(json);
                    if (legacy != null)
                    {
                        ExportOptions.Html = legacy;
                        Save(); // Save migrated settings to main file
                    }
                    
                    // Remove legacy file after successful migration
                    File.Delete(legacyPath);
                }
            }
            catch { }
        }

        /// <summary>Saves settings to disk.</summary>
        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
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
