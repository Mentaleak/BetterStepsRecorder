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
        /// <summary>
        /// Exports the current steps recording to RTF format
        /// </summary>
        /// <param name="filePath">The full path where the RTF file should be saved</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public override bool Export(string filePath)
        {
            try
            {
                EnsureDirectoryExists(filePath);
                
                // Get the filename without extension to use as title
                string title = Path.GetFileNameWithoutExtension(filePath);
                
                using (RichTextBox rtfBox = new RichTextBox())
                {
                    // Set document properties
                    rtfBox.Font = new Font("Segoe UI", 10);
                    
                    // Add title using the filename
                    rtfBox.SelectionFont = new Font("Segoe UI", 16, FontStyle.Bold);
                    rtfBox.AppendText($"{title}\n\n");
                    
                    // Add each step
                    foreach (var recordEvent in Program._recordEvents)
                    {
                        // Add step header
                        rtfBox.SelectionFont = new Font("Segoe UI", 12, FontStyle.Bold);
                        rtfBox.AppendText($"Step {recordEvent.Step}: {recordEvent._StepText}\n");

                        /* Add element details if available
                        if (!string.IsNullOrEmpty(recordEvent.ElementName))
                        {
                            rtfBox.SelectionFont = new Font("Segoe UI", 9);
                            rtfBox.AppendText($"Element: {recordEvent.ElementName}\n");
                            
                            // Get automation ID if available
                            string automationId = RecordEvent.GetAutomationId(recordEvent.UIElement);
                            if (!string.IsNullOrEmpty(automationId))
                            {
                                rtfBox.AppendText($"Automation ID: {automationId}\n");
                            }
                        }
                        */
                        
                        // Add screenshot if available
                        if (!string.IsNullOrEmpty(recordEvent.Screenshotb64))
                        {
                            rtfBox.AppendText("\n");

                            // Convert base64 to image and insert into RTF
                            Image img = GetRtfImage(recordEvent.Screenshotb64);
                            if (img != null)
                            {
                                Clipboard.SetImage(img);
                                rtfBox.Paste();
                                rtfBox.AppendText("\n");
                            }
                        }
                        
                        // Add separator between steps
                        rtfBox.SelectionFont = new Font("Segoe UI", 9);
                        rtfBox.AppendText("\n----------------------------\n\n");
                    }
                    
                    // Add footer with link to GitHub
                    rtfBox.SelectionAlignment = HorizontalAlignment.Center;
                    rtfBox.AppendText("\n");
                    rtfBox.SelectionFont = new Font("Segoe UI", 8);
                    rtfBox.AppendText("Generated with ");
                    
                    // Add the hyperlink text
                    rtfBox.SelectionColor = Color.Blue;
                    rtfBox.SelectionFont = new Font("Segoe UI", 8, FontStyle.Underline);
                    rtfBox.AppendText("Better Steps Recorder");
                    
                    // Add the URL in parentheses
                    rtfBox.SelectionFont = new Font("Segoe UI", 8);
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
        /// Converts a base64 encoded image string to an Image object suitable for RTF
        /// </summary>
        private Image GetRtfImage(string base64Image)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64Image);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    // Create a new image from the base64 data
                    Image originalImage = Image.FromStream(ms);
                    
                    // Resize if necessary to fit in document
                    const int maxWidth = 800;
                    if (originalImage.Width > maxWidth)
                    {
                        int newHeight = (int)((double)originalImage.Height / originalImage.Width * maxWidth);
                        Image resizedImage = new Bitmap(originalImage, maxWidth, newHeight);
                        originalImage.Dispose();
                        return resizedImage;
                    }
                    
                    return originalImage;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}