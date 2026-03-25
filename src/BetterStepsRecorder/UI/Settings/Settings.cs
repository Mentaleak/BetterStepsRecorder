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
        private Control currentView;
        private Dictionary<string, List<string>> nodeSearchIndex;
        private System.Windows.Forms.Timer searchDebounceTimer;
        private Dictionary<UserControl, TreeNode> controlNodeMap;

        public Settings()
        {
            InitializeComponent();
            treeView_Settings.AfterSelect += TreeView_Settings_AfterSelect;
            textBox_SearchSettings.TextChanged += TextBox_SearchSettings_TextChanged;

            // Initialize search debounce timer
            searchDebounceTimer = new System.Windows.Forms.Timer();
            searchDebounceTimer.Interval = 300; // 300ms delay
            searchDebounceTimer.Tick += SearchDebounceTimer_Tick;

            // Initialize control-node mapping
            controlNodeMap = new Dictionary<UserControl, TreeNode>();

            // Build search index
            BuildSearchIndex();

            treeView_Settings.ExpandAll();

            // Initialize node states based on current settings
            UpdateNodeStates();
        }

        private void TreeView_Settings_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;

            // Check if this is a parent node with children
            if (e.Node.Nodes.Count > 0)
            {
                LoadCompositeView(e.Node);
                return;
            }

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
            // Clear the control-node mapping for single views
            controlNodeMap.Clear();

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

        private void LoadCompositeView(TreeNode parentNode)
        {
            // Ensure node states are current before building the view
            UpdateNodeStates();

            // Clear the control-node mapping
            controlNodeMap.Clear();

            panel_settings.SuspendLayout();

            // Remove and dispose current view
            if (currentView != null)
            {
                // Dispose child controls if it's a composite panel
                if (currentView is Panel panel)
                {
                    foreach (Control ctrl in panel.Controls)
                    {
                        if (ctrl is FlowLayoutPanel flow)
                        {
                            foreach (Control child in flow.Controls)
                            {
                                if (child is UserControl uc)
                                {
                                    uc.Dispose();
                                }
                            }
                        }
                    }
                }

                panel_settings.Controls.Remove(currentView);
                currentView.Dispose();
            }

            // Create a scrollable panel to hold all child controls
            var compositePanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10)
            };

            // Create a flow layout panel for vertical stacking
            var flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0, 0, 20, 0) // Right padding for scrollbar
            };

            // Add title label
            var titleLabel = new Label
            {
                Text = parentNode.Text,
                Font = new Font(Font.FontFamily, 14, FontStyle.Bold),
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 15)
            };
            flowPanel.Controls.Add(titleLabel);

            // First, add the parent node's own control if it has one
            var parentControl = CreateControlForNode(parentNode);
            if (parentControl != null)
            {
                parentControl.AutoSize = true;
                parentControl.Margin = new Padding(0, 0, 0, 15);

                // Map this control to its node
                controlNodeMap[parentControl] = parentNode;

                bool isEnabled = parentNode.ForeColor == SystemColors.ControlText || parentNode.ForeColor == Color.Empty;
                SetControlsEnabled(parentControl, isEnabled);

                flowPanel.Controls.Add(parentControl);

                // Add separator if there are children
                if (parentNode.Nodes.Count > 0)
                {
                    var separator = new Panel
                    {
                        Height = 1,
                        Width = 650,
                        BackColor = SystemColors.ControlLight,
                        Margin = new Padding(0, 0, 0, 10)
                    };
                    flowPanel.Controls.Add(separator);
                }
            }

            // Add all descendants recursively
            AddDescendantControls(flowPanel, parentNode, 0);

            compositePanel.Controls.Add(flowPanel);
            currentView = compositePanel;
            panel_settings.Controls.Add(compositePanel);

            panel_settings.ResumeLayout();
        }

        private void AddDescendantControls(FlowLayoutPanel flowPanel, TreeNode parentNode, int indentLevel)
        {
            foreach (TreeNode childNode in parentNode.Nodes)
            {
                var childControl = CreateControlForNode(childNode);
                if (childControl != null)
                {
                    // Add section header with appropriate indentation
                    var sectionLabel = new Label
                    {
                        Text = new string(' ', indentLevel * 4) + childNode.Text,
                        Font = new Font(Font.FontFamily, 11, FontStyle.Bold),
                        AutoSize = true,
                        Padding = new Padding(indentLevel * 20, 10, 0, 5),
                        ForeColor = SystemColors.ControlDarkDark
                    };
                    flowPanel.Controls.Add(sectionLabel);

                    // Add the control with indentation
                    childControl.AutoSize = true;
                    childControl.Margin = new Padding(indentLevel * 20, 0, 0, 15);

                    // Map this control to its node
                    controlNodeMap[childControl] = childNode;

                    // Disable if node is greyed out
                    bool isEnabled = childNode.ForeColor == SystemColors.ControlText || childNode.ForeColor == Color.Empty;
                    SetControlsEnabled(childControl, isEnabled);

                    flowPanel.Controls.Add(childControl);
                }

                // Recursively add this node's children
                if (childNode.Nodes.Count > 0)
                {
                    AddDescendantControls(flowPanel, childNode, indentLevel + 1);
                }

                // Add separator line except for last item at this level
                if (childNode.Index < parentNode.Nodes.Count - 1 || indentLevel > 0)
                {
                    var separator = new Panel
                    {
                        Height = 1,
                        Width = 650,
                        BackColor = SystemColors.ControlLight,
                        Margin = new Padding(indentLevel * 20, 0, 0, 5)
                    };
                    flowPanel.Controls.Add(separator);
                }
            }
        }

        private UserControl CreateControlForNode(TreeNode node)
        {
            switch (node.Name)
            {
                case "Settings_General":
                    return new GeneralSettings();
                case "Settings_IndicatorStyle":
                    return new IndicatorStyle();
                case "Settings_IndicatorColor":
                    return new IndicatorColor();
                case "Settings_ScreenshotClick":
                    return new ScreenshotClick();
                case "Settings_ScreenshotClickCropped":
                    return new ScreenshotClickCropped();
                case "Settings_ScreenshotDrag":
                    return new ScreenshotDrag();
                case "Settings_ScreenshotDragCropped":
                    return new ScreenshotDragCropped();
                case "Settings_ScreenshotDragFallback":
                    return new ScreenshotDragFallback();
                case "Settings_ExportHtml":
                    return new ExportHtml();
                default:
                    return null;
            }
        }

        public void UpdateNodeStates()
        {
            var settings = BSRSettings.Current;

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
                bool fallbackIsCropped = settings.DragFallbackMode == FallbackDragScreenshotMode.Cropped;
                dragCroppedNode.ForeColor = (isCropped || fallbackIsCropped) ? SystemColors.ControlText : SystemColors.GrayText;
            }

            // Enable/disable Drag -> Fallback node based on mode
            if (dragFallbackNode != null)
            {
                bool isActiveWindow = settings.DragScreenshotMode == DragScreenshotMode.ActiveWindow;
                dragFallbackNode.ForeColor = isActiveWindow ? SystemColors.ControlText : SystemColors.GrayText;
            }

            // Refresh current view's enabled states
            RefreshCompositeViewStates();
        }

        private void RefreshCompositeViewStates()
        {
            if (currentView == null) return;

            // Check if current view is a composite panel
            if (currentView is Panel compositePanel)
            {
                // Use the control-node map to update all UserControl states
                foreach (var kvp in controlNodeMap)
                {
                    UserControl control = kvp.Key;
                    TreeNode node = kvp.Value;

                    bool isEnabled = node.ForeColor == SystemColors.ControlText || node.ForeColor == Color.Empty;
                    SetControlsEnabled(control, isEnabled);
                }
            }
            // If it's a single view, just refresh it
            else if (treeView_Settings.SelectedNode != null)
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

        private void BuildSearchIndex()
        {
            nodeSearchIndex = new Dictionary<string, List<string>>();

            // Index all nodes and their searchable content
            foreach (TreeNode rootNode in treeView_Settings.Nodes)
            {
                IndexNodeAndChildren(rootNode);
            }
        }

        private void IndexNodeAndChildren(TreeNode node)
        {
            List<string> searchTerms = new List<string>();

            // Add the node text itself
            searchTerms.Add(node.Text.ToLowerInvariant());

            // Add parent path for context
            TreeNode parent = node.Parent;
            while (parent != null)
            {
                searchTerms.Add(parent.Text.ToLowerInvariant());
                parent = parent.Parent;
            }

            // Add content from the associated UserControl
            var controlContent = GetControlSearchableContent(node.Name);
            if (controlContent != null)
            {
                searchTerms.AddRange(controlContent);
            }

            nodeSearchIndex[node.Name] = searchTerms;

            // Index children
            foreach (TreeNode child in node.Nodes)
            {
                IndexNodeAndChildren(child);
            }
        }

        private List<string> GetControlSearchableContent(string nodeName)
        {
            List<string> content = new List<string>();

            // Create temporary instance to extract searchable text
            UserControl tempControl = null;
            try
            {
                switch (nodeName)
                {
                    case "Settings_General":
                        tempControl = new GeneralSettings();
                        break;
                    case "Settings_IndicatorStyle":
                        tempControl = new IndicatorStyle();
                        break;
                    case "Settings_IndicatorColor":
                        tempControl = new IndicatorColor();
                        break;
                    case "Settings_ScreenshotClick":
                        tempControl = new ScreenshotClick();
                        break;
                    case "Settings_ScreenshotClickCropped":
                        tempControl = new ScreenshotClickCropped();
                        break;
                    case "Settings_ScreenshotDrag":
                        tempControl = new ScreenshotDrag();
                        break;
                    case "Settings_ScreenshotDragCropped":
                        tempControl = new ScreenshotDragCropped();
                        break;
                    case "Settings_ScreenshotDragFallback":
                        tempControl = new ScreenshotDragFallback();
                        break;
                    case "Settings_ExportHtml":
                        tempControl = new ExportHtml();
                        break;
                }

                if (tempControl != null)
                {
                    ExtractTextFromControls(tempControl, content);
                    tempControl.Dispose();
                }
            }
            catch
            {
                tempControl?.Dispose();
            }

            return content;
        }

        private void ExtractTextFromControls(Control parent, List<string> content)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Label label && !string.IsNullOrWhiteSpace(label.Text))
                {
                    content.Add(label.Text.ToLowerInvariant());
                }
                else if (ctrl is RadioButton radio && !string.IsNullOrWhiteSpace(radio.Text))
                {
                    content.Add(radio.Text.ToLowerInvariant());
                }
                else if (ctrl is CheckBox check && !string.IsNullOrWhiteSpace(check.Text))
                {
                    content.Add(check.Text.ToLowerInvariant());
                }
                else if (ctrl is ComboBox combo)
                {
                    foreach (var item in combo.Items)
                    {
                        if (item != null)
                            content.Add(item.ToString().ToLowerInvariant());
                    }
                }

                if (ctrl.HasChildren)
                {
                    ExtractTextFromControls(ctrl, content);
                }
            }
        }

        private void TextBox_SearchSettings_TextChanged(object sender, EventArgs e)
        {
            // Restart debounce timer
            searchDebounceTimer.Stop();
            searchDebounceTimer.Start();
        }

        private void SearchDebounceTimer_Tick(object sender, EventArgs e)
        {
            searchDebounceTimer.Stop();
            PerformSearch();
        }

        private void PerformSearch()
        {
            string searchText = textBox_SearchSettings.Text.Trim().ToLowerInvariant();

            treeView_Settings.BeginUpdate();

            if (string.IsNullOrEmpty(searchText))
            {
                // Show all nodes
                ShowAllNodes(treeView_Settings.Nodes);
                treeView_Settings.ExpandAll();
                UpdateNodeStates();
                treeView_Settings.EndUpdate();
                return;
            }

            // Hide all nodes first
            HideAllNodes(treeView_Settings.Nodes);

            // Show matching nodes and their parents
            bool anyMatches = false;
            foreach (var kvp in nodeSearchIndex)
            {
                bool matches = false;
                foreach (var term in kvp.Value)
                {
                    if (term.Contains(searchText))
                    {
                        matches = true;
                        break;
                    }
                }

                if (matches)
                {
                    anyMatches = true;
                    TreeNode node = FindNodeByName(kvp.Key);
                    if (node != null)
                    {
                        ShowNodeAndParents(node);
                    }
                }
            }

            if (anyMatches)
            {
                treeView_Settings.ExpandAll();
                UpdateNodeStates();
            }

            treeView_Settings.EndUpdate();
        }

        private void ShowAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.BackColor = Color.Empty;
                ShowAllNodes(node.Nodes);
            }
        }

        private void HideAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.BackColor = SystemColors.Control;
                HideAllNodes(node.Nodes);
            }
        }

        private void ShowNodeAndParents(TreeNode node)
        {
            if (node == null) return;

            // Highlight the matching node with yellow background
            node.BackColor = Color.LightYellow;

            // Show parent nodes without highlight
            TreeNode parent = node.Parent;
            while (parent != null)
            {
                parent.BackColor = Color.Empty;
                parent = parent.Parent;
            }

            // Show child nodes without highlight
            ShowChildNodes(node);
        }

        private void ShowChildNodes(TreeNode node)
        {
            foreach (TreeNode child in node.Nodes)
            {
                child.BackColor = Color.Empty;
                ShowChildNodes(child);
            }
        }
    }
}
