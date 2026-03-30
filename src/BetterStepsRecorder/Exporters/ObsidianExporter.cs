using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Text.Json;

namespace BetterStepsRecorder.Exporters
{
    /// <summary>
    /// Exporter for Obsidian markdown files with images
    /// </summary>
    public class ObsidianExporter : ExporterBase
    {
        private static string FormatDuration(TimeSpan ts)
        {
            if (ts.TotalHours >= 1)
                return $"{(int)ts.TotalHours}h {ts.Minutes:D2}m {ts.Seconds:D2}s";
            if (ts.TotalMinutes >= 1)
                return $"{ts.Minutes}m {ts.Seconds:D2}s";
            return $"{ts.Seconds}s";
        }

        /// <summary>
        /// Exports the current steps recording to an Obsidian vault
        /// </summary>
        /// <param name="filePath">Not used in this exporter - use ExportToObsidianVault method instead</param>
        /// <returns>Always returns false as this method is not used</returns>
        public override bool Export(string filePath)
        {
            // This method is not used for Obsidian export
            // Use ExportToObsidianVault instead
            return false;
        }

        /// <summary>
        /// Exports the current steps recording to an Obsidian vault as a markdown file with images
        /// </summary>
        /// <param name="vaultPath">The root path of the Obsidian vault</param>
        /// <param name="fileName">The name of the markdown file to create (without extension)</param>
        /// <param name="subfolderPath">Optional subfolder path within the vault</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public bool ExportToObsidianVault(string vaultPath, string fileName, string subfolderPath = "")
        {
            var cfg = BSRSettings.Current.ExportOptions.Obsidian;
            return ExportToObsidianVault(vaultPath, fileName, subfolderPath, cfg);
        }

