using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BetterStepsRecorder
{
    public partial class MainForm
    {
        /// <summary>
        /// Creates a new recording file
        /// </summary>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.zip != null)
            {
                Program.SaveRecordEvents();
            }
            string zipFilePath = FileDialogHelper.ShowSaveFileDialog();
            if (zipFilePath != null && zipFilePath != "")
            {
                EnableRecording();
                Program.zip = new ZipFileHandler(zipFilePath);
                Program._recordEvents = new List<RecordEvent>();
                Listbox_Events.Items.Clear();
                Program.EventCounter = 1;
                EnableDisable_exportToolStripMenuItem();
                propertyGrid_RecordEvent.SelectedObject = null;
                pictureBox1.Image = null;
                richTextBox_stepText.Text = null;
            }
        }

        /// <summary>
        /// Opens an existing recording file
        /// </summary>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.zip != null)
            {
                Program.SaveRecordEvents();
            }
            string zipFilePath = FileDialogHelper.ShowOpenFileDialog();
            if (zipFilePath != null && zipFilePath != "")
            {
                propertyGrid_RecordEvent.SelectedObject = null;
                pictureBox1.Image = null;
                richTextBox_stepText.Text = null;
                EnableRecording();
                Program.zip = new ZipFileHandler(zipFilePath);
                Program.LoadRecordEventsFromFile(zipFilePath);
                EnableDisable_exportToolStripMenuItem();
            }
        }

        /// <summary>
        /// Handles the Save As operation
        /// </summary>
        private void toolStripMenuItem1_SaveAs_Click(object sender, EventArgs e)
        {
            Program.SaveRecordEvents();
            FileDialogHelper.SaveAs();
        }

        /// <summary>
        /// Handles the text changes in the step text rich text box
        /// </summary>
        private void richTextBox_stepText_TextChanged(object sender, EventArgs e)
        {
            if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
            {
                var recordEvent = Program._recordEvents.Find(ev => ev.ID == selectedEvent.ID);
                if (recordEvent != null)
                {
                    if (recordEvent._StepText != richTextBox_stepText.Text)
                    {
                        recordEvent._StepText = richTextBox_stepText.Text;
                        activityTimer.Stop();
                        activityTimer.Start();
                    }
                }
                else
                {
                    // Handle the case where the event is not found, if necessary
                    // This might be logging an error, notifying the user, etc.
                }
                // UpdateListItems();
            }
        }

        /// <summary>
        /// Updates the list items when the rich text box loses focus
        /// </summary>
        private void richTextBox_stepText_Leave(object sender, EventArgs e)
        {
            UpdateListItems();
        }

        /// <summary>
        /// Shows the help popup
        /// </summary>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpPopup helpPopup = new HelpPopup();
            helpPopup.Show();
        }
    }
}