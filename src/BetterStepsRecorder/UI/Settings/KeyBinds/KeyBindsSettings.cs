using System;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class KeyBindsSettings : UserControl
    {
        public KeyBindsSettings()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            var keyBinds = BSRSettings.Current.KeyBinds;
            
            chkEnableGlobalHotkeys.Checked = keyBinds.EnableGlobalHotkeys;
            txtStartRecording.KeyBindString = keyBinds.StartRecording;
            txtPauseRecording.KeyBindString = keyBinds.PauseRecording;
            txtWindowSnap.KeyBindString = keyBinds.WindowSnap;
            txtScreenSnap.KeyBindString = keyBinds.ScreenSnap;
            txtAllScreensSnap.KeyBindString = keyBinds.AllScreensSnap;

            UpdateKeyBindControlsEnabled();
        }

        private void UpdateKeyBindControlsEnabled()
        {
            bool enabled = chkEnableGlobalHotkeys.Checked;
            txtStartRecording.Enabled = enabled;
            txtPauseRecording.Enabled = enabled;
            txtWindowSnap.Enabled = enabled;
            txtScreenSnap.Enabled = enabled;
            txtAllScreensSnap.Enabled = enabled;
            btnResetDefaults.Enabled = enabled;
        }

        private void chkEnableGlobalHotkeys_CheckedChanged(object sender, EventArgs e)
        {
            BSRSettings.Current.KeyBinds.EnableGlobalHotkeys = chkEnableGlobalHotkeys.Checked;
            BSRSettings.Current.Save();
            UpdateKeyBindControlsEnabled();
        }

        private void txtStartRecording_KeyBindChanged(object sender, Keys e)
        {
            if (ValidateAndSave("Start Recording", e, out string? error))
            {
                BSRSettings.Current.KeyBinds.StartRecording = KeyBindHelper.KeysToString(e);
                BSRSettings.Current.Save();
                ClearValidationError(txtStartRecording);
            }
            else
            {
                ShowValidationError(txtStartRecording, error);
            }
        }

        private void txtPauseRecording_KeyBindChanged(object sender, Keys e)
        {
            if (ValidateAndSave("Pause Recording", e, out string? error))
            {
                BSRSettings.Current.KeyBinds.PauseRecording = KeyBindHelper.KeysToString(e);
                BSRSettings.Current.Save();
                ClearValidationError(txtPauseRecording);
            }
            else
            {
                ShowValidationError(txtPauseRecording, error);
            }
        }

        private void txtWindowSnap_KeyBindChanged(object sender, Keys e)
        {
            if (ValidateAndSave("Window Snap", e, out string? error))
            {
                BSRSettings.Current.KeyBinds.WindowSnap = KeyBindHelper.KeysToString(e);
                BSRSettings.Current.Save();
                ClearValidationError(txtWindowSnap);
            }
            else
            {
                ShowValidationError(txtWindowSnap, error);
            }
        }

        private void txtScreenSnap_KeyBindChanged(object sender, Keys e)
        {
            if (ValidateAndSave("Screen Snap", e, out string? error))
            {
                BSRSettings.Current.KeyBinds.ScreenSnap = KeyBindHelper.KeysToString(e);
                BSRSettings.Current.Save();
                ClearValidationError(txtScreenSnap);
            }
            else
            {
                ShowValidationError(txtScreenSnap, error);
            }
        }

        private void txtAllScreensSnap_KeyBindChanged(object sender, Keys e)
        {
            if (ValidateAndSave("All Screens Snap", e, out string? error))
            {
                BSRSettings.Current.KeyBinds.AllScreensSnap = KeyBindHelper.KeysToString(e);
                BSRSettings.Current.Save();
                ClearValidationError(txtAllScreensSnap);
            }
            else
            {
                ShowValidationError(txtAllScreensSnap, error);
            }
        }

        private bool ValidateAndSave(string actionName, Keys keys, out string? errorMessage)
        {
            // Allow clearing (None)
            if (keys == Keys.None)
            {
                errorMessage = null;
                return true;
            }

            // Validate the key combination
            if (!KeyBindHelper.ValidateKeyBind(keys, out errorMessage))
            {
                return false;
            }

            // Check for duplicates within this settings page
            string keyString = KeyBindHelper.KeysToString(keys);
            var keyBinds = BSRSettings.Current.KeyBinds;

            if (actionName != "Start Recording" && keyBinds.StartRecording == keyString)
            {
                errorMessage = "This key combination is already assigned to Start Recording.";
                return false;
            }
            if (actionName != "Pause Recording" && keyBinds.PauseRecording == keyString)
            {
                errorMessage = "This key combination is already assigned to Pause Recording.";
                return false;
            }
            if (actionName != "Window Snap" && keyBinds.WindowSnap == keyString)
            {
                errorMessage = "This key combination is already assigned to Window Snap.";
                return false;
            }
            if (actionName != "Screen Snap" && keyBinds.ScreenSnap == keyString)
            {
                errorMessage = "This key combination is already assigned to Screen Snap.";
                return false;
            }
            if (actionName != "All Screens Snap" && keyBinds.AllScreensSnap == keyString)
            {
                errorMessage = "This key combination is already assigned to All Screens Snap.";
                return false;
            }

            return true;
        }

        private void ShowValidationError(KeyBindTextBox textBox, string? error)
        {
            textBox.BackColor = System.Drawing.Color.MistyRose;
            if (!string.IsNullOrEmpty(error))
            {
                toolTip1.SetToolTip(textBox, error);
            }
        }

        private void ClearValidationError(KeyBindTextBox textBox)
        {
            textBox.BackColor = System.Drawing.SystemColors.Window;
            toolTip1.SetToolTip(textBox, null);
        }

        private void btnResetDefaults_Click(object sender, EventArgs e)
        {
            var defaultKeyBinds = new BSRSettings.KeyBindSettings();
            
            BSRSettings.Current.KeyBinds.StartRecording = defaultKeyBinds.StartRecording;
            BSRSettings.Current.KeyBinds.PauseRecording = defaultKeyBinds.PauseRecording;
            BSRSettings.Current.KeyBinds.WindowSnap = defaultKeyBinds.WindowSnap;
            BSRSettings.Current.KeyBinds.ScreenSnap = defaultKeyBinds.ScreenSnap;
            BSRSettings.Current.KeyBinds.AllScreensSnap = defaultKeyBinds.AllScreensSnap;
            BSRSettings.Current.Save();

            LoadSettings();
        }
    }
}
