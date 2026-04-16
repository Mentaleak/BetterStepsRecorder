using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace BetterStepsRecorder.Exporters
{
    /// <summary>
    /// Exporter for Rich Text Format (RTF) files
    /// </summary>
    public class RtfExporter : ExporterBase
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
        /// Exports the current steps recording to RTF format
        /// </summary>
        /// <param name="filePath">The full path where the RTF file should be saved</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public override bool Export(string filePath)
        {
            var cfg = BSRSettings.Current.ExportOptions.Rtf;
            return Export(filePath, cfg);
        }

        /// <summary>
        /// Exports the current steps recording to RTF format using the supplied settings
        /// </summary>
        public bool Export(string filePath, BSRSettings.RtfSettings cfg)
        {
            try
            {
                EnsureDirectoryExists(filePath);

                // Get the filename without extension to use as title
                string title = Path.GetFileNameWithoutExtension(filePath);

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

                using (RichTextBox rtfBox = new RichTextBox())
                using (var fontBody     = new Font("Segoe UI", 10))
                using (var fontTitle    = new Font("Segoe UI", 16, FontStyle.Bold))
                using (var fontMeta     = new Font("Segoe UI", 9))
                using (var fontStep     = new Font("Segoe UI", 12, FontStyle.Bold))
                using (var fontDetail   = new Font("Segoe UI", 9))
                using (var fontDetailLabel = new Font("Segoe UI", 9, FontStyle.Bold))
                using (var fontSep      = new Font("Segoe UI", 9))
                using (var fontFooter   = new Font("Segoe UI", 8))
                using (var fontLink     = new Font("Segoe UI", 8, FontStyle.Underline))
                {
                    // Set document properties
                    rtfBox.Font = fontBody;

                    // Add title using the filename
                    rtfBox.SelectionFont = fontTitle;
                    rtfBox.AppendText($"{title}\n");

                    // Add generated date
                    if (cfg.ShowGeneratedDate)
                    {
                        rtfBox.SelectionFont = fontMeta;
                        rtfBox.SelectionColor = Color.Gray;
                        rtfBox.AppendText($"Generated {generated}\n");
                        rtfBox.SelectionColor = rtfBox.ForeColor;
                    }

                    rtfBox.AppendText("\n");

                    // Add summary section
                    if (cfg.ShowSummary)
                    {
                        rtfBox.SelectionFont = fontDetailLabel;
                        rtfBox.AppendText("Summary\n");
                        rtfBox.SelectionFont = fontDetail;
                        rtfBox.AppendText($"Steps: {totalSteps}\n");
                        rtfBox.AppendText($"Started: {startStr}\n");
                        rtfBox.AppendText($"Finished: {endStr}\n");
                        rtfBox.AppendText($"Duration: {durationStr}\n");
                        rtfBox.AppendText("\n");
                    }

                    // Add each step
                    DateTime? prevTime = null;
                    foreach (var recordEvent in Program._recordEvents)
                    {
                        // Keep this step together where the renderer supports it
                        // (Word honors \keep/\keepn; other viewers may ignore)
                        InsertRtfContent(rtfBox, @"{\rtf1\ansi\pard\keepn } ");

                        // Add step header with step number
                        rtfBox.SelectionFont = fontStep;
                        rtfBox.AppendText($"Step {recordEvent.Step}: ");

                        InsertRtfContent(rtfBox, @"{\rtf1\ansi\pard\keep } ");

                        // Insert formatted step text if available, otherwise plain text
                        if (!string.IsNullOrEmpty(recordEvent._StepRtf))
                        {
                            InsertRtfContent(rtfBox, recordEvent._StepRtf);
                        }
                        else
                        {
                            rtfBox.AppendText(recordEvent._StepText ?? string.Empty);
                        }
                        rtfBox.AppendText("\n");

                        // Add timestamp
                        if (cfg.ShowStepTimestamps)
                        {
                            rtfBox.SelectionFont = fontMeta;
                            rtfBox.SelectionColor = Color.Gray;
                            string timeStr;
                            if (prevTime.HasValue)
                            {
                                TimeSpan delta = recordEvent.CreationTime - prevTime.Value;
                                timeStr = $"{prevTime.Value:HH:mm:ss} → {recordEvent.CreationTime:HH:mm:ss} (+{FormatDuration(delta)})";
                            }
                            else
                            {
                                timeStr = recordEvent.CreationTime.ToString("HH:mm:ss");
                            }
                            rtfBox.AppendText($"{timeStr}\n");
                            rtfBox.SelectionColor = rtfBox.ForeColor;
                        }
                        prevTime = recordEvent.CreationTime;

                        // Add detail strip - only if at least one detail option is on
                        if (!cfg.IsDetailStripEmpty)
                        {
                            rtfBox.AppendText("\n");
                            if (cfg.ShowAction && !string.IsNullOrWhiteSpace(recordEvent.EventType))
                            {
                                rtfBox.SelectionFont = fontDetailLabel;
                                rtfBox.AppendText("Action: ");
                                rtfBox.SelectionFont = fontDetail;
                                rtfBox.AppendText($"{recordEvent.EventType}\n");
                            }
                            if (cfg.ShowApplication && !string.IsNullOrWhiteSpace(recordEvent.ApplicationName))
                            {
                                rtfBox.SelectionFont = fontDetailLabel;
                                rtfBox.AppendText("Application: ");
                                rtfBox.SelectionFont = fontDetail;
                                rtfBox.AppendText($"{recordEvent.ApplicationName}\n");
                            }
                            if (cfg.ShowWindow && !string.IsNullOrWhiteSpace(recordEvent.WindowTitle))
                            {
                                rtfBox.SelectionFont = fontDetailLabel;
                                rtfBox.AppendText("Window: ");
                                rtfBox.SelectionFont = fontDetail;
                                rtfBox.AppendText($"{recordEvent.WindowTitle}\n");
                            }
                            if (cfg.ShowElement && !string.IsNullOrWhiteSpace(recordEvent.ElementName))
                            {
                                rtfBox.SelectionFont = fontDetailLabel;
                                rtfBox.AppendText("Element: ");
                                rtfBox.SelectionFont = fontDetail;
                                rtfBox.AppendText($"{recordEvent.ElementName}\n");
                            }
                            if (cfg.ShowElementType && !string.IsNullOrWhiteSpace(recordEvent.ElementType))
                            {
                                rtfBox.SelectionFont = fontDetailLabel;
                                rtfBox.AppendText("Element Type: ");
                                rtfBox.SelectionFont = fontDetail;
                                rtfBox.AppendText($"{recordEvent.ElementType}\n");
                            }
                            if (cfg.ShowMousePosition && (recordEvent.MouseCoordinates.X != 0 || recordEvent.MouseCoordinates.Y != 0))
                            {
                                rtfBox.SelectionFont = fontDetailLabel;
                                rtfBox.AppendText("Mouse Position: ");
                                rtfBox.SelectionFont = fontDetail;
                                rtfBox.AppendText($"{recordEvent.MouseCoordinates.X}, {recordEvent.MouseCoordinates.Y}\n");
                            }
                        }

                        // Add screenshot if available
                        if (recordEvent.HasScreenshot)
                        {
                            rtfBox.AppendText("\n");

                            using (Image img = GetRtfImage(recordEvent))
                            {
                                if (img != null)
                                {
                                    // Embed image directly via RTF to avoid clipboard errors
                                    rtfBox.Select(rtfBox.TextLength, 0);
                                    rtfBox.SelectedRtf = ImageToRtf(img);
                                    rtfBox.AppendText("\n");
                                }
                            }
                        }

                        // Page break after each step (avoids step content spanning multiple pages)
                        rtfBox.AppendText("\n");
                        InsertRtfContent(rtfBox, @"{\rtf1\ansi \page }");
                    }

                    // Add footer with link to GitHub
                    rtfBox.SelectionAlignment = HorizontalAlignment.Center;
                    rtfBox.AppendText("\n");
                    rtfBox.SelectionFont = fontFooter;
                    rtfBox.AppendText("Generated with ");

                    // Add the hyperlink text
                    rtfBox.SelectionColor = Color.Blue;
                    rtfBox.SelectionFont = fontLink;
                    rtfBox.AppendText("Better Steps Recorder");

                    // Add the URL in parentheses
                    rtfBox.SelectionFont = fontFooter;
                    rtfBox.SelectionColor = rtfBox.ForeColor;
                    rtfBox.AppendText(" (https://github.com/Mentaleak/BetterStepsRecorder)");

                    // Save the RTF file
                    rtfBox.SaveFile(filePath);
                }

                ShowExportSuccess(filePath);
                return true;
            }
            catch (Exception ex)
            {
                ShowExportError("Error exporting to RTF", ex);
                return false;
            }
        }
        
        /// <summary>
        /// Loads and scales a screenshot from a RecordEvent for embedding in RTF.
        /// </summary>
        private Image GetRtfImage(RecordEvent recordEvent)
        {
            try
            {
                byte[]? imageBytes = Program.GetScreenshotBytes(recordEvent);
                if (imageBytes == null) return null;
                using (var ms = new MemoryStream(imageBytes))
                using (var original = new Bitmap(ms))
                {
                    const int maxWidth = 800;
                    int targetWidth = Math.Min(original.Width, maxWidth);
                    int targetHeight = (int)((double)original.Height / original.Width * targetWidth);
                    return new Bitmap(original, targetWidth, targetHeight);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetRtfImage failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Converts an Image to an RTF string with an embedded PNG picture,
        /// avoiding clipboard operations entirely.
        /// </summary>
        private static string ImageToRtf(Image img)
        {
            using var ms = new MemoryStream();
            img.Save(ms, ImageFormat.Png);
            byte[] bytes = ms.ToArray();

            var hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:X2}", b);

            int widthTwips = (int)(img.Width * 1440.0 / img.HorizontalResolution);
            int heightTwips = (int)(img.Height * 1440.0 / img.VerticalResolution);

            return $@"{{\rtf1\ansi{{\pict\pngblip\picw{img.Width}\pich{img.Height}\picwgoal{widthTwips}\pichgoal{heightTwips} {hex}}}}}";
        }

        /// <summary>
        /// Inserts RTF-formatted content from a source RTF string into the target RichTextBox
        /// at the current cursor position, preserving all formatting including colors.
        /// Uses SelectedRtf instead of clipboard operations to avoid "Requested Clipboard operation did not succeed" errors.
        /// </summary>
        private static void InsertRtfContent(RichTextBox target, string sourceRtf)
        {
            // Insert at end of target using the original RTF directly;
            // the RichTextBox control handles color/font table merging automatically
            target.Select(target.TextLength, 0);
            target.SelectedRtf = sourceRtf;
        }
    }
}