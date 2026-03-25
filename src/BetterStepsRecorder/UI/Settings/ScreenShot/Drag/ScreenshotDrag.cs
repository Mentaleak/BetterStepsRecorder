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
        }

        private void LoadSettings()
        {
            var settings = BSRSettings.Load();

            rdoCropped.Checked = settings.DragScreenshotMode == DragScreenshotMode.Cropped;
            rdoActiveWindow.Checked = settings.DragScreenshotMode == DragScreenshotMode.ActiveWindow;
            rdoActiveScreen.Checked = settings.DragScreenshotMode == DragScreenshotMode.ActiveScreen;
            rdoAllScreens.Checked = settings.DragScreenshotMode == DragScreenshotMode.AllScreens;
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton { Checked: true, Tag: DragScreenshotMode mode })
            {
                Program.DragScreenshotMode = mode;
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
