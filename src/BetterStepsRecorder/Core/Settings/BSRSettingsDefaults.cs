using System.Drawing;

namespace BetterStepsRecorder
{
    public partial class BSRSettings
    {
        /// <summary>
        /// Default values for all settings, organized to match the TreeView hierarchy.
        /// </summary>
        public static class Defaults
        {
            // ══════════════════════════════════════════════════════════════════
            // General
            // ══════════════════════════════════════════════════════════════════

            public static readonly bool MinimizeOnStartRecording = true;

            // ══════════════════════════════════════════════════════════════════
            // Indicator
            // ══════════════════════════════════════════════════════════════════

            public static readonly ClickIndicatorStyle IndicatorStyle = ClickIndicatorStyle.Arrow;
            public static readonly Color IndicatorColor = Color.Magenta;
            public static readonly int IndicatorColorArgb = Color.Magenta.ToArgb();

            // ══════════════════════════════════════════════════════════════════
            // Screenshot → Click
            // ══════════════════════════════════════════════════════════════════

            public static readonly ClickScreenshotMode ClickScreenshotMode = ClickScreenshotMode.ActiveWindow;
            public static readonly int ClickCroppedPadding = 200;

            // ══════════════════════════════════════════════════════════════════
            // Screenshot → Drag
            // ══════════════════════════════════════════════════════════════════

            public static readonly DragScreenshotMode DragScreenshotMode = DragScreenshotMode.ActiveWindow;
            public static readonly int DragCroppedPadding = 120;
            public static readonly FallbackDragScreenshotMode DragFallbackMode = FallbackDragScreenshotMode.Cropped;

            // ══════════════════════════════════════════════════════════════════
            // Export → HTML
            // ══════════════════════════════════════════════════════════════════

            public static readonly bool HtmlShowSummary = true;
            public static readonly bool HtmlShowGeneratedDate = true;
            public static readonly bool HtmlShowStepTimestamps = false;
            public static readonly bool HtmlShowAction = false;
            public static readonly bool HtmlShowApplication = false;
            public static readonly bool HtmlShowWindow = false;
            public static readonly bool HtmlShowElement = false;
            public static readonly bool HtmlShowElementType = false;
            public static readonly bool HtmlShowMousePosition = false;
        }
    }
}
