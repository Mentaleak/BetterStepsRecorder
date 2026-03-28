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
            var settings = BSRSettings.Current;

            rdoCropped.Checked = settings.Screenshot.Drag.Mode == DragScreenshotMode.Cropped;
            rdoActiveWindow.Checked = settings.Screenshot.Drag.Mode == DragScreenshotMode.ActiveWindow;
            rdoActiveScreen.Checked = settings.Screenshot.Drag.Mode == DragScreenshotMode.ActiveScreen;
            rdoAllScreens.Checked = settings.Screenshot.Drag.Mode == DragScreenshotMode.AllScreens;
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton { Checked: true, Tag: DragScreenshotMode mode })
            {
                BSRSettings.Current.Screenshot.Drag.Mode = mode;
                BSRSettings.Current.Save();

                // Update the parent form's node states
                if (ParentForm is Settings settingsForm)
                {
                    settingsForm.UpdateNodeStates();
                }
            }
        }
    }
}
