using System;
using System.IO;
using System.Text.Json;

namespace BetterStepsRecorder
{
    /// <summary>
    /// Persisted state for a pending update. Saved alongside other app settings in
    /// %LOCALAPPDATA%\BetterStepsRecorder\updatestate.json.
    /// Load at startup via UpdateState.Load(). Save on any change via instance Save().
    /// Clear all fields (and save) when an install succeeds or fails.
    /// </summary>
    public class UpdateState
    {
        private static readonly string StatePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BetterStepsRecorder",
            "updatestate.json");

        public string PendingUpdateVersion { get; set; } = string.Empty;
        public string PendingUpdateUrl { get; set; } = string.Empty;
        public bool SilentInstallOnNextLaunch { get; set; }

        // ── Persistence ────────────────────────────────────────────────────────

        public static UpdateState Load()
        {
            try
            {
                if (File.Exists(StatePath))
                {
                    string json = File.ReadAllText(StatePath);
                    return JsonSerializer.Deserialize<UpdateState>(json) ?? new UpdateState();
                }
            }
            catch { }
            return new UpdateState();
        }

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(StatePath)!);
                File.WriteAllText(StatePath, JsonSerializer.Serialize(this, _jsonOptions));
            }
            catch { }
        }

        /// <summary>Clears all pending update fields and persists the cleared state.</summary>
        public void Clear()
        {
            PendingUpdateVersion = string.Empty;
            PendingUpdateUrl = string.Empty;
            SilentInstallOnNextLaunch = false;
            Save();
        }
    }
}
