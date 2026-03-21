namespace BetterStepsRecorder.UI.Settings
{
    partial class ScreenshotDrag
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
            lblNote.Size = new Size(360, 20);
            lblNote.TabIndex = 0;
            lblNote.Text = "Choose how screenshots are captured for drag actions:";
            // 
            // rdoCropped
            // 
            rdoCropped.AutoSize = true;
            rdoCropped.Location = new Point(23, 61);
            rdoCropped.Name = "rdoCropped";
            rdoCropped.Size = new Size(280, 24);
            rdoCropped.TabIndex = 1;
            rdoCropped.TabStop = true;
            rdoCropped.Text = "Cropped  — tight crop around the drag path";
            rdoCropped.UseVisualStyleBackColor = true;
            // 
            // rdoActiveWindow
            // 
            rdoActiveWindow.AutoSize = true;
            rdoActiveWindow.Location = new Point(23, 91);
            rdoActiveWindow.Name = "rdoActiveWindow";
            rdoActiveWindow.Size = new Size(330, 24);
            rdoActiveWindow.TabIndex = 2;
            rdoActiveWindow.TabStop = true;
            rdoActiveWindow.Text = "Active window  — window containing the drag";
            rdoActiveWindow.UseVisualStyleBackColor = true;
            // 
            // rdoActiveScreen
            // 
            rdoActiveScreen.AutoSize = true;
            rdoActiveScreen.Location = new Point(23, 121);
            rdoActiveScreen.Name = "rdoActiveScreen";
            rdoActiveScreen.Size = new Size(320, 24);
            rdoActiveScreen.TabIndex = 3;
            rdoActiveScreen.TabStop = true;
            rdoActiveScreen.Text = "Active screen  — screen containing the drag";
            rdoActiveScreen.UseVisualStyleBackColor = true;
            // 
            // rdoAllScreens
            // 
            rdoAllScreens.AutoSize = true;
            rdoAllScreens.Location = new Point(23, 151);
            rdoAllScreens.Name = "rdoAllScreens";
            rdoAllScreens.Size = new Size(360, 24);
            rdoAllScreens.TabIndex = 4;
            rdoAllScreens.TabStop = true;
            rdoAllScreens.Text = "All screens  — entire virtual desktop captured";
            rdoAllScreens.UseVisualStyleBackColor = true;
            // 
            // lblFallback
            // 
            lblFallback.AutoSize = true;
            lblFallback.Location = new Point(43, 185);
            lblFallback.Name = "lblFallback";
            lblFallback.Size = new Size(280, 20);
            lblFallback.TabIndex = 5;
            lblFallback.Text = "Fallback when drag spans multiple windows:";
            // 
            // cmbFallbackMode
            // 
            cmbFallbackMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFallbackMode.FormattingEnabled = true;
            cmbFallbackMode.Location = new Point(43, 210);
            cmbFallbackMode.Name = "cmbFallbackMode";
            cmbFallbackMode.Size = new Size(200, 28);
            cmbFallbackMode.TabIndex = 6;
            // 
            // ScreenshotDrag
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(cmbFallbackMode);
            Controls.Add(lblFallback);
            Controls.Add(rdoAllScreens);
            Controls.Add(rdoActiveScreen);
            Controls.Add(rdoActiveWindow);
            Controls.Add(rdoCropped);
            Controls.Add(lblNote);
            Name = "ScreenshotDrag";
            Size = new Size(472, 260);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNote;
        private RadioButton rdoCropped;
        private RadioButton rdoActiveWindow;
        private RadioButton rdoActiveScreen;
        private RadioButton rdoAllScreens;
        private Label lblFallback;
        private ComboBox cmbFallbackMode;
    }
}
