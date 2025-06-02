using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BetterStepsRecorder.UI;
using BetterStepsRecorder; // Add this to reference the Program class

namespace Better_Steps_Recorder
{
    public partial class Form1 : Form
    {
        // Add the StatusStripManager as a class member
        private StatusStripManager _statusManager;
        
        // Rest of the existing Form1 code...
        
        private void InitializeStatusStrip()
        {
            // Initialize the status strip manager
            _statusManager = new StatusStripManager(this);
            
            // Example of how to show a message (this would be called from various actions)
            // _statusManager.ShowMessage("Ready to record steps");
        }
        
        // Example of how to use the status strip in various operations
        private void ShowRecordingStarted()
        {
            _statusManager.ShowMessage("Recording started");
        }
        
        private void ShowRecordingStopped()
        {
            _statusManager.ShowMessage("Recording stopped");
        }
        
        private void ShowSaveSuccess(string filePath)
        {
            _statusManager.ShowSuccess($"File saved successfully to: {filePath}");
        }
        
        private void ShowExportSuccess(string format, string filePath)
        {
            _statusManager.ShowSuccess($"Successfully exported to {format}: {filePath}");
        }
        
        private void ShowError(string errorMessage)
        {
            _statusManager.ShowMessage(errorMessage, true);
        }
        
        // Example of how to integrate with existing methods
        
        // Modify the existing EnableRecording method to show status message
        public void EnableRecording()
        {
            // Existing code...
            
            // Add status message
            ShowRecordingStarted();
        }
        
        // Modify the existing DisableRecording method to show status message
        public void DisableRecording()
        {
            // Existing code...
            
            // Add status message
            ShowRecordingStopped();
        }
        
        // Modify the toolStripMenuItem1_SaveAs_Click method to show status message
        private void toolStripMenuItem1_SaveAs_Click(object sender, EventArgs e)
        {
            // Existing code...
            
            // Add status message after successful save
            if (BetterStepsRecorder.Program.zip != null && !string.IsNullOrEmpty(BetterStepsRecorder.Program.zip.ZipFilePath))
            {
                ShowSaveSuccess(BetterStepsRecorder.Program.zip.ZipFilePath);
            }
        }
        
        // Modify export methods to show status messages
        private void exportToRtfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Existing code...
            
            // Add status message after successful export
            // ShowExportSuccess("RTF", filePath);
        }
    }
}