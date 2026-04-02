namespace BetterStepsRecorder.UI.Settings
{
    partial class KeyBindsSettings
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
            components = new System.ComponentModel.Container();
            lblNote = new Label();
            lblKeyBinds = new Label();
            chkEnableGlobalHotkeys = new CheckBox();
            grpKeyBinds = new GroupBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            lblStartRecording = new Label();
            lblPauseRecording = new Label();
            lblWindowSnap = new Label();
            lblScreenSnap = new Label();
            lblAllScreensSnap = new Label();
            txtStartRecording = new KeyBindTextBox();
            txtPauseRecording = new KeyBindTextBox();
            txtWindowSnap = new KeyBindTextBox();
            txtScreenSnap = new KeyBindTextBox();
            txtAllScreensSnap = new KeyBindTextBox();
            btnResetDefaults = new Button();
            lblInstructions = new Label();
            toolTip1 = new ToolTip(components);
            grpKeyBinds.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // lblNote
            // 
            lblNote.ForeColor = SystemColors.GrayText;
            lblNote.Location = new Point(3, 25);
            lblNote.Name = "lblNote";
            lblNote.Size = new Size(500, 32);
            lblNote.TabIndex = 0;
            lblNote.Text = "Configure keyboard shortcuts for recording actions:";
            // 
            // lblKeyBinds
            // 
            lblKeyBinds.AutoSize = true;
            lblKeyBinds.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblKeyBinds.Location = new Point(3, 70);
            lblKeyBinds.Name = "lblKeyBinds";
            lblKeyBinds.Size = new Size(75, 19);
            lblKeyBinds.TabIndex = 1;
            lblKeyBinds.Text = "Key Binds";
            // 
            // chkEnableGlobalHotkeys
            // 
            chkEnableGlobalHotkeys.AutoSize = true;
            chkEnableGlobalHotkeys.Location = new Point(20, 95);
            chkEnableGlobalHotkeys.Name = "chkEnableGlobalHotkeys";
            chkEnableGlobalHotkeys.Size = new Size(180, 24);
            chkEnableGlobalHotkeys.TabIndex = 2;
            chkEnableGlobalHotkeys.Text = "Enable global hotkeys";
            chkEnableGlobalHotkeys.UseVisualStyleBackColor = true;
            chkEnableGlobalHotkeys.CheckedChanged += chkEnableGlobalHotkeys_CheckedChanged;
            // 
            // grpKeyBinds
            // 
            grpKeyBinds.Controls.Add(tableLayoutPanel1);
            grpKeyBinds.Location = new Point(20, 125);
            grpKeyBinds.Name = "grpKeyBinds";
            grpKeyBinds.Size = new Size(450, 200);
            grpKeyBinds.TabIndex = 3;
            grpKeyBinds.TabStop = false;
            grpKeyBinds.Text = "Hotkey Assignments";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(lblStartRecording, 0, 0);
            tableLayoutPanel1.Controls.Add(lblPauseRecording, 0, 1);
            tableLayoutPanel1.Controls.Add(lblWindowSnap, 0, 2);
            tableLayoutPanel1.Controls.Add(lblScreenSnap, 0, 3);
            tableLayoutPanel1.Controls.Add(lblAllScreensSnap, 0, 4);
            tableLayoutPanel1.Controls.Add(txtStartRecording, 1, 0);
            tableLayoutPanel1.Controls.Add(txtPauseRecording, 1, 1);
            tableLayoutPanel1.Controls.Add(txtWindowSnap, 1, 2);
            tableLayoutPanel1.Controls.Add(txtScreenSnap, 1, 3);
            tableLayoutPanel1.Controls.Add(txtAllScreensSnap, 1, 4);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 23);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(10, 5, 10, 5);
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.Size = new Size(444, 174);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // lblStartRecording
            // 
            lblStartRecording.Anchor = AnchorStyles.Left;
            lblStartRecording.AutoSize = true;
            lblStartRecording.Location = new Point(13, 13);
            lblStartRecording.Name = "lblStartRecording";
            lblStartRecording.Size = new Size(112, 20);
            lblStartRecording.TabIndex = 0;
            lblStartRecording.Text = "Start Recording:";
            // 
            // lblPauseRecording
            // 
            lblPauseRecording.Anchor = AnchorStyles.Left;
            lblPauseRecording.AutoSize = true;
            lblPauseRecording.Location = new Point(13, 45);
            lblPauseRecording.Name = "lblPauseRecording";
            lblPauseRecording.Size = new Size(119, 20);
            lblPauseRecording.TabIndex = 1;
            lblPauseRecording.Text = "Pause Recording:";
            // 
            // lblWindowSnap
            // 
            lblWindowSnap.Anchor = AnchorStyles.Left;
            lblWindowSnap.AutoSize = true;
            lblWindowSnap.Location = new Point(13, 78);
            lblWindowSnap.Name = "lblWindowSnap";
            lblWindowSnap.Size = new Size(102, 20);
            lblWindowSnap.TabIndex = 2;
            lblWindowSnap.Text = "Window Snap:";
            // 
            // lblScreenSnap
            // 
            lblScreenSnap.Anchor = AnchorStyles.Left;
            lblScreenSnap.AutoSize = true;
            lblScreenSnap.Location = new Point(13, 110);
            lblScreenSnap.Name = "lblScreenSnap";
            lblScreenSnap.Size = new Size(93, 20);
            lblScreenSnap.TabIndex = 3;
            lblScreenSnap.Text = "Screen Snap:";
            // 
            // lblAllScreensSnap
            // 
            lblAllScreensSnap.Anchor = AnchorStyles.Left;
            lblAllScreensSnap.AutoSize = true;
            lblAllScreensSnap.Location = new Point(13, 143);
            lblAllScreensSnap.Name = "lblAllScreensSnap";
            lblAllScreensSnap.Size = new Size(117, 20);
            lblAllScreensSnap.TabIndex = 4;
            lblAllScreensSnap.Text = "All Screens Snap:";
            // 
            // txtStartRecording
            // 
            txtStartRecording.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtStartRecording.BackColor = SystemColors.Window;
            txtStartRecording.Cursor = Cursors.Hand;
            txtStartRecording.Location = new Point(163, 10);
            txtStartRecording.Name = "txtStartRecording";
            txtStartRecording.ReadOnly = true;
            txtStartRecording.Size = new Size(268, 27);
            txtStartRecording.TabIndex = 5;
            txtStartRecording.Text = "Click to set...";
            txtStartRecording.KeyBindChanged += txtStartRecording_KeyBindChanged;
            // 
            // txtPauseRecording
            // 
            txtPauseRecording.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtPauseRecording.BackColor = SystemColors.Window;
            txtPauseRecording.Cursor = Cursors.Hand;
            txtPauseRecording.Location = new Point(163, 42);
            txtPauseRecording.Name = "txtPauseRecording";
            txtPauseRecording.ReadOnly = true;
            txtPauseRecording.Size = new Size(268, 27);
            txtPauseRecording.TabIndex = 6;
            txtPauseRecording.Text = "Click to set...";
            txtPauseRecording.KeyBindChanged += txtPauseRecording_KeyBindChanged;
            // 
            // txtWindowSnap
            // 
            txtWindowSnap.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtWindowSnap.BackColor = SystemColors.Window;
            txtWindowSnap.Cursor = Cursors.Hand;
            txtWindowSnap.Location = new Point(163, 75);
            txtWindowSnap.Name = "txtWindowSnap";
            txtWindowSnap.ReadOnly = true;
            txtWindowSnap.Size = new Size(268, 27);
            txtWindowSnap.TabIndex = 7;
            txtWindowSnap.Text = "Click to set...";
            txtWindowSnap.KeyBindChanged += txtWindowSnap_KeyBindChanged;
            // 
            // txtScreenSnap
            // 
            txtScreenSnap.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtScreenSnap.BackColor = SystemColors.Window;
            txtScreenSnap.Cursor = Cursors.Hand;
            txtScreenSnap.Location = new Point(163, 107);
            txtScreenSnap.Name = "txtScreenSnap";
            txtScreenSnap.ReadOnly = true;
            txtScreenSnap.Size = new Size(268, 27);
            txtScreenSnap.TabIndex = 8;
            txtScreenSnap.Text = "Click to set...";
            txtScreenSnap.KeyBindChanged += txtScreenSnap_KeyBindChanged;
            // 
            // txtAllScreensSnap
            // 
            txtAllScreensSnap.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtAllScreensSnap.BackColor = SystemColors.Window;
            txtAllScreensSnap.Cursor = Cursors.Hand;
            txtAllScreensSnap.Location = new Point(163, 140);
            txtAllScreensSnap.Name = "txtAllScreensSnap";
            txtAllScreensSnap.ReadOnly = true;
            txtAllScreensSnap.Size = new Size(268, 27);
            txtAllScreensSnap.TabIndex = 9;
            txtAllScreensSnap.Text = "Click to set...";
            txtAllScreensSnap.KeyBindChanged += txtAllScreensSnap_KeyBindChanged;
            // 
            // btnResetDefaults
            // 
            btnResetDefaults.Location = new Point(20, 335);
            btnResetDefaults.Name = "btnResetDefaults";
            btnResetDefaults.Size = new Size(130, 30);
            btnResetDefaults.TabIndex = 4;
            btnResetDefaults.Text = "Reset to Defaults";
            btnResetDefaults.UseVisualStyleBackColor = true;
            btnResetDefaults.Click += btnResetDefaults_Click;
            // 
            // lblInstructions
            // 
            lblInstructions.ForeColor = SystemColors.GrayText;
            lblInstructions.Location = new Point(20, 375);
            lblInstructions.Name = "lblInstructions";
            lblInstructions.Size = new Size(450, 60);
            lblInstructions.TabIndex = 5;
            lblInstructions.Text = "Click on a field and press the desired key combination.\nPress Escape to clear a binding.\nNote: Key binds require at least one modifier (Ctrl, Shift, or Alt).";
            // 
            // KeyBindsSettings
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblInstructions);
            Controls.Add(btnResetDefaults);
            Controls.Add(grpKeyBinds);
            Controls.Add(chkEnableGlobalHotkeys);
            Controls.Add(lblKeyBinds);
            Controls.Add(lblNote);
            Name = "KeyBindsSettings";
            Size = new Size(500, 450);
            grpKeyBinds.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNote;
        private Label lblKeyBinds;
        private CheckBox chkEnableGlobalHotkeys;
        private GroupBox grpKeyBinds;
        private TableLayoutPanel tableLayoutPanel1;
        private Label lblStartRecording;
        private Label lblPauseRecording;
        private Label lblWindowSnap;
        private Label lblScreenSnap;
        private Label lblAllScreensSnap;
        private KeyBindTextBox txtStartRecording;
        private KeyBindTextBox txtPauseRecording;
        private KeyBindTextBox txtWindowSnap;
        private KeyBindTextBox txtScreenSnap;
        private KeyBindTextBox txtAllScreensSnap;
        private Button btnResetDefaults;
        private Label lblInstructions;
        private ToolTip toolTip1;
    }
}
