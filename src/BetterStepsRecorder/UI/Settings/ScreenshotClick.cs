using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class ScreenshotClick : UserControl
    {
        public ScreenshotClick()
        {
            InitializeComponent();
            LoadSettings();

            // Auto-save when selection changes
            rdoCropped.CheckedChanged += RadioButton_CheckedChanged;
            rdoActiveWindow.CheckedChanged += RadioButton_CheckedChanged;
            rdoActiveScreen.CheckedChanged += RadioButton_CheckedChanged;
            rdoAllScreens.CheckedChanged += RadioButton_CheckedChanged;
        }

        private void LoadSettings()
        {
            var settings = RecordingSettings.Load();

            rdoCropped.Checked = settings.ClickScreenshotMode == ClickScreenshotMode.Cropped;
            rdoActiveWindow.Checked = settings.ClickScreenshotMode == ClickScreenshotMode.ActiveWindow;
            rdoActiveScreen.Checked = settings.ClickScreenshotMode == ClickScreenshotMode.ActiveScreen;
            rdoAllScreens.Checked = settings.ClickScreenshotMode == ClickScreenshotMode.AllScreens;
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Only save when a radio button is checked (not when unchecked)
            if (sender is RadioButton rb && rb.Checked)
            {
                SaveSettings();

                // Update the parent form's node states
                if (ParentForm is Settings settingsForm)
                {
                    settingsForm.UpdateNodeStates();
                }
            }
        }

        private void SaveSettings()
        {
            ClickScreenshotMode selectedMode;

            if (rdoAllScreens.Checked)
                selectedMode = ClickScreenshotMode.AllScreens;
            else if (rdoActiveScreen.Checked)
                selectedMode = ClickScreenshotMode.ActiveScreen;
            else if (rdoActiveWindow.Checked)
                selectedMode = ClickScreenshotMode.ActiveWindow;
            else
                selectedMode = ClickScreenshotMode.Cropped;

            Program.ClickScreenshotMode = selectedMode;
            RecordingSettings.SaveCurrent();
        }
    }
}
