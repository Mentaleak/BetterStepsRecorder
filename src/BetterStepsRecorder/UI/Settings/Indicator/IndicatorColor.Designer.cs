namespace BetterStepsRecorder.UI.Settings
{
    partial class IndicatorColor
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
            lblCurrentColor = new Label();
            panelColorPreview = new Panel();
            lblColorValue = new Label();
            btnChooseColor = new Button();
            SuspendLayout();
            // 
            // lblNote
            // 
            lblNote.ForeColor = SystemColors.GrayText;
            lblNote.Location = new Point(3, 25);
            lblNote.Name = "lblNote";
            lblNote.Size = new Size(450, 32);
            lblNote.TabIndex = 0;
            lblNote.Text = "Choose the color for click indicators on captured screenshots:";
            // 
            // lblCurrentColor
            // 
            lblCurrentColor.AutoSize = true;
            lblCurrentColor.Location = new Point(3, 75);
            lblCurrentColor.Name = "lblCurrentColor";
            lblCurrentColor.Size = new Size(99, 20);
            lblCurrentColor.TabIndex = 1;
            lblCurrentColor.Text = "Current Color:";
            // 
            // panelColorPreview
            // 
            panelColorPreview.BorderStyle = BorderStyle.FixedSingle;
            panelColorPreview.Location = new Point(23, 105);
            panelColorPreview.Name = "panelColorPreview";
            panelColorPreview.Size = new Size(100, 60);
            panelColorPreview.TabIndex = 2;
            // 
            // lblColorValue
            // 
            lblColorValue.AutoSize = true;
            lblColorValue.Location = new Point(135, 120);
            lblColorValue.Name = "lblColorValue";
            lblColorValue.Size = new Size(115, 20);
            lblColorValue.TabIndex = 3;
            lblColorValue.Text = "RGB(255, 0, 255)";
            // 
            // btnChooseColor
            // 
            btnChooseColor.Location = new Point(23, 181);
            btnChooseColor.Name = "btnChooseColor";
            btnChooseColor.Size = new Size(120, 30);
            btnChooseColor.TabIndex = 4;
            btnChooseColor.Text = "Choose Color...";
            btnChooseColor.UseVisualStyleBackColor = true;
            btnChooseColor.Click += btnChooseColor_Click;
            // 
            // IndicatorColor
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnChooseColor);
            Controls.Add(lblColorValue);
            Controls.Add(panelColorPreview);
            Controls.Add(lblCurrentColor);
            Controls.Add(lblNote);
            Name = "IndicatorColor";
            Size = new Size(472, 280);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNote;
        private Label lblCurrentColor;
        private Panel panelColorPreview;
        private Label lblColorValue;
        private Button btnChooseColor;
    }
}
