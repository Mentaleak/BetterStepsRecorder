using System;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class ScreenshotDragCropped : UserControl
    {
        public ScreenshotDragCropped()
        {
            InitializeComponent();
            LoadSettings();
            
            nudPadding.ValueChanged += NudPadding_ValueChanged;
        }

        private void LoadSettings()
        {
            nudPadding.Value = BSRSettings.Current.Screenshot.Drag.Cropped.Padding;
        }

        private void NudPadding_ValueChanged(object sender, EventArgs e)
        {
            BSRSettings.Current.Screenshot.Drag.Cropped.Padding = (int)nudPadding.Value;
            BSRSettings.Current.Save();
        }
    }
}
