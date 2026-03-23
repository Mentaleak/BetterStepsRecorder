using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetterStepsRecorder
{
    public partial class HelpPopup : Form
    {
        private const string ReleasesUrl = "https://github.com/Mentaleak/BetterStepsRecorder/releases";

        public HelpPopup()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Mentaleak/BetterStepsRecorder",
                UseShellExecute = true
            });
        }

        private void button_CloseHelp_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version != null ? version.ToString() : "Unknown Version";
        }

        private async void HelpPopup_Load(object sender, EventArgs e)
        {
            VersionLabel.Text = $"Version: {GetVersion()}";
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
                buttonDownloadInstall.Visible = true;
                buttonDownloadInstall.Tag = result.DownloadUrl;
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

            string downloadUrl = buttonDownloadInstall.Tag as string ?? string.Empty;
            bool success = await UpdaterService.DownloadAndApplyUpdateAsync(downloadUrl);

            if (!success)
            {
                labelUpdateStatus.Text = "Update failed.";
                buttonDownloadInstall.Visible = false;
                linkLabelReleasesPage.Text = "Download from GitHub";
                linkLabelReleasesPage.Visible = true;
            }
            // On success the app shuts down — we never reach here.
        }

        private void linkLabelReleasesPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = ReleasesUrl,
                UseShellExecute = true
            });
        }
    }
}
