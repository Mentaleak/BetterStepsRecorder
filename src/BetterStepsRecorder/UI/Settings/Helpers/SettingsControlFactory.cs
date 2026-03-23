using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings.Helpers
{
    /// <summary>
    /// Factory for creating settings UserControls based on TreeNode names.
    /// Centralizes control instantiation to reduce duplication.
    /// </summary>
    public static class SettingsControlFactory
    {
        /// <summary>
        /// Creates the appropriate UserControl for a given TreeNode name.
        /// Returns null if the node doesn't have an associated control.
        /// </summary>
        public static UserControl CreateControl(string nodeName)
        {
            return nodeName switch
            {
                "Settings_General" => new GeneralSettings(),
                "Settings_IndicatorStyle" => new IndicatorStyle(),
                "Settings_IndicatorColor" => new IndicatorColor(),
                "Settings_ScreenshotClick" => new ScreenshotClick(),
                "Settings_ScreenshotClickCropped" => new ScreenshotClickCropped(),
                "Settings_ScreenshotDrag" => new ScreenshotDrag(),
                "Settings_ScreenshotDragCropped" => new ScreenshotDragCropped(),
                "Settings_ScreenshotDragFallback" => new ScreenshotDragFallback(),
                "Settings_ExportHtml" => new ExportHtml(),
                _ => null
            };
        }
    }
}