        /// <summary>
        /// Exports the current steps recording to an Obsidian vault using the supplied settings
        /// </summary>
        public bool ExportToObsidianVault(string vaultPath, string fileName, string subfolderPath, BSRSettings.ObsidianSettings cfg)
        {
            try
            {
                // Validate Obsidian vault
                if (!IsValidObsidianVault(vaultPath))
                {
                    MessageBox.Show("The selected folder is not a valid Obsidian vault. Please select a folder containing a .obsidian directory.", 
                        "Invalid Obsidian Vault", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Determine image folder path
                string imageFolderPath = GetImageFolderPath(vaultPath);

                // Create full path for markdown file
                string mdFilePath;
                if (string.IsNullOrEmpty(subfolderPath))
                {
                    mdFilePath = Path.Combine(vaultPath, $"{fileName}.md");
                }
                else
                {
                    string fullSubfolderPath = Path.Combine(vaultPath, subfolderPath);
                    if (!Directory.Exists(fullSubfolderPath))
                    {
                        Directory.CreateDirectory(fullSubfolderPath);
                    }
                    mdFilePath = Path.Combine(fullSubfolderPath, $"{fileName}.md");
                }

                int totalSteps = Program._recordEvents.Count;
                string generated = DateTime.Now.ToString("dd MMM yyyy, HH:mm");

                // Compute recording start/end/duration from event timestamps
                DateTime? recordingStart = totalSteps > 0 ? Program._recordEvents[0].CreationTime : (DateTime?)null;
                DateTime? recordingEnd = totalSteps > 0 ? Program._recordEvents[totalSteps - 1].CreationTime : (DateTime?)null;
                TimeSpan totalDuration = (recordingStart.HasValue && recordingEnd.HasValue)
                    ? recordingEnd.Value - recordingStart.Value
                    : TimeSpan.Zero;

                string startStr = recordingStart?.ToString("dd MMM yyyy, HH:mm:ss") ?? "—";
                string endStr = recordingEnd?.ToString("HH:mm:ss") ?? "—";
                string durationStr = totalSteps > 1 ? FormatDuration(totalDuration) : "—";

                // Track used image filenames to avoid duplicates
                HashSet<string> usedImageNames = new HashSet<string>();

                // Create the markdown content
                using (StreamWriter writer = new StreamWriter(mdFilePath))
                {
                    // Add title (using the filename)
                    writer.WriteLine($"# {fileName}");
                    writer.WriteLine();

                    // Generated date
                    if (cfg.ShowGeneratedDate)
                    {
                        writer.WriteLine($"*Generated {generated}*");
                        writer.WriteLine();
                    }

                    // Summary section
                    if (cfg.ShowSummary)
                    {
                        writer.WriteLine("## Summary");
                        writer.WriteLine();
                        writer.WriteLine("| Property | Value |");
                        writer.WriteLine("|----------|-------|");
                        writer.WriteLine($"| Steps | {totalSteps} |");
                        writer.WriteLine($"| Started | {startStr} |");
                        writer.WriteLine($"| Finished | {endStr} |");
                        writer.WriteLine($"| Duration | {durationStr} |");
                        writer.WriteLine();
                    }

                    // Steps section
                    writer.WriteLine("## Steps");
                    writer.WriteLine();

                    // Add each step
                    DateTime? prevTime = null;
                    foreach (var recordEvent in Program._recordEvents)
                    {
                        string stepText = recordEvent._StepText ?? string.Empty;

                        // Step header
                        writer.WriteLine($"### Step {recordEvent.Step}");
                        writer.WriteLine();
                        writer.WriteLine($"**{stepText}**");
                        writer.WriteLine();

                        // Timestamp
                        if (cfg.ShowStepTimestamps)
                        {
                            string timeStr;
                            if (prevTime.HasValue)
                            {
                                TimeSpan delta = recordEvent.CreationTime - prevTime.Value;
                                timeStr = $"⏱️ {prevTime.Value:HH:mm:ss} → {recordEvent.CreationTime:HH:mm:ss} (+{FormatDuration(delta)})";
                            }
                            else
                            {
                                timeStr = $"⏱️ {recordEvent.CreationTime:HH:mm:ss}";
                            }
                            writer.WriteLine(timeStr);
                            writer.WriteLine();
                        }
                        prevTime = recordEvent.CreationTime;

                        // Detail table - only rendered when at least one detail option is on
                        if (!cfg.IsDetailTableEmpty)
                        {
                            writer.WriteLine("| Detail | Value |");
                            writer.WriteLine("|--------|-------|");
                            if (cfg.ShowAction && !string.IsNullOrWhiteSpace(recordEvent.EventType))
                                writer.WriteLine($"| Action | {recordEvent.EventType} |");
                            if (cfg.ShowApplication && !string.IsNullOrWhiteSpace(recordEvent.ApplicationName))
                                writer.WriteLine($"| Application | {recordEvent.ApplicationName} |");
                            if (cfg.ShowWindow && !string.IsNullOrWhiteSpace(recordEvent.WindowTitle))
                                writer.WriteLine($"| Window | {recordEvent.WindowTitle} |");
                            if (cfg.ShowElement && !string.IsNullOrWhiteSpace(recordEvent.ElementName))
                                writer.WriteLine($"| Element | {recordEvent.ElementName} |");
                            if (cfg.ShowElementType && !string.IsNullOrWhiteSpace(recordEvent.ElementType))
                                writer.WriteLine($"| Element Type | {recordEvent.ElementType} |");
                            if (cfg.ShowMousePosition && (recordEvent.MouseCoordinates.X != 0 || recordEvent.MouseCoordinates.Y != 0))
                                writer.WriteLine($"| Mouse Position | {recordEvent.MouseCoordinates.X}, {recordEvent.MouseCoordinates.Y} |");
                            writer.WriteLine();
                        }

                        // Process and save image if available
                        if (recordEvent.HasScreenshot)
                        {
                            string baseImageName = $"{fileName}_step{recordEvent.Step}";
                            string imageFileName = baseImageName + ".png";

                            if (usedImageNames.Contains(imageFileName))
                                imageFileName = $"{baseImageName}_{recordEvent.ShortId}.png";

                            usedImageNames.Add(imageFileName);
                            string imageFilePath = Path.Combine(imageFolderPath, imageFileName);

                            SaveImageFromEvent(recordEvent, imageFilePath, ImageFormat.Png);

                            // Get the relative path for the image link
                            string relativeImagePath = GetRelativeImagePath(vaultPath, imageFolderPath, imageFileName);

                            // Add the image link to the markdown
                            writer.WriteLine($"![[{relativeImagePath}]]");
                            writer.WriteLine();
                        }

                        // Add separator between steps (except after last step)
                        if (recordEvent.Step < totalSteps)
                        {
                            writer.WriteLine("---");
                            writer.WriteLine();
                        }
                    }

                    // Add footer with link to GitHub
                    writer.WriteLine();
                    writer.WriteLine("---");
                    writer.WriteLine();
                    writer.WriteLine("*Generated with [Better Steps Recorder](https://github.com/Mentaleak/BetterStepsRecorder)*");

                }

                ShowExportSuccess(mdFilePath);
                return true;
            }
            catch (Exception ex)
            {
                ShowExportError("Error exporting to Obsidian vault", ex);
                return false;
            }
        }

        /// <summary>
        /// Checks if the selected folder is a valid Obsidian vault by looking for the .obsidian folder
        /// </summary>
        private bool IsValidObsidianVault(string vaultPath)
        {
            return Directory.Exists(Path.Combine(vaultPath, ".obsidian"));
        }

        /// <summary>
        /// Determines the image folder path based on Obsidian settings or creates a default one
        /// </summary>
        private string GetImageFolderPath(string vaultPath)
        {
            string appJsonPath = Path.Combine(vaultPath, ".obsidian", "app.json");
            string imageFolderPath;

            // Check if app.json exists and try to read attachmentFolderPath
            if (File.Exists(appJsonPath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(appJsonPath);
                    using (JsonDocument document = JsonDocument.Parse(jsonContent))
                    {
                        if (document.RootElement.TryGetProperty("attachmentFolderPath", out JsonElement attachmentFolder))
                        {
                            string attachmentFolderPath = attachmentFolder.GetString();
                            if (!string.IsNullOrEmpty(attachmentFolderPath))
                            {
                                // Use the configured attachment folder
                                imageFolderPath = Path.Combine(vaultPath, attachmentFolderPath);
                                if (!Directory.Exists(imageFolderPath))
                                {
                                    Directory.CreateDirectory(imageFolderPath);
                                }
                                return imageFolderPath;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // If there's any error reading the JSON, fall back to default
                    System.Diagnostics.Debug.WriteLine($"Failed to read Obsidian vault settings: {ex.Message}");
                }
            }

            // Default folder if not found in settings
            imageFolderPath = Path.Combine(vaultPath, "BSR", "images");
            if (!Directory.Exists(imageFolderPath))
            {
                Directory.CreateDirectory(imageFolderPath);
            }
            return imageFolderPath;
        }

        /// <summary>
        /// Gets the relative path for the image to be used in Obsidian markdown
        /// </summary>
        private string GetRelativeImagePath(string vaultPath, string imageFolderPath, string imageFileName)
        {
            // Get the path relative to the vault root
            string relativePath = imageFolderPath.Substring(vaultPath.Length).TrimStart(Path.DirectorySeparatorChar);
            return Path.Combine(relativePath, imageFileName).Replace('\\', '/');
        }
    }
}