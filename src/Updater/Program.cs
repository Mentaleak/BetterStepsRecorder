using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Updater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string logPath = Path.Combine(Path.GetTempPath(), "BSRUpdater.log");
            var log = new StringBuilder();
            try
            {
                log.AppendLine($"[start] args={args.Length}");

                if (args.Length < 2)
                {
                    log.AppendLine("[abort] fewer than 2 args");
                    return;
                }

                string installPath = args[0];
                string exeFilename = args[1];
                log.AppendLine($"[args] installPath={installPath} exeFilename={exeFilename}");

                bool exited = WaitForProcessExit(exeFilename, timeoutMs: 10_000, intervalMs: 500);
                log.AppendLine($"[wait] exited={exited}");
                if (!exited)
                    return;

                string updateDir = Path.Combine(Path.GetTempPath(), "BSR_update");
                log.AppendLine($"[updateDir] exists={Directory.Exists(updateDir)}");

                foreach (string sourceFile in Directory.EnumerateFiles(updateDir))
                {
                    string destFile = Path.Combine(installPath, Path.GetFileName(sourceFile));
                    log.AppendLine($"[copy] {sourceFile} -> {destFile}");
                    File.Copy(sourceFile, destFile, overwrite: true);
                }

                string bsrExe = Path.Combine(installPath, exeFilename);
                log.AppendLine($"[launch] {bsrExe}");
                Process.Start(new ProcessStartInfo
                {
                    FileName = bsrExe,
                    UseShellExecute = true
                });
                log.AppendLine("[done]");
            }
            catch (Exception ex)
            {
                log.AppendLine($"[exception] {ex}");
            }
            finally
            {
                try { File.WriteAllText(logPath, log.ToString()); } catch { }
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
