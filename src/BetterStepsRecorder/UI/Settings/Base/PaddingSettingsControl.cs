using System;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings.Base
{
    /// <summary>
    /// Generic control for configuring padding values with a NumericUpDown.
    /// </summary>
    public partial class PaddingSettingsControl : SettingsControlBase
    {
        private readonly Func<int> _getter;
        private readonly Action<int> _setter;

        public PaddingSettingsControl(Func<int> getter, Action<int> setter, string labelText = "Padding (pixels):")
        {
            _getter = getter;
            _setter = setter;

            InitializeComponent();

            label1.Text = labelText;
            nudPadding.ValueChanged += NudPadding_ValueChanged;

            // Load settings after controls are initialized
            LoadSettings();
        }

        protected override void LoadSettings()
        {
            if (_getter != null)
            {
                nudPadding.Value = _getter();
            }
        }

        private void NudPadding_ValueChanged(object sender, EventArgs e)
        {
            _setter?.Invoke((int)nudPadding.Value);
            SaveAndRefresh();
        }
    }
}
