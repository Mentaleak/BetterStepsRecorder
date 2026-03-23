using System;
using System.IO;
using System.Text.Json;

namespace BetterStepsRecorder
{
    /// <summary>
    /// Persisted settings controlling which metadata sections appear in the HTML export.
    /// All options default to true (full output). Saved to %LOCALAPPDATA%\BetterStepsRecorder\htmlexport.json.
    /// </summary>
    public class HtmlExportSettings
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BetterStepsRecorder",
            "htmlexport.json");

        // Summary bar (Steps / Started / Finished / Duration)
        public bool ShowSummary { get; set; } = true;

        // "Generated <date>" line in the page header
        public bool ShowGeneratedDate { get; set; } = true;

        // Timestamp row on each step card
        public bool ShowStepTimestamps { get; set; } = true;

        // Detail strip: Action / Application / Window / Element / Element Type / Mouse Position
        public bool ShowAction { get; set; } = true;
        public bool ShowApplication { get; set; } = true;
        public bool ShowWindow { get; set; } = true;
        public bool ShowElement { get; set; } = true;
        public bool ShowElementType { get; set; } = true;
        public bool ShowMousePosition { get; set; } = true;

        /// <summary>Returns true when every detail-strip option is off (so the strip itself can be omitted).</summary>
        public bool IsDetailStripEmpty =>
            !ShowAction && !ShowApplication && !ShowWindow &&
            !ShowElement && !ShowElementType && !ShowMousePosition;

        // ── Persistence ────────────────────────────────────────────────────────

        public static HtmlExportSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<HtmlExportSettings>(json) ?? new HtmlExportSettings();
                }
            }
            catch { }
            return new HtmlExportSettings();
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
