using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
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
            using (var zip = ZipFile.Open(ZipFilePath, ZipArchiveMode.Update))
            {
                var existingEntries = new HashSet<string>(zip.Entries.Select(e => e.FullName));
                var validEntries = new HashSet<string>();

                for (int i = 0; i < Program._recordEvents.Count; i++)
                {
                    // Update the Step based on the list position
                    Program._recordEvents[i].Step = i + 1;

                    var eventEntryName = $"events/event_{Program._recordEvents[i].ID}.json";

                    // Check if the entry already exists and remove it
                    var existingEntry = zip.GetEntry(eventEntryName);
                    if (existingEntry != null)
                    {
                        existingEntry.Delete(); // Remove the existing entry
                    }

                    // Serialize the RecordEvent object to JSON
                    var eventEntry = zip.CreateEntry(eventEntryName);
                    using (var entryStream = eventEntry.Open())
                    using (var writer = new StreamWriter(entryStream))
                    {
                        string json = JsonSerializer.Serialize(Program._recordEvents[i]);
                        writer.Write(json);
                    }

                    // Add the new entry to the set of valid entries
                    validEntries.Add(eventEntryName);

                    // Check for and add screenshot if not already processed
                }

                // Remove entries from the zip archive that are not in validEntries
                foreach (var entryName in existingEntries)
                {
                    if (!validEntries.Contains(entryName))
                    {
                        var entryToDelete = zip.GetEntry(entryName);
                        entryToDelete?.Delete();
                    }
                }
            }
        }
    }
}