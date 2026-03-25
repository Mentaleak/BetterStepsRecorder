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
            var originalValue = BSRSettings.Current.ClickCroppedPadding;
            var clampedValue = Math.Clamp(originalValue, 
                                          BSRSettings.Bounds.MinCroppedPadding, 
                                          BSRSettings.Bounds.MaxCroppedPadding);

            nudPadding.Value = clampedValue;

            // If clamping occurred, save the corrected value back to settings
            if (clampedValue != originalValue)
            {
                BSRSettings.Current.ClickCroppedPadding = clampedValue;
                BSRSettings.Current.Save();
            }
        }

        private void NudPadding_ValueChanged(object sender, EventArgs e)
        {
            Program.ClickCroppedPadding = (int)nudPadding.Value;
            BSRSettings.SaveCurrent();
        }
    }
}
