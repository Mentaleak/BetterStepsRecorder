using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.IO.Compression;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using static BetterStepsRecorder.WindowHelper;
using System.IO;
using System.ComponentModel;
using Debug = System.Diagnostics.Debug;
using Application = System.Windows.Forms.Application;
using BetterStepsRecorder.Exporters;
using BetterStepsRecorder.UI;

namespace BetterStepsRecorder
{
    internal static partial class Program
    {
        public static ZipFileHandler? zip;

        public static List<RecordEvent> _recordEvents = new List<RecordEvent>();
        public static readonly object _recordEventsLock = new object();
        private static MainForm? _form1Instance;
        public static int EventCounter = 1;

        /// <summary>
        /// Per-session spool directory under %LOCALAPPDATA%\BetterStepsRecorder\spool\{guid}.
        /// Screenshot PNG files are written here instead of held in RAM as Base64 strings.
        /// </summary>
        public static readonly string SessionSpoolDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BetterStepsRecorder", "spool", Guid.NewGuid().ToString("N"));

        private const string ArgForceUpdate = "--force-update-check";
        private const string ArgTestUpdate = "--test-update-check";

        [STAThread]
        static void Main()
        {
            // Parse CLI flags before anything else
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.Equals(ArgForceUpdate, StringComparison.OrdinalIgnoreCase))
                    UpdaterService.ForceUpdateCheck = true;
                else if (arg.Equals(ArgTestUpdate, StringComparison.OrdinalIgnoreCase))
                    UpdaterService.TestUpdateCheck = true;
            }

            ApplicationConfiguration.Initialize();

            // Load persisted recording settings (arrow colour, indicator style, update prefs)
            RecordingSettings.Load().Apply();

            // Load persisted update state
            UpdateState updateState = UpdateState.Load();

            // Create the spool directory for this session
            Directory.CreateDirectory(SessionSpoolDir);

            // Clean up spool folders from previous sessions (older than 7 days)
            CleanOldSpoolSessions();

            _form1Instance = new MainForm();

            // Wire up post-show update logic before Application.Run blocks
            _form1Instance.Shown += async (s, e) =>
            {
                // Silent install path — runs before standard check
                if (updateState.SilentInstallOnNextLaunch &&
                    !string.IsNullOrEmpty(updateState.PendingUpdateUrl))
                {
                    string pendingUrl = updateState.PendingUpdateUrl;
                    // Clear state first so a failure does not loop
                    updateState.Clear();

                    bool installed = await UpdaterService.DownloadAndApplyUpdateAsync(pendingUrl);
                    if (installed)
                        return; // App is shutting down

                    // Failure: state already cleared — fall through to normal launch
                }

                // Standard launch-time check
                if (CheckForUpdatesAtLaunch)
                {
                    _ = CheckAndShowBannerAsync(updateState);
                }
            };

            Application.Run(_form1Instance);

            // Ensure proper cleanup of FlaUI resources
            WindowHelper.Cleanup();

            // Clean up this session's spool directory on clean exit
            TryDeleteSpoolDir(SessionSpoolDir);
        }

        private static async System.Threading.Tasks.Task CheckAndShowBannerAsync(UpdateState updateState)
        {
            try
            {
                UpdateCheckResult result = await UpdaterService.CheckForUpdateAsync();
                if (result.IsUpdateAvailable && _form1Instance != null && !_form1Instance.IsDisposed)
                {
                    _form1Instance.Invoke(() =>
                        _form1Instance.ShowUpdateBanner(result.LatestVersion, result.DownloadUrl, updateState));
                }
            }
            catch
            {
                // Silently swallowed — app continues normally
            }
        }

        /// <summary>Deletes spool session folders older than 7 days.</summary>
        private static void CleanOldSpoolSessions()
        {
            try
            {
                string spoolRoot = Path.GetDirectoryName(SessionSpoolDir)!;
                if (!Directory.Exists(spoolRoot)) return;
                foreach (string dir in Directory.GetDirectories(spoolRoot))
                {
                    try
                    {
                        if (Directory.GetCreationTime(dir) < DateTime.Now.AddDays(-7))
                            Directory.Delete(dir, true);
                    }
                    catch { /* ignore individual failures */ }
                }
            }
            catch { /* ignore */ }
        }

        /// <summary>Attempts to delete a spool directory, silently ignoring failures.</summary>
        public static void TryDeleteSpoolDir(string path)
        {
            try { if (Directory.Exists(path)) Directory.Delete(path, true); }
            catch { /* ignore */ }
        }

    }
}
