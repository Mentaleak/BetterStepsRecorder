using System.Diagnostics;
using System.Text.Json;
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
        }
        public void ClearListBox()
        {
            Listbox_Events.Items.Clear();
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
            }
            else
            {
                Program.IsRecording = true;
                ToolStripMenuItem_Recording.Text = "Stop Recording";
                ToolStripMenuItem_Recording.BackColor = Color.IndianRed;
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string zipFilePath = FileDialogHelper.ShowSaveFileDialog();
            if(zipFilePath != null && zipFilePath != "") { 
                EnableRecording();
                Program.zip = new ZipFileHandler(zipFilePath);
                Program._recordEvents = new List<RecordEvent>();
                Listbox_Events.Items.Clear();
                Program.EventCounter = 1;
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
            }
            
        }

        
 


    private void richTextBox_stepText_TextChanged(object sender, EventArgs e)
        {
            if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
            {
                Program._recordEvents.Find(e => e.ID == selectedEvent.ID)._StepText = richTextBox_stepText.Text;
                Program.zip.SaveToZip();
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
    }
}
