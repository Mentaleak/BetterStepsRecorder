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
            string logPath = Path.Combine(Path.GetTempPath(), "BSRUpdater.log");
            try
            {
                File.WriteAllText(logPath, $"[start] args={args.Length}\n");

                if (args.Length < 2)
                {
                    File.AppendAllText(logPath, "[abort] fewer than 2 args\n");
                    return;
                }

                string installPath = args[0];
                string exeFilename = args[1];
                File.AppendAllText(logPath, $"[args] installPath={installPath} exeFilename={exeFilename}\n");

                // Poll for BSR process exit — max 10 seconds, 500ms interval
                bool exited = WaitForProcessExit(exeFilename, timeoutMs: 10_000, intervalMs: 500);
                File.AppendAllText(logPath, $"[wait] exited={exited}\n");
                if (!exited)
                    return; // Abort — process still running, do not modify any files

                string updateDir = Path.Combine(Path.GetTempPath(), "BSR_update");
                File.AppendAllText(logPath, $"[updateDir] exists={Directory.Exists(updateDir)}\n");

                // Copy all files from %TEMP%\BSR_update\ to install path, overwriting
                foreach (string sourceFile in Directory.GetFiles(updateDir))
                {
                    string destFile = Path.Combine(installPath, Path.GetFileName(sourceFile));
                    File.AppendAllText(logPath, $"[copy] {sourceFile} -> {destFile}\n");
                    File.Copy(sourceFile, destFile, overwrite: true);
                }

                // Relaunch BSR
                string bsrExe = Path.Combine(installPath, exeFilename);
                File.AppendAllText(logPath, $"[launch] {bsrExe}\n");
                Process.Start(new ProcessStartInfo
                {
                    FileName = bsrExe,
                    UseShellExecute = true
                });
                File.AppendAllText(logPath, "[done]\n");
            }
            catch (Exception ex)
            {
                try { File.AppendAllText(logPath, $"[exception] {ex}\n"); } catch { }
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
