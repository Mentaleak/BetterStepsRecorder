using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text.Json;

namespace BetterStepsRecorder
{
    public class ZipFileHandler
    {
        private string? _zipFilePath;

        /// <summary>
        /// Gets the full path to the current BSR zip file
        /// </summary>
        public string ZipFilePath
        {
            get { return _zipFilePath; }
            private set { _zipFilePath = value; }
        }

        public ZipFileHandler(string zipFilePath)
        {
            this.ZipFilePath = zipFilePath;
        }

        public void SaveToZip()
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "_SAVED");

            // Snapshot the list under the lock so the hook thread can't mutate it mid-save
            List<RecordEvent> snapshot;
            lock (Program._recordEventsLock)
            {
                snapshot = new List<RecordEvent>(Program._recordEvents);
            }

            // Write to a temp file using ZipArchiveMode.Create (sequential, low memory) then
            // atomically replace the real file. This avoids ZipArchiveMode.Update which loads
            // the entire existing zip into RAM before writing anything.
            string tempPath = ZipFilePath + ".tmp";

            try
            {
                using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var zip = new ZipArchive(fs, ZipArchiveMode.Create, leaveOpen: false))
                {
                    for (int i = 0; i < snapshot.Count; i++)
                    {
                        // Update the Step based on list position
                        snapshot[i].Step = i + 1;

                        var eventEntryName = $"events/event_{snapshot[i].ID}.json";

                        // Prepare the event for saving (convert operations to DTOs)
                        snapshot[i].PrepareForSave();

                        // For new save format: Save BASE screenshot (without operations applied)
                        // Priority: BaseScreenshotSpoolPath > BaseScreenshotb64 > fall back to annotated
                        bool borrowedBase64 = false;

                        // Try to get the base screenshot first
                        if (!string.IsNullOrEmpty(snapshot[i].BaseScreenshotSpoolPath) &&
                            File.Exists(snapshot[i].BaseScreenshotSpoolPath))
                        {
                            try
                            {
                                snapshot[i].BaseScreenshotb64 = Convert.ToBase64String(
                                    File.ReadAllBytes(snapshot[i].BaseScreenshotSpoolPath));
                                // Clear the legacy Screenshotb64 since we're using the new format
                                snapshot[i].Screenshotb64 = null;
                                borrowedBase64 = true;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"SaveToZip: could not read base spool file: {ex.Message}");
                            }
                        }

                        // If no base screenshot available, fall back to annotated screenshot (for backward compatibility)
                        if (string.IsNullOrEmpty(snapshot[i].BaseScreenshotb64))
                        {
                            if (string.IsNullOrEmpty(snapshot[i].Screenshotb64) &&
                                !string.IsNullOrEmpty(snapshot[i].ScreenshotSpoolPath) &&
                                File.Exists(snapshot[i].ScreenshotSpoolPath))
                            {
                                try
                                {
                                    snapshot[i].Screenshotb64 = Convert.ToBase64String(
                                        File.ReadAllBytes(snapshot[i].ScreenshotSpoolPath));
                                    borrowedBase64 = true;
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"SaveToZip: could not read spool file: {ex.Message}");
                                }
                            }
                        }

                        var eventEntry = zip.CreateEntry(eventEntryName, CompressionLevel.Fastest);
                        using (var entryStream = eventEntry.Open())
                        using (var writer = new StreamWriter(entryStream))
                        {
                            writer.Write(JsonSerializer.Serialize(snapshot[i]));
                        }

                        // Release borrowed base64 immediately — don't hold it for the next iteration
                        if (borrowedBase64)
                        {
                            snapshot[i].BaseScreenshotb64 = null;
                            snapshot[i].Screenshotb64 = null;
                            // Nudge GC: the large base64 string just became unreachable
                            GC.Collect(2, GCCollectionMode.Optimized, blocking: false);
                        }
                    }
                }

                // Atomically replace the real file
                if (File.Exists(ZipFilePath))
                    File.Delete(ZipFilePath);
                File.Move(tempPath, ZipFilePath);
            }
            catch
            {
                // Clean up the temp file on failure so we don't leave orphans
                try { if (File.Exists(tempPath)) File.Delete(tempPath); } catch { }
                throw;
            }
        }
    }
}
