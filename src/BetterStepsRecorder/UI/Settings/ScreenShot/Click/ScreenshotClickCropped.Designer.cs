namespace BetterStepsRecorder.UI.Settings
{
    partial class ScreenshotClickCropped
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
            lblPadding = new Label();
            nudPadding = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)nudPadding).BeginInit();
            SuspendLayout();
            // 
            // lblNote
            // 
            lblNote.AutoSize = true;
            lblNote.ForeColor = SystemColors.GrayText;
            lblNote.Location = new Point(3, 25);
            lblNote.Name = "lblNote";
            lblNote.Size = new Size(380, 20);
            lblNote.TabIndex = 0;
            lblNote.Text = "Configure padding around the click point for cropped screenshots:";
            // 
            // lblPadding
            // 
            lblPadding.AutoSize = true;
            lblPadding.Location = new Point(23, 65);
            lblPadding.Name = "lblPadding";
            lblPadding.Size = new Size(95, 20);
            lblPadding.TabIndex = 1;
            lblPadding.Text = "Padding (px):";
            // 
            // nudPadding
            // 
            nudPadding.Location = new Point(124, 63);
            nudPadding.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            nudPadding.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            nudPadding.Name = "nudPadding";
            nudPadding.Size = new Size(80, 27);
            nudPadding.TabIndex = 2;
            nudPadding.Value = new decimal(new int[] { 200, 0, 0, 0 });
            // 
            // ScreenshotClickCropped
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(nudPadding);
            Controls.Add(lblPadding);
            Controls.Add(lblNote);
            Name = "ScreenshotClickCropped";
            Size = new Size(472, 120);
            ((System.ComponentModel.ISupportInitialize)nudPadding).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNote;
        private Label lblPadding;
        private NumericUpDown nudPadding;
    }
}
