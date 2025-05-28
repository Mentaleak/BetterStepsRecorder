using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Text.Json;
using System.Runtime.InteropServices;

namespace Better_Steps_Recorder
{
    public class ObsidianExporter
    {
        // Import required Windows API functions to restrict folder browsing
        [DllImport("user32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        // Constants for folder browser customization
        private const uint BFFM_INITIALIZED = 1;
        private const uint BFFM_SETSELECTION = 1024 + 102;

        /// <summary>
        /// Exports the current steps recording to an Obsidian vault as a markdown file with images
        /// </summary>
        /// <param name="vaultPath">The root path of the Obsidian vault</param>
        /// <param name="fileName">The name of the markdown file to create (without extension)</param>
        /// <param name="subfolderPath">Optional subfolder path within the vault</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public static bool ExportToObsidianVault(string vaultPath, string fileName, string subfolderPath = "")
        {
            try
            {
                // Validate Obsidian vault
                if (!IsValidObsidianVault(vaultPath))
                {
                    MessageBox.Show("The selected folder is not a valid Obsidian vault. Please select a folder containing a .obsidian directory.", 
                        "Invalid Obsidian Vault", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Determine image folder path
                string imageFolderPath = GetImageFolderPath(vaultPath);

                // Create full path for markdown file
                string mdFilePath;
                if (string.IsNullOrEmpty(subfolderPath))
                {
                    mdFilePath = Path.Combine(vaultPath, $"{fileName}.md");
                }
                else
                {
                    string fullSubfolderPath = Path.Combine(vaultPath, subfolderPath);
                    if (!Directory.Exists(fullSubfolderPath))
                    {
                        Directory.CreateDirectory(fullSubfolderPath);
                    }
                    mdFilePath = Path.Combine(fullSubfolderPath, $"{fileName}.md");
                }

                // Track used image filenames to avoid duplicates
                HashSet<string> usedImageNames = new HashSet<string>();

                // Create the markdown content
                using (StreamWriter writer = new StreamWriter(mdFilePath))
                {
                    // Add title
                    writer.WriteLine($"# {fileName}");
                    writer.WriteLine();
                    writer.WriteLine("Created with Better Steps Recorder");
                    writer.WriteLine();

                    // Add each step
                    foreach (var recordEvent in Program._recordEvents)
                    {
                        // Write the step number and text
                        writer.WriteLine($"## Step {recordEvent.Step}: {recordEvent._StepText}");
                        writer.WriteLine();

                        // Process and save image if available
                        if (!string.IsNullOrEmpty(recordEvent.Screenshotb64))
                        {
                            // Create a simpler image filename without GUID
                            string baseImageName = $"{fileName}_step{recordEvent.Step}";
                            string imageFileName = baseImageName + ".png";
                            
                            // Check if this filename is already used, if so, add GUID to make it unique
                            if (usedImageNames.Contains(imageFileName))
                            {
                                // Add a shortened GUID to make the filename unique
                                string shortGuid = recordEvent.ID.ToString().Substring(0, 8);
                                imageFileName = $"{baseImageName}_{shortGuid}.png";
                            }
                            
                            usedImageNames.Add(imageFileName);
                            string imageFilePath = Path.Combine(imageFolderPath, imageFileName);
                            
                            // Save the image
                            byte[] imageBytes = Convert.FromBase64String(recordEvent.Screenshotb64);
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                using (Image image = Image.FromStream(ms))
                                {
                                    image.Save(imageFilePath, ImageFormat.Png);
                                }
                            }

                            // Get the relative path for the image link
                            string relativeImagePath = GetRelativeImagePath(vaultPath, imageFolderPath, imageFileName);
                            
                            // Add the image link to the markdown
                            writer.WriteLine($"![[{relativeImagePath}]]");
                            writer.WriteLine();
                        }

                        // Add separator between steps
                        writer.WriteLine("---");
                        writer.WriteLine();
                    }
                }

                MessageBox.Show($"Successfully exported to Obsidian vault at:\n{mdFilePath}", 
                    "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to Obsidian vault: {ex.Message}", 
                    "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Checks if the selected folder is a valid Obsidian vault by looking for the .obsidian folder
        /// </summary>
        private static bool IsValidObsidianVault(string vaultPath)
        {
            return Directory.Exists(Path.Combine(vaultPath, ".obsidian"));
        }

        /// <summary>
        /// Determines the image folder path based on Obsidian settings or creates a default one
        /// </summary>
        private static string GetImageFolderPath(string vaultPath)
        {
            string appJsonPath = Path.Combine(vaultPath, ".obsidian", "app.json");
            string imageFolderPath;

            // Check if app.json exists and try to read attachmentFolderPath
            if (File.Exists(appJsonPath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(appJsonPath);
                    using (JsonDocument document = JsonDocument.Parse(jsonContent))
                    {
                        if (document.RootElement.TryGetProperty("attachmentFolderPath", out JsonElement attachmentFolder))
                        {
                            string attachmentFolderPath = attachmentFolder.GetString();
                            if (!string.IsNullOrEmpty(attachmentFolderPath))
                            {
                                // Use the configured attachment folder
                                imageFolderPath = Path.Combine(vaultPath, attachmentFolderPath);
                                if (!Directory.Exists(imageFolderPath))
                                {
                                    Directory.CreateDirectory(imageFolderPath);
                                }
                                return imageFolderPath;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // If there's any error reading the JSON, fall back to default
                }
            }

            // Default folder if not found in settings
            imageFolderPath = Path.Combine(vaultPath, "BSR", "images");
            if (!Directory.Exists(imageFolderPath))
            {
                Directory.CreateDirectory(imageFolderPath);
            }
            return imageFolderPath;
        }

        /// <summary>
        /// Gets the relative path for the image to be used in Obsidian markdown
        /// </summary>
        private static string GetRelativeImagePath(string vaultPath, string imageFolderPath, string imageFileName)
        {
            // Get the path relative to the vault root
            string relativePath = imageFolderPath.Substring(vaultPath.Length).TrimStart(Path.DirectorySeparatorChar);
            return Path.Combine(relativePath, imageFileName).Replace('\\', '/');
        }

        /// <summary>
        /// Shows a dialog to select an Obsidian vault folder
        /// </summary>
        public static string SelectObsidianVault()
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select Obsidian Vault Folder";
                folderDialog.UseDescriptionForTitle = true;
                folderDialog.ShowNewFolderButton = false;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    return folderDialog.SelectedPath;
                }
                return null;
            }
        }

        /// <summary>
        /// Shows a dialog to select a subfolder within the Obsidian vault
        /// This restricts browsing to within the vault only
        /// </summary>
        public static string SelectSubfolder(string vaultPath)
        {
            // Create a custom folder browser that restricts navigation to within the vault
            using (RestrictedFolderBrowser restrictedBrowser = new RestrictedFolderBrowser(vaultPath))
            {
                if (restrictedBrowser.ShowDialog() == DialogResult.OK)
                {
                    // Return the path relative to the vault
                    string selectedPath = restrictedBrowser.SelectedPath;
                    if (selectedPath.StartsWith(vaultPath))
                    {
                        string relativePath = selectedPath.Substring(vaultPath.Length).TrimStart(Path.DirectorySeparatorChar);
                        return relativePath;
                    }
                }
                return "";
            }
        }

        /// <summary>
        /// Prompts the user for a file name
        /// </summary>
        public static string PromptForFileName(string defaultName = "BSR Export")
        {
            using (Form prompt = new Form())
            {
                prompt.Width = 400;
                prompt.Height = 150;
                prompt.Text = "Enter File Name";
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.MaximizeBox = false;
                prompt.MinimizeBox = false;

                Label textLabel = new Label() { Left = 20, Top = 20, Width = 360, Text = "Enter a name for your markdown file (without extension):" };
                TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 360, Text = defaultName };
                Button confirmation = new Button() { Text = "OK", Left = 200, Width = 80, Top = 80 };
                Button cancel = new Button() { Text = "Cancel", Left = 300, Width = 80, Top = 80 };

                confirmation.Click += (sender, e) => { prompt.DialogResult = DialogResult.OK; prompt.Close(); };
                cancel.Click += (sender, e) => { prompt.DialogResult = DialogResult.Cancel; prompt.Close(); };

                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(cancel);
                prompt.AcceptButton = confirmation;
                prompt.CancelButton = cancel;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
            }
        }
    }

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