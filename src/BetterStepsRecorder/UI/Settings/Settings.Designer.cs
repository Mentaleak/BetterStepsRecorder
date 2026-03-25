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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                searchDebounceTimer?.Dispose();
                currentView?.Dispose();
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
            TreeNode treeNode5 = new TreeNode("Cropped");
            TreeNode treeNode6 = new TreeNode("Click", new TreeNode[] { treeNode5 });
            TreeNode treeNode7 = new TreeNode("Cropped");
            TreeNode treeNode8 = new TreeNode("Fallback");
            TreeNode treeNode9 = new TreeNode("Drag", new TreeNode[] { treeNode7, treeNode8 });
            TreeNode treeNode10 = new TreeNode("Screenshot", new TreeNode[] { treeNode6, treeNode9 });
            TreeNode treeNode11 = new TreeNode("HTML");
            TreeNode treeNode12 = new TreeNode("Export", new TreeNode[] { treeNode11 });
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
            treeNode5.Name = "Settings_ScreenshotClickCropped";
            treeNode5.Text = "Cropped";
            treeNode6.Name = "Settings_ScreenshotClick";
            treeNode6.Text = "Click";
            treeNode7.Name = "Settings_ScreenshotDragCropped";
            treeNode7.Text = "Cropped";
            treeNode8.Name = "Settings_ScreenshotDragFallback";
            treeNode8.Text = "Fallback";
            treeNode9.Name = "Settings_ScreenshotDrag";
            treeNode9.Text = "Drag";
            treeNode10.Name = "Settings_ScreenShots";
            treeNode10.Text = "Screenshot";
            treeNode11.Name = "Settings_ExportHtml";
            treeNode11.Text = "HTML";
            treeNode12.Name = "Settings_Export";
            treeNode12.Text = "Export";
            treeView_Settings.Nodes.AddRange(new TreeNode[] { treeNode1, treeNode4, treeNode10, treeNode12 });
            treeView_Settings.Size = new Size(244, 367);
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
            panel_settings.Size = new Size(600, 430);
            panel_settings.TabIndex = 2;
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(874, 454);
            Controls.Add(panel_settings);
            Controls.Add(textBox_SearchSettings);
            Controls.Add(treeView_Settings);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
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