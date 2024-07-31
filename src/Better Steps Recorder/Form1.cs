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
    }
}
