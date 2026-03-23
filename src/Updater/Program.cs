using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Updater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                    return;

                string installPath = args[0];
                string exeFilename = args[1];

                // Poll for BSR process exit — max 10 seconds, 500ms interval
                bool exited = WaitForProcessExit(exeFilename, timeoutMs: 10_000, intervalMs: 500);
                if (!exited)
                    return; // Abort — process still running, do not modify any files

                string updateDir = Path.Combine(Path.GetTempPath(), "BSR_update");

                // Copy all files from %TEMP%\BSR_update\ to install path, overwriting
                foreach (string sourceFile in Directory.GetFiles(updateDir))
                {
                    string destFile = Path.Combine(installPath, Path.GetFileName(sourceFile));
                    File.Copy(sourceFile, destFile, overwrite: true);
                }

                // Relaunch BSR
                string bsrExe = Path.Combine(installPath, exeFilename);
                Process.Start(new ProcessStartInfo
                {
                    FileName = bsrExe,
                    UseShellExecute = true
                });
            }
            catch
            {
                // Exit silently without throwing
            }
        }

        private static bool WaitForProcessExit(string exeFilename, int timeoutMs, int intervalMs)
        {
            string processName = Path.GetFileNameWithoutExtension(exeFilename);
            int elapsed = 0;
            while (elapsed < timeoutMs)
            {
                bool running = Process.GetProcessesByName(processName).Any();
                if (!running)
                    return true;
                Thread.Sleep(intervalMs);
                elapsed += intervalMs;
            }
            // Final check
            return !Process.GetProcessesByName(processName).Any();
        }
    }
}
