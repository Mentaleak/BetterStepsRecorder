using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetterStepsRecorder
{
    /// <summary>
    /// Checks GitHub for newer releases and applies updates via the Updater helper process.
    /// Neither public method throws — all exceptions are caught internally.
    /// </summary>
    public static class UpdaterService
    {
        private const string ReleasesApiUrl =
            "https://api.github.com/repos/Mentaleak/BetterStepsRecorder/releases/latest";

        /// <summary>Current assembly version, resolved once at startup.</summary>
        public static readonly Version? CurrentVersion =
            Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// When true, version comparison is skipped and any returned release is treated as newer.
        /// Set at startup when --force-update-check is present on the command line.
        /// </summary>
        public static bool ForceUpdateCheck { get; set; }

        /// <summary>
        /// When true, the download step is skipped and %TEMP%\BSR_update.zip is used directly.
        /// Set at startup when --test-update-check is present on the command line.
        /// </summary>
        public static bool TestUpdateCheck { get; set; }

        /// <summary>Cached result from the last check — avoids duplicate API calls within a session.</summary>
        private static UpdateCheckResult? _cachedResult;

        private static readonly HttpClient _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        static UpdaterService()
        {
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("BetterStepsRecorder");
        }

        /// <summary>
        /// Queries the GitHub releases API and returns whether a newer version exists.
        /// Result is cached for the lifetime of the process — subsequent calls return the cached value.
        /// </summary>
        public static async Task<UpdateCheckResult> CheckForUpdateAsync()
        {
            if (_cachedResult != null)
                return _cachedResult;

            try
            {
                string json = await _http.GetStringAsync(ReleasesApiUrl);

                using JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;

                string tagName = root.GetProperty("tag_name").GetString() ?? string.Empty;
                string downloadUrl = string.Empty;
                if (root.TryGetProperty("assets", out JsonElement assets) &&
                    assets.GetArrayLength() > 0)
                {
                    downloadUrl = assets[0].GetProperty("browser_download_url").GetString() ?? string.Empty;
                }

                // Strip leading 'v' from tag (e.g. "v2026.3.20.0" → "2026.3.20.0")
                string versionStr = tagName.TrimStart('v');

                bool isNewer = ForceUpdateCheck ||
                               (Version.TryParse(versionStr, out Version? latest) &&
                                latest != null &&
                                CurrentVersion != null &&
                                latest > CurrentVersion);

                _cachedResult = new UpdateCheckResult
                {
                    IsUpdateAvailable = isNewer,
                    LatestVersion = versionStr,
                    DownloadUrl = downloadUrl
                };
                return _cachedResult;
            }
            catch (Exception ex)
            {
                return new UpdateCheckResult
                {
                    IsUpdateAvailable = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Downloads the release zip, extracts it, then launches Updater.exe and shuts down BSR.
        /// Returns false on any failure.
        /// </summary>
        public static async Task<bool> DownloadAndApplyUpdateAsync(string downloadUrl)
        {
            try
            {
                // Resolve paths using the same mechanism used throughout the codebase
                string? mainModulePath = Process.GetCurrentProcess().MainModule?.FileName;
                if (string.IsNullOrEmpty(mainModulePath))
                    return false;

                string installPath = Path.GetDirectoryName(mainModulePath)!;
                string exeFilename = Path.GetFileName(mainModulePath);

                string tempZip = Path.Combine(Path.GetTempPath(), "BSR_update.zip");
                string updateDir = Path.Combine(Path.GetTempPath(), "BSR_update");

                // Download zip (skipped when --test-update-check is active)
                if (!TestUpdateCheck)
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                    using HttpResponseMessage response = await _http.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                    response.EnsureSuccessStatusCode();
                    using Stream stream = await response.Content.ReadAsStreamAsync(cts.Token);
                    using FileStream fs = new FileStream(tempZip, FileMode.Create, FileAccess.Write, FileShare.None);
                    await stream.CopyToAsync(fs, cts.Token);
                }
                else if (!File.Exists(tempZip))
                {
                    return false; // test mode but no local zip found
                }

                // Extract zip, overwriting previous contents
                if (Directory.Exists(updateDir))
                    Directory.Delete(updateDir, true);
                ZipFile.ExtractToDirectory(tempZip, updateDir);

                // Verify Updater.exe exists in extracted folder
                string updaterInExtract = Path.Combine(updateDir, "Updater.exe");
                if (!File.Exists(updaterInExtract))
                    return false;

                // Launch Updater.exe directly from the extracted folder —
                // all its runtime dependencies are already alongside it there
                Process.Start(new ProcessStartInfo
                {
                    FileName = updaterInExtract,
                    Arguments = $"\"{installPath}\" \"{exeFilename}\"",
                    UseShellExecute = true
                });

                // Shut down BSR
                Application.Exit();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
