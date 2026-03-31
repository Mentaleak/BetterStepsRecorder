using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text.Json;

using FlaUI.Core.AutomationElements;
using System.Windows.Forms;
using ListBox = System.Windows.Forms.ListBox;
using BetterStepsRecorder.Exporters;
using BetterStepsRecorder.UI.Dialogs;
using BetterStepsRecorder.UI;

namespace BetterStepsRecorder
{
    public partial class MainForm : Form
    {
        public System.Windows.Forms.Timer activityTimer;
        private const int DefaultActivityDelay = 5000;
        private int ActivityDelay = DefaultActivityDelay;
        private Point _mouseDownLocation;
        private HelpPopup? _helpPopup;

        public MainForm()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("Loaded");
            Listbox_Events.KeyDown += new KeyEventHandler(ListBox1_KeyDown);
            activityTimer = new System.Windows.Forms.Timer();
            activityTimer.Interval = ActivityDelay;
            activityTimer.Tick += activityTimer_Tick;

            // Initialize the status strip
            InitializeStatusStrip();
        }
        private void InitializeStatusStrip()
        {
            // Initialize the global status manager instead of a local instance
            StatusManager.Initialize(this);

            // Show initial ready message using the global manager
            StatusManager.ShowMessage("Ready to record steps");
        }

        private void InitializeTrayIcon()
        {
            notifyIcon.Text = "Better Steps Recorder";
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                var minimizeBehavior = BSRSettings.Current.General.MinimizeOnStartRecording;
                if (minimizeBehavior == MinimizeBehavior.MinimizeToSystemTray && Program.IsRecording)
                {
                    Hide();
                    notifyIcon.Visible = true;
                    StatusManager.ShowMessage("Minimized to system tray");
                }
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void RestoreFromTray()
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
            Activate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DisableRecording();

            // Initialize selection highlight timer and event handlers
            InitializeSelectionFlashTimer();
            listBox_Edits.SelectedIndexChanged += listBox_Edits_SelectedIndexChanged;
            pictureBox1.Paint += SelectionHighlight_Paint;

            // Wire up operation selection and drag handlers
            pictureBox1.MouseDown += PictureBox_SelectionMouseDown;
            pictureBox1.MouseMove += PictureBox_SelectionMouseMove;
            pictureBox1.MouseUp += PictureBox_SelectionMouseUp;
            pictureBox1.MouseLeave += PictureBox_SelectionMouseLeave;
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Z) && undoToolStripButton.Enabled)
            {
                undoToolStripButton_Click(this, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.SaveRecordEvents();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new UI.Settings.Settings())
            {
                settingsForm.ShowDialog(this);
            }
        }
    }
}