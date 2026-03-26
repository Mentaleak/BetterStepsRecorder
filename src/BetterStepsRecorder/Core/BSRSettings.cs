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
    /// </summary>
    public class BSRSettings
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

        /// <summary>
        /// Default values for all settings.
        /// </summary>
        public static class Defaults
        {
            //General
            public static readonly bool MinimizeOnStartRecording = true;
            //Indicator
            public static readonly Color IndicatorColor = Color.Magenta;
            public static readonly int IndicatorColorArgb = Color.Magenta.ToArgb();
            public static readonly ClickIndicatorStyle IndicatorStyle = ClickIndicatorStyle.Arrow;
            //Click Screenshot
            public static readonly ClickScreenshotMode ClickScreenshotMode = ClickScreenshotMode.ActiveWindow;
            public static readonly int ClickCroppedPadding = 200;
            //Drag Screenshot
            public static readonly DragScreenshotMode DragScreenshotMode = DragScreenshotMode.ActiveWindow;
            public static readonly FallbackDragScreenshotMode DragFallbackMode = FallbackDragScreenshotMode.Cropped;
            public static readonly int DragCroppedPadding = 120;
        }

        // ── Nested Settings Classes ───────────────────────────────────────────

        public class GeneralSettings
        {
            public bool MinimizeOnStartRecording { get; set; } = Defaults.MinimizeOnStartRecording;
        }

        public class IndicatorSettings
        {
            public ClickIndicatorStyle Style { get; set; } = Defaults.IndicatorStyle;

            [JsonConverter(typeof(JsonTools.ArgbHexConverter))]
            public int Color { get; set; } = Defaults.IndicatorColorArgb;
        }

        public class CroppedSettings
        {
            public int Padding { get; set; }
        }

        public class ClickSettings
        {
            public ClickScreenshotMode Mode { get; set; } = Defaults.ClickScreenshotMode;
            public CroppedSettings Cropped { get; set; } = new CroppedSettings { Padding = Defaults.ClickCroppedPadding };
        }

        public class DragFallbackSettings
        {
            public FallbackDragScreenshotMode Mode { get; set; } = Defaults.DragFallbackMode;
        }

        public class DragSettings
        {
            public DragScreenshotMode Mode { get; set; } = Defaults.DragScreenshotMode;
            public CroppedSettings Cropped { get; set; } = new CroppedSettings { Padding = Defaults.DragCroppedPadding };
            public DragFallbackSettings Fallback { get; set; } = new DragFallbackSettings();
        }

        public class ScreenshotSettings
        {
            public ClickSettings Click { get; set; } = new ClickSettings();
            public DragSettings Drag { get; set; } = new DragSettings();
        }

        public class HtmlSettings
        {
            public bool ShowSummary { get; set; } = true;
            public bool ShowGeneratedDate { get; set; } = true;
            public bool ShowStepTimestamps { get; set; } = false;
            public bool ShowAction { get; set; } = false;
            public bool ShowApplication { get; set; } = false;
            public bool ShowWindow { get; set; } = false;
            public bool ShowElement { get; set; } = false;
            public bool ShowElementType { get; set; } = false;
            public bool ShowMousePosition { get; set; } = false;

            [JsonIgnore]
            public bool IsDetailStripEmpty =>
                !ShowAction && !ShowApplication && !ShowWindow &&
                !ShowElement && !ShowElementType && !ShowMousePosition;
        }

        public class ExportSettings
        {
            public HtmlSettings Html { get; set; } = new HtmlSettings();
        }

        // ── Top-Level Properties ──────────────────────────────────────────────

        public GeneralSettings General { get; set; } = new GeneralSettings();
        public IndicatorSettings Indicator { get; set; } = new IndicatorSettings();
        public ScreenshotSettings Screenshot { get; set; } = new ScreenshotSettings();
        public ExportSettings ExportOptions { get; set; } = new ExportSettings();

        // ── Backward Compatibility Properties (JsonIgnore) ───────────────────

        [JsonIgnore]
        public bool MinimizeOnStartRecording
        {
            get => General.MinimizeOnStartRecording;
            set => General.MinimizeOnStartRecording = value;
        }

        [JsonIgnore]
        public int IndicatorColorArgb
        {
            get => Indicator.Color;
            set => Indicator.Color = value;
        }

        [JsonIgnore]
        public ClickIndicatorStyle IndicatorStyle
        {
            get => Indicator.Style;
            set => Indicator.Style = value;
        }

        [JsonIgnore]
        public ClickScreenshotMode ClickScreenshotMode
        {
            get => Screenshot.Click.Mode;
            set => Screenshot.Click.Mode = value;
        }

        [JsonIgnore]
        public int ClickCroppedPadding
        {
            get => Screenshot.Click.Cropped.Padding;
            set => Screenshot.Click.Cropped.Padding = Math.Clamp(value, Bounds.MinCroppedPadding, Bounds.MaxCroppedPadding);
        }

        [JsonIgnore]
        public DragScreenshotMode DragScreenshotMode
        {
            get => Screenshot.Drag.Mode;
            set => Screenshot.Drag.Mode = value;
        }

        [JsonIgnore]
        public FallbackDragScreenshotMode DragFallbackMode
        {
            get => Screenshot.Drag.Fallback.Mode;
            set => Screenshot.Drag.Fallback.Mode = value;
        }

        [JsonIgnore]
        public int DragCroppedPadding
        {
            get => Screenshot.Drag.Cropped.Padding;
            set => Screenshot.Drag.Cropped.Padding = Math.Clamp(value, Bounds.MinCroppedPadding, Bounds.MaxCroppedPadding);
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        [JsonIgnore]
        public Color IndicatorColor
        {
            get => Color.FromArgb(IndicatorColorArgb);
            set => IndicatorColorArgb = value.ToArgb();
        }

        // ── Default Settings Management ───────────────────────────────────────

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
        }

        /// <summary>Resets a specific setting to its default value.</summary>
        /// <param name="settingName">The name of the setting property to reset.</param>
        public void ResetToDefault(string settingName)
        {
            switch (settingName)
            {
                case nameof(IndicatorColorArgb):
                case nameof(IndicatorColor):
                    Indicator.Color = Defaults.IndicatorColorArgb;
                    break;
                case nameof(IndicatorStyle):
                    Indicator.Style = Defaults.IndicatorStyle;
                    break;
                case nameof(ClickScreenshotMode):
                    Screenshot.Click.Mode = Defaults.ClickScreenshotMode;
                    break;
                case nameof(DragScreenshotMode):
                    Screenshot.Drag.Mode = Defaults.DragScreenshotMode;
                    break;
                case nameof(MinimizeOnStartRecording):
                    General.MinimizeOnStartRecording = Defaults.MinimizeOnStartRecording;
                    break;
                case nameof(DragFallbackMode):
                    Screenshot.Drag.Fallback.Mode = Defaults.DragFallbackMode;
                    break;
                case nameof(ClickCroppedPadding):
                    Screenshot.Click.Cropped.Padding = Defaults.ClickCroppedPadding;
                    break;
                case nameof(DragCroppedPadding):
                    Screenshot.Drag.Cropped.Padding = Defaults.DragCroppedPadding;
                    break;
            }
        }

        // ── Persistence ────────────────────────────────────────────────────────

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
