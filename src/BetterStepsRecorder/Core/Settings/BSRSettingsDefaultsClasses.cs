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
            public MinimizeBehavior MinimizeOnStartRecording { get; set; } = MinimizeBehavior.MinimizeToTaskbar;
            public bool AllowRecordSelf { get; set; } = false;
        }

        // ══════════════════════════════════════════════════════════════════════
        // Indicator Settings
        // ══════════════════════════════════════════════════════════════════════

        public class IndicatorSettings
        {
            public ClickIndicatorStyle Style { get; set; } = ClickIndicatorStyle.Arrow;

            [JsonConverter(typeof(JsonTools.ArgbHexConverter))]
            public int Color { get; set; } = -65281; // Color.Magenta.ToArgb() = #FFFF00FF
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
            public ClickScreenshotMode Mode { get; set; } = ClickScreenshotMode.ActiveWindow;
            public CroppedSettings Cropped { get; set; } = new CroppedSettings { Padding = 200 };
        }

        public class DragFallbackSettings
        {
            public FallbackDragScreenshotMode Mode { get; set; } = FallbackDragScreenshotMode.Cropped;
        }

        public class DragSettings
        {
            public DragScreenshotMode Mode { get; set; } = DragScreenshotMode.ActiveWindow;
            public CroppedSettings Cropped { get; set; } = new CroppedSettings { Padding = 120 };
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
            public bool ShowSummary { get; set; } = true;
            public bool ShowGeneratedDate { get; set; } = true;
            public bool ShowStepTimestamps { get; set; } = false;
            public bool ShowAction { get; set; } = false;
            public bool ShowApplication { get; set; } = false;
            public bool ShowWindow { get; set; } = false;
            public bool ShowElement { get; set; } = false;
            public bool ShowElementType { get; set; } = false;
            public bool ShowMousePosition { get; set; } = false;

            [JsonIgnore]
            public bool IsDetailStripEmpty =>
                !ShowAction && !ShowApplication && !ShowWindow &&
                !ShowElement && !ShowElementType && !ShowMousePosition;
        }

        public class MarkdownSettings
        {
            public bool ShowSummary { get; set; } = true;
            public bool ShowGeneratedDate { get; set; } = true;
            public bool ShowStepTimestamps { get; set; } = false;
            public bool ShowAction { get; set; } = false;
            public bool ShowApplication { get; set; } = false;
            public bool ShowWindow { get; set; } = false;
            public bool ShowElement { get; set; } = false;
            public bool ShowElementType { get; set; } = false;
            public bool ShowMousePosition { get; set; } = false;

            [JsonIgnore]
            public bool IsDetailTableEmpty =>
                !ShowAction && !ShowApplication && !ShowWindow &&
                !ShowElement && !ShowElementType && !ShowMousePosition;
        }

        public class RtfSettings
        {
            public bool ShowSummary { get; set; } = true;
            public bool ShowGeneratedDate { get; set; } = true;
            public bool ShowStepTimestamps { get; set; } = false;
            public bool ShowAction { get; set; } = false;
            public bool ShowApplication { get; set; } = false;
            public bool ShowWindow { get; set; } = false;
            public bool ShowElement { get; set; } = false;
            public bool ShowElementType { get; set; } = false;
            public bool ShowMousePosition { get; set; } = false;

            [JsonIgnore]
            public bool IsDetailStripEmpty =>
                !ShowAction && !ShowApplication && !ShowWindow &&
                !ShowElement && !ShowElementType && !ShowMousePosition;
        }

        public class OdtSettings
        {
            public bool ShowSummary { get; set; } = true;
            public bool ShowGeneratedDate { get; set; } = true;
            public bool ShowStepTimestamps { get; set; } = false;
            public bool ShowAction { get; set; } = false;
            public bool ShowApplication { get; set; } = false;
            public bool ShowWindow { get; set; } = false;
            public bool ShowElement { get; set; } = false;
            public bool ShowElementType { get; set; } = false;
            public bool ShowMousePosition { get; set; } = false;

            [JsonIgnore]
            public bool IsDetailTableEmpty =>
                !ShowAction && !ShowApplication && !ShowWindow &&
                !ShowElement && !ShowElementType && !ShowMousePosition;
        }

        public class ObsidianSettings
        {
            public bool ShowSummary { get; set; } = true;
            public bool ShowGeneratedDate { get; set; } = true;
            public bool ShowStepTimestamps { get; set; } = false;
            public bool ShowAction { get; set; } = false;
            public bool ShowApplication { get; set; } = false;
            public bool ShowWindow { get; set; } = false;
            public bool ShowElement { get; set; } = false;
            public bool ShowElementType { get; set; } = false;
            public bool ShowMousePosition { get; set; } = false;

            [JsonIgnore]
            public bool IsDetailTableEmpty =>
                !ShowAction && !ShowApplication && !ShowWindow &&
                !ShowElement && !ShowElementType && !ShowMousePosition;
        }

        public class ExportSettings
        {
            public HtmlSettings Html { get; set; } = new HtmlSettings();
            public MarkdownSettings Markdown { get; set; } = new MarkdownSettings();
            public RtfSettings Rtf { get; set; } = new RtfSettings();
            public OdtSettings Odt { get; set; } = new OdtSettings();
            public ObsidianSettings Obsidian { get; set; } = new ObsidianSettings();
        }
    }





    // ══════════════════════════════════════════════════════════════════════
    // Setting Enums
    // ══════════════════════════════════════════════════════════════════════



    /// <summary>
    /// Indicator style for click visualization.
    /// </summary>
    public enum ClickIndicatorStyle
    {
        Arrow,
        Circle,
        Cursor
    }

    /// <summary>
    /// Screenshot mode for click events.
    /// </summary>
    public enum ClickScreenshotMode
    {
        Cropped,
        ActiveWindow,
        ActiveScreen,
        AllScreens
    }

    /// <summary>
    /// Screenshot mode for drag events.
    /// </summary>
    public enum DragScreenshotMode
    {
        Cropped,
        ActiveWindow,
        ActiveScreen,
        AllScreens
    }

    /// <summary>
    /// Screenshot mode for drag events.
    /// </summary>
    public enum FallbackDragScreenshotMode
    {
        Cropped,
        ActiveScreen,
        AllScreens
        // ActiveWindow is skipped since it is the primary mode that triggers fallback when it fails
    }

    /// <summary>
    /// Mode of minimize on recording start.
    /// </summary>
    public enum MinimizeBehavior
    {
        DoNotMinimize = 0,
        MinimizeToTaskbar = 1,
        MinimizeToSystemTray = 2
    }


}
