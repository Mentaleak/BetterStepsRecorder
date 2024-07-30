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
            // Format the RecordEvent details to display in the ListBox
            string displayText = $"{recordEvent.ID}: {recordEvent.EventType} - {recordEvent.WindowTitle}";
            Listbox_Events.Items.Add(displayText);
        }

        private void RecordingButton_Click(object sender, EventArgs e)
        {
                if (Program.IsRecording)
                {
                    Program.IsRecording = false;
                    RecordingButton.Text = "Start Recording";
                }
                else
                {
                    Program.IsRecording = true;
                    RecordingButton.Text = "Stop Recording";
                }
        }

        
    }
}
