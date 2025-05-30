using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using BetterStepsRecorder.Exporters;

namespace BetterStepsRecorder.UI.Dialogs
{
    /// <summary>
    /// Provides common dialog functionality for all exporters
    /// </summary>
    public static class ExportDialogs
    {
        // Import required Windows API functions for folder dialog customization
        [DllImport("user32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Shows a save file dialog for RTF export
        /// </summary>
        /// <param name="defaultFileName">The default filename to use (without extension)</param>
        /// <returns>The selected file path, or null if canceled</returns>
        public static string ShowRtfSaveDialog(string defaultFileName = "Steps Recording")
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Rich Text Format (*.rtf)|*.rtf";
                saveDialog.Title = "Save Steps as RTF";
                saveDialog.DefaultExt = "rtf";
                saveDialog.FileName = $"{defaultFileName}.rtf";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    return saveDialog.FileName;
                }
                return null;
            }
        }

        /// <summary>
        /// Shows a save file dialog for HTML export
        /// </summary>
        /// <param name="defaultFileName">The default filename to use (without extension)</param>
        /// <returns>The selected file path, or null if canceled</returns>
        public static string ShowHtmlSaveDialog(string defaultFileName = "Steps Recording")
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "HTML Files (*.html)|*.html";
                saveDialog.Title = "Save Steps as HTML";
                saveDialog.DefaultExt = "html";
                saveDialog.FileName = $"{defaultFileName}.html";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    return saveDialog.FileName;
                }
                return null;
            }
        }

        /// <summary>
        /// Shows a dialog to select an Obsidian vault folder
        /// </summary>
        /// <returns>The selected vault path, or null if canceled</returns>
        public static string SelectObsidianVault()
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select Obsidian Vault Folder";
                folderDialog.UseDescriptionForTitle = true;
                folderDialog.ShowNewFolderButton = false;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string vaultPath = folderDialog.SelectedPath;
                    
                    // Validate that this is an Obsidian vault
                    if (!Directory.Exists(Path.Combine(vaultPath, ".obsidian")))
                    {
                        MessageBox.Show("The selected folder is not a valid Obsidian vault. Please select a folder containing a .obsidian directory.", 
                            "Invalid Obsidian Vault", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                    
                    return vaultPath;
                }
                return null;
            }
        }

        /// <summary>
        /// Shows a dialog to select a subfolder within the Obsidian vault
        /// This restricts browsing to within the vault only
        /// </summary>
        /// <param name="vaultPath">The root path of the Obsidian vault</param>
        /// <returns>The selected subfolder path relative to the vault, or empty string if root selected</returns>
        public static string SelectSubfolder(string vaultPath)
        {
            // Create a custom folder browser that restricts navigation to within the vault
            using (RestrictedFolderBrowser restrictedBrowser = new RestrictedFolderBrowser(vaultPath))
            {
                if (restrictedBrowser.ShowDialog() == DialogResult.OK)
                {
                    // Return the path relative to the vault
                    string selectedPath = restrictedBrowser.SelectedPath;
                    if (selectedPath.StartsWith(vaultPath))
                    {
                        string relativePath = selectedPath.Substring(vaultPath.Length).TrimStart(Path.DirectorySeparatorChar);
                        return relativePath;
                    }
                }
                return "";
            }
        }

        /// <summary>
        /// Prompts the user for a file name
        /// </summary>
        /// <param name="defaultName">The default file name to display</param>
        /// <returns>The file name entered by the user, or null if canceled</returns>
        public static string PromptForFileName(string defaultName = "BSR Export")
        {
            return FileNamePrompt.PromptForFileName(defaultName);
        }

        /// <summary>
        /// Handles the complete Obsidian export process including all dialogs
        /// </summary>
        /// <param name="defaultFileName">The default filename to use (without extension)</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public static bool HandleObsidianExport(string defaultFileName = "Steps Recording")
        {
            // Select Obsidian vault
            string vaultPath = SelectObsidianVault();
            if (string.IsNullOrEmpty(vaultPath))
                return false;

            // Select subfolder (optional)
            string subfolderPath = SelectSubfolder(vaultPath);
            
            // Prompt for file name
            string fileName = PromptForFileName(defaultFileName);
            if (string.IsNullOrEmpty(fileName))
                return false;

            // Perform the export
            ObsidianExporter exporter = new ObsidianExporter();
            return exporter.ExportToObsidianVault(vaultPath, fileName, subfolderPath);
        }

        /// <summary>
        /// Handles the complete HTML export process including all dialogs
        /// </summary>
        /// <param name="defaultFileName">The default filename to use (without extension)</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public static bool HandleHtmlExport(string defaultFileName = "Steps Recording")
        {
            string filePath = ShowHtmlSaveDialog(defaultFileName);
            if (string.IsNullOrEmpty(filePath))
                return false;

            HtmlExporter exporter = new HtmlExporter();
            return exporter.Export(filePath);
        }

        /// <summary>
        /// Handles the complete RTF export process including all dialogs
        /// </summary>
        /// <param name="defaultFileName">The default filename to use (without extension)</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public static bool HandleRtfExport(string defaultFileName = "Steps Recording")
        {
            string filePath = ShowRtfSaveDialog(defaultFileName);
            if (string.IsNullOrEmpty(filePath))
                return false;

            RtfExporter exporter = new RtfExporter();
            return exporter.Export(filePath);
        }
    }
}