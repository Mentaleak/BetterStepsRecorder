using BetterStepsRecorder.UI.Settings.Base;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class ScreenshotDrag : ScreenshotModeSelector
    {
        public ScreenshotDrag()
        {
            InitializeComponent();
            InitializeBase();
        }

        protected override void LoadSettings()
        {
            var settings = RecordingSettings.Load();
            SetSelectedModeIndex((int)settings.DragScreenshotMode);
        }

        protected override void SaveSettings()
        {
            Program.DragScreenshotMode = (DragScreenshotMode)GetSelectedModeIndex();
        }
    }
}
