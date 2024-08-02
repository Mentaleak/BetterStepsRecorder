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
        public string zipFilePath;
        //private ZipArchive zipArchive;

        public ZipFileHandler(string zipFilePath)
        {
            this.zipFilePath = zipFilePath;
            //zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Update);
        }
        /* public Stream CreateEntry(string entryName)
         {
             var entry = zipArchive.CreateEntry(entryName, CompressionLevel.Fastest);
             return entry.Open();
         }

         public Stream GetEntryStream(string entryName)
         {
             var entry = zipArchive.GetEntry(entryName);
             return entry?.Open();
         }
        */
        public void SaveToZip()
        {
            using (var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Update))
            {
                var existingEntries = new HashSet<string>(zip.Entries.Select(e => e.FullName));
                var validEntries = new HashSet<string>();

                for (int i = 0; i < Program._recordEvents.Count; i++)
                {
                    // Update the Step based on the list position
                    Program._recordEvents[i].Step = i + 1;

                    var eventEntryName = $"events/event_{Program._recordEvents[i].Step}.json";

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
