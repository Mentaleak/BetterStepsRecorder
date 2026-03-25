namespace BetterStepsRecorder.UI.Settings
{
    partial class GeneralSettings
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblNote = new Label();
            lblRecording = new Label();
            chkMinimizeOnStart = new CheckBox();
            SuspendLayout();
            // 
            // lblNote
            // 
            lblNote.ForeColor = SystemColors.GrayText;
            lblNote.Location = new Point(3, 25);
            lblNote.Name = "lblNote";
            lblNote.Size = new Size(450, 32);
            lblNote.TabIndex = 0;
            lblNote.Text = "General application settings:";
            // 
            // lblRecording
            // 
            lblRecording.AutoSize = true;
            lblRecording.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblRecording.Location = new Point(3, 70);
            lblRecording.Name = "lblRecording";
            lblRecording.Size = new Size(75, 19);
            lblRecording.TabIndex = 1;
            lblRecording.Text = "Recording";
            // 
            // chkMinimizeOnStart
            // 
            chkMinimizeOnStart.AutoSize = true;
            chkMinimizeOnStart.Location = new Point(20, 95);
            chkMinimizeOnStart.Name = "chkMinimizeOnStart";
            chkMinimizeOnStart.Size = new Size(200, 24);
            chkMinimizeOnStart.TabIndex = 2;
            chkMinimizeOnStart.Text = "Minimize on start recording";
            chkMinimizeOnStart.UseVisualStyleBackColor = true;
            // 
            // GeneralSettings
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(chkMinimizeOnStart);
            Controls.Add(lblRecording);
            Controls.Add(lblNote);
            Name = "GeneralSettings";
            Size = new Size(472, 280);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNote;
        private Label lblRecording;
        private CheckBox chkMinimizeOnStart;
    }
}
