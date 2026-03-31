using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using BetterStepsRecorder.Core;
using BetterStepsRecorder.UI;

namespace BetterStepsRecorder
{
    internal static partial class Program
    {
        /// <summary>
        /// Loads record events from a zip file and populates the UI
        /// </summary>
        /// <param name="filePath">Path to the zip file containing record events</param>
        public static void LoadRecordEventsFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    using (ZipArchive archive = ZipFile.OpenRead(filePath))
                    {
                        var loadedEvents = new List<RecordEvent>();
                        EventCounter = 0;
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (Path.GetDirectoryName(entry.FullName) == "events" && entry.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                            {
                                using (StreamReader reader = new StreamReader(entry.Open()))
                                {
                                    string jsonContent = reader.ReadToEnd();
                                    var recordEvent = System.Text.Json.JsonSerializer.Deserialize<RecordEvent>(jsonContent);

                                    if (recordEvent != null)
                                    {
                                        // Restore operations from serialized DTOs
                                        recordEvent.RestoreFromLoad();

                                        // Handle new format: BaseScreenshotb64 contains the base image
                                        if (!string.IsNullOrEmpty(recordEvent.BaseScreenshotb64))
                                        {
                                            try
                                            {
                                                byte[] pngBytes = Convert.FromBase64String(recordEvent.BaseScreenshotb64);

                                                // Spool the base screenshot
                                                string? baseSpoolPath = SpoolBaseScreenshot(pngBytes, recordEvent.ID);
                                                if (baseSpoolPath != null)
                                                {
                                                    recordEvent.BaseScreenshotSpoolPath = baseSpoolPath;
                                                }

                                                // Also create the annotated version by applying operations
                                                if (recordEvent.ImageOperations.Count > 0)
                                                {
                                                    using (var ms = new System.IO.MemoryStream(pngBytes))
                                                    using (var baseBitmap = new System.Drawing.Bitmap(ms))
                                                    {
                                                        using (var annotatedBitmap = recordEvent.ImageOperations.ApplyOperationsToImage(baseBitmap))
                                                        using (var annotatedMs = new System.IO.MemoryStream())
                                                        {
                                                            annotatedBitmap.Save(annotatedMs, System.Drawing.Imaging.ImageFormat.Png);
                                                            string? spoolPath = SpoolScreenshot(annotatedMs.ToArray(), recordEvent.ID);
                                                            if (spoolPath != null)
                                                            {
                                                                recordEvent.ScreenshotSpoolPath = spoolPath;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    // No operations, base and annotated are the same
                                                    string? spoolPath = SpoolScreenshot(pngBytes, recordEvent.ID);
                                                    if (spoolPath != null)
                                                    {
                                                        recordEvent.ScreenshotSpoolPath = spoolPath;
                                                    }
                                                }

                                                recordEvent.BaseScreenshotb64 = null;
                                            }
                                            catch (Exception ex)
                                            {
                                                System.Diagnostics.Debug.WriteLine($"Failed to process base screenshot: {ex.Message}");
                                            }
                                        }
                                        // Handle legacy format: Screenshotb64 contains the annotated image
                                        else if (!string.IsNullOrEmpty(recordEvent.Screenshotb64))
                                        {
                                            try
                                            {
                                                byte[] pngBytes = Convert.FromBase64String(recordEvent.Screenshotb64);
                                                string? spoolPath = SpoolScreenshot(pngBytes, recordEvent.ID);
                                                if (spoolPath != null)
                                                {
                                                    recordEvent.ScreenshotSpoolPath = spoolPath;
                                                    recordEvent.Screenshotb64 = null;
                                                }
                                                // If spool fails, Screenshotb64 stays set as fallback
                                            }
                                            catch { /* leave Screenshotb64 as-is on decode error */ }
                                        }

                                        loadedEvents.Add(recordEvent);
                                        EventCounter++;
                                    }
                                }
                            }
                        }

                        // Sort the events by the Step attribute
                        loadedEvents.Sort((x, y) => x.Step.CompareTo(y.Step));

                        // Atomically replace the list so the hook thread never sees a partial state
                        lock (_recordEventsLock)
                        {
                            _recordEvents = loadedEvents;
                        }

                        // Update the UI — clear then populate
                        _form1Instance?.Invoke((Action)(() => _form1Instance.ClearListBox()));
                        foreach (var recordEvent in loadedEvents)
                        {
                            _form1Instance?.Invoke((Action)(() => _form1Instance.AddRecordEventToListBox(recordEvent)));
                        }
                    }
                }
                catch (System.Text.Json.JsonException ex)
                {
                    MessageBox.Show($"Invalid JSON format: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"File I/O error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("File does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Saves the current record events to a new zip file
        /// </summary>
        /// <param name="filePath">Path where the zip file should be saved</param>
        public static void SaveRecordEventsToNewFile(string filePath)
        {
            try
            {
                // Create a new zip file handler
                zip = new ZipFileHandler(filePath);
                
                // Save all current record events to the zip file
                zip.SaveToZip();
                
                // Show success message
                StatusManager.ShowSuccess($"File saved successfully to: {filePath}");
            }
            catch (IOException ex)
            {
                MessageBox.Show($"File I/O error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Saves the current record events to the existing zip file
        /// </summary>
        public static void SaveRecordEvents()
        {
            try
            {
                // Nothing to save if no file is open yet
                if (zip == null)
                    return;
                
                // Save all current record events to the zip file
                zip.SaveToZip();
                
                // Show success message (optional, depending on context)
                // StatusManager.ShowSuccess("File saved successfully");
            }
            catch (IOException ex)
            {
                MessageBox.Show($"File I/O error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}