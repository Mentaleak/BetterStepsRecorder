using System.Text.Json.Serialization;

namespace BetterStepsRecorder
{
    public partial class BSRSettings
    {
        // ══════════════════════════════════════════════════════════════════════
        // General Settings
        // ══════════════════════════════════════════════════════════════════════

        public class GeneralSettings
        {
            public bool MinimizeOnStartRecording { get; set; } = Defaults.MinimizeOnStartRecording;
        }

        // ══════════════════════════════════════════════════════════════════════
        // Indicator Settings
        // ══════════════════════════════════════════════════════════════════════

        public class IndicatorSettings
        {
            public ClickIndicatorStyle Style { get; set; } = Defaults.IndicatorStyle;

            [JsonConverter(typeof(JsonTools.ArgbHexConverter))]
            public int Color { get; set; } = Defaults.IndicatorColorArgb;
        }

        // ══════════════════════════════════════════════════════════════════════
        // Screenshot Settings
        // ══════════════════════════════════════════════════════════════════════

        public class CroppedSettings
        {
            public int Padding { get; set; }
        }

        public class ClickSettings
        {
            public ClickScreenshotMode Mode { get; set; } = Defaults.ClickScreenshotMode;
            public CroppedSettings Cropped { get; set; } = new CroppedSettings { Padding = Defaults.ClickCroppedPadding };
        }

        public class DragFallbackSettings
        {
            public FallbackDragScreenshotMode Mode { get; set; } = Defaults.DragFallbackMode;
        }

        public class DragSettings
        {
            public DragScreenshotMode Mode { get; set; } = Defaults.DragScreenshotMode;
            public CroppedSettings Cropped { get; set; } = new CroppedSettings { Padding = Defaults.DragCroppedPadding };
            public DragFallbackSettings Fallback { get; set; } = new DragFallbackSettings();
        }

        public class ScreenshotSettings
        {
            public ClickSettings Click { get; set; } = new ClickSettings();
            public DragSettings Drag { get; set; } = new DragSettings();
        }

        // ══════════════════════════════════════════════════════════════════════
        // Export Settings
        // ══════════════════════════════════════════════════════════════════════

        public class HtmlSettings
        {
            public bool ShowSummary { get; set; } = Defaults.HtmlShowSummary;
            public bool ShowGeneratedDate { get; set; } = Defaults.HtmlShowGeneratedDate;
            public bool ShowStepTimestamps { get; set; } = Defaults.HtmlShowStepTimestamps;
            public bool ShowAction { get; set; } = Defaults.HtmlShowAction;
            public bool ShowApplication { get; set; } = Defaults.HtmlShowApplication;
            public bool ShowWindow { get; set; } = Defaults.HtmlShowWindow;
            public bool ShowElement { get; set; } = Defaults.HtmlShowElement;
            public bool ShowElementType { get; set; } = Defaults.HtmlShowElementType;
            public bool ShowMousePosition { get; set; } = Defaults.HtmlShowMousePosition;

            [JsonIgnore]
            public bool IsDetailStripEmpty =>
                !ShowAction && !ShowApplication && !ShowWindow &&
                !ShowElement && !ShowElementType && !ShowMousePosition;
        }

        public class ExportSettings
        {
            public HtmlSettings Html { get; set; } = new HtmlSettings();
        }
    }
}
