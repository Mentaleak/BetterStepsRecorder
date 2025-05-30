using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Better_Steps_Recorder.Exporters
{
    /// <summary>
    /// Abstract base class for all exporters with common export functionality
    /// </summary>
    public abstract class ExporterBase
    {
        /// <summary>
        /// Exports the current steps recording to the specified format
        /// </summary>
        /// <param name="filePath">The full path where the export file should be saved</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public abstract bool Export(string filePath);

        /// <summary>
        /// Saves an image from base64 string to a file
        /// </summary>
        /// <param name="base64Image">The base64 encoded image string</param>
        /// <param name="filePath">The path where to save the image</param>
        /// <param name="format">The image format to use</param>
        /// <returns>True if successful, false otherwise</returns>
        protected bool SaveImageFromBase64(string base64Image, string filePath, ImageFormat format = null)
        {
            try
            {
                if (string.IsNullOrEmpty(base64Image))
                    return false;

                byte[] imageBytes = Convert.FromBase64String(base64Image);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    using (Image image = Image.FromStream(ms))
                    {
                        if (format == null)
                            format = ImageFormat.Png;
                        
                        image.Save(filePath, format);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}", 
                    "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Ensures the directory for a file path exists, creating it if necessary
        /// </summary>
        /// <param name="filePath">The full path to a file</param>
        protected void EnsureDirectoryExists(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// Shows an error message for export failures
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="ex">The exception that occurred</param>
        protected void ShowExportError(string message, Exception ex = null)
        {
            string errorMessage = message;
            if (ex != null)
            {
                errorMessage += $": {ex.Message}";
            }
            
            MessageBox.Show(errorMessage, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows a success message after export
        /// </summary>
        /// <param name="filePath">The path where the file was exported</param>
        protected void ShowExportSuccess(string filePath)
        {
            MessageBox.Show($"Successfully exported to:\n{filePath}", 
                "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}