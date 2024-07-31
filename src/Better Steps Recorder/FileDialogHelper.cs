using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Better_Steps_Recorder
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
    }
}
