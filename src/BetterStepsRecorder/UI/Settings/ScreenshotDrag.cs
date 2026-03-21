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

            rdoCropped.Checked = settings.DragScreenshotMode == DragScreenshotMode.Cropped;
            rdoActiveWindow.Checked = settings.DragScreenshotMode == DragScreenshotMode.ActiveWindow;
            rdoActiveScreen.Checked = settings.DragScreenshotMode == DragScreenshotMode.ActiveScreen;
            rdoAllScreens.Checked = settings.DragScreenshotMode == DragScreenshotMode.AllScreens;
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
            RecordingSettings.SaveCurrent();
        }
    }
}
