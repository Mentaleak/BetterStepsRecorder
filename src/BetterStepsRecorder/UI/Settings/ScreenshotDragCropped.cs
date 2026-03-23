using BetterStepsRecorder.UI.Settings.Base;

namespace BetterStepsRecorder.UI.Settings
{
    /// <summary>
    /// Wrapper for drag cropped padding settings.
    /// Uses the generic PaddingSettingsControl to reduce duplication.
    /// </summary>
    public partial class ScreenshotDragCropped : PaddingSettingsControl
    {
        public ScreenshotDragCropped() 
            : base(
                getter: () => RecordingSettings.Load().DragCroppedPadding,
                setter: value => Program.DragCroppedPadding = value,
                labelText: "Padding (pixels):")
        {
        }
    }
}
