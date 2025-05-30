using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace BetterStepsRecorder.Exporters
{
    /// <summary>
    /// Exporter for HTML files
    /// </summary>
    public class HtmlExporter : ExporterBase
    {
        /// <summary>
        /// Exports the current steps recording to HTML format
        /// </summary>
        /// <param name="filePath">The full path where the HTML file should be saved</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public override bool Export(string filePath)
        {
            try
            {
                EnsureDirectoryExists(filePath);
                
                // Create images folder
                string folderPath = Path.GetDirectoryName(filePath);
                string imagesFolder = Path.Combine(folderPath, "images");
                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }
                
                // Get the filename without extension to use as title
                string title = Path.GetFileNameWithoutExtension(filePath);
                
                // Start building the HTML content
                StringBuilder html = new StringBuilder();
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html lang=\"en\">");
                html.AppendLine("<head>");
                html.AppendLine("    <meta charset=\"UTF-8\">");
                html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
                html.AppendLine($"    <title>{title}</title>");
                html.AppendLine("    <style>");
                html.AppendLine("        body { font-family: 'Segoe UI', Arial, sans-serif; margin: 20px; }");
                html.AppendLine("        h1 { color: #2c3e50; }");
                html.AppendLine("        h2 { color: #3498db; margin-top: 30px; }");
                html.AppendLine("        .element-info { margin: 10px 0; font-size: 0.95em; }");
                html.AppendLine("        img { max-width: 100%; border: 1px solid #ddd; margin: 15px 0; }");
                html.AppendLine("        .separator { border-bottom: 1px solid #eee; margin: 20px 0; }");
                html.AppendLine("        .footer { color: #7f8c8d; font-size: 0.8em; margin-top: 30px; text-align: center; }");
                html.AppendLine("        .footer a { color: #3498db; text-decoration: none; }");
                html.AppendLine("        .footer a:hover { text-decoration: underline; }");
                html.AppendLine("    </style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");
                html.AppendLine($"    <h1>{title}</h1>");
                
                // Add each step
                foreach (var recordEvent in Program._recordEvents)
                {
                    html.AppendLine($"    <h2>Step {recordEvent.Step}: {recordEvent._StepText}</h2>");

                    /* Add element details if available
                    if (!string.IsNullOrEmpty(recordEvent.ElementName))
                    {
                        html.AppendLine("    <div class=\"element-info\">");
                        html.AppendLine($"        <p>Element: {recordEvent.ElementName}</p>");
                        
                        // Get automation ID if available
                        string automationId = RecordEvent.GetAutomationId(recordEvent.UIElement);
                        if (!string.IsNullOrEmpty(automationId))
                        {
                            html.AppendLine($"        <p>Automation ID: {automationId}</p>");
                        }
                        html.AppendLine("    </div>");
                    }
                    */
                    
                    // Add screenshot if available
                    if (!string.IsNullOrEmpty(recordEvent.Screenshotb64))
                    {
                        string imageFileName = $"step_{recordEvent.Step}_{recordEvent.ID.ToString().Substring(0, 8)}.png";
                        string imageFilePath = Path.Combine(imagesFolder, imageFileName);

                        // Save the image
                        if (SaveImageFromBase64(recordEvent.Screenshotb64, imageFilePath))
                        {
                            html.AppendLine($"    <img src=\"images/{imageFileName}\" alt=\"Screenshot for Step {recordEvent.Step}\">");
                        }
                    }
                    
                    // Add separator between steps
                    html.AppendLine("    <div class=\"separator\"></div>");
                }
                
                // Add footer with link to GitHub
                html.AppendLine("    <div class=\"footer\">");
                html.AppendLine("        Generated with <a href=\"https://github.com/Mentaleak/BetterStepsRecorder\" target=\"_blank\">Better Steps Recorder</a>");
                html.AppendLine("    </div>");
                
                // Close the HTML document
                html.AppendLine("</body>");
                html.AppendLine("</html>");
                
                // Write the HTML file
                File.WriteAllText(filePath, html.ToString());
                
                ShowExportSuccess(filePath);
                return true;
            }
            catch (Exception ex)
            {
                ShowExportError("Error exporting to HTML", ex);
                return false;
            }
        }
    }
}