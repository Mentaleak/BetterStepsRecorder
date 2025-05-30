using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Better_Steps_Recorder.UI.Dialogs
{
    /// <summary>
    /// A custom folder browser that restricts navigation to within a specific root folder
    /// </summary>
    public class RestrictedFolderBrowser : Form
    {
        private TreeView folderTreeView;
        private Button okButton;
        private Button cancelButton;
        private Button newFolderButton;
        private TextBox currentPathTextBox;
        private readonly string rootPath;
        public string SelectedPath { get; private set; }

        public RestrictedFolderBrowser(string rootPath)
        {
            this.rootPath = rootPath;
            InitializeComponents();
            PopulateTreeView();
        }

        private void InitializeComponents()
        {
            this.Text = "Select Folder in Obsidian Vault";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Current path display
            currentPathTextBox = new TextBox
            {
                Location = new Point(12, 12),
                Size = new Size(460, 23),
                ReadOnly = true,
                Text = rootPath
            };

            // Folder tree view
            folderTreeView = new TreeView
            {
                Location = new Point(12, 41),
                Size = new Size(460, 280),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            folderTreeView.AfterSelect += FolderTreeView_AfterSelect;

            // Buttons
            okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(316, 327),
                Size = new Size(75, 23)
            };
            okButton.Click += (s, e) => this.DialogResult = DialogResult.OK;

            cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(397, 327),
                Size = new Size(75, 23)
            };
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            newFolderButton = new Button
            {
                Text = "New Folder",
                Location = new Point(12, 327),
                Size = new Size(100, 23)
            };
            newFolderButton.Click += NewFolderButton_Click;

            // Add controls to form
            this.Controls.Add(currentPathTextBox);
            this.Controls.Add(folderTreeView);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
            this.Controls.Add(newFolderButton);

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private void PopulateTreeView()
        {
            folderTreeView.Nodes.Clear();
            DirectoryInfo rootDir = new DirectoryInfo(rootPath);
            
            // Create the root node
            TreeNode rootNode = new TreeNode(rootDir.Name)
            {
                Tag = rootDir.FullName,
                ImageIndex = 0
            };
            folderTreeView.Nodes.Add(rootNode);
            
            // Populate the tree
            PopulateDirectories(rootNode, rootDir);
            
            // Expand the root node
            rootNode.Expand();
            folderTreeView.SelectedNode = rootNode;
            SelectedPath = rootPath;
        }

        private void PopulateDirectories(TreeNode parentNode, DirectoryInfo dirInfo)
        {
            try
            {
                DirectoryInfo[] subDirs = dirInfo.GetDirectories();
                foreach (DirectoryInfo dir in subDirs)
                {
                    // Skip folders that start with a dot (like .obsidian)
                    if (dir.Name.StartsWith("."))
                        continue;

                    // Skip hidden folders
                    if ((dir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                        continue;

                    TreeNode dirNode = new TreeNode(dir.Name)
                    {
                        Tag = dir.FullName,
                        ImageIndex = 1
                    };
                    parentNode.Nodes.Add(dirNode);
                    
                    // Check if this directory has subdirectories (excluding hidden ones)
                    bool hasVisibleSubdirs = false;
                    foreach (DirectoryInfo subDir in dir.GetDirectories())
                    {
                        if (!subDir.Name.StartsWith(".") && 
                            (subDir.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                        {
                            hasVisibleSubdirs = true;
                            break;
                        }
                    }
                    
                    if (hasVisibleSubdirs)
                    {
                        // Add a dummy node to show the expand icon
                        dirNode.Nodes.Add(new TreeNode("Loading...") { Tag = "dummy" });
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Skip directories we don't have access to
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error accessing directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FolderTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is string path && path != "dummy")
            {
                SelectedPath = path;
                currentPathTextBox.Text = path;
                
                // If this node was expanded for the first time, populate its subdirectories
                if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Tag?.ToString() == "dummy")
                {
                    e.Node.Nodes.Clear();
                    PopulateDirectories(e.Node, new DirectoryInfo(path));
                }
            }
        }

        private void NewFolderButton_Click(object sender, EventArgs e)
        {
            if (folderTreeView.SelectedNode == null)
                return;

            string parentPath = SelectedPath;
            
            using (Form prompt = new Form())
            {
                prompt.Width = 300;
                prompt.Height = 150;
                prompt.Text = "New Folder";
                prompt.StartPosition = FormStartPosition.CenterParent;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.MaximizeBox = false;
                prompt.MinimizeBox = false;

                Label textLabel = new Label() { Left = 20, Top = 20, Width = 260, Text = "Enter folder name:" };
                TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 260 };
                Button confirmation = new Button() { Text = "OK", Left = 120, Width = 75, Top = 80 };
                Button cancel = new Button() { Text = "Cancel", Left = 205, Width = 75, Top = 80 };

                confirmation.Click += (s, ev) => { prompt.DialogResult = DialogResult.OK; prompt.Close(); };
                cancel.Click += (s, ev) => { prompt.DialogResult = DialogResult.Cancel; prompt.Close(); };

                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(cancel);
                prompt.AcceptButton = confirmation;
                prompt.CancelButton = cancel;

                if (prompt.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(textBox.Text))
                {
                    try
                    {
                        // Don't allow creating folders that start with a dot
                        if (textBox.Text.StartsWith("."))
                        {
                            MessageBox.Show("Folder names starting with a dot (.) are reserved for system use.", 
                                "Invalid Folder Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        
                        string newFolderPath = Path.Combine(parentPath, textBox.Text);
                        Directory.CreateDirectory(newFolderPath);
                        
                        // Refresh the current node
                        TreeNode currentNode = folderTreeView.SelectedNode;
                        currentNode.Nodes.Clear();
                        PopulateDirectories(currentNode, new DirectoryInfo(parentPath));
                        currentNode.Expand();
                        
                        // Find and select the new folder
                        foreach (TreeNode childNode in currentNode.Nodes)
                        {
                            if (childNode.Tag.ToString() == newFolderPath)
                            {
                                folderTreeView.SelectedNode = childNode;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error creating folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}