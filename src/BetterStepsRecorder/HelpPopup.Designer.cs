namespace BetterStepsRecorder
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
            labelUpdatesSectionHeader = new Label();
            labelUpdateStatus = new Label();
            buttonDownloadInstall = new Button();
            linkLabelReleasesPage = new LinkLabel();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // button_CloseHelp
            // 
            button_CloseHelp.Location = new Point(378, 145);
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
            //
            // labelUpdatesSectionHeader
            //
            labelUpdatesSectionHeader.AutoSize = true;
            labelUpdatesSectionHeader.Font = new Font("Segoe UI", 8.5f, System.Drawing.FontStyle.Bold);
            labelUpdatesSectionHeader.Location = new Point(116, 100);
            labelUpdatesSectionHeader.Name = "labelUpdatesSectionHeader";
            labelUpdatesSectionHeader.Size = new Size(55, 15);
            labelUpdatesSectionHeader.TabIndex = 6;
            labelUpdatesSectionHeader.Text = "Updates";
            //
            // labelUpdateStatus
            //
            labelUpdateStatus.AutoSize = true;
            labelUpdateStatus.Location = new Point(116, 120);
            labelUpdateStatus.MaximumSize = new Size(330, 0);
            labelUpdateStatus.Name = "labelUpdateStatus";
            labelUpdateStatus.Size = new Size(120, 15);
            labelUpdateStatus.TabIndex = 7;
            labelUpdateStatus.Text = "Checking for updates…";
            //
            // buttonDownloadInstall
            //
            buttonDownloadInstall.Location = new Point(116, 143);
            buttonDownloadInstall.Name = "buttonDownloadInstall";
            buttonDownloadInstall.Size = new Size(130, 23);
            buttonDownloadInstall.TabIndex = 8;
            buttonDownloadInstall.Text = "Download && Install";
            buttonDownloadInstall.UseVisualStyleBackColor = true;
            buttonDownloadInstall.Visible = false;
            buttonDownloadInstall.Click += buttonDownloadInstall_Click;
            //
            // linkLabelReleasesPage
            //
            linkLabelReleasesPage.AutoSize = true;
            linkLabelReleasesPage.Location = new Point(116, 143);
            linkLabelReleasesPage.Name = "linkLabelReleasesPage";
            linkLabelReleasesPage.Size = new Size(120, 15);
            linkLabelReleasesPage.TabIndex = 9;
            linkLabelReleasesPage.TabStop = true;
            linkLabelReleasesPage.Text = "Download from GitHub";
            linkLabelReleasesPage.Visible = false;
            linkLabelReleasesPage.LinkClicked += linkLabelReleasesPage_LinkClicked;
            //
            // HelpPopup
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(472, 180);
            Controls.Add(linkLabelReleasesPage);
            Controls.Add(buttonDownloadInstall);
            Controls.Add(labelUpdateStatus);
            Controls.Add(labelUpdatesSectionHeader);
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
        private Label labelUpdatesSectionHeader;
        private Label labelUpdateStatus;
        private Button buttonDownloadInstall;
        private LinkLabel linkLabelReleasesPage;
    }
}