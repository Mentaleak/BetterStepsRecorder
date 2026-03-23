namespace BetterStepsRecorder
{
    /// <summary>Result DTO returned by UpdaterService.CheckForUpdateAsync().</summary>
    public class UpdateCheckResult
    {
        public bool IsUpdateAvailable { get; set; }
        public string LatestVersion { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
