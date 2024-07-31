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



        private void Listbox_Events_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
            {
                propertyGrid_RecordEvent.SelectedObject = selectedEvent;
                string filePath = selectedEvent.ScreenshotPath;
                pictureBox1.Image = new Bitmap(filePath);
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
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox_stepText_TextChanged(object sender, EventArgs e)
        {
            if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
            {
                selectedEvent._StepText = richTextBox_stepText.Text;
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
