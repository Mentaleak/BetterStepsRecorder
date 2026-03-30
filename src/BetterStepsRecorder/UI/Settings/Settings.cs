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
        private List<TreeNodeData> originalTreeStructure;

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

            // Store original tree structure for search restore
            originalTreeStructure = CaptureTreeStructure();

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
                case "Settings_ExportMarkdown":
                    newView = new ExportMarkdown();
                    break;
                case "Settings_ExportRtf":
                    newView = new ExportRtf();
                    break;
                case "Settings_ExportOdt":
                    newView = new ExportOdt();
                    break;
                case "Settings_ExportObsidian":
                    newView = new ExportObsidian();
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
                case "Settings_ExportMarkdown":
                    return new ExportMarkdown();
                case "Settings_ExportRtf":
                    return new ExportRtf();
                case "Settings_ExportOdt":
                    return new ExportOdt();
                case "Settings_ExportObsidian":
                    return new ExportObsidian();
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
                bool isCropped = settings.Screenshot.Click.Mode == ClickScreenshotMode.Cropped;
                clickCroppedNode.ForeColor = isCropped ? SystemColors.ControlText : SystemColors.GrayText;
            }

            // Enable/disable Drag -> Cropped node based on mode OR fallback mode
            if (dragCroppedNode != null)
            {
                bool isCropped = settings.Screenshot.Drag.Mode == DragScreenshotMode.Cropped;
                bool fallbackIsCropped = settings.Screenshot.Drag.Fallback.Mode == FallbackDragScreenshotMode.Cropped;
                dragCroppedNode.ForeColor = (isCropped || fallbackIsCropped) ? SystemColors.ControlText : SystemColors.GrayText;
            }

            // Enable/disable Drag -> Fallback node based on mode
            if (dragFallbackNode != null)
            {
                bool isActiveWindow = settings.Screenshot.Drag.Mode == DragScreenshotMode.ActiveWindow;
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
                    case "Settings_ExportMarkdown":
                        tempControl = new ExportMarkdown();
                        break;
                    case "Settings_ExportRtf":
                        tempControl = new ExportRtf();
                        break;
                    case "Settings_ExportOdt":
                        tempControl = new ExportOdt();
                        break;
                    case "Settings_ExportObsidian":
                        tempControl = new ExportObsidian();
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
                // Restore all nodes from original structure
                RestoreTreeStructure();
                treeView_Settings.ExpandAll();
                UpdateNodeStates();
                treeView_Settings.EndUpdate();
                return;
            }

            // Find all matching node names
            HashSet<string> matchingNodeNames = new HashSet<string>();
            foreach (var kvp in nodeSearchIndex)
            {
                foreach (var term in kvp.Value)
                {
                    if (term.Contains(searchText))
                    {
                        matchingNodeNames.Add(kvp.Key);
                        break;
                    }
                }
            }

            // Rebuild tree with only matching nodes (and their parents)
            RebuildTreeWithMatches(matchingNodeNames);

            treeView_Settings.ExpandAll();
            UpdateNodeStates();
            treeView_Settings.EndUpdate();
        }

        private class TreeNodeData
        {
            public string Name { get; set; }
            public string Text { get; set; }
            public List<TreeNodeData> Children { get; set; } = new List<TreeNodeData>();
        }

        private List<TreeNodeData> CaptureTreeStructure()
        {
            var result = new List<TreeNodeData>();
            foreach (TreeNode node in treeView_Settings.Nodes)
            {
                result.Add(CaptureNode(node));
            }
            return result;
        }

        private TreeNodeData CaptureNode(TreeNode node)
        {
            var data = new TreeNodeData
            {
                Name = node.Name,
                Text = node.Text
            };
            foreach (TreeNode child in node.Nodes)
            {
                data.Children.Add(CaptureNode(child));
            }
            return data;
        }

        private void RestoreTreeStructure()
        {
            treeView_Settings.Nodes.Clear();
            foreach (var nodeData in originalTreeStructure)
            {
                treeView_Settings.Nodes.Add(RestoreNode(nodeData));
            }
        }

        private TreeNode RestoreNode(TreeNodeData data)
        {
            var node = new TreeNode(data.Text) { Name = data.Name };
            foreach (var childData in data.Children)
            {
                node.Nodes.Add(RestoreNode(childData));
            }
            return node;
        }

        private void RebuildTreeWithMatches(HashSet<string> matchingNodeNames)
        {
            treeView_Settings.Nodes.Clear();
            foreach (var nodeData in originalTreeStructure)
            {
                var filteredNode = BuildFilteredNode(nodeData, matchingNodeNames);
                if (filteredNode != null)
                {
                    treeView_Settings.Nodes.Add(filteredNode);
                }
            }
        }

        private TreeNode BuildFilteredNode(TreeNodeData data, HashSet<string> matchingNodeNames)
        {
            bool selfMatches = matchingNodeNames.Contains(data.Name);
            List<TreeNode> matchingChildren = new List<TreeNode>();

            foreach (var childData in data.Children)
            {
                var filteredChild = BuildFilteredNode(childData, matchingNodeNames);
                if (filteredChild != null)
                {
                    matchingChildren.Add(filteredChild);
                }
            }

            // Include this node if it matches or has matching descendants
            if (selfMatches || matchingChildren.Count > 0)
            {
                var node = new TreeNode(data.Text) { Name = data.Name };
                foreach (var child in matchingChildren)
                {
                    node.Nodes.Add(child);
                }
                return node;
            }

            return null;
        }

        private void button_settings_default_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "This will reset all settings to their default values.\n\nAll custom settings will be lost. Do you want to continue?",
                "Reset to Defaults",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                BSRSettings.Current.ResetToDefaults();
                BSRSettings.Current.Save();

                // Refresh the current view to show updated values
                if (treeView_Settings.SelectedNode != null)
                {
                    var selectedNode = treeView_Settings.SelectedNode;

                    // Reload the current view
                    if (selectedNode.Nodes.Count > 0)
                    {
                        LoadCompositeView(selectedNode);
                    }
                    else
                    {
                        var newView = CreateControlForNode(selectedNode);
                        if (newView != null)
                        {
                            LoadView(newView, selectedNode);
                        }
                    }
                }

                // Update node states
                UpdateNodeStates();

                MessageBox.Show(
                    "All settings have been reset to their default values.",
                    "Reset Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void button_settings_export_Click(object sender, EventArgs e)
        {
            using var dlg = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                DefaultExt = "json",
                FileName = "bsrsettings.json",
                Title = "Export Settings"
            };

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                if (BSRSettings.Current.Export(dlg.FileName))
                {
                    MessageBox.Show(
                        $"Settings exported successfully to:\n{dlg.FileName}",
                        "Export Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        "Failed to export settings. Please check file permissions and try again.",
                        "Export Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void button_settings_import_Click(object sender, EventArgs e)
        {
            DialogResult confirmResult = MessageBox.Show(
                "This will overwrite all current settings with values from the imported file.\n\nAll current settings will be lost. Do you want to continue?",
                "Import Settings",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirmResult != DialogResult.Yes) return;

            using var dlg = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                DefaultExt = "json",
                Title = "Import Settings"
            };

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                if (BSRSettings.Import(dlg.FileName))
                {
                    // Refresh the current view to show imported values
                    if (treeView_Settings.SelectedNode != null)
                    {
                        var selectedNode = treeView_Settings.SelectedNode;

                        // Reload the current view
                        if (selectedNode.Nodes.Count > 0)
                        {
                            LoadCompositeView(selectedNode);
                        }
                        else
                        {
                            var newView = CreateControlForNode(selectedNode);
                            if (newView != null)
                            {
                                LoadView(newView, selectedNode);
                            }
                        }
                    }

                    // Update node states
                    UpdateNodeStates();

                    MessageBox.Show(
                        "Settings imported successfully.",
                        "Import Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        "Failed to import settings. The file may be invalid or corrupted.",
                        "Import Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}
