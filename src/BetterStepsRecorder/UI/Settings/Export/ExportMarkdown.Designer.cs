namespace BetterStepsRecorder.UI.Settings
{
    partial class ExportMarkdown
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
            lblHeader = new Label();
            chkSummary = new CheckBox();
            chkGeneratedDate = new CheckBox();
            lblPerStep = new Label();
            chkStepTimestamps = new CheckBox();
            lblDetailTable = new Label();
            chkAction = new CheckBox();
            chkApplication = new CheckBox();
            chkWindow = new CheckBox();
            chkElement = new CheckBox();
            chkElementType = new CheckBox();
            chkMousePosition = new CheckBox();
            SuspendLayout();
            // 
            // lblNote
            // 
            lblNote.ForeColor = SystemColors.GrayText;
            lblNote.Location = new Point(3, 12);
            lblNote.Name = "lblNote";
            lblNote.Size = new Size(450, 46);
            lblNote.TabIndex = 0;
            lblNote.Text = "Choose which metadata to include in the Markdown export.\r\nImages are saved to an 'images' subfolder and linked in the .md file.";
            // 
            // lblHeader
            // 
            lblHeader.AutoSize = true;
            lblHeader.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblHeader.Location = new Point(3, 70);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new Size(53, 19);
            lblHeader.TabIndex = 1;
            lblHeader.Text = "Header";
            // 
            // chkSummary
            // 
            chkSummary.AutoSize = true;
            chkSummary.Location = new Point(20, 95);
            chkSummary.Name = "chkSummary";
            chkSummary.Size = new Size(343, 24);
            chkSummary.TabIndex = 2;
            chkSummary.Text = "Summary table (Steps / Start / End / Duration)";
            chkSummary.UseVisualStyleBackColor = true;
            // 
            // chkGeneratedDate
            // 
            chkGeneratedDate.AutoSize = true;
            chkGeneratedDate.Location = new Point(20, 119);
            chkGeneratedDate.Name = "chkGeneratedDate";
            chkGeneratedDate.Size = new Size(137, 24);
            chkGeneratedDate.TabIndex = 3;
            chkGeneratedDate.Text = "Generated date";
            chkGeneratedDate.UseVisualStyleBackColor = true;
            // 
            // lblPerStep
            // 
            lblPerStep.AutoSize = true;
            lblPerStep.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblPerStep.Location = new Point(3, 155);
            lblPerStep.Name = "lblPerStep";
            lblPerStep.Size = new Size(65, 19);
            lblPerStep.TabIndex = 4;
            lblPerStep.Text = "Per-Step";
            // 
            // chkStepTimestamps
            // 
            chkStepTimestamps.AutoSize = true;
            chkStepTimestamps.Location = new Point(20, 180);
            chkStepTimestamps.Name = "chkStepTimestamps";
            chkStepTimestamps.Size = new Size(147, 24);
            chkStepTimestamps.TabIndex = 5;
            chkStepTimestamps.Text = "Step timestamps";
            chkStepTimestamps.UseVisualStyleBackColor = true;
            // 
            // lblDetailTable
            // 
            lblDetailTable.AutoSize = true;
            lblDetailTable.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblDetailTable.Location = new Point(3, 216);
            lblDetailTable.Name = "lblDetailTable";
            lblDetailTable.Size = new Size(88, 19);
            lblDetailTable.TabIndex = 6;
            lblDetailTable.Text = "Detail Table";
            // 
            // chkAction
            // 
            chkAction.AutoSize = true;
            chkAction.Location = new Point(20, 241);
            chkAction.Name = "chkAction";
            chkAction.Size = new Size(75, 24);
            chkAction.TabIndex = 7;
            chkAction.Text = "Action";
            chkAction.UseVisualStyleBackColor = true;
            // 
            // chkApplication
            // 
            chkApplication.AutoSize = true;
            chkApplication.Location = new Point(20, 265);
            chkApplication.Name = "chkApplication";
            chkApplication.Size = new Size(110, 24);
            chkApplication.TabIndex = 8;
            chkApplication.Text = "Application";
            chkApplication.UseVisualStyleBackColor = true;
            // 
            // chkWindow
            // 
            chkWindow.AutoSize = true;
            chkWindow.Location = new Point(20, 289);
            chkWindow.Name = "chkWindow";
            chkWindow.Size = new Size(87, 24);
            chkWindow.TabIndex = 9;
            chkWindow.Text = "Window";
            chkWindow.UseVisualStyleBackColor = true;
            // 
            // chkElement
            // 
            chkElement.AutoSize = true;
            chkElement.Location = new Point(20, 313);
            chkElement.Name = "chkElement";
            chkElement.Size = new Size(86, 24);
            chkElement.TabIndex = 10;
            chkElement.Text = "Element";
            chkElement.UseVisualStyleBackColor = true;
            // 
            // chkElementType
            // 
            chkElementType.AutoSize = true;
            chkElementType.Location = new Point(20, 337);
            chkElementType.Name = "chkElementType";
            chkElementType.Size = new Size(121, 24);
            chkElementType.TabIndex = 11;
            chkElementType.Text = "Element Type";
            chkElementType.UseVisualStyleBackColor = true;
            // 
            // chkMousePosition
            // 
            chkMousePosition.AutoSize = true;
            chkMousePosition.Location = new Point(20, 361);
            chkMousePosition.Name = "chkMousePosition";
            chkMousePosition.Size = new Size(133, 24);
            chkMousePosition.TabIndex = 12;
            chkMousePosition.Text = "Mouse Position";
            chkMousePosition.UseVisualStyleBackColor = true;
            // 
            // ExportMarkdown
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(chkMousePosition);
            Controls.Add(chkElementType);
            Controls.Add(chkElement);
            Controls.Add(chkWindow);
            Controls.Add(chkApplication);
            Controls.Add(chkAction);
            Controls.Add(lblDetailTable);
            Controls.Add(chkStepTimestamps);
            Controls.Add(lblPerStep);
            Controls.Add(chkGeneratedDate);
            Controls.Add(chkSummary);
            Controls.Add(lblHeader);
            Controls.Add(lblNote);
            Name = "ExportMarkdown";
            Size = new Size(472, 400);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNote;
        private Label lblHeader;
        private CheckBox chkSummary;
        private CheckBox chkGeneratedDate;
        private Label lblPerStep;
        private CheckBox chkStepTimestamps;
        private Label lblDetailTable;
        private CheckBox chkAction;
        private CheckBox chkApplication;
        private CheckBox chkWindow;
        private CheckBox chkElement;
        private CheckBox chkElementType;
        private CheckBox chkMousePosition;
    }
}
