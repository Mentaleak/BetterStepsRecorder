using System;
using System.Drawing;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI
{
    /// <summary>
    /// Provides global access to status strip functionality
    /// </summary>
    public static class StatusManager
    {
        private static StatusStripManager _instance;
        private static bool _initialized = false;

        /// <summary>
        /// Initializes the StatusManager with a form
        /// </summary>
        /// <param name="parentForm">The form to which the status strip will be added</param>
        public static void Initialize(Form parentForm)
        {
            if (!_initialized)
            {
                _instance = new StatusStripManager(parentForm);
                _initialized = true;
            }
        }

        /// <summary>
        /// Shows a message in the status strip that will fade out after a few seconds
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="isError">Whether the message is an error (displayed in red)</param>
        public static void ShowMessage(string message, bool isError = false)
        {
            if (!_initialized)
                throw new InvalidOperationException("StatusManager has not been initialized. Call StatusManager.Initialize() first.");
            
            _instance.ShowMessage(message, isError);
        }

        /// <summary>
        /// Shows a success message in green
        /// </summary>
        /// <param name="message">The success message to display</param>
        public static void ShowSuccess(string message)
        {
            if (!_initialized)
                throw new InvalidOperationException("StatusManager has not been initialized. Call StatusManager.Initialize() first.");
            
            _instance.ShowSuccess(message);
        }

        /// <summary>
        /// Clears the current message and hides the status strip
        /// </summary>
        public static void ClearMessage()
        {
            if (!_initialized)
                return;
                
            _instance.ClearMessage();
        }

        /// <summary>
        /// Gets whether the StatusManager has been initialized
        /// </summary>
        public static bool IsInitialized => _initialized;
    }
}