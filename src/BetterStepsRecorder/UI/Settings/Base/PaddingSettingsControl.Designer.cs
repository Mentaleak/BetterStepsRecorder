namespace BetterStepsRecorder.UI.Settings.Base
{
    partial class PaddingSettingsControl
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
            label1 = new Label();
            nudPadding = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)nudPadding).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 10);
            label1.Name = "label1";
            label1.Size = new Size(95, 20);
            label1.TabIndex = 0;
            label1.Text = "Padding (px):";
            // 
            // nudPadding
            // 
            nudPadding.Location = new Point(104, 8);
            nudPadding.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            nudPadding.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            nudPadding.Name = "nudPadding";
            nudPadding.Size = new Size(80, 27);
            nudPadding.TabIndex = 1;
            nudPadding.Value = new decimal(new int[] { 200, 0, 0, 0 });
            // 
            // PaddingSettingsControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(nudPadding);
            Controls.Add(label1);
            Name = "PaddingSettingsControl";
            Size = new Size(400, 45);
            ((System.ComponentModel.ISupportInitialize)nudPadding).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private NumericUpDown nudPadding;
    }
}
