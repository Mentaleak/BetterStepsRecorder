using System;
using System.IO;
using System.Text;

namespace BetterStepsRecorder.Exporters
{
    /// <summary>
    /// Exporter for HTML files
    /// </summary>
    public class HtmlExporter : ExporterBase
    {
        private static string HtmlEncode(string value) =>
            value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");

        private static string FormatDuration(TimeSpan ts)
        {
            if (ts.TotalHours >= 1)
                return $"{(int)ts.TotalHours}h {ts.Minutes:D2}m {ts.Seconds:D2}s";
            if (ts.TotalMinutes >= 1)
                return $"{ts.Minutes}m {ts.Seconds:D2}s";
            return $"{ts.Seconds}s";
        }

        /// <summary>
        /// Exports the current steps recording to HTML format
        /// </summary>
        /// <param name="filePath">The full path where the HTML file should be saved</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public override bool Export(string filePath)
        {
            var cfg = BSRSettings.Current.ExportOptions.Html;
            return Export(filePath, cfg);
        }

        /// <summary>
        /// Exports the current steps recording to HTML format using the supplied settings
        /// </summary>
        public bool Export(string filePath, BSRSettings.HtmlSettings cfg)
        {
            try
            {
                EnsureDirectoryExists(filePath);

                // Get the filename without extension to use as title
                string title = Path.GetFileNameWithoutExtension(filePath);

                // Create images folder (match the HTML file name)
                string folderPath = Path.GetDirectoryName(filePath);
                string imagesFolderName = $"{title}_images";
                string imagesFolder = Path.Combine(folderPath, imagesFolderName);
                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                int totalSteps = Program._recordEvents.Count;
                string generated = DateTime.Now.ToString("dd MMM yyyy, HH:mm");

                // Compute recording start/end/duration from event timestamps
                DateTime? recordingStart = totalSteps > 0 ? Program._recordEvents[0].CreationTime : (DateTime?)null;
                DateTime? recordingEnd   = totalSteps > 0 ? Program._recordEvents[totalSteps - 1].CreationTime : (DateTime?)null;
                TimeSpan  totalDuration  = (recordingStart.HasValue && recordingEnd.HasValue)
                    ? recordingEnd.Value - recordingStart.Value
                    : TimeSpan.Zero;

                string startStr    = recordingStart?.ToString("dd MMM yyyy, HH:mm:ss") ?? "—";
                string endStr      = recordingEnd?.ToString("HH:mm:ss") ?? "—";
                string durationStr = totalSteps > 1 ? FormatDuration(totalDuration) : "—";

                // Start building the HTML content
                StringBuilder html = new StringBuilder();
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html lang=\"en\">");
                html.AppendLine("<head>");
                html.AppendLine("    <meta charset=\"UTF-8\">");
                html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
                html.AppendLine($"    <title>{title}</title>");
                html.AppendLine("    <style>");
                html.AppendLine("        *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }");
                html.AppendLine("        body { font-family: 'Segoe UI', system-ui, Arial, sans-serif; background: #f0f2f5; color: #1a1a2e; min-height: 100vh; }");
                html.AppendLine("        .page-header { background: linear-gradient(135deg, #1a1a2e 0%, #16213e 60%, #0f3460 100%); color: #fff; padding: 48px 40px 40px; }");
                html.AppendLine("        .page-header h1 { font-size: 2rem; font-weight: 700; letter-spacing: -0.5px; margin-bottom: 8px; }");
                html.AppendLine("        .page-header .meta { font-size: 0.85rem; opacity: 0.65; }");
                html.AppendLine("        .summary-grid { display: flex; gap: 24px; margin-top: 20px; flex-wrap: wrap; }");
                html.AppendLine("        .summary-item { background: rgba(255,255,255,0.1); border-radius: 8px; padding: 12px 20px; min-width: 140px; }");
                html.AppendLine("        .summary-item .label { font-size: 0.72rem; text-transform: uppercase; letter-spacing: 0.8px; opacity: 0.6; margin-bottom: 4px; }");
                html.AppendLine("        .summary-item .value { font-size: 1.1rem; font-weight: 600; }");
                html.AppendLine("        .progress-bar-wrap { background: rgba(255,255,255,0.15); border-radius: 4px; height: 4px; margin-top: 24px; }");
                html.AppendLine("        .progress-bar-fill { background: #e94560; border-radius: 4px; height: 4px; width: 100%; }");
                html.AppendLine("        .container { max-width: 960px; margin: 0 auto; padding: 40px 20px 60px; }");
                html.AppendLine("        .step-card { background: #fff; border-radius: 12px; box-shadow: 0 2px 8px rgba(0,0,0,0.07), 0 1px 2px rgba(0,0,0,0.04); margin-bottom: 28px; overflow: hidden; transition: box-shadow 0.2s; }");
                html.AppendLine("        .step-card:hover { box-shadow: 0 8px 24px rgba(0,0,0,0.11), 0 2px 6px rgba(0,0,0,0.06); }");
                html.AppendLine("        .step-header { display: flex; align-items: flex-start; gap: 16px; padding: 20px 24px; border-bottom: 1px solid #f0f0f0; }");
                html.AppendLine("        .step-badge { background: #e94560; color: #fff; font-size: 0.72rem; font-weight: 700; letter-spacing: 0.5px; text-transform: uppercase; border-radius: 20px; padding: 4px 12px; white-space: nowrap; flex-shrink: 0; }");
                html.AppendLine("        .step-header-text { flex: 1; }");
                html.AppendLine("        .step-title { font-size: 0.97rem; font-weight: 500; color: #1a1a2e; line-height: 1.45; }");
                html.AppendLine("        .step-time { font-size: 0.78rem; color: #888; margin-top: 4px; }");
                html.AppendLine("        .step-delta { display: inline-block; background: #f0f2f5; border-radius: 10px; padding: 1px 8px; font-size: 0.72rem; color: #555; margin-left: 8px; }");
                html.AppendLine("        .step-details { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 8px 20px; padding: 14px 24px; background: #f8f9fb; border-bottom: 1px solid #f0f0f0; font-size: 0.82rem; }");
                html.AppendLine("        .detail-item .detail-label { color: #999; font-size: 0.72rem; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 2px; }");
                html.AppendLine("        .detail-item .detail-value { color: #1a1a2e; word-break: break-word; }");
                html.AppendLine("        .step-body { padding: 20px 24px; }");
                html.AppendLine("        .step-body img { width: 100%; height: auto; border-radius: 8px; border: 1px solid #e8e8e8; display: block; cursor: zoom-in; }");
                html.AppendLine("        .no-screenshot { color: #aaa; font-size: 0.85rem; font-style: italic; padding: 8px 0; }");
                html.AppendLine("        .footer { text-align: center; color: #aaa; font-size: 0.78rem; padding-bottom: 20px; }");
                html.AppendLine("        .footer a { color: #0f3460; text-decoration: none; }");
                html.AppendLine("        .footer a:hover { text-decoration: underline; }");
                // Lightbox overlay
                html.AppendLine("        #lb-overlay { display:none; position:fixed; inset:0; background:rgba(0,0,0,0.85); z-index:9999; align-items:center; justify-content:center; cursor:zoom-out; }");
                html.AppendLine("        #lb-overlay.active { display:flex; }");
                html.AppendLine("        #lb-img { max-width:92vw; max-height:92vh; border-radius:6px; box-shadow:0 8px 40px rgba(0,0,0,0.6); }");
                html.AppendLine("    </style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");

                // Page header
                html.AppendLine("    <div class=\"page-header\">");
                html.AppendLine($"        <h1>{HtmlEncode(title)}</h1>");

                if (cfg.ShowGeneratedDate)
                    html.AppendLine($"        <div class=\"meta\">Generated {generated}</div>");

                if (cfg.ShowSummary)
                {
                    html.AppendLine("        <div class=\"summary-grid\">");
                    html.AppendLine($"            <div class=\"summary-item\"><div class=\"label\">Steps</div><div class=\"value\">{totalSteps}</div></div>");
                    html.AppendLine($"            <div class=\"summary-item\"><div class=\"label\">Started</div><div class=\"value\">{startStr}</div></div>");
                    html.AppendLine($"            <div class=\"summary-item\"><div class=\"label\">Finished</div><div class=\"value\">{endStr}</div></div>");
                    html.AppendLine($"            <div class=\"summary-item\"><div class=\"label\">Total Duration</div><div class=\"value\">{durationStr}</div></div>");
                    html.AppendLine("        </div>");
                    html.AppendLine("        <div class=\"progress-bar-wrap\"><div class=\"progress-bar-fill\"></div></div>");
                }

                html.AppendLine("    </div>");

                // Table of Contents
                if (cfg.ShowTableOfContents && Program._recordEvents.Count > 0)
                {
                    html.AppendLine("    <div class=\"toc\" style=\"background:#f8f9fa; border-radius:12px; padding:20px 24px; margin:20px auto; max-width:900px;\">");
                    html.AppendLine("        <h2 style=\"margin:0 0 12px 0; font-size:1.1rem; color:#1a1a2e;\">Table of Contents</h2>");
                    html.AppendLine("        <ul style=\"margin:0; padding-left:20px; list-style:none;\">");
                    foreach (var recordEvent in Program._recordEvents)
                    {
                        string stepDesc = HtmlEncode(RtfFormatConverter.SanitizeForExport(recordEvent._StepText));
                        if (stepDesc.Length > 80)
                            stepDesc = stepDesc.Substring(0, 77) + "...";
                        html.AppendLine($"            <li style=\"margin:6px 0;\"><a href=\"#step{recordEvent.Step}\" style=\"color:#0f3460; text-decoration:none;\">Step {recordEvent.Step}: {stepDesc}</a></li>");
                    }
                    html.AppendLine("        </ul>");
                    html.AppendLine("    </div>");
                }

                html.AppendLine("    <div class=\"container\">");

                // Add each step
                DateTime? prevTime = null;
                foreach (var recordEvent in Program._recordEvents)
                {
                    // Convert step text to HTML with formatting if RTF is available
                    string stepTextHtml;
                    if (!string.IsNullOrEmpty(recordEvent._StepRtf) && RtfFormatConverter.HasFormatting(recordEvent._StepRtf))
                    {
                        stepTextHtml = RtfFormatConverter.ToHtml(recordEvent._StepRtf);
                    }
                    else
                    {
                        stepTextHtml = HtmlEncode(RtfFormatConverter.SanitizeForExport(recordEvent._StepText)).Replace("\n", "<br>");
                    }

                    html.AppendLine($"        <div class=\"step-card\" id=\"step{recordEvent.Step}\">");
                    html.AppendLine("            <div class=\"step-header\">");
                    html.AppendLine($"                <span class=\"step-badge\">Step {recordEvent.Step}</span>");
                    html.AppendLine("                <div class=\"step-header-text\">");
                    html.AppendLine($"                    <div class=\"step-title\">{stepTextHtml}</div>");

                    if (cfg.ShowStepTimestamps)
                    {
                        string timeStr;
                        if (prevTime.HasValue)
                        {
                            TimeSpan delta = recordEvent.CreationTime - prevTime.Value;
                            timeStr = $"{prevTime.Value:HH:mm:ss} → {recordEvent.CreationTime:HH:mm:ss} <span class=\"step-delta\">({FormatDuration(delta)})</span>";
                        }
                        else
                        {
                            timeStr = recordEvent.CreationTime.ToString("HH:mm:ss");
                        }
                        html.AppendLine($"                    <div class=\"step-time\">{timeStr}</div>");
                    }

                    prevTime = recordEvent.CreationTime;

                    html.AppendLine("                </div>");
                    html.AppendLine("            </div>");

                    // Detail strip — only rendered when at least one detail option is on
                    if (!cfg.IsDetailStripEmpty)
                    {
                        html.AppendLine("            <div class=\"step-details\">");
                        if (cfg.ShowAction)      AppendDetail(html, "Action",        recordEvent.EventType);
                        if (cfg.ShowApplication) AppendDetail(html, "Application",   recordEvent.ApplicationName);
                        if (cfg.ShowWindow)      AppendDetail(html, "Window",         recordEvent.WindowTitle);
                        if (cfg.ShowElement)     AppendDetail(html, "Element",        recordEvent.ElementName);
                        if (cfg.ShowElementType) AppendDetail(html, "Element Type",   recordEvent.ElementType);
                        if (cfg.ShowMousePosition && (recordEvent.MouseCoordinates.X != 0 || recordEvent.MouseCoordinates.Y != 0))
                            AppendDetail(html, "Mouse Position", $"{recordEvent.MouseCoordinates.X}, {recordEvent.MouseCoordinates.Y}");
                        html.AppendLine("            </div>");
                    }

                    html.AppendLine("            <div class=\"step-body\">");

                    if (recordEvent.HasScreenshot)
                    {
                        string imageFileName = $"step_{recordEvent.Step}_{recordEvent.ShortId}.png";
                        string imageFilePath = Path.Combine(imagesFolder, imageFileName);

                        if (SaveImageFromEvent(recordEvent, imageFilePath))
                        {
                            html.AppendLine($"                <img src=\"{imagesFolderName}/{imageFileName}\" alt=\"Screenshot for Step {recordEvent.Step}\" onclick=\"openLb(this)\">");
                        }
                    }
                    else
                    {
                        html.AppendLine("                <span class=\"no-screenshot\">No screenshot captured for this step.</span>");
                    }

                    html.AppendLine("            </div>");
                    html.AppendLine("        </div>");
                }

                html.AppendLine("    </div>"); // .container

                // Lightbox markup
                html.AppendLine("    <div id=\"lb-overlay\" onclick=\"closeLb()\"><img id=\"lb-img\" src=\"\" alt=\"\"></div>");

                html.AppendLine("    <div class=\"footer\">");
                html.AppendLine("        Generated with <a href=\"https://github.com/Mentaleak/BetterStepsRecorder\" target=\"_blank\">Better Steps Recorder</a>");
                html.AppendLine("    </div>");

                // Lightweight lightbox script — no dependencies
                html.AppendLine("    <script>");
                html.AppendLine("        function openLb(img) { document.getElementById('lb-img').src = img.src; document.getElementById('lb-overlay').classList.add('active'); }");
                html.AppendLine("        function closeLb() { document.getElementById('lb-overlay').classList.remove('active'); }");
                html.AppendLine("        document.addEventListener('keydown', function(e) { if (e.key === 'Escape') closeLb(); });");
                html.AppendLine("    </script>");

                // Close the HTML document
                html.AppendLine("</body>");
                html.AppendLine("</html>");

                // Write the HTML file directly from the StringBuilder to avoid a full string copy
                using (var writer = new StreamWriter(filePath, append: false, encoding: Encoding.UTF8))
                {
                    writer.Write(html);
                }

                ShowExportSuccess(filePath);
                return true;
            }
            catch (Exception ex)
            {
                ShowExportError("Error exporting to HTML", ex);
                return false;
            }
        }

        private static void AppendDetail(StringBuilder html, string label, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            html.AppendLine($"                <div class=\"detail-item\"><div class=\"detail-label\">{HtmlEncode(label)}</div><div class=\"detail-value\">{HtmlEncode(value)}</div></div>");
        }
    }
}
