using System;
using System.Drawing;
using System.Windows.Forms;
using BetterStepsRecorder.UI;

namespace BetterStepsRecorder
{
    public partial class MainForm
    {
        /// <summary>
        /// Handles the recording button click to start or pause recording
        /// </summary>
      private void ToolStripMenuItem_Recording_Click(object sender, EventArgs e)
        {
            if (Program.IsRecording)
            {
                Program.UnHookMouseOperations();
                ToolStripMenuItem_Recording.Text = "Start Recording";
                ToolStripMenuItem_Recording.BackColor = SystemColors.Control;
                ToolStripMenuItem_Recording.Image = Properties.Resources.RecordTiny;
                ActivityDelay = DefaultActivityDelay;
                activityTimer_Tick(sender, e);
            }
            else
            {
                Program.HookMouseOperations();
                ToolStripMenuItem_Recording.Text = "Pause Recording";
                ToolStripMenuItem_Recording.BackColor = Color.IndianRed;
                ToolStripMenuItem_Recording.Image = Properties.Resources.RecordPauseTiny;
                ActivityDelay = 15000;
            }
        }

        /// <summary>
        /// Enables the recording functionality
        /// </summary>
    private void EnableRecording()
        {
            ToolStripMenuItem_Recording.Enabled = true;
            ToolStripMenuItem_Recording.Visible = true;
        }

        /// <summary>
        /// Disables the recording functionality
        /// </summary>
        private void DisableRecording()
        {
            ToolStripMenuItem_Recording.Enabled = false;
            ToolStripMenuItem_Recording.Visible = false;
        }

        /// <summary>
        /// Handles the activity timer tick event to save the recording
        /// </summary>
        private void activityTimer_Tick(object? sender, EventArgs e)
        {
            Program.zip?.SaveToZip();
            StatusManager.ShowSuccess($"Data Saved");
            activityTimer.Stop();
        }

        /// <summary>
        /// Adds a record event to the listbox
        /// </summary>
        /// <param name="recordEvent">The record event to add</param>
        public void AddRecordEventToListBox(RecordEvent recordEvent)
        {
            Listbox_Events.Items.Add(recordEvent);
            EnableDisable_exportToolStripMenuItem();
        }

        /// <summary>
        /// Clears the listbox and related controls
        /// </summary>
        public void ClearListBox()
        {
            Listbox_Events.Items.Clear();
            EnableDisable_exportToolStripMenuItem();
            propertyGrid_RecordEvent.SelectedObject = null;
            pictureBox1.Image = null;
            richTextBox_stepText.Text = null;
        }

        /// <summary>
        /// Updates the list items based on the current state of Program._recordEvents
        /// </summary>
        private void UpdateListItems()
        {
            ClearListBox();
            // Update the Step property based on the new order in the list
            for (int i = 0; i < Program._recordEvents.Count; i++)
            {
                Program._recordEvents[i].Step = i + 1;
                AddRecordEventToListBox(Program._recordEvents[i]);
                //Debug.WriteLine(Program._recordEvents[i].ToString());
            }

            // Optionally, update the display to reflect new step numbers if shown
            Listbox_Events.Refresh();
            activityTimer.Stop();
            activityTimer.Start();
        }
    }
}