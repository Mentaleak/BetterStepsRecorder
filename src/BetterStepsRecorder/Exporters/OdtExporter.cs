using System;
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
        /// <summary>
        /// Exports the current steps recording to ODT format
        /// </summary>
        /// <param name="filePath">The full path where the ODT file should be saved</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public override bool Export(string filePath)
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
                    
                    // Create content files
                    CreateContentFile(tempDir);
                    CreateStylesFile(tempDir);
                    CreateMetaFile(tempDir);
                    
                    // Create mimetype file
                    File.WriteAllText(Path.Combine(tempDir, "mimetype"), "application/vnd.oasis.opendocument.text");
                    
                    // Save images
                    SaveImages(tempDir);
                    
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
                    if (!string.IsNullOrEmpty(recordEvent.Screenshotb64))
                    {
                        string imageFileName = $"Pictures/step_{recordEvent.Step}_{recordEvent.ID.ToString().Substring(0, 8)}.png";
                        
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
        
        private void CreateContentFile(string tempDir)
        {
            string contentPath = Path.Combine(tempDir, "content.xml");
            
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
                
                // Define paragraph styles
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
                
                // Add each step
                foreach (var recordEvent in Program._recordEvents)
                {
                    // Step header
                    writer.WriteStartElement("p", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                    writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "StepHeader");
                    writer.WriteString($"Step {recordEvent.Step}: {recordEvent._StepText}");
                    writer.WriteEndElement(); // text:p
                    
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
                    if (!string.IsNullOrEmpty(recordEvent.Screenshotb64))
                    {
                        string imageFileName = $"Pictures/step_{recordEvent.Step}_{recordEvent.ID.ToString().Substring(0, 8)}.png";
                        
                        // Get image dimensions for proper aspect ratio
                        Size imageSize = GetImageDimensions(recordEvent.Screenshotb64);
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
                    
                    // Separator between steps
                    writer.WriteStartElement("p", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                    writer.WriteAttributeString("style-name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", "Separator");
                    writer.WriteString(" ");
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
        
        private void SaveImages(string tempDir)
        {
            string imagesFolder = Path.Combine(tempDir, "Pictures");
            
            foreach (var recordEvent in Program._recordEvents)
            {
                if (!string.IsNullOrEmpty(recordEvent.Screenshotb64))
                {
                    string imageFileName = $"step_{recordEvent.Step}_{recordEvent.ID.ToString().Substring(0, 8)}.png";
                    string imageFilePath = Path.Combine(imagesFolder, imageFileName);
                    
                    try
                    {
                        // Convert base64 to image and save
                        byte[] imageBytes = Convert.FromBase64String(recordEvent.Screenshotb64);
                        using (var ms = new MemoryStream(imageBytes))
                        {
                            using (var image = Image.FromStream(ms))
                            {
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
        }
        
        private Size GetImageDimensions(string base64String)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (var ms = new MemoryStream(imageBytes))
                {
                    using (var image = Image.FromStream(ms))
                    {
                        return new Size(image.Width, image.Height);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to get image dimensions: {ex.Message}");
                return new Size(800, 600); // Default size if we can't determine actual size
            }
        }
        
        private void EnsureDirectoryExists(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        
        private void ShowExportSuccess(string filePath)
        {
            StatusManager.ShowSuccess($"Export completed successfully to:\n{filePath}");
        }
        
        private void ShowExportError(string message, Exception ex)
        {
            MessageBox.Show($"{message}: {ex.Message}", 
                "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}