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
            grpMinimizeBehavior = new GroupBox();
            rbDoNotMinimize = new RadioButton();
            rbMinimizeToTaskbar = new RadioButton();
            rbMinimizeToSystemTray = new RadioButton();
            chkAllowRecordSelf = new CheckBox();
            grpMinimizeBehavior.SuspendLayout();
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
            // grpMinimizeBehavior
            // 
            grpMinimizeBehavior.Controls.Add(rbMinimizeToSystemTray);
            grpMinimizeBehavior.Controls.Add(rbMinimizeToTaskbar);
            grpMinimizeBehavior.Controls.Add(rbDoNotMinimize);
            grpMinimizeBehavior.Location = new Point(20, 95);
            grpMinimizeBehavior.Name = "grpMinimizeBehavior";
            grpMinimizeBehavior.Size = new Size(400, 120);
            grpMinimizeBehavior.TabIndex = 2;
            grpMinimizeBehavior.TabStop = false;
            grpMinimizeBehavior.Text = "When starting recording:";
            // 
            // rbDoNotMinimize
            // 
            rbDoNotMinimize.AutoSize = true;
            rbDoNotMinimize.Location = new Point(15, 30);
            rbDoNotMinimize.Name = "rbDoNotMinimize";
            rbDoNotMinimize.Size = new Size(130, 24);
            rbDoNotMinimize.TabIndex = 0;
            rbDoNotMinimize.Text = "Do not minimize";
            rbDoNotMinimize.UseVisualStyleBackColor = true;
            rbDoNotMinimize.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // rbMinimizeToTaskbar
            // 
            rbMinimizeToTaskbar.AutoSize = true;
            rbMinimizeToTaskbar.Checked = true;
            rbMinimizeToTaskbar.Location = new Point(15, 60);
            rbMinimizeToTaskbar.Name = "rbMinimizeToTaskbar";
            rbMinimizeToTaskbar.Size = new Size(160, 24);
            rbMinimizeToTaskbar.TabIndex = 1;
            rbMinimizeToTaskbar.TabStop = true;
            rbMinimizeToTaskbar.Text = "Minimize to taskbar";
            rbMinimizeToTaskbar.UseVisualStyleBackColor = true;
            rbMinimizeToTaskbar.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // rbMinimizeToSystemTray
            // 
            rbMinimizeToSystemTray.AutoSize = true;
            rbMinimizeToSystemTray.Location = new Point(15, 90);
            rbMinimizeToSystemTray.Name = "rbMinimizeToSystemTray";
            rbMinimizeToSystemTray.Size = new Size(195, 24);
            rbMinimizeToSystemTray.TabIndex = 2;
            rbMinimizeToSystemTray.Text = "Minimize to system tray";
            rbMinimizeToSystemTray.UseVisualStyleBackColor = true;
            rbMinimizeToSystemTray.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // chkAllowRecordSelf
            // 
            chkAllowRecordSelf.AutoSize = true;
            chkAllowRecordSelf.Location = new Point(20, 230);
            chkAllowRecordSelf.Name = "chkAllowRecordSelf";
            chkAllowRecordSelf.Size = new Size(200, 24);
            chkAllowRecordSelf.TabIndex = 3;
            chkAllowRecordSelf.Text = "Allow recording BSR itself";
            chkAllowRecordSelf.UseVisualStyleBackColor = true;
            chkAllowRecordSelf.CheckedChanged += Checkbox_CheckedChanged;
            // 
            // GeneralSettings
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(chkAllowRecordSelf);
            Controls.Add(grpMinimizeBehavior);
            Controls.Add(lblRecording);
            Controls.Add(lblNote);
            Name = "GeneralSettings";
            Size = new Size(472, 280);
            grpMinimizeBehavior.ResumeLayout(false);
            grpMinimizeBehavior.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNote;
        private Label lblRecording;
        private GroupBox grpMinimizeBehavior;
        private RadioButton rbDoNotMinimize;
        private RadioButton rbMinimizeToTaskbar;
        private RadioButton rbMinimizeToSystemTray;
        private CheckBox chkAllowRecordSelf;
    }
}
