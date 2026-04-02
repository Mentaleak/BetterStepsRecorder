namespace BetterStepsRecorder
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            Listbox_Events = new ListBox();
            contextMenu_ListBox_Events = new ContextMenuStrip(components);
            moveUpToolStripMenuItem = new ToolStripMenuItem();
            moveDownToolStripMenuItem = new ToolStripMenuItem();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            contextMenu_ListBox_Edits = new ContextMenuStrip(components);
            moveUpEditToolStripMenuItem = new ToolStripMenuItem();
            moveDownEditToolStripMenuItem = new ToolStripMenuItem();
            moveToFrontEditToolStripMenuItem = new ToolStripMenuItem();
            sendToBackEditToolStripMenuItem = new ToolStripMenuItem();
            editTextToolStripMenuItem = new ToolStripMenuItem();
            deleteEditToolStripMenuItem = new ToolStripMenuItem();
            contextMenu_PictureBox = new ContextMenuStrip(components);
            moveUpPictureBoxMenuItem = new ToolStripMenuItem();
            moveDownPictureBoxMenuItem = new ToolStripMenuItem();
            moveToFrontPictureBoxMenuItem = new ToolStripMenuItem();
            sendToBackPictureBoxMenuItem = new ToolStripMenuItem();
            editTextPictureBoxMenuItem = new ToolStripMenuItem();
            deletePictureBoxMenuItem = new ToolStripMenuItem();
            propertyGrid_RecordEvent = new PropertyGrid();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            splitContainer3 = new SplitContainer();
            splitContainer4 = new SplitContainer();
            pictureBox1 = new PictureBox();
            pictureBoxToolStrip = new ToolStrip();
            undoToolStripButton = new ToolStripButton();
            resetIndicatorToolStripButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            blurRegionToolStripButton = new ToolStripButton();
            highlightToolStripButton = new ToolStripButton();
            highlightColourToolStripButton = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            textLabelToolStripButton = new ToolStripButton();
            toolStripSeparator5 = new ToolStripSeparator();
            textInnerColourToolStripButton = new ToolStripButton();
            textOuterColourToolStripButton = new ToolStripButton();
            arrowToolStripButton = new ToolStripButton();
            arrowColourToolStripButton = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            cropToolStripButton = new ToolStripButton();
            listBox_Edits = new ListBox();
            label_Edits = new Label();
            richTextBox_stepText = new RichTextBox();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1_SaveAs = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exportToolStripMenuItem = new ToolStripMenuItem();
            exportToFileToolStripMenuItem = new ToolStripMenuItem();
            exportToHtmlToolStripMenuItem = new ToolStripMenuItem();
            exportToMarkdownToolStripMenuItem = new ToolStripMenuItem();
            exportToOdtToolStripMenuItem = new ToolStripMenuItem();
            exportToRtfToolStripMenuItem = new ToolStripMenuItem();
            exportToObsidianVaultToolStripMenuItem = new ToolStripMenuItem();
            ToolStripMenuItem_Recording = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            notifyIcon = new NotifyIcon(components);
            notifyIconContextMenu = new ContextMenuStrip(components);
            restoreToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            contextMenu_ListBox_Events.SuspendLayout();
            contextMenu_ListBox_Edits.SuspendLayout();
            contextMenu_PictureBox.SuspendLayout();
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
            ((System.ComponentModel.ISupportInitialize)splitContainer4).BeginInit();
            splitContainer4.Panel1.SuspendLayout();
            splitContainer4.Panel2.SuspendLayout();
            splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            pictureBoxToolStrip.SuspendLayout();
            menuStrip1.SuspendLayout();
            notifyIconContextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // Listbox_Events
            // 
            Listbox_Events.AllowDrop = true;
            Listbox_Events.Dock = DockStyle.Fill;
            Listbox_Events.FormattingEnabled = true;
            Listbox_Events.Location = new Point(0, 0);
            Listbox_Events.Margin = new Padding(3, 4, 3, 4);
            Listbox_Events.Name = "Listbox_Events";
            Listbox_Events.SelectionMode = SelectionMode.MultiExtended;
            Listbox_Events.Size = new Size(260, 209);
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
            contextMenu_ListBox_Events.Size = new Size(159, 76);
            // 
            // moveUpToolStripMenuItem
            // 
            moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            moveUpToolStripMenuItem.Size = new Size(158, 24);
            moveUpToolStripMenuItem.Text = "Move Up";
            moveUpToolStripMenuItem.Click += moveUpToolStripMenuItem_Click;
            // 
            // moveDownToolStripMenuItem
            // 
            moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            moveDownToolStripMenuItem.Size = new Size(158, 24);
            moveDownToolStripMenuItem.Text = "Move Down";
            moveDownToolStripMenuItem.Click += moveDownToolStripMenuItem_Click;
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(158, 24);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // contextMenu_ListBox_Edits
            // 
            contextMenu_ListBox_Edits.Items.AddRange(new ToolStripItem[] { moveUpEditToolStripMenuItem, moveDownEditToolStripMenuItem, moveToFrontEditToolStripMenuItem, sendToBackEditToolStripMenuItem, editTextToolStripMenuItem, deleteEditToolStripMenuItem });
            contextMenu_ListBox_Edits.Name = "contextMenu_ListBox_Edits";
            contextMenu_ListBox_Edits.Size = new Size(172, 154);
            contextMenu_ListBox_Edits.Opening += contextMenu_ListBox_Edits_Opening;
            // 
            // moveUpEditToolStripMenuItem
            // 
            moveUpEditToolStripMenuItem.Name = "moveUpEditToolStripMenuItem";
            moveUpEditToolStripMenuItem.Size = new Size(171, 24);
            moveUpEditToolStripMenuItem.Text = "Move Up";
            moveUpEditToolStripMenuItem.Click += moveUpEditToolStripMenuItem_Click;
            // 
            // moveDownEditToolStripMenuItem
            // 
            moveDownEditToolStripMenuItem.Name = "moveDownEditToolStripMenuItem";
            moveDownEditToolStripMenuItem.Size = new Size(171, 24);
            moveDownEditToolStripMenuItem.Text = "Move Down";
            moveDownEditToolStripMenuItem.Click += moveDownEditToolStripMenuItem_Click;
            // 
            // moveToFrontEditToolStripMenuItem
            // 
            moveToFrontEditToolStripMenuItem.Name = "moveToFrontEditToolStripMenuItem";
            moveToFrontEditToolStripMenuItem.Size = new Size(171, 24);
            moveToFrontEditToolStripMenuItem.Text = "Move to Front";
            moveToFrontEditToolStripMenuItem.Click += moveToFrontEditToolStripMenuItem_Click;
            // 
            // sendToBackEditToolStripMenuItem
            // 
            sendToBackEditToolStripMenuItem.Name = "sendToBackEditToolStripMenuItem";
            sendToBackEditToolStripMenuItem.Size = new Size(171, 24);
            sendToBackEditToolStripMenuItem.Text = "Send to Back";
            sendToBackEditToolStripMenuItem.Click += sendToBackEditToolStripMenuItem_Click;
            // 
            // editTextToolStripMenuItem
            // 
            editTextToolStripMenuItem.Name = "editTextToolStripMenuItem";
            editTextToolStripMenuItem.Size = new Size(171, 24);
            editTextToolStripMenuItem.Text = "Edit Text";
            editTextToolStripMenuItem.Click += editTextToolStripMenuItem_Click;
            // 
            // deleteEditToolStripMenuItem
            // 
            deleteEditToolStripMenuItem.Name = "deleteEditToolStripMenuItem";
            deleteEditToolStripMenuItem.Size = new Size(171, 24);
            deleteEditToolStripMenuItem.Text = "Delete";
            deleteEditToolStripMenuItem.Click += deleteEditToolStripMenuItem_Click;
            // 
            // contextMenu_PictureBox
            // 
            contextMenu_PictureBox.Items.AddRange(new ToolStripItem[] { moveUpPictureBoxMenuItem, moveDownPictureBoxMenuItem, moveToFrontPictureBoxMenuItem, sendToBackPictureBoxMenuItem, editTextPictureBoxMenuItem, deletePictureBoxMenuItem });
            contextMenu_PictureBox.Name = "contextMenu_PictureBox";
            contextMenu_PictureBox.Size = new Size(172, 154);
            contextMenu_PictureBox.Opening += contextMenu_PictureBox_Opening;
            // 
            // moveUpPictureBoxMenuItem
            // 
            moveUpPictureBoxMenuItem.Name = "moveUpPictureBoxMenuItem";
            moveUpPictureBoxMenuItem.Size = new Size(171, 24);
            moveUpPictureBoxMenuItem.Text = "Move Up";
            moveUpPictureBoxMenuItem.Click += moveUpEditToolStripMenuItem_Click;
            // 
            // moveDownPictureBoxMenuItem
            // 
            moveDownPictureBoxMenuItem.Name = "moveDownPictureBoxMenuItem";
            moveDownPictureBoxMenuItem.Size = new Size(171, 24);
            moveDownPictureBoxMenuItem.Text = "Move Down";
            moveDownPictureBoxMenuItem.Click += moveDownEditToolStripMenuItem_Click;
            // 
            // moveToFrontPictureBoxMenuItem
            // 
            moveToFrontPictureBoxMenuItem.Name = "moveToFrontPictureBoxMenuItem";
            moveToFrontPictureBoxMenuItem.Size = new Size(171, 24);
            moveToFrontPictureBoxMenuItem.Text = "Move to Front";
            moveToFrontPictureBoxMenuItem.Click += moveToFrontEditToolStripMenuItem_Click;
            // 
            // sendToBackPictureBoxMenuItem
            // 
            sendToBackPictureBoxMenuItem.Name = "sendToBackPictureBoxMenuItem";
            sendToBackPictureBoxMenuItem.Size = new Size(171, 24);
            sendToBackPictureBoxMenuItem.Text = "Send to Back";
            sendToBackPictureBoxMenuItem.Click += sendToBackEditToolStripMenuItem_Click;
            // 
            // editTextPictureBoxMenuItem
            // 
            editTextPictureBoxMenuItem.Name = "editTextPictureBoxMenuItem";
            editTextPictureBoxMenuItem.Size = new Size(171, 24);
            editTextPictureBoxMenuItem.Text = "Edit Text";
            editTextPictureBoxMenuItem.Click += editTextPictureBoxMenuItem_Click;
            // 
            // deletePictureBoxMenuItem
            // 
            deletePictureBoxMenuItem.Name = "deletePictureBoxMenuItem";
            deletePictureBoxMenuItem.Size = new Size(171, 24);
            deletePictureBoxMenuItem.Text = "Delete";
            deletePictureBoxMenuItem.Click += deleteEditToolStripMenuItem_Click;
            // 
            // propertyGrid_RecordEvent
            // 
            propertyGrid_RecordEvent.BackColor = SystemColors.Control;
            propertyGrid_RecordEvent.Dock = DockStyle.Fill;
            propertyGrid_RecordEvent.Enabled = false;
            propertyGrid_RecordEvent.Location = new Point(0, 0);
            propertyGrid_RecordEvent.Margin = new Padding(3, 4, 3, 4);
            propertyGrid_RecordEvent.Name = "propertyGrid_RecordEvent";
            propertyGrid_RecordEvent.RightToLeft = RightToLeft.Yes;
            propertyGrid_RecordEvent.Size = new Size(260, 573);
            propertyGrid_RecordEvent.TabIndex = 4;
            propertyGrid_RecordEvent.ToolbarVisible = false;
            // 
            // splitContainer1
            // 
            splitContainer1.BackColor = SystemColors.Control;
            splitContainer1.BorderStyle = BorderStyle.Fixed3D;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Margin = new Padding(3, 4, 3, 4);
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
            splitContainer1.Size = new Size(264, 803);
            splitContainer1.SplitterDistance = 213;
            splitContainer1.SplitterIncrement = 5;
            splitContainer1.SplitterWidth = 13;
            splitContainer1.TabIndex = 5;
            // 
            // splitContainer2
            // 
            splitContainer2.BackColor = SystemColors.Control;
            splitContainer2.BorderStyle = BorderStyle.Fixed3D;
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 30);
            splitContainer2.Margin = new Padding(3, 4, 3, 4);
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
            splitContainer2.Size = new Size(1326, 803);
            splitContainer2.SplitterDistance = 264;
            splitContainer2.SplitterIncrement = 5;
            splitContainer2.SplitterWidth = 11;
            splitContainer2.TabIndex = 7;
            // 
            // splitContainer3
            // 
            splitContainer3.Dock = DockStyle.Fill;
            splitContainer3.Location = new Point(0, 0);
            splitContainer3.Margin = new Padding(3, 4, 3, 4);
            splitContainer3.Name = "splitContainer3";
            splitContainer3.Orientation = Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(splitContainer4);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(richTextBox_stepText);
            splitContainer3.Size = new Size(1047, 799);
            splitContainer3.SplitterDistance = 670;
            splitContainer3.SplitterWidth = 5;
            splitContainer3.TabIndex = 2;
            // 
            // splitContainer4
            // 
            splitContainer4.Dock = DockStyle.Fill;
            splitContainer4.Location = new Point(0, 0);
            splitContainer4.Margin = new Padding(3, 4, 3, 4);
            splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            splitContainer4.Panel1.Controls.Add(pictureBox1);
            splitContainer4.Panel1.Controls.Add(pictureBoxToolStrip);
            // 
            // splitContainer4.Panel2
            // 
            splitContainer4.Panel2.Controls.Add(listBox_Edits);
            splitContainer4.Panel2.Controls.Add(label_Edits);
            splitContainer4.Size = new Size(1047, 670);
            splitContainer4.SplitterDistance = 902;
            splitContainer4.SplitterWidth = 5;
            splitContainer4.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.ControlDarkDark;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 27);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(902, 643);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // pictureBoxToolStrip
            // 
            pictureBoxToolStrip.BackColor = SystemColors.Control;
            pictureBoxToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            pictureBoxToolStrip.Items.AddRange(new ToolStripItem[] { undoToolStripButton, resetIndicatorToolStripButton, toolStripSeparator2, highlightToolStripButton, highlightColourToolStripButton, toolStripSeparator3, textLabelToolStripButton, textInnerColourToolStripButton, textOuterColourToolStripButton, toolStripSeparator5, arrowToolStripButton, arrowColourToolStripButton, toolStripSeparator4, cropToolStripButton, blurRegionToolStripButton });
            pictureBoxToolStrip.Location = new Point(0, 0);
            pictureBoxToolStrip.Name = "pictureBoxToolStrip";
            pictureBoxToolStrip.Size = new Size(902, 27);
            pictureBoxToolStrip.TabIndex = 1;
            // 
            // undoToolStripButton
            // 
            undoToolStripButton.Enabled = false;
            undoToolStripButton.Name = "undoToolStripButton";
            undoToolStripButton.Size = new Size(66, 24);
            undoToolStripButton.Text = "↩ Undo";
            undoToolStripButton.ToolTipText = "Undo last annotation (Ctrl+Z)";
            undoToolStripButton.Click += undoToolStripButton_Click;
            // 
            // resetIndicatorToolStripButton
            // 
            resetIndicatorToolStripButton.Enabled = false;
            resetIndicatorToolStripButton.Name = "resetIndicatorToolStripButton";
            resetIndicatorToolStripButton.Size = new Size(137, 24);
            resetIndicatorToolStripButton.Text = "🎯 Reset Indicator";
            resetIndicatorToolStripButton.ToolTipText = "Reset the click/drag indicator to its original position";
            resetIndicatorToolStripButton.Click += resetIndicatorToolStripButton_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 27);
            // 
            // blurRegionToolStripButton
            // 
            blurRegionToolStripButton.CheckOnClick = true;
            blurRegionToolStripButton.Name = "blurRegionToolStripButton";
            blurRegionToolStripButton.Size = new Size(39, 24);
            blurRegionToolStripButton.Text = "Blur";
            blurRegionToolStripButton.ToolTipText = "Draw a rectangle to blur/redact sensitive info";
            blurRegionToolStripButton.Click += blurRegionToolStripButton_Click;
            // 
            // highlightToolStripButton
            // 
            highlightToolStripButton.CheckOnClick = true;
            highlightToolStripButton.Name = "highlightToolStripButton";
            highlightToolStripButton.Size = new Size(75, 24);
            highlightToolStripButton.Text = "Highlight";
            highlightToolStripButton.ToolTipText = "Draw a coloured highlight rectangle";
            highlightToolStripButton.Click += highlightToolStripButton_Click;
            // 
            // highlightColourToolStripButton
            // 
            highlightColourToolStripButton.Name = "highlightColourToolStripButton";
            highlightColourToolStripButton.Size = new Size(34, 24);
            highlightColourToolStripButton.Text = "🎨";
            highlightColourToolStripButton.ToolTipText = "Pick highlight colour";
            highlightColourToolStripButton.Click += highlightColourToolStripButton_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 27);
            // 
            // textLabelToolStripButton
            // 
            textLabelToolStripButton.CheckOnClick = true;
            textLabelToolStripButton.Name = "textLabelToolStripButton";
            textLabelToolStripButton.Size = new Size(40, 24);
            textLabelToolStripButton.Text = "Text";
            textLabelToolStripButton.ToolTipText = "Click to place a text label on the screenshot";
            textLabelToolStripButton.Click += textLabelToolStripButton_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 27);
            // 
            // textInnerColourToolStripButton
            // 
            textInnerColourToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            textInnerColourToolStripButton.Name = "textInnerColourToolStripButton";
            textInnerColourToolStripButton.Size = new Size(34, 24);
            textInnerColourToolStripButton.Text = "🎨";
            textInnerColourToolStripButton.ToolTipText = "Pick text inner colour";
            textInnerColourToolStripButton.Click += textInnerColourToolStripButton_Click;
            // 
            // textOuterColourToolStripButton
            // 
            textOuterColourToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            textOuterColourToolStripButton.Name = "textOuterColourToolStripButton";
            textOuterColourToolStripButton.Size = new Size(34, 24);
            textOuterColourToolStripButton.Text = "🖍";
            textOuterColourToolStripButton.ToolTipText = "Pick text outer/border colour";
            textOuterColourToolStripButton.Click += textOuterColourToolStripButton_Click;
            // 
            // arrowToolStripButton
            // 
            arrowToolStripButton.CheckOnClick = true;
            arrowToolStripButton.Name = "arrowToolStripButton";
            arrowToolStripButton.Size = new Size(53, 24);
            arrowToolStripButton.Text = "Arrow";
            arrowToolStripButton.ToolTipText = "Draw an arrow on the screenshot";
            arrowToolStripButton.Click += arrowToolStripButton_Click;
            // 
            // arrowColourToolStripButton
            // 
            arrowColourToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            arrowColourToolStripButton.Name = "arrowColourToolStripButton";
            arrowColourToolStripButton.Size = new Size(34, 24);
            arrowColourToolStripButton.Text = "🎨";
            arrowColourToolStripButton.ToolTipText = "Pick arrow colour";
            arrowColourToolStripButton.Click += arrowColourToolStripButton_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 27);
            // 
            // cropToolStripButton
            // 
            cropToolStripButton.CheckOnClick = true;
            cropToolStripButton.Name = "cropToolStripButton";
            cropToolStripButton.Size = new Size(45, 24);
            cropToolStripButton.Text = "Crop";
            cropToolStripButton.ToolTipText = "Crop the screenshot to the selected area";
            cropToolStripButton.Click += cropToolStripButton_Click;
            // 
            // listBox_Edits
            // 
            listBox_Edits.AllowDrop = true;
            listBox_Edits.ContextMenuStrip = contextMenu_ListBox_Edits;
            listBox_Edits.Dock = DockStyle.Fill;
            listBox_Edits.FormattingEnabled = true;
            listBox_Edits.Location = new Point(0, 24);
            listBox_Edits.Margin = new Padding(3, 4, 3, 4);
            listBox_Edits.Name = "listBox_Edits";
            listBox_Edits.Size = new Size(140, 646);
            listBox_Edits.TabIndex = 0;
            listBox_Edits.DragDrop += listBox_Edits_DragDrop;
            listBox_Edits.DragEnter += listBox_Edits_DragEnter;
            listBox_Edits.DragOver += listBox_Edits_DragOver;
            listBox_Edits.DoubleClick += listBox_Edits_DoubleClick;
            listBox_Edits.KeyDown += listBox_Edits_KeyDown;
            listBox_Edits.MouseDown += listBox_Edits_MouseDown;
            listBox_Edits.MouseMove += listBox_Edits_MouseMove;
            // 
            // label_Edits
            // 
            label_Edits.BackColor = SystemColors.ControlLight;
            label_Edits.Dock = DockStyle.Top;
            label_Edits.Location = new Point(0, 0);
            label_Edits.Name = "label_Edits";
            label_Edits.Padding = new Padding(3, 0, 0, 0);
            label_Edits.Size = new Size(140, 24);
            label_Edits.TabIndex = 1;
            label_Edits.Text = "Edit History";
            label_Edits.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // richTextBox_stepText
            // 
            richTextBox_stepText.Dock = DockStyle.Fill;
            richTextBox_stepText.Location = new Point(0, 0);
            richTextBox_stepText.Margin = new Padding(3, 4, 3, 4);
            richTextBox_stepText.Name = "richTextBox_stepText";
            richTextBox_stepText.Size = new Size(1047, 124);
            richTextBox_stepText.TabIndex = 1;
            richTextBox_stepText.Text = "";
            richTextBox_stepText.TextChanged += richTextBox_stepText_TextChanged;
            richTextBox_stepText.Leave += richTextBox_stepText_Leave;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, ToolStripMenuItem_Recording, settingsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 3, 0, 3);
            menuStrip1.Size = new Size(1326, 30);
            menuStrip1.TabIndex = 8;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, toolStripMenuItem1_SaveAs, toolStripSeparator1, exportToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(44, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new Size(129, 24);
            newToolStripMenuItem.Text = "New";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(129, 24);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1_SaveAs
            // 
            toolStripMenuItem1_SaveAs.Enabled = false;
            toolStripMenuItem1_SaveAs.Name = "toolStripMenuItem1_SaveAs";
            toolStripMenuItem1_SaveAs.Size = new Size(129, 24);
            toolStripMenuItem1_SaveAs.Text = "Save As";
            toolStripMenuItem1_SaveAs.Click += toolStripMenuItem1_SaveAs_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(126, 6);
            // 
            // exportToolStripMenuItem
            // 
            exportToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportToFileToolStripMenuItem, exportToObsidianVaultToolStripMenuItem });
            exportToolStripMenuItem.Enabled = false;
            exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            exportToolStripMenuItem.Size = new Size(129, 24);
            exportToolStripMenuItem.Text = "Export";
            // 
            // exportToFileToolStripMenuItem
            // 
            exportToFileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportToHtmlToolStripMenuItem, exportToMarkdownToolStripMenuItem, exportToOdtToolStripMenuItem, exportToRtfToolStripMenuItem });
            exportToFileToolStripMenuItem.Name = "exportToFileToolStripMenuItem";
            exportToFileToolStripMenuItem.Size = new Size(194, 24);
            exportToFileToolStripMenuItem.Text = "To File";
            // 
            // exportToHtmlToolStripMenuItem
            // 
            exportToHtmlToolStripMenuItem.Name = "exportToHtmlToolStripMenuItem";
            exportToHtmlToolStripMenuItem.Size = new Size(148, 24);
            exportToHtmlToolStripMenuItem.Text = "HTML";
            exportToHtmlToolStripMenuItem.Click += exportToHtmlToolStripMenuItem_Click;
            // 
            // exportToMarkdownToolStripMenuItem
            // 
            exportToMarkdownToolStripMenuItem.Name = "exportToMarkdownToolStripMenuItem";
            exportToMarkdownToolStripMenuItem.Size = new Size(148, 24);
            exportToMarkdownToolStripMenuItem.Text = "Markdown";
            exportToMarkdownToolStripMenuItem.Click += exportToMarkdownToolStripMenuItem_Click;
            // 
            // exportToOdtToolStripMenuItem
            // 
            exportToOdtToolStripMenuItem.Name = "exportToOdtToolStripMenuItem";
            exportToOdtToolStripMenuItem.Size = new Size(148, 24);
            exportToOdtToolStripMenuItem.Text = "ODT";
            exportToOdtToolStripMenuItem.Click += exportToOdtToolStripMenuItem_Click;
            // 
            // exportToRtfToolStripMenuItem
            // 
            exportToRtfToolStripMenuItem.Name = "exportToRtfToolStripMenuItem";
            exportToRtfToolStripMenuItem.Size = new Size(148, 24);
            exportToRtfToolStripMenuItem.Text = "RTF";
            exportToRtfToolStripMenuItem.Click += exportToRtfToolStripMenuItem_Click;
            // 
            // exportToObsidianVaultToolStripMenuItem
            // 
            exportToObsidianVaultToolStripMenuItem.Name = "exportToObsidianVaultToolStripMenuItem";
            exportToObsidianVaultToolStripMenuItem.Size = new Size(194, 24);
            exportToObsidianVaultToolStripMenuItem.Text = "To Obsidian Vault";
            exportToObsidianVaultToolStripMenuItem.Click += exportToObsidianVaultToolStripMenuItem_Click;
            // 
            // ToolStripMenuItem_Recording
            // 
            ToolStripMenuItem_Recording.Alignment = ToolStripItemAlignment.Right;
            ToolStripMenuItem_Recording.Enabled = false;
            ToolStripMenuItem_Recording.Image = Properties.Resources.RecordTiny;
            ToolStripMenuItem_Recording.Name = "ToolStripMenuItem_Recording";
            ToolStripMenuItem_Recording.RightToLeft = RightToLeft.No;
            ToolStripMenuItem_Recording.Size = new Size(140, 24);
            ToolStripMenuItem_Recording.Text = "Start Recording";
            ToolStripMenuItem_Recording.Click += ToolStripMenuItem_Recording_Click;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(74, 24);
            settingsToolStripMenuItem.Text = "Settings";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(53, 24);
            helpToolStripMenuItem.Text = "Help";
            helpToolStripMenuItem.Click += helpToolStripMenuItem_Click;
            // 
            // notifyIcon
            // 
            notifyIcon.ContextMenuStrip = notifyIconContextMenu;
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "Better Steps Recorder";
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            // 
            // notifyIconContextMenu
            // 
            notifyIconContextMenu.Items.AddRange(new ToolStripItem[] { restoreToolStripMenuItem, exitToolStripMenuItem });
            notifyIconContextMenu.Name = "notifyIconContextMenu";
            notifyIconContextMenu.Size = new Size(129, 52);
            // 
            // restoreToolStripMenuItem
            // 
            restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            restoreToolStripMenuItem.Size = new Size(128, 24);
            restoreToolStripMenuItem.Text = "Restore";
            restoreToolStripMenuItem.Click += restoreToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(128, 24);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1326, 833);
            Controls.Add(splitContainer2);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new Padding(3, 4, 3, 4);
            Name = "MainForm";
            Text = "Better Steps Recorder";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            Resize += Form_Resize;
            contextMenu_ListBox_Events.ResumeLayout(false);
            contextMenu_ListBox_Edits.ResumeLayout(false);
            contextMenu_PictureBox.ResumeLayout(false);
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
            splitContainer4.Panel1.ResumeLayout(false);
            splitContainer4.Panel1.PerformLayout();
            splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer4).EndInit();
            splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            pictureBoxToolStrip.ResumeLayout(false);
            pictureBoxToolStrip.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            notifyIconContextMenu.ResumeLayout(false);
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
        private ToolStripMenuItem exportToFileToolStripMenuItem;
        private ToolStripMenuItem exportToRtfToolStripMenuItem;
        private ToolStripMenuItem exportToHtmlToolStripMenuItem;
        private ToolStripMenuItem exportToOdtToolStripMenuItem;
        private ToolStripMenuItem exportToMarkdownToolStripMenuItem;
        private ContextMenuStrip contextMenu_ListBox_Events;
        private ToolStripMenuItem moveUpToolStripMenuItem;
        private ToolStripMenuItem moveDownToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ContextMenuStrip contextMenu_ListBox_Edits;
        private ToolStripMenuItem moveUpEditToolStripMenuItem;
        private ToolStripMenuItem moveDownEditToolStripMenuItem;
        private ToolStripMenuItem moveToFrontEditToolStripMenuItem;
        private ToolStripMenuItem sendToBackEditToolStripMenuItem;
        private ToolStripMenuItem editTextToolStripMenuItem;
        private ToolStripMenuItem deleteEditToolStripMenuItem;
        private ContextMenuStrip contextMenu_PictureBox;
        private ToolStripMenuItem moveUpPictureBoxMenuItem;
        private ToolStripMenuItem moveDownPictureBoxMenuItem;
        private ToolStripMenuItem moveToFrontPictureBoxMenuItem;
        private ToolStripMenuItem sendToBackPictureBoxMenuItem;
        private ToolStripMenuItem editTextPictureBoxMenuItem;
        private ToolStripMenuItem deletePictureBoxMenuItem;
        private ToolStripMenuItem toolStripMenuItem1_SaveAs;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem exportToObsidianVaultToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStrip pictureBoxToolStrip;
        private ToolStripButton undoToolStripButton;
        private ToolStripButton resetIndicatorToolStripButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton blurRegionToolStripButton;
        private ToolStripButton highlightToolStripButton;
        private ToolStripButton highlightColourToolStripButton;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton textLabelToolStripButton;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton textInnerColourToolStripButton;
        private ToolStripButton textOuterColourToolStripButton;
        private ToolStripButton arrowToolStripButton;
        private ToolStripButton arrowColourToolStripButton;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton cropToolStripButton;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip notifyIconContextMenu;
        private ToolStripMenuItem restoreToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private SplitContainer splitContainer4;
        private ListBox listBox_Edits;
        private Label label_Edits;
    }
}