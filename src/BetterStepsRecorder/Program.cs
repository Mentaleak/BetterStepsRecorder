using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.IO.Compression;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using static BetterStepsRecorder.WindowHelper;
using System.IO;
using System.ComponentModel;
using Debug = System.Diagnostics.Debug;
using Application = System.Windows.Forms.Application;
using System.Windows; // Add this for System.Windows.Point
using BetterStepsRecorder.Exporters;
using BetterStepsRecorder.UI;

namespace BetterStepsRecorder
{
    internal static partial class Program
    {
        public static ZipFileHandler? zip;

        public static List<RecordEvent> _recordEvents = new List<RecordEvent>();
        private static MainForm? _form1Instance;
        public static int EventCounter = 1;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            _form1Instance = new MainForm();

            Application.Run(_form1Instance);

            // Ensure proper cleanup of FlaUI resources
            WindowHelper.Cleanup();
        }

    }
}