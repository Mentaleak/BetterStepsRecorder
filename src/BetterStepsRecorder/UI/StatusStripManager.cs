using System;
using System.Drawing;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI
{
    /// <summary>
    /// Manages a status strip with fade-out message functionality
    /// </summary>
    public class StatusStripManager
    {
        private readonly StatusStrip _statusStrip;
        private readonly ToolStripStatusLabel _statusLabel;
        private readonly System.Windows.Forms.Timer _fadeTimer;  // Explicitly use System.Windows.Forms.Timer
        private float _opacity = 1.0f;
        private const float FADE_STEP = 0.05f;
        private const int FADE_INTERVAL = 50; // milliseconds
        private const int MESSAGE_DISPLAY_TIME = 3000; // 3 seconds before fading starts

        /// <summary>
        /// Creates a new StatusStripManager
        /// </summary>
        /// <param name="parentForm">The form to which the status strip will be added</param>
        public StatusStripManager(Form parentForm)
        {
            // Create the status strip and label
            _statusStrip = new StatusStrip
            {
                Dock = DockStyle.Bottom,
                SizingGrip = false
            };

            _statusLabel = new ToolStripStatusLabel
            {
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _statusStrip.Items.Add(_statusLabel);
            parentForm.Controls.Add(_statusStrip);

            // Create and configure the fade timer
            _fadeTimer = new System.Windows.Forms.Timer  // Explicitly use System.Windows.Forms.Timer
            {
                Interval = FADE_INTERVAL,
                Enabled = false
            };
            _fadeTimer.Tick += FadeTimer_Tick;
        }

        /// <summary>
        /// Shows a message in the status strip that will fade out after a few seconds
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="isError">Whether the message is an error (displayed in red)</param>
        public void ShowMessage(string message, bool isError = false)
        {
            // Reset any ongoing fade
            _fadeTimer.Stop();
            _opacity = 1.0f;

            // Set the message and color
            _statusLabel.Text = message;
            _statusLabel.ForeColor = isError ? Color.Red : SystemColors.ControlText;
            
            // Ensure the status strip is visible
            _statusStrip.Visible = true;
            
            // Start the timer to begin fading after the display time
            System.Windows.Forms.Timer displayTimer = new System.Windows.Forms.Timer  // Explicitly use System.Windows.Forms.Timer
            {
                Interval = MESSAGE_DISPLAY_TIME,
                Enabled = true
            };
            
            displayTimer.Tick += (sender, e) =>
            {
                displayTimer.Stop();
                displayTimer.Dispose();
                _fadeTimer.Start();
            };
            
            displayTimer.Start();
        }

        /// <summary>
        /// Shows a success message in green
        /// </summary>
        /// <param name="message">The success message to display</param>
        public void ShowSuccess(string message)
        {
            // Reset any ongoing fade
            _fadeTimer.Stop();
            _opacity = 1.0f;

            // Set the message and color
            _statusLabel.Text = message;
            _statusLabel.ForeColor = Color.Green;
            
            // Ensure the status strip is visible
            _statusStrip.Visible = true;
            
            // Start the timer to begin fading after the display time
            System.Windows.Forms.Timer displayTimer = new System.Windows.Forms.Timer  // Explicitly use System.Windows.Forms.Timer
            {
                Interval = MESSAGE_DISPLAY_TIME,
                Enabled = true
            };
            
            displayTimer.Tick += (sender, e) =>
            {
                displayTimer.Stop();
                displayTimer.Dispose();
                _fadeTimer.Start();
            };
            
            displayTimer.Start();
        }

        /// <summary>
        /// Clears the current message and hides the status strip
        /// </summary>
        public void ClearMessage()
        {
            _fadeTimer.Stop();
            _statusLabel.Text = string.Empty;
            _statusStrip.Visible = false;
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            // Reduce opacity
            _opacity -= FADE_STEP;
            
            if (_opacity <= 0)
            {
                // When fully transparent, stop the timer and hide the message
                _fadeTimer.Stop();
                _statusLabel.Text = string.Empty;
                _statusLabel.ForeColor = SystemColors.ControlText;
                _opacity = 1.0f;
            }
            else
            {
                // Apply fading effect by adjusting the color's alpha component
                Color currentColor = _statusLabel.ForeColor;
                _statusLabel.ForeColor = Color.FromArgb(
                    (int)(255 * _opacity),
                    currentColor.R,
                    currentColor.G,
                    currentColor.B
                );
            }
        }

        /// <summary>
        /// Gets the underlying StatusStrip control
        /// </summary>
        public StatusStrip StatusStrip => _statusStrip;
    }
}