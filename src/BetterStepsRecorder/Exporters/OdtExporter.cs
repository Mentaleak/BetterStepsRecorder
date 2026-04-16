using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO.Compression;
using System.Xml;
using BetterStepsRecorder.UI;

namespace BetterStepsRecorder.Exporters
{
    /// <summary>
    /// Exporter for OpenDocument Text (ODT) files
    /// </summary>
    public class OdtExporter : ExporterBase
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
        /// Exports the current steps recording to ODT format
        /// </summary>
        /// <param name="filePath">The full path where the ODT file should be saved</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public override bool Export(string filePath)
        {
            var cfg = BSRSettings.Current.ExportOptions.Odt;
            return Export(filePath, cfg);
        }

        /// <summary>
        /// Exports the current steps recording to ODT format using the supplied settings
        /// </summary>
        public bool Export(string filePath, BSRSettings.OdtSettings cfg)
        {
            try
            {
                EnsureDirectoryExists(filePath);

                // Create a temporary directory for ODT contents
                string tempDir = Path.Combine(Path.GetTempPath(), "BSR_ODT_" + Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                try
                {
                    // Create ODT structure
                    Directory.CreateDirectory(Path.Combine(tempDir, "META-INF"));
                    Directory.CreateDirectory(Path.Combine(tempDir, "Pictures"));

                    // Create manifest file
                    CreateManifestFile(tempDir);

                    // Save images first — returns cached dimensions so CreateContentFile doesn't decode again
                    var imageDimensions = SaveImages(tempDir);

                    // Create content files
                    CreateContentFile(tempDir, imageDimensions, cfg);
                    CreateStylesFile(tempDir);
                    CreateMetaFile(tempDir);

                    // Create mimetype file
                    File.WriteAllText(Path.Combine(tempDir, "mimetype"), "application/vnd.oasis.opendocument.text");

                    // Create the ODT file (ZIP)
                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    ZipFile.CreateFromDirectory(tempDir, filePath);

                    ShowExportSuccess(filePath);
                    return true;
                }
                finally
                {
                    // Clean up temporary directory
                    if (Directory.Exists(tempDir))
                        Directory.Delete(tempDir, true);
                }
            }
            catch (Exception ex)
            {
                ShowExportError("Error exporting to ODT", ex);
                return false;
            }
        }
        
        private void CreateManifestFile(string tempDir)
        {
            string manifestPath = Path.Combine(tempDir, "META-INF", "manifest.xml");
            
            XmlWriterSettings settings = new XmlWriterSettings { 
                Indent = true,
                IndentChars = "  "
            };
            
            using (XmlWriter writer = XmlWriter.Create(manifestPath, settings))
            {
                writer.WriteStartDocument();
                
                // Write manifest element with proper namespace declarations
                writer.WriteStartElement("manifest", "manifest", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0");
                writer.WriteAttributeString("xmlns", "manifest", null, "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0");
                
                // Add file entries
                writer.WriteStartElement("file-entry", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0");
                writer.WriteAttributeString("media-type", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0", "application/vnd.oasis.opendocument.text");
                writer.WriteAttributeString("full-path", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0", "/");
                writer.WriteEndElement();
                
                writer.WriteStartElement("file-entry", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0");
                writer.WriteAttributeString("media-type", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0", "text/xml");
                writer.WriteAttributeString("full-path", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0", "content.xml");
                writer.WriteEndElement();
                
                writer.WriteStartElement("file-entry", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0");
                writer.WriteAttributeString("media-type", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0", "text/xml");
                writer.WriteAttributeString("full-path", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0", "styles.xml");
                writer.WriteEndElement();
                
                writer.WriteStartElement("file-entry", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0");
                writer.WriteAttributeString("media-type", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0", "text/xml");
                writer.WriteAttributeString("full-path", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0", "meta.xml");
                writer.WriteEndElement();
                
                // Add image entries
                foreach (var recordEvent in Program._recordEvents)
                {
                    if (recordEvent.HasScreenshot)
                    {
                        string imageFileName = $"Pictures/step_{recordEvent.Step}_{recordEvent.ShortId}.png";

                        writer.WriteStartElement("file-entry", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0");
                        writer.WriteAttributeString("media-type", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0", "image/png");
                        writer.WriteAttributeString("full-path", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0", imageFileName);
                        writer.WriteEndElement();
                    }
                }
                
                writer.WriteEndElement(); // manifest:manifest
                writer.WriteEndDocument();
            }
        }
        
        private void CreateContentFile(string tempDir, Dictionary<Guid, Size> imageDimensions, BSRSettings.OdtSettings cfg)
        {
            string contentPath = Path.Combine(tempDir, "content.xml");

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

            // Pre-compute unique text span styles for formatted runs
            var textSpanStyles = PrecomputeTextSpanStyles();

            XmlWriterSettings settings = new XmlWriterSettings { 
                Indent = true,
                IndentChars = "  "
            };

            using (XmlWriter writer = XmlWriter.Create(contentPath, settings))
            {
                writer.WriteStartDocument();

                // Write document-content element with proper namespace declarations
                writer.WriteStartElement("office", "document-content", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
                writer.WriteAttributeString("xmlns", "office", null, "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
                writer.WriteAttributeString("xmlns", "text", null, "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                writer.WriteAttributeString("xmlns", "draw", null, "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0");
                writer.WriteAttributeString("xmlns", "svg", null, "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0");
                writer.WriteAttributeString("xmlns", "xlink", null, "http://www.w3.org/1999/xlink");
                writer.WriteAttributeString("xmlns", "style", null, "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("xmlns", "fo", null, "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0");
                writer.WriteAttributeString("xmlns", "table", null, "urn:oasis:names:tc:opendocument:xmlns:table:1.0");
                
                // Automatic styles
                writer.WriteStartElement("automatic-styles", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
                
                // Define section/paragraph styles
                writer.WriteStartElement("style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "StepKeepTogether");
                writer.WriteAttributeString("family", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "section");
                writer.WriteStartElement("section-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("keep-together", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "always");
                writer.WriteEndElement(); // style:section-properties
                writer.WriteEndElement(); // style:style

                writer.WriteStartElement("style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "Title");
                writer.WriteAttributeString("family", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "paragraph");
                writer.WriteStartElement("paragraph-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("margin-bottom", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "0.25in");
                writer.WriteEndElement(); // style:paragraph-properties
                writer.WriteStartElement("text-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("font-size", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "18pt");
                writer.WriteAttributeString("font-weight", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "bold");
                writer.WriteEndElement(); // style:text-properties
                writer.WriteEndElement(); // style:style
                
                writer.WriteStartElement("style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "StepHeader");
                writer.WriteAttributeString("family", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "paragraph");
                writer.WriteStartElement("paragraph-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("margin-top", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "0.2in");
                writer.WriteAttributeString("margin-bottom", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "0.1in");
                writer.WriteEndElement(); // style:paragraph-properties
                writer.WriteStartElement("text-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("font-size", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "14pt");
                writer.WriteAttributeString("font-weight", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "bold");
                writer.WriteEndElement(); // style:text-properties
                writer.WriteEndElement(); // style:style
                
                writer.WriteStartElement("style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "Normal");
                writer.WriteAttributeString("family", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "paragraph");
                writer.WriteStartElement("paragraph-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("margin-bottom", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "0.1in");
                writer.WriteEndElement(); // style:paragraph-properties
                writer.WriteStartElement("text-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("font-size", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "11pt");
                writer.WriteEndElement(); // style:text-properties
                writer.WriteEndElement(); // style:style
                
                writer.WriteStartElement("style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "Separator");
                writer.WriteAttributeString("family", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "paragraph");
                writer.WriteStartElement("paragraph-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("border-bottom", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "0.5pt solid #cccccc");
                writer.WriteAttributeString("padding-bottom", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "0.05in");
                writer.WriteAttributeString("margin-top", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "0.2in");
                writer.WriteAttributeString("margin-bottom", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "0.2in");
                writer.WriteEndElement(); // style:paragraph-properties
                writer.WriteEndElement(); // style:style

                // Page break style - hard break before the paragraph
                writer.WriteStartElement("style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "PageBreak");
                writer.WriteAttributeString("family", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "paragraph");
                writer.WriteStartElement("paragraph-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("break-before", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "page");
                writer.WriteEndElement(); // style:paragraph-properties
                writer.WriteEndElement(); // style:style
                
                writer.WriteStartElement("style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "Footer");
                writer.WriteAttributeString("family", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "paragraph");
                writer.WriteStartElement("paragraph-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("text-align", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "center");
                writer.WriteAttributeString("margin-top", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "0.2in");
                writer.WriteEndElement(); // style:paragraph-properties
                writer.WriteStartElement("text-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("font-size", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "9pt");
                writer.WriteAttributeString("color", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "#666666");
                writer.WriteEndElement(); // style:text-properties
                writer.WriteEndElement(); // style:style
                
                // Style for hyperlinks with blue color
                writer.WriteStartElement("style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "Hyperlink");
                writer.WriteAttributeString("family", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "text");
                writer.WriteStartElement("text-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("color", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "#0066cc");
                writer.WriteAttributeString("text-underline-style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "solid");
                writer.WriteAttributeString("text-underline-width", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "auto");
                writer.WriteAttributeString("text-underline-color", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "font-color");
                writer.WriteEndElement(); // style:text-properties
                writer.WriteEndElement(); // style:style
                
                // Frame style for images
                writer.WriteStartElement("style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "fr1");
                writer.WriteAttributeString("family", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "graphic");
                writer.WriteStartElement("graphic-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("stroke", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0", "none");
                writer.WriteAttributeString("fill", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0", "none");
                writer.WriteAttributeString("vertical-pos", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "middle");
                writer.WriteAttributeString("vertical-rel", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "paragraph");
                writer.WriteAttributeString("horizontal-pos", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "center");
                writer.WriteAttributeString("horizontal-rel", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "paragraph");
                writer.WriteAttributeString("wrap", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "none");
                writer.WriteAttributeString("padding", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "0.1in");
                writer.WriteEndElement(); // style:graphic-properties
                writer.WriteEndElement(); // style:style
                
                // Style for image paragraph
                writer.WriteStartElement("style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "ImageParagraph");
                writer.WriteAttributeString("family", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "paragraph");
                writer.WriteStartElement("paragraph-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("text-align", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "center");
                writer.WriteAttributeString("margin-top", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "0.1in");
                writer.WriteAttributeString("margin-bottom", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "0.2in");
                writer.WriteEndElement(); // style:paragraph-properties
                writer.WriteEndElement(); // style:style

                // Write pre-computed text span styles for formatted step text
                WriteTextSpanStyles(writer, textSpanStyles);

                writer.WriteEndElement(); // office:automatic-styles

                // Document content
                writer.WriteStartElement("body", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
                writer.WriteStartElement("text", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");

                // Title
                writer.WriteStartElement("p", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "Title");

                // Use the filename if available
                string title = "Steps Recording";
                if (Program.zip?.ZipFilePath != null)
                {
                    title += ": " + Path.GetFileNameWithoutExtension(Program.zip.ZipFilePath);
                }
                writer.WriteString(title);
                writer.WriteEndElement(); // text:p

                // Generated date
                if (cfg.ShowGeneratedDate)
                {
                    writer.WriteStartElement("p", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                    writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "Normal");
                    writer.WriteString($"Generated {generated}");
                    writer.WriteEndElement(); // text:p
                }

                // Summary table
                if (cfg.ShowSummary)
                {
                    writer.WriteStartElement("table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0");
                    writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:table:1.0", "SummaryTable");

                    // Define columns
                    writer.WriteStartElement("table-column", "urn:oasis:names:tc:opendocument:xmlns:table:1.0");
                    writer.WriteEndElement();
                    writer.WriteStartElement("table-column", "urn:oasis:names:tc:opendocument:xmlns:table:1.0");
                    writer.WriteEndElement();

                    // Add rows
                    WriteTableRow(writer, "Steps", totalSteps.ToString());
                    WriteTableRow(writer, "Started", startStr);
                    WriteTableRow(writer, "Finished", endStr);
                    WriteTableRow(writer, "Duration", durationStr);

                    writer.WriteEndElement(); // table:table

                    // Empty paragraph for spacing
                    writer.WriteStartElement("p", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                    writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "Normal");
                    writer.WriteString(" ");
                    writer.WriteEndElement(); // text:p
                }

                // Add each step
                DateTime? prevTime = null;
                foreach (var recordEvent in Program._recordEvents)
                {
                    // Wrap the whole step in a keep-together section (layout engine may still override)
                    writer.WriteStartElement("section", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                    writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", $"StepSection{recordEvent.Step}");
                    writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "StepKeepTogether");

                    // Step header (bold/14pt paragraph for the step number)
                    writer.WriteStartElement("p", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                    writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "StepHeader");
                    writer.WriteString($"Step {recordEvent.Step}");
                    writer.WriteEndElement(); // text:p

                    // Step text in Normal style so it doesn't inherit StepHeader's bold/14pt
                    writer.WriteStartElement("p", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                    writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "Normal");
                    if (!string.IsNullOrEmpty(recordEvent._StepRtf) && RtfFormatConverter.HasFormatting(recordEvent._StepRtf))
                    {
                        WriteFormattedRuns(writer, RtfFormatConverter.GetFormattedRuns(recordEvent._StepRtf), textSpanStyles);
                    }
                    else
                    {
                        writer.WriteString(RtfFormatConverter.SanitizeForExport(recordEvent._StepText));
                    }
                    writer.WriteEndElement(); // text:p

                    // Timestamp
                    if (cfg.ShowStepTimestamps)
                    {
                        writer.WriteStartElement("p", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                        writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "Normal");
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
                        writer.WriteString(timeStr);
                        writer.WriteEndElement(); // text:p
                    }
                    prevTime = recordEvent.CreationTime;

                    // Detail table - only rendered when at least one detail option is on
                    if (!cfg.IsDetailTableEmpty)
                    {
                        writer.WriteStartElement("table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0");
                        writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:table:1.0", $"DetailTable{recordEvent.Step}");

                        // Define columns
                        writer.WriteStartElement("table-column", "urn:oasis:names:tc:opendocument:xmlns:table:1.0");
                        writer.WriteEndElement();
                        writer.WriteStartElement("table-column", "urn:oasis:names:tc:opendocument:xmlns:table:1.0");
                        writer.WriteEndElement();

                        // Add rows
                        if (cfg.ShowAction && !string.IsNullOrWhiteSpace(recordEvent.EventType))
                            WriteTableRow(writer, "Action", recordEvent.EventType);
                        if (cfg.ShowApplication && !string.IsNullOrWhiteSpace(recordEvent.ApplicationName))
                            WriteTableRow(writer, "Application", recordEvent.ApplicationName);
                        if (cfg.ShowWindow && !string.IsNullOrWhiteSpace(recordEvent.WindowTitle))
                            WriteTableRow(writer, "Window", recordEvent.WindowTitle);
                        if (cfg.ShowElement && !string.IsNullOrWhiteSpace(recordEvent.ElementName))
                            WriteTableRow(writer, "Element", recordEvent.ElementName);
                        if (cfg.ShowElementType && !string.IsNullOrWhiteSpace(recordEvent.ElementType))
                            WriteTableRow(writer, "Element Type", recordEvent.ElementType);
                        if (cfg.ShowMousePosition && (recordEvent.MouseCoordinates.X != 0 || recordEvent.MouseCoordinates.Y != 0))
                            WriteTableRow(writer, "Mouse Position", $"{recordEvent.MouseCoordinates.X}, {recordEvent.MouseCoordinates.Y}");

                        writer.WriteEndElement(); // table:table

                        // Empty paragraph for spacing
                        writer.WriteStartElement("p", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                        writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "Normal");
                        writer.WriteString(" ");
                        writer.WriteEndElement(); // text:p
                    }

                    /* Add description text if there is any (split by line breaks)
                    if (!string.IsNullOrEmpty(recordEvent._StepText))
                    {
                        string[] lines = recordEvent._StepText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                        foreach (var line in lines)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                writer.WriteStartElement("p", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                                writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "Normal");
                                writer.WriteString(line);
                                writer.WriteEndElement(); // text:p
                            }
                        }
                    }
                    */
                    // Add screenshot if available
                    if (recordEvent.HasScreenshot)
                    {
                        string imageFileName = $"Pictures/step_{recordEvent.Step}_{recordEvent.ShortId}.png";

                        // Look up pre-computed dimensions (avoid re-decoding the image)
                        Size imageSize = imageDimensions.TryGetValue(recordEvent.ID, out var dim) ? dim : new Size(800, 600);
                        float aspectRatio = (float)imageSize.Width / imageSize.Height;
                        
                        // Calculate dimensions to fit within page while maintaining aspect ratio
                        // Assuming a max width of 6 inches for the image
                        float maxWidth = 6.0f;
                        float width = Math.Min(maxWidth, imageSize.Width / 96.0f); // Convert pixels to inches (96 DPI)
                        float height = width / aspectRatio;
                        
                        writer.WriteStartElement("p", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                        writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "ImageParagraph");
                        
                        writer.WriteStartElement("frame", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0");
                        writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0", "fr1");
                        writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0", $"Image{recordEvent.Step}");
                        writer.WriteAttributeString("anchor-type", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "paragraph");
                        writer.WriteAttributeString("width", "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0", $"{width}in");
                        writer.WriteAttributeString("height", "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0", $"{height}in");
                        writer.WriteAttributeString("z-index", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0", "0");
                        
                        writer.WriteStartElement("image", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0");
                        writer.WriteAttributeString("href", "http://www.w3.org/1999/xlink", imageFileName);
                        writer.WriteAttributeString("type", "http://www.w3.org/1999/xlink", "simple");
                        writer.WriteAttributeString("show", "http://www.w3.org/1999/xlink", "embed");
                        writer.WriteAttributeString("actuate", "http://www.w3.org/1999/xlink", "onLoad");
                        writer.WriteEndElement(); // draw:image
                        
                        writer.WriteEndElement(); // draw:frame
                        writer.WriteEndElement(); // text:p
                    }
                    
                    writer.WriteEndElement(); // text:section

                                        // Page break after each step (empty paragraph with break-before style)
                                        writer.WriteStartElement("p", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                                        writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "PageBreak");
                                        writer.WriteEndElement(); // text:p
                }

                // Footer with hyperlink
                writer.WriteStartElement("p", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "Footer");
                writer.WriteString("Generated with ");

                // Create hyperlink with blue color
                writer.WriteStartElement("a", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                writer.WriteAttributeString("href", "http://www.w3.org/1999/xlink", "https://github.com/Mentaleak/BetterStepsRecorder");
                writer.WriteAttributeString("type", "http://www.w3.org/1999/xlink", "simple");
                writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "Hyperlink");
                writer.WriteString("Better Steps Recorder");
                writer.WriteEndElement(); // text:a

                writer.WriteEndElement(); // text:p
                
                writer.WriteEndElement(); // office:text
                writer.WriteEndElement(); // office:body
                
                writer.WriteEndElement(); // office:document-content
                writer.WriteEndDocument();
            }
        }
        
        private void CreateStylesFile(string tempDir)
        {
            string stylesPath = Path.Combine(tempDir, "styles.xml");
            
            XmlWriterSettings settings = new XmlWriterSettings { 
                Indent = true,
                IndentChars = "  "
            };
            
            using (XmlWriter writer = XmlWriter.Create(stylesPath, settings))
            {
                writer.WriteStartDocument();
                
                // Write document-styles element with proper namespace declarations
                writer.WriteStartElement("office", "document-styles", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
                writer.WriteAttributeString("xmlns", "office", null, "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
                writer.WriteAttributeString("xmlns", "style", null, "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("xmlns", "fo", null, "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0");
                writer.WriteAttributeString("xmlns", "draw", null, "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0");
                
                // Styles
                writer.WriteStartElement("styles", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
                
                // Default style for paragraphs
                writer.WriteStartElement("default-style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("family", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "paragraph");
                writer.WriteStartElement("paragraph-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("line-spacing", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "120%");
                writer.WriteEndElement(); // style:paragraph-properties
                writer.WriteStartElement("text-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("font-family", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "Segoe UI");
                writer.WriteAttributeString("font-size", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "11pt");
                writer.WriteAttributeString("language", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "en");
                writer.WriteAttributeString("country", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "US");
                writer.WriteEndElement(); // style:text-properties
                writer.WriteEndElement(); // style:default-style
                
                // Style for hyperlinks
                writer.WriteStartElement("style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "Internet_20_link");
                writer.WriteAttributeString("display-name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "Internet Link");
                writer.WriteAttributeString("family", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "text");
                writer.WriteStartElement("text-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("color", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "#0000ff");
                writer.WriteAttributeString("text-underline-style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "solid");
                writer.WriteAttributeString("text-underline-width", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "auto");
                writer.WriteAttributeString("text-underline-color", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "font-color");
                writer.WriteEndElement(); // style:text-properties
                writer.WriteEndElement(); // style:style
                
                writer.WriteEndElement(); // office:styles
                
                // Page layout
                writer.WriteStartElement("automatic-styles", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
                writer.WriteStartElement("page-layout", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "pm1");
                
                writer.WriteStartElement("page-layout-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("margin-top", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "1in");
                writer.WriteAttributeString("margin-bottom", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "1in");
                writer.WriteAttributeString("margin-left", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "1in");
                writer.WriteAttributeString("margin-right", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "1in");
                writer.WriteAttributeString("page-width", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "8.5in");
                writer.WriteAttributeString("page-height", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "11in");
                writer.WriteEndElement(); // style:page-layout-properties
                
                writer.WriteEndElement(); // style:page-layout
                writer.WriteEndElement(); // office:automatic-styles
                
                // Master styles
                writer.WriteStartElement("master-styles", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
                writer.WriteStartElement("master-page", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "Standard");
                writer.WriteAttributeString("page-layout-name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "pm1");
                writer.WriteEndElement(); // style:master-page
                writer.WriteEndElement(); // office:master-styles
                
                writer.WriteEndElement(); // office:document-styles
                writer.WriteEndDocument();
            }
        }
        
        private void CreateMetaFile(string tempDir)
        {
            string metaPath = Path.Combine(tempDir, "meta.xml");
            
            XmlWriterSettings settings = new XmlWriterSettings { 
                Indent = true,
                IndentChars = "  "
            };
            
            using (XmlWriter writer = XmlWriter.Create(metaPath, settings))
            {
                writer.WriteStartDocument();
                
                // Write document-meta element with proper namespace declarations
                writer.WriteStartElement("office", "document-meta", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
                writer.WriteAttributeString("xmlns", "office", null, "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
                writer.WriteAttributeString("xmlns", "dc", null, "http://purl.org/dc/elements/1.1/");
                writer.WriteAttributeString("xmlns", "meta", null, "urn:oasis:names:tc:opendocument:xmlns:meta:1.0");
                
                writer.WriteStartElement("meta", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
                
                // Title
                string title = "Steps Recording";
                if (Program.zip?.ZipFilePath != null)
                {
                    title += ": " + Path.GetFileNameWithoutExtension(Program.zip.ZipFilePath);
                }
                
                writer.WriteStartElement("title", "http://purl.org/dc/elements/1.1/");
                writer.WriteString(title);
                writer.WriteEndElement(); // dc:title
                
                // Creator
                writer.WriteStartElement("creator", "http://purl.org/dc/elements/1.1/");
                writer.WriteString("Better Steps Recorder");
                writer.WriteEndElement(); // dc:creator
                
                // Date
                writer.WriteStartElement("date", "http://purl.org/dc/elements/1.1/");
                writer.WriteString(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                writer.WriteEndElement(); // dc:date
                
                // Generator
                writer.WriteStartElement("generator", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0");
                writer.WriteString("Better Steps Recorder");
                writer.WriteEndElement(); // meta:generator
                
                writer.WriteEndElement(); // office:meta
                
                writer.WriteEndElement(); // office:document-meta
                writer.WriteEndDocument();
            }
        }
        
        private Dictionary<Guid, Size> SaveImages(string tempDir)
        {
            string imagesFolder = Path.Combine(tempDir, "Pictures");
            var dimensions = new Dictionary<Guid, Size>();

            foreach (var recordEvent in Program._recordEvents)
            {
                if (recordEvent.HasScreenshot)
                {
                    string imageFileName = $"step_{recordEvent.Step}_{recordEvent.ShortId}.png";
                    string imageFilePath = Path.Combine(imagesFolder, imageFileName);

                    try
                    {
                        byte[]? imageBytes = Program.GetScreenshotBytes(recordEvent);
                        if (imageBytes != null)
                        {
                            using (var ms = new MemoryStream(imageBytes))
                            using (var image = new Bitmap(ms))
                            {
                                dimensions[recordEvent.ID] = new Size(image.Width, image.Height);
                                image.Save(imageFilePath, ImageFormat.Png);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to save image: {ex.Message}");
                    }
                }
            }

            return dimensions;
        }
        
        private Size GetImageDimensions(string base64String)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                using var ms = new MemoryStream(imageBytes);
                using (var image = Image.FromStream(ms))
                {
                    return new Size(image.Width, image.Height);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to get image dimensions: {ex.Message}");
                return new Size(800, 600); // Default size if we can't determine actual size
            }
        }

        /// <summary>
        /// Represents a unique text formatting combination for ODF style generation.
        /// </summary>
        private record struct TextStyleKey(bool Bold, bool Italic, bool Underline, bool Strikeout, int ForeColorArgb, int BackColorArgb);

        /// <summary>
        /// Pre-computes unique text styles needed for formatted runs across all events.
        /// Returns a dictionary mapping TextStyleKey to automatic style name.
        /// </summary>
        private static Dictionary<TextStyleKey, string> PrecomputeTextSpanStyles()
        {
            var styles = new Dictionary<TextStyleKey, string>();
            int counter = 0;

            foreach (var recordEvent in Program._recordEvents)
            {
                if (string.IsNullOrEmpty(recordEvent._StepRtf) || !RtfFormatConverter.HasFormatting(recordEvent._StepRtf))
                    continue;

                var runs = RtfFormatConverter.GetFormattedRuns(recordEvent._StepRtf);
                foreach (var run in runs)
                {
                    if (!RunNeedsStyle(run)) continue;

                    var key = MakeStyleKey(run);
                    if (!styles.ContainsKey(key))
                    {
                        styles[key] = $"T{++counter}";
                    }
                }
            }

            return styles;
        }

        private static bool RunNeedsStyle(FormattedRun run)
        {
            return run.Bold || run.Italic || run.Underline || run.Strikeout
                || (!run.ForeColor.IsEmpty && run.ForeColor != Color.Black && run.ForeColor != SystemColors.WindowText)
                || (!run.BackColor.IsEmpty && run.BackColor != Color.White && run.BackColor != SystemColors.Window);
        }

        private static TextStyleKey MakeStyleKey(FormattedRun run)
        {
            int fc = (!run.ForeColor.IsEmpty && run.ForeColor != Color.Black && run.ForeColor != SystemColors.WindowText)
                ? run.ForeColor.ToArgb() : 0;
            int bc = (!run.BackColor.IsEmpty && run.BackColor != Color.White && run.BackColor != SystemColors.Window)
                ? run.BackColor.ToArgb() : 0;
            return new TextStyleKey(run.Bold, run.Italic, run.Underline, run.Strikeout, fc, bc);
        }

        /// <summary>
        /// Writes automatic text span style definitions into the ODF automatic-styles section.
        /// </summary>
        private static void WriteTextSpanStyles(XmlWriter writer, Dictionary<TextStyleKey, string> styles)
        {
            foreach (var (key, styleName) in styles)
            {
                writer.WriteStartElement("style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
                writer.WriteAttributeString("name", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", styleName);
                writer.WriteAttributeString("family", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "text");

                writer.WriteStartElement("text-properties", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");

                if (key.Bold)
                    writer.WriteAttributeString("font-weight", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "bold");
                if (key.Italic)
                    writer.WriteAttributeString("font-style", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", "italic");
                if (key.Underline)
                {
                    writer.WriteAttributeString("text-underline-style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "solid");
                    writer.WriteAttributeString("text-underline-width", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "auto");
                    writer.WriteAttributeString("text-underline-color", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "font-color");
                }
                if (key.Strikeout)
                    writer.WriteAttributeString("text-line-through-style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0", "solid");
                if (key.ForeColorArgb != 0)
                {
                    var c = Color.FromArgb(key.ForeColorArgb);
                    writer.WriteAttributeString("color", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", $"#{c.R:X2}{c.G:X2}{c.B:X2}");
                }
                if (key.BackColorArgb != 0)
                {
                    var c = Color.FromArgb(key.BackColorArgb);
                    writer.WriteAttributeString("background-color", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0", $"#{c.R:X2}{c.G:X2}{c.B:X2}");
                }

                writer.WriteEndElement(); // style:text-properties
                writer.WriteEndElement(); // style:style
            }
        }

        /// <summary>
        /// Writes formatted text runs as ODF text:span elements referencing pre-computed automatic styles.
        /// Newlines are converted to text:line-break elements for valid ODF output.
        /// </summary>
        private static void WriteFormattedRuns(XmlWriter writer, FormattedRun[] runs, Dictionary<TextStyleKey, string> styles)
        {
            foreach (var run in runs)
            {
                if (RunNeedsStyle(run))
                {
                    var key = MakeStyleKey(run);
                    if (styles.TryGetValue(key, out var styleName))
                    {
                        writer.WriteStartElement("span", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                        writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", styleName);
                        WriteOdfText(writer, run.Text);
                        writer.WriteEndElement(); // text:span
                    }
                    else
                    {
                        WriteOdfText(writer, run.Text);
                    }
                }
                else
                {
                    WriteOdfText(writer, run.Text);
                }
            }
        }

        /// <summary>
        /// Writes text to ODF, converting newlines to text:line-break elements.
        /// </summary>
        private static void WriteOdfText(XmlWriter writer, string text)
        {
            var parts = text.Split('\n');
            for (int i = 0; i < parts.Length; i++)
            {
                if (i > 0)
                    writer.WriteStartElement("line-break", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                if (i > 0)
                    writer.WriteEndElement(); // text:line-break
                if (parts[i].Length > 0)
                    writer.WriteString(parts[i]);
            }
        }

        private void WriteTableRow(XmlWriter writer, string label, string value)
        {
            writer.WriteStartElement("table-row", "urn:oasis:names:tc:opendocument:xmlns:table:1.0");

            // First cell (label)
            writer.WriteStartElement("table-cell", "urn:oasis:names:tc:opendocument:xmlns:table:1.0");
            writer.WriteStartElement("p", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
            writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "Normal");
            writer.WriteString(label);
            writer.WriteEndElement(); // text:p
            writer.WriteEndElement(); // table:table-cell

            // Second cell (value)
            writer.WriteStartElement("table-cell", "urn:oasis:names:tc:opendocument:xmlns:table:1.0");
            writer.WriteStartElement("p", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
            writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "Normal");
            writer.WriteString(value);
            writer.WriteEndElement(); // text:p
            writer.WriteEndElement(); // table:table-cell

            writer.WriteEndElement(); // table:table-row
        }

    }
}