using System;
using System.Windows.Forms;
using System.Drawing;

namespace BetterStepsRecorder.UI.Dialogs
{
    /// <summary>
    /// A dialog that prompts the user to enter a file name
    /// </summary>
    public class FileNamePrompt : Form
    {
        private TextBox fileNameTextBox;
        private Button okButton;
        private Button cancelButton;
        private Label promptLabel;

        /// <summary>
        /// Gets the file name entered by the user
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Creates a new file name prompt dialog
        /// </summary>
        /// <param name="defaultName">The default file name to display</param>
        /// <param name="title">The title of the dialog</param>
        /// <param name="prompt">The prompt text to display</param>
        public FileNamePrompt(string defaultName = "BSR Export", string title = "Enter File Name", string prompt = "Enter a name for your file (without extension):")
        {
            InitializeComponents(defaultName, title, prompt);
        }

        private void InitializeComponents(string defaultName, string title, string prompt)
        {
            // Form properties
            this.Width = 400;
            this.Height = 150;
            this.Text = title;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create controls
            promptLabel = new Label()
            {
                Left = 20,
                Top = 20,
                Width = 360,
                Text = prompt
            };

            fileNameTextBox = new TextBox()
            {
                Left = 20,
                Top = 50,
                Width = 360,
                Text = defaultName
            };

            okButton = new Button()
            {
                Text = "OK",
                Left = 200,
                Width = 80,
                Top = 80,
                DialogResult = DialogResult.OK
            };

            cancelButton = new Button()
            {
                Text = "Cancel",
                Left = 300,
                Width = 80,
                Top = 80,
                DialogResult = DialogResult.Cancel
            };

            // Wire up events
            okButton.Click += (sender, e) => 
            {
                this.FileName = fileNameTextBox.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            cancelButton.Click += (sender, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            // Add controls to form
            this.Controls.Add(promptLabel);
            this.Controls.Add(fileNameTextBox);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);

            // Set default buttons
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        /// <summary>
        /// Shows the dialog and returns the file name entered by the user
        /// </summary>
        /// <param name="defaultName">The default file name to display</param>
        /// <returns>The file name entered by the user, or null if canceled</returns>
        public static string PromptForFileName(string defaultName = "BSR Export")
        {
            using (FileNamePrompt prompt = new FileNamePrompt(defaultName))
            {
                // Call the Form.ShowDialog() method explicitly to avoid confusion with our static method
                if (((Form)prompt).ShowDialog() == DialogResult.OK)
                {
                    return prompt.FileName;
                }
                return null;
            }
        }
    }
}