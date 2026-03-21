using System;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class ScreenshotDragFallback : UserControl
    {
        public ScreenshotDragFallback()
        {
            InitializeComponent();
            
            // Populate fallback mode combo box
            cmbFallbackMode.Items.Add("Cropped");
            cmbFallbackMode.Items.Add("Active screen");
            cmbFallbackMode.Items.Add("All screens");
            
            LoadSettings();
            
            cmbFallbackMode.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
        }

        private void LoadSettings()
        {
            var settings = RecordingSettings.Load();

            // Load fallback mode
            switch (settings.DragFallbackMode)
            {
                case DragScreenshotMode.ActiveScreen:
                    cmbFallbackMode.SelectedIndex = 1;
                    break;
                case DragScreenshotMode.AllScreens:
                    cmbFallbackMode.SelectedIndex = 2;
                    break;
                default:
                    cmbFallbackMode.SelectedIndex = 0; // Cropped
                    break;
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DragScreenshotMode fallbackMode;
            switch (cmbFallbackMode.SelectedIndex)
            {
                case 1:
                    fallbackMode = DragScreenshotMode.ActiveScreen;
                    break;
                case 2:
                    fallbackMode = DragScreenshotMode.AllScreens;
                    break;
                default:
                    fallbackMode = DragScreenshotMode.Cropped;
                    break;
            }
            Program.DragFallbackMode = fallbackMode;
            RecordingSettings.SaveCurrent();

            // Update the parent form's node states
            if (ParentForm is Settings settingsForm)
            {
                settingsForm.UpdateNodeStates();
            }
        }
    }
}
