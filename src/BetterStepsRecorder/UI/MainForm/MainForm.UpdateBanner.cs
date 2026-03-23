using System;
using System.Drawing;
using System.Windows.Forms;

namespace BetterStepsRecorder
{
    public partial class MainForm
    {
        private Panel? _updateBannerPanel;

        /// <summary>
        /// Displays a non-blocking banner at the top of the main window informing the user
        /// that a new version is available.
        /// </summary>
        public void ShowUpdateBanner(string latestVersion, string downloadUrl, UpdateState updateState)
        {
            // Don't show twice
            if (_updateBannerPanel != null && !_updateBannerPanel.IsDisposed)
                return;

            _updateBannerPanel = new Panel
            {
                Height = 36,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(255, 240, 180),
                Padding = new Padding(6, 0, 6, 0)
            };

            var lblMsg = new Label
            {
                Text = $"Version {latestVersion} is available.",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Left,
                Padding = new Padding(0, 0, 8, 0)
            };

            var btnUpdate = new Button
            {
                Text = "Update Now",
                AutoSize = true,
                Dock = DockStyle.Left,
                UseVisualStyleBackColor = true
            };

            var btnPostpone = new Button
            {
                Text = "Postpone",
                AutoSize = true,
                Dock = DockStyle.Left,
                UseVisualStyleBackColor = true
            };

            var lblError = new Label
            {
                Text = string.Empty,
                AutoSize = true,
                ForeColor = Color.DarkRed,
                Dock = DockStyle.Left,
                Visible = false
            };

            var lnkFallback = new LinkLabel
            {
                Text = "Download from GitHub",
                AutoSize = true,
                Dock = DockStyle.Left,
                Visible = false
            };
            lnkFallback.LinkClicked += (s, e) =>
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/Mentaleak/BetterStepsRecorder/releases",
                    UseShellExecute = true
                });

            btnUpdate.Click += async (s, e) =>
            {
                btnUpdate.Enabled = false;
                btnUpdate.Text = "Updating…";
                btnPostpone.Enabled = false;
                lblError.Visible = false;
                lnkFallback.Visible = false;

                bool success = await UpdaterService.DownloadAndApplyUpdateAsync(downloadUrl);
                if (!success)
                {
                    // Show inline error — do NOT shut down
                    lblMsg.Text = "Update failed.";
                    lblError.Visible = false;
                    lnkFallback.Visible = true;
                    btnUpdate.Visible = false;
                }
                // On success the app is shutting down — we never reach here.
            };

            btnPostpone.Click += (s, e) =>
            {
                updateState.PendingUpdateVersion = latestVersion;
                updateState.PendingUpdateUrl = downloadUrl;
                updateState.SilentInstallOnNextLaunch = true;
                updateState.Save();
                DismissUpdateBanner();
            };

            // Add controls right-to-left via Dock so layout is natural left-to-right
            _updateBannerPanel.Controls.Add(lnkFallback);
            _updateBannerPanel.Controls.Add(lblError);
            _updateBannerPanel.Controls.Add(btnPostpone);
            _updateBannerPanel.Controls.Add(btnUpdate);
            _updateBannerPanel.Controls.Add(lblMsg);

            Controls.Add(_updateBannerPanel);
            _updateBannerPanel.BringToFront();
        }

        private void DismissUpdateBanner()
        {
            if (_updateBannerPanel == null || _updateBannerPanel.IsDisposed)
                return;
            Controls.Remove(_updateBannerPanel);
            _updateBannerPanel.Dispose();
            _updateBannerPanel = null;
        }
    }
}
