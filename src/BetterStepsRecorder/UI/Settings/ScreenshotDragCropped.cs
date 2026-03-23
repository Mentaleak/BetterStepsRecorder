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
            var settings = RecordingSettings.Load();
            nudPadding.Value = settings.DragCroppedPadding;
        }

        private void NudPadding_ValueChanged(object sender, EventArgs e)
        {
            Program.DragCroppedPadding = (int)nudPadding.Value;
            RecordingSettings.SaveCurrent();
        }
    }
}
