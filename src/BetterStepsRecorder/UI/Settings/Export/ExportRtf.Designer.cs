namespace BetterStepsRecorder.UI.Settings
{
    partial class ExportRtf
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            lblNote = new Label();
            lblHeader = new Label();
            chkSummary = new CheckBox();
            chkGeneratedDate = new CheckBox();
            lblPerStep = new Label();
            chkStepTimestamps = new CheckBox();
            lblDetailStrip = new Label();
            chkAction = new CheckBox();
            chkApplication = new CheckBox();
            chkWindow = new CheckBox();
            chkElement = new CheckBox();
            chkElementType = new CheckBox();
            chkMousePosition = new CheckBox();
            SuspendLayout();
            
            lblNote.ForeColor = System.Drawing.SystemColors.GrayText;
            lblNote.Location = new System.Drawing.Point(3, 12);
            lblNote.Name = "lblNote";
            lblNote.Size = new System.Drawing.Size(450, 46);
            lblNote.TabIndex = 0;
            lblNote.Text = "Choose which metadata to include in the RTF export.\r\nThe step description and screenshot are always included.";
            
            lblHeader.AutoSize = true;
            lblHeader.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            lblHeader.Location = new System.Drawing.Point(3, 70);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new System.Drawing.Size(53, 19);
            lblHeader.TabIndex = 1;
            lblHeader.Text = "Header";
            
            chkSummary.AutoSize = true;
            chkSummary.Location = new System.Drawing.Point(20, 95);
            chkSummary.Name = "chkSummary";
            chkSummary.Size = new System.Drawing.Size(343, 24);
            chkSummary.TabIndex = 2;
            chkSummary.Text = "Summary bar (Steps / Start / End / Duration)";
            chkSummary.UseVisualStyleBackColor = true;
            
            chkGeneratedDate.AutoSize = true;
            chkGeneratedDate.Location = new System.Drawing.Point(20, 119);
            chkGeneratedDate.Name = "chkGeneratedDate";
            chkGeneratedDate.Size = new System.Drawing.Size(137, 24);
            chkGeneratedDate.TabIndex = 3;
            chkGeneratedDate.Text = "Generated date";
            chkGeneratedDate.UseVisualStyleBackColor = true;
            
            lblPerStep.AutoSize = true;
            lblPerStep.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            lblPerStep.Location = new System.Drawing.Point(3, 155);
            lblPerStep.Name = "lblPerStep";
            lblPerStep.Size = new System.Drawing.Size(65, 19);
            lblPerStep.TabIndex = 4;
            lblPerStep.Text = "Per-Step";
            
            chkStepTimestamps.AutoSize = true;
            chkStepTimestamps.Location = new System.Drawing.Point(20, 180);
            chkStepTimestamps.Name = "chkStepTimestamps";
            chkStepTimestamps.Size = new System.Drawing.Size(147, 24);
            chkStepTimestamps.TabIndex = 5;
            chkStepTimestamps.Text = "Step timestamps";
            chkStepTimestamps.UseVisualStyleBackColor = true;
            
            lblDetailStrip.AutoSize = true;
            lblDetailStrip.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            lblDetailStrip.Location = new System.Drawing.Point(3, 216);
            lblDetailStrip.Name = "lblDetailStrip";
            lblDetailStrip.Size = new System.Drawing.Size(83, 19);
            lblDetailStrip.TabIndex = 6;
            lblDetailStrip.Text = "Detail Strip";
            
            chkAction.AutoSize = true;
            chkAction.Location = new System.Drawing.Point(20, 241);
            chkAction.Name = "chkAction";
            chkAction.Size = new System.Drawing.Size(75, 24);
            chkAction.TabIndex = 7;
            chkAction.Text = "Action";
            chkAction.UseVisualStyleBackColor = true;
            
            chkApplication.AutoSize = true;
            chkApplication.Location = new System.Drawing.Point(20, 265);
            chkApplication.Name = "chkApplication";
            chkApplication.Size = new System.Drawing.Size(110, 24);
            chkApplication.TabIndex = 8;
            chkApplication.Text = "Application";
            chkApplication.UseVisualStyleBackColor = true;
            
            chkWindow.AutoSize = true;
            chkWindow.Location = new System.Drawing.Point(20, 289);
            chkWindow.Name = "chkWindow";
            chkWindow.Size = new System.Drawing.Size(87, 24);
            chkWindow.TabIndex = 9;
            chkWindow.Text = "Window";
            chkWindow.UseVisualStyleBackColor = true;
            
            chkElement.AutoSize = true;
            chkElement.Location = new System.Drawing.Point(20, 313);
            chkElement.Name = "chkElement";
            chkElement.Size = new System.Drawing.Size(86, 24);
            chkElement.TabIndex = 10;
            chkElement.Text = "Element";
            chkElement.UseVisualStyleBackColor = true;
            
            chkElementType.AutoSize = true;
            chkElementType.Location = new System.Drawing.Point(20, 337);
            chkElementType.Name = "chkElementType";
            chkElementType.Size = new System.Drawing.Size(121, 24);
            chkElementType.TabIndex = 11;
            chkElementType.Text = "Element Type";
            chkElementType.UseVisualStyleBackColor = true;
            
            chkMousePosition.AutoSize = true;
            chkMousePosition.Location = new System.Drawing.Point(20, 361);
            chkMousePosition.Name = "chkMousePosition";
            chkMousePosition.Size = new System.Drawing.Size(133, 24);
            chkMousePosition.TabIndex = 12;
            chkMousePosition.Text = "Mouse Position";
            chkMousePosition.UseVisualStyleBackColor = true;
            
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(chkMousePosition);
            Controls.Add(chkElementType);
            Controls.Add(chkElement);
            Controls.Add(chkWindow);
            Controls.Add(chkApplication);
            Controls.Add(chkAction);
            Controls.Add(lblDetailStrip);
            Controls.Add(chkStepTimestamps);
            Controls.Add(lblPerStep);
            Controls.Add(chkGeneratedDate);
            Controls.Add(chkSummary);
            Controls.Add(lblHeader);
            Controls.Add(lblNote);
            Name = "ExportRtf";
            Size = new System.Drawing.Size(472, 400);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.CheckBox chkSummary;
        private System.Windows.Forms.CheckBox chkGeneratedDate;
        private System.Windows.Forms.Label lblPerStep;
        private System.Windows.Forms.CheckBox chkStepTimestamps;
        private System.Windows.Forms.Label lblDetailStrip;
        private System.Windows.Forms.CheckBox chkAction;
        private System.Windows.Forms.CheckBox chkApplication;
        private System.Windows.Forms.CheckBox chkWindow;
        private System.Windows.Forms.CheckBox chkElement;
        private System.Windows.Forms.CheckBox chkElementType;
        private System.Windows.Forms.CheckBox chkMousePosition;
    }
}
