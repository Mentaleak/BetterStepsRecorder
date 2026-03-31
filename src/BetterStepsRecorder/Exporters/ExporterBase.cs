using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using BetterStepsRecorder.UI;

namespace BetterStepsRecorder.Exporters
{
    /// <summary>
    /// Abstract base class for all exporters with common export functionality
    /// </summary>
    public abstract class ExporterBase
    {
        // Remove the instance property - we'll use the static StatusManager directly
        // protected StatusStripManager StatusManager { get; private set; }

        /// <summary>
        /// Exports the current steps recording to the specified format
        /// </summary>
        /// <param name="filePath">The full path where the export file should be saved</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public abstract bool Export(string filePath);

        /// <summary>
        /// Saves an image from base64 string to a file (kept for backward compatibility).
        /// </summary>
        protected bool SaveImageFromBase64(string base64Image, string filePath, ImageFormat format = null)
        {
            if (string.IsNullOrEmpty(base64Image)) return false;
            try { return SaveImageBytes(Convert.FromBase64String(base64Image), filePath, format); }
            catch (Exception ex) { ReportImageError(ex); return false; }
        }

        /// <summary>
        /// Saves a screenshot from a RecordEvent to a file, rendering the final image by applying all operations.
        /// </summary>
        protected bool SaveImageFromEvent(RecordEvent recordEvent, string filePath, ImageFormat format = null)
        {
            try
            {
                // Get the rendered image with all operations applied
                using (var renderedBitmap = GetRenderedImage(recordEvent))
                {
                    if (renderedBitmap == null) return false;

                    using (var ms = new MemoryStream())
                    {
                        renderedBitmap.Save(ms, format ?? ImageFormat.Png);
                        return SaveImageBytes(ms.ToArray(), filePath, format);
                    }
                }
            }
            catch (Exception ex)
            {
                ReportImageError(ex);
                return false;
            }
        }

        /// <summary>
        /// Gets the fully rendered image for a RecordEvent by applying all operations to the base screenshot.
        /// </summary>
        /// <param name="recordEvent">The record event to render</param>
        /// <returns>A bitmap with all operations applied, or null if no screenshot is available. Caller must dispose.</returns>
        protected Bitmap? GetRenderedImage(RecordEvent recordEvent)
        {
            // First, try to get the base screenshot and apply operations
            byte[]? baseBytes = Program.GetBaseScreenshotBytes(recordEvent);
            if (baseBytes != null && recordEvent.ImageOperations.Count > 0)
            {
                try
                {
                    using (var ms = new MemoryStream(baseBytes))
                    using (var baseBitmap = new Bitmap(ms))
                    {
                        // Apply all operations to get the final rendered image
                        return recordEvent.ImageOperations.ApplyOperationsToImage(baseBitmap);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to render image with operations: {ex.Message}");
                    // Fall through to try other methods
                }
            }

            // Fall back to the already-rendered screenshot (for legacy files or when no operations)
            byte[]? bytes = Program.GetScreenshotBytes(recordEvent);
            if (bytes != null)
            {
                try
                {
                    using (var ms = new MemoryStream(bytes))
                    {
                        return new Bitmap(ms);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load screenshot: {ex.Message}");
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the rendered image as a byte array for a RecordEvent.
        /// </summary>
        protected byte[]? GetRenderedImageBytes(RecordEvent recordEvent, ImageFormat? format = null)
        {
            using (var bitmap = GetRenderedImage(recordEvent))
            {
                if (bitmap == null) return null;

                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, format ?? ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        private bool SaveImageBytes(byte[] imageBytes, string filePath, ImageFormat format = null)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(imageBytes))
                using (Image image = Image.FromStream(ms))
                {
                    image.Save(filePath, format ?? ImageFormat.Png);
                }
                return true;
            }
            catch (Exception ex) { ReportImageError(ex); return false; }
        }

        private void ReportImageError(Exception ex)
        {
            if (StatusManager.IsInitialized)
                StatusManager.ShowMessage($"Error saving image: {ex.Message}", true);
            else
                MessageBox.Show($"Error saving image: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Ensures the directory for a file path exists, creating it if necessary
        /// </summary>
        /// <param name="filePath">The full path to a file</param>
        protected void EnsureDirectoryExists(string filePath)
        {
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
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
            // Use the static StatusManager which will throw an exception if not initialized
            StatusManager.ShowSuccess($"Successfully exported to: {filePath}");
        }
    }
}