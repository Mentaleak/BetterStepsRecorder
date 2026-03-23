using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings.Base
{
    /// <summary>
    /// Base class for screenshot mode selection with radio buttons.
    /// Reduces duplication between ScreenshotClick and ScreenshotDrag.
    /// </summary>
    public abstract class ScreenshotModeSelector : SettingsControlBase
    {
        private RadioButton rdoCropped;
        private RadioButton rdoActiveWindow;
        private RadioButton rdoActiveScreen;
        private RadioButton rdoAllScreens;

        private readonly List<RadioButton> _radioButtons = new List<RadioButton>();

        protected ScreenshotModeSelector()
        {
            // Initialization happens after InitializeComponent is called by derived classes
        }

        /// <summary>
        /// Call this from derived class constructor after InitializeComponent.
        /// Wires up event handlers and loads initial settings.
        /// </summary>
        protected void InitializeBase()
        {
            // Locate the radio buttons from the derived class's Controls
            FindRadioButtons();

            // Collect all radio buttons
            _radioButtons.AddRange(new[] { rdoCropped, rdoActiveWindow, rdoActiveScreen, rdoAllScreens });

            // Wire up event handlers
            foreach (var rb in _radioButtons)
            {
                if (rb != null)
                {
                    rb.CheckedChanged += RadioButton_CheckedChanged;
                }
            }

            // Load settings after controls are initialized
            LoadSettings();
        }

        /// <summary>
        /// Finds the radio buttons from the derived class's Controls collection.
        /// </summary>
        private void FindRadioButtons()
        {
            foreach (Control control in Controls)
            {
                if (control is RadioButton rb)
                {
                    switch (rb.Name)
                    {
                        case "rdoCropped":
                            rdoCropped = rb;
                            break;
                        case "rdoActiveWindow":
                            rdoActiveWindow = rb;
                            break;
                        case "rdoActiveScreen":
                            rdoActiveScreen = rb;
                            break;
                        case "rdoAllScreens":
                            rdoAllScreens = rb;
                            break;
                    }
                }
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Only save when a radio button is checked (not when unchecked)
            if (sender is RadioButton rb && rb.Checked)
            {
                SaveSettings();
                SaveAndRefresh();
            }
        }

        /// <summary>
        /// Override to save the selected mode to Program properties.
        /// </summary>
        protected abstract void SaveSettings();

        /// <summary>
        /// Helper to get selected mode index (0=Cropped, 1=ActiveWindow, 2=ActiveScreen, 3=AllScreens).
        /// </summary>
        protected int GetSelectedModeIndex()
        {
            if (rdoAllScreens.Checked) return 3;
            if (rdoActiveScreen.Checked) return 2;
            if (rdoActiveWindow.Checked) return 1;
            return 0; // Cropped
        }

        /// <summary>
        /// Helper to set selected mode by index (0=Cropped, 1=ActiveWindow, 2=ActiveScreen, 3=AllScreens).
        /// </summary>
        protected void SetSelectedModeIndex(int index)
        {
            switch (index)
            {
                case 3:
                    rdoAllScreens.Checked = true;
                    break;
                case 2:
                    rdoActiveScreen.Checked = true;
                    break;
                case 1:
                    rdoActiveWindow.Checked = true;
                    break;
                default:
                    rdoCropped.Checked = true;
                    break;
            }
        }
    }
}
