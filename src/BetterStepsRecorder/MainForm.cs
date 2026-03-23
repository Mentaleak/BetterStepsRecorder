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

            // Sync the "Check for updates at launch" menu item with the persisted setting
            checkForUpdatesAtLaunchToolStripMenuItem.Checked = Program.CheckForUpdatesAtLaunch;

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
        
        private void Form1_Load(object sender, EventArgs e)
        {
            DisableRecording();
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
    }
}