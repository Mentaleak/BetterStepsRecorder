using System;
using System.IO;
using System.Windows.Forms;
using BetterStepsRecorder.Exporters;
using BetterStepsRecorder.UI.Dialogs;

namespace BetterStepsRecorder
{
    public partial class MainForm
    {
        /// <summary>
        /// Gets the default filename for exports based on the current BSR file
        /// </summary>
        /// <returns>The filename without extension</returns>
        private string GetDefaultExportFileName()
        {
            if (Program.zip != null && !string.IsNullOrEmpty(Program.zip.ZipFilePath))
            {
                // Extract the filename without extension
                string fileName = Path.GetFileNameWithoutExtension(Program.zip.ZipFilePath);
                return fileName;
            }
            
            // Default if no file is loaded
            return "Steps Recording";
        }

        /// <summary>
        /// Handles the main export menu item click (defaults to RTF export)
        /// </summary>
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.zip?.SaveToZip();
            // Use the new RtfExporter class through the ExportDialogs helper
            ExportDialogs.HandleRtfExport(GetDefaultExportFileName());
        }

        /// <summary>
        /// Handles export to RTF format
        /// </summary>
        private void exportToRtfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.zip?.SaveToZip();
            // Use the new RtfExporter class through the ExportDialogs helper with default filename
            ExportDialogs.HandleRtfExport(GetDefaultExportFileName());
        }

        /// <summary>
        /// Handles export to HTML format
        /// </summary>
        private void exportToHtmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.zip?.SaveToZip();
            // Use the new HtmlExporter class through the ExportDialogs helper with default filename
            ExportDialogs.HandleHtmlExport(GetDefaultExportFileName());
        }

        /// <summary>
        /// Handles export to ODT (OpenDocument Text) format
        /// </summary>
        private void exportToOdtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.zip?.SaveToZip();
            // Use the new OdtExporter class through the ExportDialogs helper with default filename
            ExportDialogs.HandleOdtExport(GetDefaultExportFileName());
        }

        /// <summary>
        /// Handles export to Obsidian vault format
        /// </summary>
        private void exportToObsidianVaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.zip?.SaveToZip();
            // Use the new ObsidianExporter class through the ExportDialogs helper with default filename
            ExportDialogs.HandleObsidianExport(GetDefaultExportFileName());
        }

        /// <summary>
        /// Enables or disables the export menu items based on whether there are items to export
        /// </summary>
        private void EnableDisable_exportToolStripMenuItem()
        {
            if (Listbox_Events.Items.Count > 0)
            {
                exportToolStripMenuItem.Enabled = true;
                toolStripMenuItem1_SaveAs.Enabled = true;
            }
            else
            {
                exportToolStripMenuItem.Enabled = false;
                toolStripMenuItem1_SaveAs.Enabled = true;
            }
        }
    }
}