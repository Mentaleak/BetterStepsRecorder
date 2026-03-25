using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetterStepsRecorder
{
    public partial class HelpPopup : Form
    {
        private const string RepoUrl = "https://github.com/Mentaleak/BetterStepsRecorder";
        private const string ReleasesUrl = RepoUrl + "/releases";

        private string _pendingDownloadUrl = string.Empty;

        public HelpPopup()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => OpenUrl(RepoUrl);
        private void linkLabelReleasesPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => OpenUrl(ReleasesUrl);
        private void button_CloseHelp_Click(object sender, EventArgs e) => Close();

        private static void OpenUrl(string url) =>
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });

        private async void HelpPopup_Load(object sender, EventArgs e)
        {
            string version = UpdaterService.CurrentVersion?.ToString() ?? "Unknown Version";
            VersionLabel.Text = $"Version: {version}";
            await RunUpdateCheckAsync();
        }

        private async Task RunUpdateCheckAsync()
        {
            labelUpdateStatus.Text = "Checking for updates…";
            buttonDownloadInstall.Visible = false;
            linkLabelReleasesPage.Visible = false;

            UpdateCheckResult result = await UpdaterService.CheckForUpdateAsync();

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                labelUpdateStatus.Text = "Could not check for updates.";
                linkLabelReleasesPage.Text = "Download from GitHub";
                linkLabelReleasesPage.Visible = true;
            }
            else if (result.IsUpdateAvailable)
            {
                labelUpdateStatus.Text = $"Version {result.LatestVersion} is available.";
                _pendingDownloadUrl = result.DownloadUrl;
                buttonDownloadInstall.Visible = true;
            }
            else
            {
                labelUpdateStatus.Text = "You are on the latest version.";
            }
        }

        private async void buttonDownloadInstall_Click(object sender, EventArgs e)
        {
            buttonDownloadInstall.Enabled = false;
            buttonDownloadInstall.Text = "Updating…";
            linkLabelReleasesPage.Visible = false;

            bool success = await UpdaterService.DownloadAndApplyUpdateAsync(_pendingDownloadUrl);

            if (!success)
            {
                labelUpdateStatus.Text = "Update failed.";
                buttonDownloadInstall.Visible = false;
                linkLabelReleasesPage.Text = "Download from GitHub";
                linkLabelReleasesPage.Visible = true;
            }
            // On success the app shuts down — we never reach here.
        }
    }
}
