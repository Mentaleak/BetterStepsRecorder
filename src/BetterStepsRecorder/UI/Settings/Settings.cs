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

            // Initialize node states based on current settings
            UpdateNodeStates();
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
                case "Settings_ScreenshotClickCropped":
                    newView = new ScreenshotClickCropped();
                    break;
                case "Settings_ScreenshotDrag":
                    newView = new ScreenshotDrag();
                    break;
                case "Settings_ScreenshotDragCropped":
                    newView = new ScreenshotDragCropped();
                    break;
                case "Settings_ScreenshotDragFallback":
                    newView = new ScreenshotDragFallback();
                    break;
                case "Settings_ExportHtml":
                    newView = new ExportHtml();
                    break;
            }

            if (newView != null)
            {
                LoadView(newView, e.Node);
            }
        }

        private void LoadView(UserControl newView, TreeNode selectedNode)
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

            // Disable controls if the node is greyed out
            bool isNodeEnabled = selectedNode.ForeColor == SystemColors.ControlText || selectedNode.ForeColor == Color.Empty;
            SetControlsEnabled(currentView, isNodeEnabled);

            panel_settings.ResumeLayout();
        }

        public void UpdateNodeStates()
        {
            var settings = RecordingSettings.Load();

            // Find the child nodes
            TreeNode clickNode = FindNodeByName("Settings_ScreenshotClick");
            TreeNode clickCroppedNode = FindNodeByName("Settings_ScreenshotClickCropped");
            TreeNode dragNode = FindNodeByName("Settings_ScreenshotDrag");
            TreeNode dragCroppedNode = FindNodeByName("Settings_ScreenshotDragCropped");
            TreeNode dragFallbackNode = FindNodeByName("Settings_ScreenshotDragFallback");

            // Enable/disable Click -> Cropped node based on mode
            if (clickCroppedNode != null)
            {
                bool isCropped = settings.ClickScreenshotMode == ClickScreenshotMode.Cropped;
                clickCroppedNode.ForeColor = isCropped ? SystemColors.ControlText : SystemColors.GrayText;
            }

            // Enable/disable Drag -> Cropped node based on mode OR fallback mode
            if (dragCroppedNode != null)
            {
                bool isCropped = settings.DragScreenshotMode == DragScreenshotMode.Cropped;
                bool fallbackIsCropped = settings.DragFallbackMode == DragScreenshotMode.Cropped;
                dragCroppedNode.ForeColor = (isCropped || fallbackIsCropped) ? SystemColors.ControlText : SystemColors.GrayText;
            }

            // Enable/disable Drag -> Fallback node based on mode
            if (dragFallbackNode != null)
            {
                bool isActiveWindow = settings.DragScreenshotMode == DragScreenshotMode.ActiveWindow;
                dragFallbackNode.ForeColor = isActiveWindow ? SystemColors.ControlText : SystemColors.GrayText;
            }

            // Refresh current view if it's one of the child nodes
            if (currentView != null && treeView_Settings.SelectedNode != null)
            {
                var currentNode = treeView_Settings.SelectedNode;
                bool isEnabled = currentNode.ForeColor == SystemColors.ControlText || currentNode.ForeColor == Color.Empty;
                SetControlsEnabled(currentView, isEnabled);
            }
        }

        private TreeNode FindNodeByName(string name)
        {
            foreach (TreeNode node in treeView_Settings.Nodes)
            {
                TreeNode found = FindNodeRecursive(node, name);
                if (found != null) return found;
            }
            return null;
        }

        private TreeNode FindNodeRecursive(TreeNode parent, string name)
        {
            if (parent.Name == name) return parent;
            foreach (TreeNode child in parent.Nodes)
            {
                TreeNode found = FindNodeRecursive(child, name);
                if (found != null) return found;
            }
            return null;
        }

        private void SetControlsEnabled(Control parent, bool enabled)
        {
            foreach (Control ctrl in parent.Controls)
            {
                // Skip labels - they should always be visible
                if (!(ctrl is Label))
                {
                    ctrl.Enabled = enabled;
                }

                // Recursively disable child controls
                if (ctrl.HasChildren)
                {
                    SetControlsEnabled(ctrl, enabled);
                }
            }
        }
    }
}
