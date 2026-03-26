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

        // ── General Settings ──────────────────────────────────────────────────

        public bool MinimizeOnStartRecording { get; set; } = Defaults.MinimizeOnStartRecording;

        // ── Indicator Settings ────────────────────────────────────────────────

        // Stored as ARGB int so System.Text.Json can round-trip it without a custom converter
        public int IndicatorColorArgb { get; set; } = Defaults.IndicatorColorArgb;

        public ClickIndicatorStyle IndicatorStyle { get; set; } = Defaults.IndicatorStyle;

        // ── Click Screenshot Settings ─────────────────────────────────────────

        public ClickScreenshotMode ClickScreenshotMode { get; set; } = Defaults.ClickScreenshotMode;

        private int _clickCroppedPadding = Defaults.ClickCroppedPadding;
        public int ClickCroppedPadding
        {
            get => _clickCroppedPadding;
            set => _clickCroppedPadding = Math.Clamp(value, Bounds.MinCroppedPadding, Bounds.MaxCroppedPadding);
        }

        // ── Drag Screenshot Settings ──────────────────────────────────────────

        public DragScreenshotMode DragScreenshotMode { get; set; } = Defaults.DragScreenshotMode;

        public FallbackDragScreenshotMode DragFallbackMode { get; set; } = Defaults.DragFallbackMode;

        private int _dragCroppedPadding = Defaults.DragCroppedPadding;
        public int DragCroppedPadding
        {
            get => _dragCroppedPadding;
            set => _dragCroppedPadding = Math.Clamp(value, Bounds.MinCroppedPadding, Bounds.MaxCroppedPadding);
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
            // General
            MinimizeOnStartRecording = Defaults.MinimizeOnStartRecording;

            // Indicator
            IndicatorColorArgb = Defaults.IndicatorColorArgb;
            IndicatorStyle = Defaults.IndicatorStyle;

            // Click Screenshot
            ClickScreenshotMode = Defaults.ClickScreenshotMode;
            ClickCroppedPadding = Defaults.ClickCroppedPadding;

            // Drag Screenshot
            DragScreenshotMode = Defaults.DragScreenshotMode;
            DragFallbackMode = Defaults.DragFallbackMode;
            DragCroppedPadding = Defaults.DragCroppedPadding;
        }

        /// <summary>Resets a specific setting to its default value.</summary>
        /// <param name="settingName">The name of the setting property to reset.</param>
        public void ResetToDefault(string settingName)
        {
            switch (settingName)
            {
                case nameof(IndicatorColorArgb):
                case nameof(IndicatorColor):
                    IndicatorColorArgb = Defaults.IndicatorColorArgb;
                    break;
                case nameof(IndicatorStyle):
                    IndicatorStyle = Defaults.IndicatorStyle;
                    break;
                case nameof(ClickScreenshotMode):
                    ClickScreenshotMode = Defaults.ClickScreenshotMode;
                    break;
                case nameof(DragScreenshotMode):
                    DragScreenshotMode = Defaults.DragScreenshotMode;
                    break;
                case nameof(MinimizeOnStartRecording):
                    MinimizeOnStartRecording = Defaults.MinimizeOnStartRecording;
                    break;
                case nameof(DragFallbackMode):
                    DragFallbackMode = Defaults.DragFallbackMode;
                    break;
                case nameof(ClickCroppedPadding):
                    ClickCroppedPadding = Defaults.ClickCroppedPadding;
                    break;
                case nameof(DragCroppedPadding):
                    DragCroppedPadding = Defaults.DragCroppedPadding;
                    break;
            }
        }

        // ── Persistence ────────────────────────────────────────────────────────

        /// <summary>Validates and clamps all settings values to their valid ranges.</summary>
        private void ValidateAndClamp()
        {
            // Properties with validation in setters will auto-clamp when assigned
            // Force clamping by re-assigning to trigger setters
            ClickCroppedPadding = _clickCroppedPadding;
            DragCroppedPadding = _dragCroppedPadding;
        }

        /// <summary>Auto-heals and saves settings if any values were clamped during validation.</summary>
        private void AutoHealIfNeeded()
        {
            int originalClick = _clickCroppedPadding;
            int originalDrag = _dragCroppedPadding;

            ValidateAndClamp();

            // If clamping occurred, save the healed values back to disk
            if (_clickCroppedPadding != originalClick || 
                _dragCroppedPadding != originalDrag)
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
                }
                else
                {
                    settings = new BSRSettings();
                }
            }
            catch
            {
                settings = new BSRSettings();
            }

            settings.AutoHealIfNeeded();

            return settings;
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
    }
}
