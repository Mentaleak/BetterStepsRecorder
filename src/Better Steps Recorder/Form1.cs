using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text.Json;
using System.Windows.Automation;
using System.Windows.Forms;
using Xceed.Document.NET;
using Xceed.Words.NET;

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

        private void EnableDisable_exportToolStripMenuItem()
        {
            if(Listbox_Events.Items.Count > 0)
            {
                exportToolStripMenuItem.Enabled = true;
            }
            else
            {
                exportToolStripMenuItem.Enabled = false;
            }
            
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set up the save file dialog to specify the output path for the Word document
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Word Document|*.docx";
                saveFileDialog.Title = "Export to Word Document";
                saveFileDialog.FileName = "ExportedEvents.docx";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string docPath = saveFileDialog.FileName;
                    ExportToWord(docPath);
                }
            }
        }

        private void ExportToWord(string docPath)
        {
            try
            {
                using (var doc = DocX.Create(docPath))
                {
                    // Initialize the list index
                    int stepNumber = 1;

                    // Iterate through each record event and add to the Word document
                    foreach (var recordEvent in Program._recordEvents)
                    {
                        // Create a numbered list item for the StepText
                        var listItem = doc.AddList($"{recordEvent._StepText}", stepNumber, ListItemType.Numbered);
                        doc.InsertList(listItem);

                        // Increment the step number for the next item
                        stepNumber++;

                        // Add a line break
                        doc.InsertParagraph(Environment.NewLine);

                        // Decode the base64 screenshot
                        if (!string.IsNullOrEmpty(recordEvent.Screenshotb64))
                        {
                            byte[] imageBytes = Convert.FromBase64String(recordEvent.Screenshotb64);
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                using (System.Drawing.Image image = System.Drawing.Image.FromStream(ms))
                                {
                                    // Scale the image if necessary to fit the page width
                                    float maxWidth = 500; // Adjust as needed
                                    float scaleFactor = Math.Min(maxWidth / image.Width, 1);
                                    int scaledWidth = (int)(image.Width * scaleFactor);
                                    int scaledHeight = (int)(image.Height * scaleFactor);

                                    // Save the image to a temporary file
                                    string tempImagePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
                                    image.Save(tempImagePath, ImageFormat.Png);

                                    // Insert the image into the document
                                    var picture = doc.AddImage(tempImagePath).CreatePicture(scaledHeight, scaledWidth);
                                    doc.InsertParagraph().AppendPicture(picture);

                                    // Clean up the temporary file
                                    File.Delete(tempImagePath);
                                }
                            }
                        }

                        // Add a line break after each event
                        doc.InsertParagraph(Environment.NewLine);
                    }

                    // Save the document
                    doc.Save();
                }

                MessageBox.Show("Export completed successfully.", "Export to Word", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (IOException ioEx)
            {
                MessageBox.Show($"Failed to save the document. {ioEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





    }
}
