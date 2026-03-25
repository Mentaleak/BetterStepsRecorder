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
            rdoCropped.Tag = ClickScreenshotMode.Cropped;
            rdoCropped.Text = "Cropped  — tight crop around the click point";
            rdoCropped.UseVisualStyleBackColor = true;
            rdoCropped.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // rdoActiveWindow
            // 
            rdoActiveWindow.AutoSize = true;
            rdoActiveWindow.Location = new Point(23, 91);
            rdoActiveWindow.Name = "rdoActiveWindow";
            rdoActiveWindow.Size = new Size(335, 24);
            rdoActiveWindow.TabIndex = 2;
            rdoActiveWindow.TabStop = true;
            rdoActiveWindow.Tag = ClickScreenshotMode.ActiveWindow;
            rdoActiveWindow.Text = "Active window  — window containing the click";
            rdoActiveWindow.UseVisualStyleBackColor = true;
            rdoActiveWindow.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // rdoActiveScreen
            // 
            rdoActiveScreen.AutoSize = true;
            rdoActiveScreen.Location = new Point(23, 121);
            rdoActiveScreen.Name = "rdoActiveScreen";
            rdoActiveScreen.Size = new Size(315, 24);
            rdoActiveScreen.TabIndex = 3;
            rdoActiveScreen.TabStop = true;
            rdoActiveScreen.Tag = ClickScreenshotMode.ActiveScreen;
            rdoActiveScreen.Text = "Active screen  — screen containing the click";
            rdoActiveScreen.UseVisualStyleBackColor = true;
            rdoActiveScreen.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // rdoAllScreens
            // 
            rdoAllScreens.AutoSize = true;
            rdoAllScreens.Location = new Point(23, 151);
            rdoAllScreens.Name = "rdoAllScreens";
            rdoAllScreens.Size = new Size(327, 24);
            rdoAllScreens.TabIndex = 4;
            rdoAllScreens.TabStop = true;
            rdoAllScreens.Tag = ClickScreenshotMode.AllScreens;
            rdoAllScreens.Text = "All screens  — entire virtual desktop captured";
            rdoAllScreens.UseVisualStyleBackColor = true;
            rdoAllScreens.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // ScreenshotClick
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(rdoAllScreens);
            Controls.Add(rdoActiveScreen);
            Controls.Add(rdoActiveWindow);
            Controls.Add(rdoCropped);
            Controls.Add(lblNote);
            Name = "ScreenshotClick";
            Size = new Size(472, 190);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNote;
        private RadioButton rdoCropped;
        private RadioButton rdoActiveWindow;
        private RadioButton rdoActiveScreen;
        private RadioButton rdoAllScreens;
    }
}
