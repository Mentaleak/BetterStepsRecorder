using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BetterStepsRecorder
{
    /// <summary>
    /// Persisted settings for the recording/capture behaviour.
    /// Saved to %LOCALAPPDATA%\BetterStepsRecorder\recording.json.
    /// </summary>
    public class RecordingSettings
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BetterStepsRecorder",
            "recording.json");

        // Stored as ARGB int so System.Text.Json can round-trip it without a custom converter
        public int ArrowColorArgb { get; set; } = Color.Magenta.ToArgb();

        public ClickIndicatorStyle IndicatorStyle { get; set; } = ClickIndicatorStyle.Arrow;

        public DragScreenshotMode DragScreenshotMode { get; set; } = DragScreenshotMode.Cropped;

        public bool CheckForUpdatesAtLaunch { get; set; } = true;

        // ── Helpers ────────────────────────────────────────────────────────────

        [JsonIgnore]
        public Color ArrowColor
        {
            get => Color.FromArgb(ArrowColorArgb);
            set => ArrowColorArgb = value.ToArgb();
        }

        // ── Persistence ────────────────────────────────────────────────────────

        public static RecordingSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<RecordingSettings>(json) ?? new RecordingSettings();
                }
            }
            catch { }
            return new RecordingSettings();
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
            Program.ArrowColor = ArrowColor;
            Program.IndicatorStyle = IndicatorStyle;
            Program.DragScreenshotMode = DragScreenshotMode;
            Program.CheckForUpdatesAtLaunch = CheckForUpdatesAtLaunch;
        }

        /// <summary>Snapshots the current live Program static properties and saves to disk.</summary>
        public static void SaveCurrent()
        {
            var s = new RecordingSettings
            {
                ArrowColor = Program.ArrowColor,
                IndicatorStyle = Program.IndicatorStyle,
                DragScreenshotMode = Program.DragScreenshotMode,
                CheckForUpdatesAtLaunch = Program.CheckForUpdatesAtLaunch
            };
            s.Save();
        }
    }
}
