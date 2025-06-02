using BetterStepsRecorder.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterStepsRecorder
{
    public class FileDialogHelper
    {
        public static string ShowSaveFileDialog()
        {
            string zipFilePath = string.Empty;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

                saveFileDialog.Filter = "Better Step Recorder Files|*.BSR";
                saveFileDialog.Title = "Save Better Step Recorder Filer";
                saveFileDialog.DefaultExt = "BSR";
                saveFileDialog.AddExtension = true;
                saveFileDialog.FileName = $"{timestamp}.BSR";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    zipFilePath = saveFileDialog.FileName;
                }
            }

            return zipFilePath;
        }

        public static string ShowOpenFileDialog()
        {
            string filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Better Step Recorder Files|*.BSR";
                openFileDialog.Title = "Open Better Step Recorder File";
                openFileDialog.DefaultExt = "BSR";
                openFileDialog.AddExtension = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                }
            }

            return filePath;
        }

        public static void SaveAs()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                saveFileDialog.Filter = "BSR Files|*.BSR";
                saveFileDialog.Title = "Save As";
                saveFileDialog.DefaultExt = "BSR";
                saveFileDialog.AddExtension = true;
                saveFileDialog.FileName = $"{timestamp}.BSR";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string newZipFilePath = saveFileDialog.FileName;

                    // Assume zipHandler is an instance of ZipFileHandler
                    Program.zip = new ZipFileHandler(newZipFilePath);
                    Program.zip.SaveToZip();
                    //MessageBox.Show("File saved successfully.", "Save As", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    StatusManager.ShowSuccess("File saved successfully");
                }
            }
        }
    }
}
