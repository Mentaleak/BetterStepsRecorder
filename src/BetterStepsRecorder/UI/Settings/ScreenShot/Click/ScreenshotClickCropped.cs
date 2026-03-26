using System;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class ScreenshotClickCropped : UserControl
    {
        public ScreenshotClickCropped()
        {
            InitializeComponent();
            LoadSettings();
            
            nudPadding.ValueChanged += NudPadding_ValueChanged;
        }

        private void LoadSettings()
        {
            nudPadding.Value = BSRSettings.Current.ClickCroppedPadding;
        }

        private void NudPadding_ValueChanged(object sender, EventArgs e)
        {
            BSRSettings.Current.ClickCroppedPadding = (int)nudPadding.Value;
            BSRSettings.Current.Save();
        }
    }
}
