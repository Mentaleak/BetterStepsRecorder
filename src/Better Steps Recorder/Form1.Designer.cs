namespace Better_Steps_Recorder
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            RecordingButton = new Button();
            Listbox_Events = new ListBox();
            SuspendLayout();
            // 
            // RecordingButton
            // 
            RecordingButton.Location = new Point(27, 35);
            RecordingButton.Name = "RecordingButton";
            RecordingButton.Size = new Size(111, 23);
            RecordingButton.TabIndex = 0;
            RecordingButton.Text = "Start Recording";
            RecordingButton.UseVisualStyleBackColor = true;
            RecordingButton.Click += RecordingButton_Click;
            // 
            // Listbox_Events
            // 
            Listbox_Events.FormattingEnabled = true;
            Listbox_Events.ItemHeight = 15;
            Listbox_Events.Location = new Point(484, 35);
            Listbox_Events.Name = "Listbox_Events";
            Listbox_Events.Size = new Size(253, 334);
            Listbox_Events.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(Listbox_Events);
            Controls.Add(RecordingButton);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button RecordingButton;
        private ListBox Listbox_Events;
    }
}
