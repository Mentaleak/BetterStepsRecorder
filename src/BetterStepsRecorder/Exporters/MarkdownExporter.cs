using System;
using System.IO;
using System.Text;

namespace BetterStepsRecorder.Exporters
{
    /// <summary>
    /// Exporter for Markdown files (GitHub-flavored markdown)
    /// </summary>
    public class MarkdownExporter : ExporterBase
    {
        private static string FormatDuration(TimeSpan ts)
        {
            if (ts.TotalHours >= 1)
                return $"{(int)ts.TotalHours}h {ts.Minutes:D2}m {ts.Seconds:D2}s";
            if (ts.TotalMinutes >= 1)
                return $"{ts.Minutes}m {ts.Seconds:D2}s";
            return $"{ts.Seconds}s";
        }

        /// <summary>
        /// Exports the current steps recording to Markdown format
        /// </summary>
        /// <param name="filePath">The full path where the Markdown file should be saved</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public override bool Export(string filePath)
        {
            var cfg = BSRSettings.Current.ExportOptions.Markdown;
            return Export(filePath, cfg);
        }

        /// <summary>
        /// Exports the current steps recording to Markdown format using the supplied settings
        /// </summary>
        public bool Export(string filePath, BSRSettings.MarkdownSettings cfg)
        {
            try
            {
                EnsureDirectoryExists(filePath);

                // Get the filename without extension to use as title and folder name
                string title = Path.GetFileNameWithoutExtension(filePath);
                string imagesFolderName = title.Replace(" ", "_") + "_images";

                // Create images folder with the same name as the .md file (with spaces replaced) + "_images"
                string folderPath = Path.GetDirectoryName(filePath);
                string imagesFolder = Path.Combine(folderPath, imagesFolderName);
                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                int totalSteps = Program._recordEvents.Count;
                string generated = DateTime.Now.ToString("dd MMM yyyy, HH:mm");

                // Compute recording start/end/duration from event timestamps
                DateTime? recordingStart = totalSteps > 0 ? Program._recordEvents[0].CreationTime : (DateTime?)null;
                DateTime? recordingEnd = totalSteps > 0 ? Program._recordEvents[totalSteps - 1].CreationTime : (DateTime?)null;
                TimeSpan totalDuration = (recordingStart.HasValue && recordingEnd.HasValue)
                    ? recordingEnd.Value - recordingStart.Value
                    : TimeSpan.Zero;

                string startStr = recordingStart?.ToString("dd MMM yyyy, HH:mm:ss") ?? "—";
                string endStr = recordingEnd?.ToString("HH:mm:ss") ?? "—";
                string durationStr = totalSteps > 1 ? FormatDuration(totalDuration) : "—";

                // Start building the Markdown content
                StringBuilder md = new StringBuilder();

                // Title
                md.AppendLine($"# {title}");
                md.AppendLine();

                // Generated date
                if (cfg.ShowGeneratedDate)
                {
                    md.AppendLine($"*Generated {generated}*");
                    md.AppendLine();
                }

                // Summary section
                if (cfg.ShowSummary)
                {
                    md.AppendLine("## Summary");
                    md.AppendLine();
                    md.AppendLine("| Property | Value |");
                    md.AppendLine("|----------|-------|");
                    md.AppendLine($"| Steps | {totalSteps} |");
                    md.AppendLine($"| Started | {startStr} |");
                    md.AppendLine($"| Finished | {endStr} |");
                    md.AppendLine($"| Duration | {durationStr} |");
                    md.AppendLine();
                }

                // Table of Contents
                if (cfg.ShowTableOfContents && Program._recordEvents.Count > 0)
                {
                    md.AppendLine("## Table of Contents");
                    md.AppendLine();
                    foreach (var recordEvent in Program._recordEvents)
                    {
                        string stepDesc = RtfFormatConverter.SanitizeForExport(recordEvent._StepText);
                        if (stepDesc.Length > 60)
                            stepDesc = stepDesc.Substring(0, 57) + "...";
                        // Markdown anchor links use lowercase and replace spaces with hyphens
                        md.AppendLine($"- [Step {recordEvent.Step}: {stepDesc}](#step-{recordEvent.Step})");
                    }
                    md.AppendLine();
                }

                // Steps section
                md.AppendLine("## Steps");
                md.AppendLine();

                // Add each step
                DateTime? prevTime = null;
                foreach (var recordEvent in Program._recordEvents)
                {
                    // Convert step text to Markdown with formatting if RTF is available
                    string stepText;
                    if (!string.IsNullOrEmpty(recordEvent._StepRtf) && RtfFormatConverter.HasFormatting(recordEvent._StepRtf))
                    {
                        stepText = RtfFormatConverter.ToMarkdown(recordEvent._StepRtf);
                    }
                    else
                    {
                        stepText = RtfFormatConverter.SanitizeForExport(recordEvent._StepText);
                    }

                    // Step header
                    md.AppendLine($"### Step {recordEvent.Step}");
                    md.AppendLine();
                    // Only wrap in bold if the text has no explicit formatting from the WYSIWYG editor;
                    // otherwise the outer ** markers conflict with inner formatting markers
                    if (!string.IsNullOrEmpty(recordEvent._StepRtf) && RtfFormatConverter.HasFormatting(recordEvent._StepRtf))
                        md.AppendLine(stepText);
                    else
                        md.AppendLine($"**{stepText}**");
                    md.AppendLine();

                    // Timestamp
                    if (cfg.ShowStepTimestamps)
                    {
                        string timeStr;
                        if (prevTime.HasValue)
                        {
                            TimeSpan delta = recordEvent.CreationTime - prevTime.Value;
                            timeStr = $"⏱️ {prevTime.Value:HH:mm:ss} → {recordEvent.CreationTime:HH:mm:ss} (+{FormatDuration(delta)})";
                        }
                        else
                        {
                            timeStr = $"⏱️ {recordEvent.CreationTime:HH:mm:ss}";
                        }
                        md.AppendLine(timeStr);
                        md.AppendLine();
                    }
                    prevTime = recordEvent.CreationTime;

                    // Detail table - only rendered when at least one detail option is on
                    if (!cfg.IsDetailTableEmpty)
                    {
                        md.AppendLine("| Detail | Value |");
                        md.AppendLine("|--------|-------|");
                        if (cfg.ShowAction && !string.IsNullOrWhiteSpace(recordEvent.EventType))
                            md.AppendLine($"| Action | {recordEvent.EventType} |");
                        if (cfg.ShowApplication && !string.IsNullOrWhiteSpace(recordEvent.ApplicationName))
                            md.AppendLine($"| Application | {recordEvent.ApplicationName} |");
                        if (cfg.ShowWindow && !string.IsNullOrWhiteSpace(recordEvent.WindowTitle))
                            md.AppendLine($"| Window | {recordEvent.WindowTitle} |");
                        if (cfg.ShowElement && !string.IsNullOrWhiteSpace(recordEvent.ElementName))
                            md.AppendLine($"| Element | {recordEvent.ElementName} |");
                        if (cfg.ShowElementType && !string.IsNullOrWhiteSpace(recordEvent.ElementType))
                            md.AppendLine($"| Element Type | {recordEvent.ElementType} |");
                        if (cfg.ShowMousePosition && (recordEvent.MouseCoordinates.X != 0 || recordEvent.MouseCoordinates.Y != 0))
                            md.AppendLine($"| Mouse Position | {recordEvent.MouseCoordinates.X}, {recordEvent.MouseCoordinates.Y} |");
                        md.AppendLine();
                    }

                    // Screenshot
                    if (recordEvent.HasScreenshot)
                    {
                        string imageFileName = $"step_{recordEvent.Step}_{recordEvent.ShortId}.png";
                        string imageFilePath = Path.Combine(imagesFolder, imageFileName);

                        if (SaveImageFromEvent(recordEvent, imageFilePath))
                        {
                            // Use AltText if available, otherwise generate a default
                            string altText = !string.IsNullOrWhiteSpace(recordEvent.AltText) 
                                ? recordEvent.AltText 
                                : $"Step {recordEvent.Step} Screenshot";
                            md.AppendLine($"![{altText}]({imagesFolderName}/{imageFileName})");
                            md.AppendLine();
                        }
                    }
                    else
                    {
                        md.AppendLine("*No screenshot captured for this step.*");
                        md.AppendLine();
                    }

                    // Add horizontal rule between steps (except after the last step)
                    if (recordEvent.Step < totalSteps)
                    {
                        md.AppendLine("---");
                        md.AppendLine();
                    }
                }

                // Footer
                md.AppendLine();
                md.AppendLine("---");
                md.AppendLine();
                md.AppendLine("*Generated with [Better Steps Recorder](https://github.com/Mentaleak/BetterStepsRecorder)*");

                // Write the Markdown file
                using (var writer = new StreamWriter(filePath, append: false, encoding: Encoding.UTF8))
                {
                    writer.Write(md);
                }

                ShowExportSuccess(filePath);
                return true;
            }
            catch (Exception ex)
            {
                ShowExportError("Error exporting to Markdown", ex);
                return false;
            }
        }
    }
}
