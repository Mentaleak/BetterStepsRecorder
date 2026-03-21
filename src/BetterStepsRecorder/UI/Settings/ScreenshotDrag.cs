using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class ScreenshotDrag : UserControl
    {
        public ScreenshotDrag()
        {
            InitializeComponent();

            // Populate fallback mode combo box
            cmbFallbackMode.Items.Add("Cropped");
            cmbFallbackMode.Items.Add("Active screen");
            cmbFallbackMode.Items.Add("All screens");

            LoadSettings();

            // Auto-save when selection changes
            rdoCropped.CheckedChanged += RadioButton_CheckedChanged;
            rdoActiveWindow.CheckedChanged += RadioButton_CheckedChanged;
            rdoActiveScreen.CheckedChanged += RadioButton_CheckedChanged;
            rdoAllScreens.CheckedChanged += RadioButton_CheckedChanged;
            cmbFallbackMode.SelectedIndexChanged += ComboBox_SelectedIndexChanged;

            // Show/hide fallback controls based on mode
            UpdateFallbackVisibility();
        }

        private void LoadSettings()
        {
            var settings = RecordingSettings.Load();

            rdoCropped.Checked = settings.DragScreenshotMode == DragScreenshotMode.Cropped;
            rdoActiveWindow.Checked = settings.DragScreenshotMode == DragScreenshotMode.ActiveWindow;
            rdoActiveScreen.Checked = settings.DragScreenshotMode == DragScreenshotMode.ActiveScreen;
            rdoAllScreens.Checked = settings.DragScreenshotMode == DragScreenshotMode.AllScreens;

            // Load fallback mode
            switch (settings.DragFallbackMode)
            {
                case DragScreenshotMode.ActiveScreen:
                    cmbFallbackMode.SelectedIndex = 1;
                    break;
                case DragScreenshotMode.AllScreens:
                    cmbFallbackMode.SelectedIndex = 2;
                    break;
                default:
                    cmbFallbackMode.SelectedIndex = 0; // Cropped
                    break;
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Only save when a radio button is checked (not when unchecked)
            if (sender is RadioButton rb && rb.Checked)
            {
                UpdateFallbackVisibility();
                SaveSettings();
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void UpdateFallbackVisibility()
        {
            // Show fallback controls only when ActiveWindow is selected
            bool showFallback = rdoActiveWindow.Checked;
            lblFallback.Visible = showFallback;
            cmbFallbackMode.Visible = showFallback;
        }

        private void SaveSettings()
        {
            DragScreenshotMode selectedMode;

            if (rdoAllScreens.Checked)
                selectedMode = DragScreenshotMode.AllScreens;
            else if (rdoActiveScreen.Checked)
                selectedMode = DragScreenshotMode.ActiveScreen;
            else if (rdoActiveWindow.Checked)
                selectedMode = DragScreenshotMode.ActiveWindow;
            else
                selectedMode = DragScreenshotMode.Cropped;

            Program.DragScreenshotMode = selectedMode;

            // Save fallback mode
            DragScreenshotMode fallbackMode;
            switch (cmbFallbackMode.SelectedIndex)
            {
                case 1:
                    fallbackMode = DragScreenshotMode.ActiveScreen;
                    break;
                case 2:
                    fallbackMode = DragScreenshotMode.AllScreens;
                    break;
                default:
                    fallbackMode = DragScreenshotMode.Cropped;
                    break;
            }
            Program.DragFallbackMode = fallbackMode;

            RecordingSettings.SaveCurrent();
        }
    }
}
