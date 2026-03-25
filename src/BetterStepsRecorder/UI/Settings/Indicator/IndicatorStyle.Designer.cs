namespace BetterStepsRecorder.UI.Settings
{
    partial class IndicatorStyle
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
            rdoArrow = new RadioButton();
            rdoCircle = new RadioButton();
            rdoCursor = new RadioButton();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 25);
            label1.Name = "label1";
            label1.Size = new Size(401, 20);
            label1.TabIndex = 0;
            label1.Text = "Choose how clicks are highlighted on captured screenshots:";
            // 
            // rdoArrow
            // 
            rdoArrow.AutoSize = true;
            rdoArrow.Location = new Point(23, 61);
            rdoArrow.Name = "rdoArrow";
            rdoArrow.Size = new Size(308, 24);
            rdoArrow.TabIndex = 1;
            rdoArrow.TabStop = true;
            rdoArrow.Tag = ClickIndicatorStyle.Arrow;
            rdoArrow.Text = "Arrow  — long pointer arrow (original style)";
            rdoArrow.UseVisualStyleBackColor = true;
            rdoArrow.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // rdoCircle
            // 
            rdoCircle.AutoSize = true;
            rdoCircle.Location = new Point(23, 91);
            rdoCircle.Name = "rdoCircle";
            rdoCircle.Size = new Size(340, 24);
            rdoCircle.TabIndex = 2;
            rdoCircle.TabStop = true;
            rdoCircle.Tag = ClickIndicatorStyle.Circle;
            rdoCircle.Text = "Circle  — highlighted ring around the click point";
            rdoCircle.UseVisualStyleBackColor = true;
            rdoCircle.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // rdoCursor
            // 
            rdoCursor.AutoSize = true;
            rdoCursor.Location = new Point(23, 121);
            rdoCursor.Name = "rdoCursor";
            rdoCursor.Size = new Size(420, 24);
            rdoCursor.TabIndex = 3;
            rdoCursor.TabStop = true;
            rdoCursor.Tag = ClickIndicatorStyle.Cursor;
            rdoCursor.Text = "Cursor  — mouse pointer shape at the click point";
            rdoCursor.UseVisualStyleBackColor = true;
            rdoCursor.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // ArrowIndicator
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(rdoCursor);
            Controls.Add(rdoCircle);
            Controls.Add(rdoArrow);
            Controls.Add(label1);
            Name = "ArrowIndicator";
            Size = new Size(472, 174);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private RadioButton rdoArrow;
        private RadioButton rdoCircle;
        private RadioButton rdoCursor;
    }
}
