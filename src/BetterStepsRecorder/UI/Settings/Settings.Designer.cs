namespace BetterStepsRecorder.UI.Settings
{
    partial class Settings
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
            TreeNode treeNode1 = new TreeNode("General");
            TreeNode treeNode2 = new TreeNode("Style");
            TreeNode treeNode3 = new TreeNode("Color");
            TreeNode treeNode4 = new TreeNode("Indicator", new TreeNode[] { treeNode2, treeNode3 });
            TreeNode treeNode5 = new TreeNode("Click");
            TreeNode treeNode6 = new TreeNode("Drag");
            TreeNode treeNode7 = new TreeNode("Screenshot", new TreeNode[] { treeNode5, treeNode6 });
            TreeNode treeNode8 = new TreeNode("HTML");
            TreeNode treeNode9 = new TreeNode("Export", new TreeNode[] { treeNode8 });
            treeView_Settings = new TreeView();
            textBox_SearchSettings = new TextBox();
            panel_settings = new Panel();
            SuspendLayout();
            // 
            // treeView_Settings
            // 
            treeView_Settings.Location = new Point(12, 45);
            treeView_Settings.Name = "treeView_Settings";
            treeNode1.Name = "Settings_General";
            treeNode1.Text = "General";
            treeNode2.Name = "Settings_IndicatorStyle";
            treeNode2.Text = "Style";
            treeNode3.Name = "Settings_IndicatorColor";
            treeNode3.Text = "Color";
            treeNode4.Name = "Settings_Indicator";
            treeNode4.Text = "Indicator";
            treeNode5.Name = "Settings_ScreenshotClick";
            treeNode5.Text = "Click";
            treeNode6.Name = "Settings_ScreenshotDrag";
            treeNode6.Text = "Drag";
            treeNode7.Name = "Settings_ScreenShots";
            treeNode7.Text = "Screenshot";
            treeNode8.Name = "Settings_ExportHtml";
            treeNode8.Text = "HTML";
            treeNode9.Name = "Settings_Export";
            treeNode9.Text = "Export";
            treeView_Settings.Nodes.AddRange(new TreeNode[] { treeNode1, treeNode4, treeNode7, treeNode9 });
            treeView_Settings.Size = new Size(244, 408);
            treeView_Settings.TabIndex = 0;
            // 
            // textBox_SearchSettings
            // 
            textBox_SearchSettings.Location = new Point(12, 12);
            textBox_SearchSettings.Name = "textBox_SearchSettings";
            textBox_SearchSettings.PlaceholderText = "Search Settings";
            textBox_SearchSettings.Size = new Size(244, 27);
            textBox_SearchSettings.TabIndex = 1;
            // 
            // panel_settings
            // 
            panel_settings.Location = new Point(262, 12);
            panel_settings.Name = "panel_settings";
            panel_settings.Size = new Size(703, 441);
            panel_settings.TabIndex = 2;
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(977, 465);
            Controls.Add(panel_settings);
            Controls.Add(textBox_SearchSettings);
            Controls.Add(treeView_Settings);
            Name = "Settings";
            ShowIcon = false;
            Text = "Settings";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TreeView treeView_Settings;
        private TextBox textBox_SearchSettings;
        private Panel panel_settings;
    }
}