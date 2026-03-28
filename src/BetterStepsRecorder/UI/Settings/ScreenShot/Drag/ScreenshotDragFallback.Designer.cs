namespace BetterStepsRecorder.UI.Settings
{
    partial class ScreenshotDragFallback
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
            lblFallback = new Label();
            cmbFallbackMode = new ComboBox();
            SuspendLayout();
            // 
            // lblNote
            // 
            lblNote.AutoSize = true;
            lblNote.ForeColor = SystemColors.GrayText;
            lblNote.Location = new Point(3, 25);
            lblNote.Name = "lblNote";
            lblNote.Size = new Size(400, 20);
            lblNote.TabIndex = 0;
            lblNote.Text = "Choose the fallback mode when a drag spans multiple windows:";
            // 
            // lblFallback
            // 
            lblFallback.AutoSize = true;
            lblFallback.Location = new Point(23, 65);
            lblFallback.Name = "lblFallback";
            lblFallback.Size = new Size(105, 20);
            lblFallback.TabIndex = 1;
            lblFallback.Text = "Fallback mode:";
            // 
            // cmbFallbackMode
            // 
            cmbFallbackMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFallbackMode.FormattingEnabled = true;
            cmbFallbackMode.Location = new Point(134, 62);
            cmbFallbackMode.Name = "cmbFallbackMode";
            cmbFallbackMode.Size = new Size(200, 28);
            cmbFallbackMode.TabIndex = 2;
            // 
            // ScreenshotDragFallback
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(cmbFallbackMode);
            Controls.Add(lblFallback);
            Controls.Add(lblNote);
            Name = "ScreenshotDragFallback";
            Size = new Size(472, 120);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNote;
        private Label lblFallback;
        private ComboBox cmbFallbackMode;
    }
}
