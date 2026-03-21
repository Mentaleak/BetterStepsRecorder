namespace BetterStepsRecorder.UI.Settings
{
    partial class ScreenshotClick
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
            rdoCropped = new RadioButton();
            rdoActiveWindow = new RadioButton();
            rdoActiveScreen = new RadioButton();
            rdoAllScreens = new RadioButton();
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
            lblNote.Size = new Size(368, 20);
            lblNote.TabIndex = 0;
            lblNote.Text = "Choose how screenshots are captured for click actions:";
            // 
            // rdoCropped
            // 
            rdoCropped.AutoSize = true;
            rdoCropped.Location = new Point(23, 61);
            rdoCropped.Name = "rdoCropped";
            rdoCropped.Size = new Size(325, 24);
            rdoCropped.TabIndex = 1;
            rdoCropped.TabStop = true;
            rdoCropped.Text = "Cropped  — tight crop around the click point";
            rdoCropped.UseVisualStyleBackColor = true;
            // 
            // rdoActiveWindow
            // 
            rdoActiveWindow.AutoSize = true;
            rdoActiveWindow.Location = new Point(23, 91);
            rdoActiveWindow.Name = "rdoActiveWindow";
            rdoActiveWindow.Size = new Size(335, 24);
            rdoActiveWindow.TabIndex = 2;
            rdoActiveWindow.TabStop = true;
            rdoActiveWindow.Text = "Active window  — window containing the click";
            rdoActiveWindow.UseVisualStyleBackColor = true;
            // 
            // rdoActiveScreen
            // 
            rdoActiveScreen.AutoSize = true;
            rdoActiveScreen.Location = new Point(23, 121);
            rdoActiveScreen.Name = "rdoActiveScreen";
            rdoActiveScreen.Size = new Size(315, 24);
            rdoActiveScreen.TabIndex = 3;
            rdoActiveScreen.TabStop = true;
            rdoActiveScreen.Text = "Active screen  — screen containing the click";
            rdoActiveScreen.UseVisualStyleBackColor = true;
            // 
            // rdoAllScreens
            // 
            rdoAllScreens.AutoSize = true;
            rdoAllScreens.Location = new Point(23, 151);
            rdoAllScreens.Name = "rdoAllScreens";
            rdoAllScreens.Size = new Size(327, 24);
            rdoAllScreens.TabIndex = 4;
            rdoAllScreens.TabStop = true;
            rdoAllScreens.Text = "All screens  — entire virtual desktop captured";
            rdoAllScreens.UseVisualStyleBackColor = true;
            // 
            // lblPadding
            // 
            lblPadding.AutoSize = true;
            lblPadding.Location = new Point(43, 185);
            lblPadding.Name = "lblPadding";
            lblPadding.Size = new Size(160, 20);
            lblPadding.TabIndex = 5;
            lblPadding.Text = "Cropped padding (px):";
            // 
            // nudPadding
            // 
            nudPadding.Location = new Point(209, 185);
            nudPadding.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            nudPadding.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            nudPadding.Name = "nudPadding";
            nudPadding.Size = new Size(80, 27);
            nudPadding.TabIndex = 6;
            nudPadding.Value = new decimal(new int[] { 200, 0, 0, 0 });
            // 
            // ScreenshotClick
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(nudPadding);
            Controls.Add(lblPadding);
            Controls.Add(rdoAllScreens);
            Controls.Add(rdoActiveScreen);
            Controls.Add(rdoActiveWindow);
            Controls.Add(rdoCropped);
            Controls.Add(lblNote);
            Name = "ScreenshotClick";
            Size = new Size(472, 220);
            ((System.ComponentModel.ISupportInitialize)nudPadding).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNote;
        private RadioButton rdoCropped;
        private RadioButton rdoActiveWindow;
        private RadioButton rdoActiveScreen;
        private RadioButton rdoAllScreens;
        private Label lblPadding;
        private NumericUpDown nudPadding;
    }
}
