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
        }

        private void LoadSettings()
        {
            var settings = BSRSettings.Current;

            rdoCropped.Checked = settings.ClickScreenshotMode == ClickScreenshotMode.Cropped;
            rdoActiveWindow.Checked = settings.ClickScreenshotMode == ClickScreenshotMode.ActiveWindow;
            rdoActiveScreen.Checked = settings.ClickScreenshotMode == ClickScreenshotMode.ActiveScreen;
            rdoAllScreens.Checked = settings.ClickScreenshotMode == ClickScreenshotMode.AllScreens;
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton { Checked: true, Tag: ClickScreenshotMode mode })
            {
                Program.ClickScreenshotMode = mode;
                BSRSettings.SaveCurrent();

                // Update the parent form's node states
                if (ParentForm is Settings settingsForm)
                {
                    settingsForm.UpdateNodeStates();
                }
            }
        }
    }
}
