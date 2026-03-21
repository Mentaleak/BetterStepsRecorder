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
            lblNote.Size = new Size(360, 20);
            lblNote.TabIndex = 0;
            lblNote.Text = "Choose how screenshots are captured for click actions:";
            // 
            // rdoActiveWindow
            // 
            rdoActiveWindow.AutoSize = true;
            rdoActiveWindow.Location = new Point(23, 61);
            rdoActiveWindow.Name = "rdoActiveWindow";
            rdoActiveWindow.Size = new Size(330, 24);
            rdoActiveWindow.TabIndex = 1;
            rdoActiveWindow.TabStop = true;
            rdoActiveWindow.Text = "Active window  — window containing the click";
            rdoActiveWindow.UseVisualStyleBackColor = true;
            // 
            // rdoActiveScreen
            // 
            rdoActiveScreen.AutoSize = true;
            rdoActiveScreen.Location = new Point(23, 91);
            rdoActiveScreen.Name = "rdoActiveScreen";
            rdoActiveScreen.Size = new Size(320, 24);
            rdoActiveScreen.TabIndex = 2;
            rdoActiveScreen.TabStop = true;
            rdoActiveScreen.Text = "Active screen  — screen containing the click";
            rdoActiveScreen.UseVisualStyleBackColor = true;
            // 
            // rdoAllScreens
            // 
            rdoAllScreens.AutoSize = true;
            rdoAllScreens.Location = new Point(23, 121);
            rdoAllScreens.Name = "rdoAllScreens";
            rdoAllScreens.Size = new Size(360, 24);
            rdoAllScreens.TabIndex = 3;
            rdoAllScreens.TabStop = true;
            rdoAllScreens.Text = "All screens  — entire virtual desktop captured";
            rdoAllScreens.UseVisualStyleBackColor = true;
            // 
            // ScreenshotClick
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(rdoAllScreens);
            Controls.Add(rdoActiveScreen);
            Controls.Add(rdoActiveWindow);
            Controls.Add(lblNote);
            Name = "ScreenshotClick";
            Size = new Size(472, 174);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNote;
        private RadioButton rdoActiveWindow;
        private RadioButton rdoActiveScreen;
        private RadioButton rdoAllScreens;
    }
}
