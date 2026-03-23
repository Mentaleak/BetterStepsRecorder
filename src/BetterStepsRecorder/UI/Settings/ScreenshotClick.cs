using BetterStepsRecorder.UI.Settings.Base;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class ScreenshotClick : ScreenshotModeSelector
    {
        public ScreenshotClick()
        {
            InitializeComponent();
            InitializeBase();
        }

        protected override void LoadSettings()
        {
            var settings = RecordingSettings.Load();
            SetSelectedModeIndex((int)settings.ClickScreenshotMode);
        }

        protected override void SaveSettings()
        {
            Program.ClickScreenshotMode = (ClickScreenshotMode)GetSelectedModeIndex();
        }
    }
}
