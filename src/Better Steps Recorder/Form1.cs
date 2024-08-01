using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text.Json;
using System.Windows.Automation;
using System.Windows.Forms;

namespace Better_Steps_Recorder
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("Loaded");
        }

        public void AddRecordEventToListBox(RecordEvent recordEvent)
        {
            Listbox_Events.Items.Add(recordEvent);
            EnableDisable_exportToolStripMenuItem();
        }
        public void ClearListBox()
        {
            Listbox_Events.Items.Clear();
            EnableDisable_exportToolStripMenuItem();

        }


        private void Listbox_Events_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
            {
                propertyGrid_RecordEvent.SelectedObject = selectedEvent;

                // Check if Screenshotb64 is not null or empty
                if (!string.IsNullOrEmpty(selectedEvent.Screenshotb64))
                {
                    try
                    {
                        // Convert the Base64 string back to a byte array
                        byte[] imageBytes = Convert.FromBase64String(selectedEvent.Screenshotb64);

                        // Create a MemoryStream from the byte array
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            // Create a Bitmap from the MemoryStream and set it to the PictureBox
                            pictureBox1.Image = new Bitmap(ms);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to load image from Base64 string: {ex.Message}");
                        pictureBox1.Image = null; // Clear the image if there was an error
                    }
                }
                else
                {
                    pictureBox1.Image = null; // Clear the image if there's no Base64 string
                }

                // Set the step text
                richTextBox_stepText.Text = selectedEvent._StepText;
            }
        }


        private void ToolStripMenuItem_Recording_Click(object sender, EventArgs e)
        {
            if (Program.IsRecording)
            {
                Program.IsRecording = false;
                ToolStripMenuItem_Recording.Text = "Start Recording";
                ToolStripMenuItem_Recording.BackColor = SystemColors.Control;
                ToolStripMenuItem_Recording.Image = Properties.Resources.RecordTiny;

            }
            else
            {
                Program.IsRecording = true;
                ToolStripMenuItem_Recording.Text = "Pause Recording";
                ToolStripMenuItem_Recording.BackColor = Color.IndianRed;
                ToolStripMenuItem_Recording.Image = Properties.Resources.RecordPauseTiny;
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string zipFilePath = FileDialogHelper.ShowSaveFileDialog();
            if (zipFilePath != null && zipFilePath != "")
            {
                EnableRecording();
                Program.zip = new ZipFileHandler(zipFilePath);
                Program._recordEvents = new List<RecordEvent>();
                Listbox_Events.Items.Clear();
                Program.EventCounter = 1;
                EnableDisable_exportToolStripMenuItem();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string zipFilePath = FileDialogHelper.ShowOpenFileDialog();
            if (zipFilePath != null && zipFilePath != "")
            {
                EnableRecording();
                Program.zip = new ZipFileHandler(zipFilePath);
                Program.LoadRecordEventsFromFile(zipFilePath);
                EnableDisable_exportToolStripMenuItem();
            }


        }

        private void richTextBox_stepText_TextChanged(object sender, EventArgs e)
        {
            if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
            {
                var recordEvent = Program._recordEvents.Find(ev => ev.ID == selectedEvent.ID);
                if (recordEvent != null)
                {
                    recordEvent._StepText = richTextBox_stepText.Text;
                    Program.zip?.SaveToZip();
                }
                else
                {
                    // Handle the case where the event is not found, if necessary
                    // This might be logging an error, notifying the user, etc.
                }
            }
        }



        private void EnableRecording()
        {
            ToolStripMenuItem_Recording.Enabled = true;
            ToolStripMenuItem_Recording.Visible = true;
        }
        private void DisableRecording()
        {
            ToolStripMenuItem_Recording.Enabled = false;
            ToolStripMenuItem_Recording.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DisableRecording();
        }

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

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set up the save file dialog to specify the output path for the Word document
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Rich Text Format|*.rtf";
                saveFileDialog.Title = "Export to RTF Document";
                if (Program.zip?.zipFilePath != null)
                {
                    saveFileDialog.FileName = Path.GetFileNameWithoutExtension(Program.zip.zipFilePath) + ".rtf";
                }
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string docPath = saveFileDialog.FileName;
                    Program.ExportToRTF(docPath);
                }
            }
        }



        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Listbox_Events.SelectedItem != null)
            {
                if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
                {
                    Listbox_Events.Items.Remove(Listbox_Events.SelectedItem);

                    var recordEvent = Program._recordEvents.Find(e => e.ID == selectedEvent.ID);
                    if (recordEvent != null)
                    {
                        recordEvent._StepText = richTextBox_stepText.Text;
                    }
                    else
                    {
                        // Handle the case where the event is not found, if necessary
                        MessageBox.Show("The selected event was not found in the record events list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    Program.zip?.SaveToZip();
                }
            }
        }


        private void Listbox_Events_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Get the index of the item under the mouse cursor
                int index = Listbox_Events.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    // Select the item
                    Listbox_Events.SelectedIndex = index;

                    // Show the context menu at the mouse position
                    contextMenu_ListBox_Events.Show(Listbox_Events, e.Location);
                }
            }
        }

        private void toolStripMenuItem1_SaveAs_Click(object sender, EventArgs e)
        {
            FileDialogHelper.SaveAs();
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int selectedIndex = Listbox_Events.SelectedIndex;
            if (selectedIndex > 0)
            {
                // Swap the selected item with the one above it
                var temp = Program._recordEvents[selectedIndex];
                Program._recordEvents[selectedIndex] = Program._recordEvents[selectedIndex - 1];
                Program._recordEvents[selectedIndex - 1] = temp;

                // Update the ListBox
                Listbox_Events.Items[selectedIndex] = Program._recordEvents[selectedIndex];
                Listbox_Events.Items[selectedIndex - 1] = Program._recordEvents[selectedIndex - 1];

                // Set the new selected index
                Listbox_Events.SelectedIndex = selectedIndex - 1;

                // Update steps
                UpdateStepNumbers();
            }
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int selectedIndex = Listbox_Events.SelectedIndex;
            if (selectedIndex < Program._recordEvents.Count - 1)
            {
                // Swap the selected item with the one below it
                var temp = Program._recordEvents[selectedIndex];
                Program._recordEvents[selectedIndex] = Program._recordEvents[selectedIndex + 1];
                Program._recordEvents[selectedIndex + 1] = temp;

                // Update the ListBox
                Listbox_Events.Items[selectedIndex] = Program._recordEvents[selectedIndex];
                Listbox_Events.Items[selectedIndex + 1] = Program._recordEvents[selectedIndex + 1];

                // Set the new selected index
                Listbox_Events.SelectedIndex = selectedIndex + 1;

                // Update steps
                UpdateStepNumbers();
            }
        }

        private void UpdateStepNumbers()
        {
            // Update the Step property based on the new order in the list
            for (int i = 0; i < Program._recordEvents.Count; i++)
            {
                Program._recordEvents[i].Step = i + 1;
            }

            // Optionally, update the display to reflect new step numbers if shown
            Listbox_Events.Refresh();
        }
    }
}
