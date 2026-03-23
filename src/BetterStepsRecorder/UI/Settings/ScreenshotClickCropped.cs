using BetterStepsRecorder.UI.Settings.Base;

namespace BetterStepsRecorder.UI.Settings
{
    /// <summary>
    /// Wrapper for click cropped padding settings.
    /// Uses the generic PaddingSettingsControl to reduce duplication.
    /// </summary>
    public partial class ScreenshotClickCropped : PaddingSettingsControl
    {
        public ScreenshotClickCropped() 
            : base(
                getter: () => RecordingSettings.Load().ClickCroppedPadding,
                setter: value => Program.ClickCroppedPadding = value,
                labelText: "Padding (pixels):")
        {
        }
    }
}
