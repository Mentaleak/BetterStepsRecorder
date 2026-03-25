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
    }


    /// <summary>
    /// Persisted settings for the recording/capture behaviour.
    /// Saved to %LOCALAPPDATA%\BetterStepsRecorder\recording.json.
    /// </summary>
    public class BSRSettings
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BetterStepsRecorder",
            "recording.json");

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

        public int ClickCroppedPadding { get; set; } = Defaults.ClickCroppedPadding;

        // ── Drag Screenshot Settings ──────────────────────────────────────────

        public DragScreenshotMode DragScreenshotMode { get; set; } = Defaults.DragScreenshotMode;

        public FallbackDragScreenshotMode DragFallbackMode { get; set; } = Defaults.DragFallbackMode;

        public int DragCroppedPadding { get; set; } = Defaults.DragCroppedPadding;

        // ── Helpers ────────────────────────────────────────────────────────────

        [JsonIgnore]
        public Color ArrowColor
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
                case nameof(ArrowColor):
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

        public static BSRSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<BSRSettings>(json) ?? new BSRSettings();
                }
            }
            catch { }
            return new BSRSettings();
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

        /// <summary>Applies loaded values to the live Program static properties.</summary>
        public void Apply()
        {
            // Indicator
            Program.ArrowColor = ArrowColor;
            Program.IndicatorStyle = IndicatorStyle;

            // Click Screenshot
            Program.ClickScreenshotMode = ClickScreenshotMode;
            Program.ClickCroppedPadding = ClickCroppedPadding;

            // Drag Screenshot
            Program.DragScreenshotMode = DragScreenshotMode;
            Program.DragFallbackMode = DragFallbackMode;
            Program.DragCroppedPadding = DragCroppedPadding;
        }

        /// <summary>Snapshots the current live Program static properties and saves to disk.</summary>
        public static void SaveCurrent()
        {
            var s = new BSRSettings
            {
                // Indicator
                ArrowColor = Program.ArrowColor,
                IndicatorStyle = Program.IndicatorStyle,

                // Click Screenshot
                ClickScreenshotMode = Program.ClickScreenshotMode,
                ClickCroppedPadding = Program.ClickCroppedPadding,

                // Drag Screenshot
                DragScreenshotMode = Program.DragScreenshotMode,
                DragFallbackMode = Program.DragFallbackMode,
                DragCroppedPadding = Program.DragCroppedPadding
            };
            s.Save();
        }
    }
}
