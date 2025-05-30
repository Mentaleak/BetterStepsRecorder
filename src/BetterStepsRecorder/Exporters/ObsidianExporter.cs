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

                // Track used image filenames to avoid duplicates
                HashSet<string> usedImageNames = new HashSet<string>();

                // Create the markdown content
                using (StreamWriter writer = new StreamWriter(mdFilePath))
                {
                    // Add title (using the filename)
                    //writer.WriteLine($"# {fileName}");
                    //writer.WriteLine();

                    // Add each step
                    foreach (var recordEvent in Program._recordEvents)
                    {
                        // Write the step number and text
                        writer.WriteLine($"## Step {recordEvent.Step}: {recordEvent._StepText}");
                        writer.WriteLine();

                        // Process and save image if available
                        if (!string.IsNullOrEmpty(recordEvent.Screenshotb64))
                        {
                            // Create a simpler image filename without GUID
                            string baseImageName = $"{fileName}_step{recordEvent.Step}";
                            string imageFileName = baseImageName + ".png";
                            
                            // Check if this filename is already used, if so, add GUID to make it unique
                            if (usedImageNames.Contains(imageFileName))
                            {
                                // Add a shortened GUID to make the filename unique
                                string shortGuid = recordEvent.ID.ToString().Substring(0, 8);
                                imageFileName = $"{baseImageName}_{shortGuid}.png";
                            }
                            
                            usedImageNames.Add(imageFileName);
                            string imageFilePath = Path.Combine(imageFolderPath, imageFileName);
                            
                            // Save the image
                            SaveImageFromBase64(recordEvent.Screenshotb64, imageFilePath, ImageFormat.Png);

                            // Get the relative path for the image link
                            string relativeImagePath = GetRelativeImagePath(vaultPath, imageFolderPath, imageFileName);
                            
                            // Add the image link to the markdown
                            writer.WriteLine($"![[{relativeImagePath}]]");
                            writer.WriteLine();
                        }

                        // Add separator between steps
                        writer.WriteLine("---");
                        writer.WriteLine();
                    }
                    
                    // Add footer with link to GitHub
                    writer.WriteLine();
                    
                    writer.WriteLine("Generated with [Better Steps Recorder](https://github.com/Mentaleak/BetterStepsRecorder)");
                    
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
                catch (Exception)
                {
                    // If there's any error reading the JSON, fall back to default
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