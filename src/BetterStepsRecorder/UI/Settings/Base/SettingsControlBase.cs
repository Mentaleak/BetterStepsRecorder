using System;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings.Base
{
    /// <summary>
    /// Base class for settings UserControls providing common load/save pattern and state updates.
    /// </summary>
    public abstract class SettingsControlBase : UserControl
    {
        protected SettingsControlBase()
        {
            // Don't load settings here - derived classes must call LoadSettings() 
            // after InitializeComponent() to ensure controls exist
        }

        /// <summary>
        /// Override to load settings into UI controls.
        /// Must be called by derived class after InitializeComponent().
        /// </summary>
        protected abstract void LoadSettings();

        /// <summary>
        /// Call this after changing any setting to persist changes.
        /// </summary>
        protected void SaveAndRefresh()
        {
            RecordingSettings.SaveCurrent();

            // Update the parent form's node states
            if (ParentForm is Settings settingsForm)
            {
                settingsForm.UpdateNodeStates();
            }
        }
    }
}
