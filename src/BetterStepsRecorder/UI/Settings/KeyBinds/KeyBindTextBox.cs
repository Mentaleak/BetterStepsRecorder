using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    /// <summary>
    /// A TextBox that captures key combinations for hotkey assignment.
    /// </summary>
    public class KeyBindTextBox : TextBox
    {
        private Keys _currentKeys = Keys.None;
        private bool _isCapturing = false;

        /// <summary>
        /// Event fired when the key combination changes.
        /// </summary>
        public event EventHandler<Keys>? KeyBindChanged;

        /// <summary>
        /// Gets or sets the current key combination.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Keys CurrentKeys
        {
            get => _currentKeys;
            set
            {
                _currentKeys = value;
                Text = KeyBindHelper.KeysToString(value);
            }
        }

        /// <summary>
        /// Gets or sets the key combination as a string.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string KeyBindString
        {
            get => KeyBindHelper.KeysToString(_currentKeys);
            set
            {
                _currentKeys = KeyBindHelper.StringToKeys(value);
                Text = KeyBindHelper.KeysToString(_currentKeys);
            }
        }

        public KeyBindTextBox()
        {
            ReadOnly = true;
            Text = "Click to set...";
            BackColor = System.Drawing.SystemColors.Window;
            Cursor = Cursors.Hand;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            StartCapture();
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            StopCapture();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            StartCapture();
        }

        private void StartCapture()
        {
            _isCapturing = true;
            Text = "Press key combination...";
            BackColor = System.Drawing.Color.LightYellow;
        }

        private void StopCapture()
        {
            _isCapturing = false;
            Text = KeyBindHelper.KeysToString(_currentKeys);
            BackColor = System.Drawing.SystemColors.Window;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_isCapturing)
            {
                // Escape clears the binding
                if (keyData == Keys.Escape)
                {
                    _currentKeys = Keys.None;
                    StopCapture();
                    KeyBindChanged?.Invoke(this, _currentKeys);
                    return true;
                }

                // Tab moves focus
                if ((keyData & Keys.KeyCode) == Keys.Tab)
                {
                    StopCapture();
                    return base.ProcessCmdKey(ref msg, keyData);
                }

                // Update display while capturing
                Text = KeyBindHelper.KeysToString(keyData);

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (_isCapturing)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (_isCapturing)
            {
                // Build the key combination
                Keys keys = e.KeyCode;
                if (e.Control) keys |= Keys.Control;
                if (e.Shift) keys |= Keys.Shift;
                if (e.Alt) keys |= Keys.Alt;

                // Only accept if we have a non-modifier key
                Keys keyCode = keys & Keys.KeyCode;
                if (keyCode != Keys.None && keyCode != Keys.ControlKey && keyCode != Keys.ShiftKey && keyCode != Keys.Menu)
                {
                    _currentKeys = keys;
                    StopCapture();
                    KeyBindChanged?.Invoke(this, _currentKeys);
                }

                e.Handled = true;
            }
            else
            {
                base.OnKeyUp(e);
            }
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (_isCapturing)
            {
                // Allow all keys to be captured
                e.IsInputKey = true;
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }
        }
    }
}
