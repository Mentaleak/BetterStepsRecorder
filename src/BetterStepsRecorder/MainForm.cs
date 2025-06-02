using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text.Json;

// Remove this line: using System.Windows.Automation;
using System.Windows.Forms;
// Add FlaUI imports if needed:
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
        
        private void Form1_Load(object sender, EventArgs e)
        {
            DisableRecording();
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.SaveRecordEvents();
        }
    }
}