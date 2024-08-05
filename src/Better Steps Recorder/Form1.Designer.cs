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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            Listbox_Events = new ListBox();
            contextMenu_ListBox_Events = new ContextMenuStrip(components);
            moveUpToolStripMenuItem = new ToolStripMenuItem();
            moveDownToolStripMenuItem = new ToolStripMenuItem();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            propertyGrid_RecordEvent = new PropertyGrid();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            splitContainer3 = new SplitContainer();
            pictureBox1 = new PictureBox();
            richTextBox_stepText = new RichTextBox();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1_SaveAs = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exportToolStripMenuItem = new ToolStripMenuItem();
            ToolStripMenuItem_Recording = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            contextMenu_ListBox_Events.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // Listbox_Events
            // 
            Listbox_Events.AllowDrop = true;
            Listbox_Events.Dock = DockStyle.Fill;
            Listbox_Events.FormattingEnabled = true;
            Listbox_Events.ItemHeight = 15;
            Listbox_Events.Location = new Point(0, 0);
            Listbox_Events.Name = "Listbox_Events";
            Listbox_Events.Size = new Size(354, 156);
            Listbox_Events.TabIndex = 1;
            Listbox_Events.SelectedIndexChanged += Listbox_Events_SelectedIndexChanged;
            Listbox_Events.DragDrop += Listbox_Events_DragDrop;
            Listbox_Events.DragEnter += Listbox_Events_DragEnter;
            Listbox_Events.DragOver += Listbox_Events_DragOver;
            Listbox_Events.MouseDown += Listbox_Events_MouseDown;
            Listbox_Events.MouseMove += Listbox_Events_MouseMove;
            // 
            // contextMenu_ListBox_Events
            // 
            contextMenu_ListBox_Events.Items.AddRange(new ToolStripItem[] { moveUpToolStripMenuItem, moveDownToolStripMenuItem, deleteToolStripMenuItem });
            contextMenu_ListBox_Events.Name = "contextMenuStrip1";
            contextMenu_ListBox_Events.Size = new Size(139, 70);
            // 
            // moveUpToolStripMenuItem
            // 
            moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            moveUpToolStripMenuItem.Size = new Size(138, 22);
            moveUpToolStripMenuItem.Text = "Move Up";
            moveUpToolStripMenuItem.Click += moveUpToolStripMenuItem_Click;
            // 
            // moveDownToolStripMenuItem
            // 
            moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            moveDownToolStripMenuItem.Size = new Size(138, 22);
            moveDownToolStripMenuItem.Text = "Move Down";
            moveDownToolStripMenuItem.Click += moveDownToolStripMenuItem_Click;
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(138, 22);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // propertyGrid_RecordEvent
            // 
            propertyGrid_RecordEvent.BackColor = SystemColors.Control;
            propertyGrid_RecordEvent.Dock = DockStyle.Fill;
            propertyGrid_RecordEvent.Enabled = false;
            propertyGrid_RecordEvent.Location = new Point(0, 0);
            propertyGrid_RecordEvent.Name = "propertyGrid_RecordEvent";
            propertyGrid_RecordEvent.RightToLeft = RightToLeft.Yes;
            propertyGrid_RecordEvent.Size = new Size(354, 427);
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
            splitContainer1.Size = new Size(358, 601);
            splitContainer1.SplitterDistance = 160;
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
            splitContainer2.Panel2.Controls.Add(splitContainer3);
            splitContainer2.Size = new Size(938, 601);
            splitContainer2.SplitterDistance = 358;
            splitContainer2.SplitterIncrement = 5;
            splitContainer2.SplitterWidth = 10;
            splitContainer2.TabIndex = 7;
            // 
            // splitContainer3
            // 
            splitContainer3.Dock = DockStyle.Fill;
            splitContainer3.Location = new Point(0, 0);
            splitContainer3.Name = "splitContainer3";
            splitContainer3.Orientation = Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(pictureBox1);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(richTextBox_stepText);
            splitContainer3.Size = new Size(566, 597);
            splitContainer3.SplitterDistance = 502;
            splitContainer3.TabIndex = 2;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.ControlDarkDark;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(566, 502);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // richTextBox_stepText
            // 
            richTextBox_stepText.Dock = DockStyle.Fill;
            richTextBox_stepText.Location = new Point(0, 0);
            richTextBox_stepText.Name = "richTextBox_stepText";
            richTextBox_stepText.Size = new Size(566, 91);
            richTextBox_stepText.TabIndex = 1;
            richTextBox_stepText.Text = "";
            richTextBox_stepText.TextChanged += richTextBox_stepText_TextChanged;
            richTextBox_stepText.Leave += richTextBox_stepText_Leave;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, ToolStripMenuItem_Recording, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(938, 24);
            menuStrip1.TabIndex = 8;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, toolStripMenuItem1_SaveAs, toolStripSeparator1, exportToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new Size(114, 22);
            newToolStripMenuItem.Text = "New";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(114, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1_SaveAs
            // 
            toolStripMenuItem1_SaveAs.Enabled = false;
            toolStripMenuItem1_SaveAs.Name = "toolStripMenuItem1_SaveAs";
            toolStripMenuItem1_SaveAs.Size = new Size(114, 22);
            toolStripMenuItem1_SaveAs.Text = "Save As";
            toolStripMenuItem1_SaveAs.Click += toolStripMenuItem1_SaveAs_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(111, 6);
            // 
            // exportToolStripMenuItem
            // 
            exportToolStripMenuItem.Enabled = false;
            exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            exportToolStripMenuItem.Size = new Size(114, 22);
            exportToolStripMenuItem.Text = "Export";
            exportToolStripMenuItem.Click += exportToolStripMenuItem_Click;
            // 
            // ToolStripMenuItem_Recording
            // 
            ToolStripMenuItem_Recording.Alignment = ToolStripItemAlignment.Right;
            ToolStripMenuItem_Recording.Enabled = false;
            ToolStripMenuItem_Recording.Image = Properties.Resources.RecordTiny;
            ToolStripMenuItem_Recording.Name = "ToolStripMenuItem_Recording";
            ToolStripMenuItem_Recording.RightToLeft = RightToLeft.No;
            ToolStripMenuItem_Recording.Size = new Size(116, 20);
            ToolStripMenuItem_Recording.Text = "Start Recording";
            ToolStripMenuItem_Recording.Click += ToolStripMenuItem_Recording_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            helpToolStripMenuItem.Click += helpToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(938, 625);
            Controls.Add(splitContainer2);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Better Steps Recorder";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            contextMenu_ListBox_Events.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
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
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private RichTextBox richTextBox_stepText;
        private SplitContainer splitContainer3;
        private ToolStripMenuItem exportToolStripMenuItem;
        private ContextMenuStrip contextMenu_ListBox_Events;
        private ToolStripMenuItem moveUpToolStripMenuItem;
        private ToolStripMenuItem moveDownToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1_SaveAs;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem helpToolStripMenuItem;
    }
}
