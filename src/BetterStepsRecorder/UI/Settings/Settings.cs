using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class Settings : Form
    {
        private UserControl currentView;

        public Settings()
        {
            InitializeComponent();
            treeView_Settings.AfterSelect += TreeView_Settings_AfterSelect;
            treeView_Settings.ExpandAll();
        }

        private void TreeView_Settings_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;

            UserControl newView = null;

            switch (e.Node.Name)
            {
                case "Settings_General":
                    newView = new GeneralSettings();
                    break;
                case "Settings_IndicatorStyle":
                    newView = new IndicatorStyle();
                    break;
                case "Settings_IndicatorColor":
                    newView = new IndicatorColor();
                    break;
                case "Settings_ScreenshotClick":
                    newView = new ScreenshotClick();
                    break;
                case "Settings_ScreenshotDrag":
                    newView = new ScreenshotDrag();
                    break;
                case "Settings_ExportHtml":
                    newView = new ExportHtml();
                    break;
            }

            if (newView != null)
            {
                LoadView(newView);
            }
        }

        private void LoadView(UserControl newView)
        {
            panel_settings.SuspendLayout();

            // Remove and dispose current view
            if (currentView != null)
            {
                panel_settings.Controls.Remove(currentView);
                currentView.Dispose();
            }

            // Add new view
            currentView = newView;
            currentView.Dock = DockStyle.Fill;
            panel_settings.Controls.Add(currentView);

            panel_settings.ResumeLayout();
        }
    }
}
