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
            var settings = RecordingSettings.Load();
            nudPadding.Value = settings.ClickCroppedPadding;
        }

        private void NudPadding_ValueChanged(object sender, EventArgs e)
        {
            Program.ClickCroppedPadding = (int)nudPadding.Value;
            RecordingSettings.SaveCurrent();
        }
    }
}
