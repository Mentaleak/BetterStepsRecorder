namespace Better_Steps_Recorder
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Listbox_Events = new ListBox();
            propertyGrid_RecordEvent = new PropertyGrid();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            pictureBox1 = new PictureBox();
            menuStrip1 = new MenuStrip();
            ToolStripMenuItem_Recording = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // Listbox_Events
            // 
            Listbox_Events.Dock = DockStyle.Fill;
            Listbox_Events.FormattingEnabled = true;
            Listbox_Events.ItemHeight = 15;
            Listbox_Events.Location = new Point(0, 0);
            Listbox_Events.Name = "Listbox_Events";
            Listbox_Events.Size = new Size(374, 224);
            Listbox_Events.TabIndex = 1;
            Listbox_Events.SelectedIndexChanged += Listbox_Events_SelectedIndexChanged;
            // 
            // propertyGrid_RecordEvent
            // 
            propertyGrid_RecordEvent.BackColor = SystemColors.Control;
            propertyGrid_RecordEvent.Dock = DockStyle.Fill;
            propertyGrid_RecordEvent.Location = new Point(0, 0);
            propertyGrid_RecordEvent.Name = "propertyGrid_RecordEvent";
            propertyGrid_RecordEvent.RightToLeft = RightToLeft.Yes;
            propertyGrid_RecordEvent.Size = new Size(374, 275);
            propertyGrid_RecordEvent.TabIndex = 4;
            propertyGrid_RecordEvent.ToolbarVisible = false;
            // 
            // splitContainer1
            // 
            splitContainer1.BackColor = SystemColors.Control;
            splitContainer1.BorderStyle = BorderStyle.Fixed3D;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.BackColor = SystemColors.Control;
            splitContainer1.Panel1.Controls.Add(Listbox_Events);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.BackColor = SystemColors.ActiveCaption;
            splitContainer1.Panel2.Controls.Add(propertyGrid_RecordEvent);
            splitContainer1.Size = new Size(378, 517);
            splitContainer1.SplitterDistance = 228;
            splitContainer1.SplitterIncrement = 5;
            splitContainer1.SplitterWidth = 10;
            splitContainer1.TabIndex = 5;
            // 
            // splitContainer2
            // 
            splitContainer2.BackColor = SystemColors.Control;
            splitContainer2.BorderStyle = BorderStyle.Fixed3D;
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 24);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.BackColor = SystemColors.Control;
            splitContainer2.Panel2.Controls.Add(pictureBox1);
            splitContainer2.Size = new Size(988, 517);
            splitContainer2.SplitterDistance = 378;
            splitContainer2.SplitterIncrement = 5;
            splitContainer2.SplitterWidth = 10;
            splitContainer2.TabIndex = 7;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.ControlDarkDark;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(596, 513);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { ToolStripMenuItem_Recording });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(988, 24);
            menuStrip1.TabIndex = 8;
            menuStrip1.Text = "menuStrip1";
            // 
            // ToolStripMenuItem_Recording
            // 
            ToolStripMenuItem_Recording.Name = "ToolStripMenuItem_Recording";
            ToolStripMenuItem_Recording.Size = new Size(100, 20);
            ToolStripMenuItem_Recording.Text = "Start Recording";
            ToolStripMenuItem_Recording.Click += ToolStripMenuItem_Recording_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(988, 541);
            Controls.Add(splitContainer2);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Better Steps Recorder";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ListBox Listbox_Events;
        private PropertyGrid propertyGrid_RecordEvent;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem ToolStripMenuItem_Recording;
        private PictureBox pictureBox1;
    }
}
