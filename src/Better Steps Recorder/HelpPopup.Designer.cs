namespace Better_Steps_Recorder
{
    partial class HelpPopup
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button_CloseHelp = new Button();
            label1 = new Label();
            linkLabel1 = new LinkLabel();
            pictureBox2 = new PictureBox();
            VersionLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // button_CloseHelp
            // 
            button_CloseHelp.Location = new Point(378, 85);
            button_CloseHelp.Name = "button_CloseHelp";
            button_CloseHelp.Size = new Size(82, 23);
            button_CloseHelp.TabIndex = 0;
            button_CloseHelp.Text = "Close Form";
            button_CloseHelp.UseVisualStyleBackColor = true;
            button_CloseHelp.Click += button_CloseHelp_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(116, 9);
            label1.MaximumSize = new Size(350, 300);
            label1.Name = "label1";
            label1.Size = new Size(344, 45);
            label1.TabIndex = 1;
            label1.Text = "Welcome to the Better Steps Recorder help menu.\r\nThis tool helps you record steps and take screenshots efficiently.\r\nFor more details and instructions, visit our GitHub repository.";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(116, 54);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(160, 15);
            linkLabel1.TabIndex = 3;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "GitHub Better Steps Recorder";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.StepsRecorder;
            pictureBox2.InitialImage = Properties.Resources.StepsRecorder;
            pictureBox2.Location = new Point(10, 12);
            pictureBox2.MaximumSize = new Size(96, 96);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(96, 96);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 4;
            pictureBox2.TabStop = false;
            // 
            // VersionLabel
            // 
            VersionLabel.AutoSize = true;
            VersionLabel.Location = new Point(118, 73);
            VersionLabel.Name = "VersionLabel";
            VersionLabel.Size = new Size(48, 15);
            VersionLabel.TabIndex = 5;
            VersionLabel.Text = "Version:";
            // 
            // HelpPopup
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(472, 120);
            Controls.Add(VersionLabel);
            Controls.Add(pictureBox2);
            Controls.Add(linkLabel1);
            Controls.Add(label1);
            Controls.Add(button_CloseHelp);
            Name = "HelpPopup";
            ShowIcon = false;
            Text = "Help";
            TopMost = true;
            Load += HelpPopup_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button_CloseHelp;
        private Label label1;
        private LinkLabel linkLabel1;
        private PictureBox pictureBox2;
        private Label VersionLabel;
    }
}