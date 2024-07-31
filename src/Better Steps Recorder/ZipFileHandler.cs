using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace Better_Steps_Recorder
{


    public class ZipFileHandler
    {
        private string zipFilePath;

        public ZipFileHandler(string zipFilePath)
        {
            this.zipFilePath = zipFilePath;
        }

        public void SaveToZip(List<RecordEvent> events)
        {
            using (var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Update))
            {
                var existingEntries = new HashSet<string>(zip.Entries.Select(e => e.FullName));

                for (int i = 0; i < events.Count; i++)
                {
                    var eventEntryName = $"events/event_{events[i].ID}.json";
                    if (existingEntries.Contains(eventEntryName))
                    {
                        continue; // Skip this event if it already exists in the zip
                    }

                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        IgnoreNullValues = true,
                        WriteIndented = true // For readability
                    };

                    // Serialize the RecordEvent object to JSON
                    var eventEntry = zip.CreateEntry(eventEntryName);
                    using (var entryStream = eventEntry.Open())
                    using (var writer = new StreamWriter(entryStream))
                    {
                        string json = JsonSerializer.Serialize(events[i], options);
                        writer.Write(json);
                    }

                    // Add the new entry to the set of existing entries
                    existingEntries.Add(eventEntryName);

                    // Check for and add screenshot if not already processed
                    if (!string.IsNullOrEmpty(events[i].ScreenshotPath))
                    {
                        var screenshotEntryName = $"screenshots/{Path.GetFileName(events[i].ScreenshotPath)}";
                        if (!existingEntries.Contains(screenshotEntryName))
                        {
                            zip.CreateEntryFromFile(events[i].ScreenshotPath, screenshotEntryName);
                            existingEntries.Add(screenshotEntryName);
                        }
                    }
                }
            }
        }
    }
}
