using System;
using System.Diagnostics;
using System.IO;
using System.Runtime;

namespace SteamGiveawaysBot.Launcher
{
    class Program
    {
        static string[] fileNameCandidates = {
            $"{nameof(SteamGiveawaysBot)}",
            $"{nameof(SteamGiveawaysBot)}.exe",
            $"{nameof(SteamGiveawaysBot)}.dll" };

        static void Main(string[] args)
        {
            ApplicationUpdater.CheckForUpdates();
            ApplicationConfigurator.ApplyTextReplacements();

            string appExePath = GetApplicationExecutablePath();

            if (string.IsNullOrWhiteSpace(appExePath))
            {
                Console.WriteLine("Executable not found");
                return;
            }

            EnsureExecutionPermissions(appExePath);
            LaunchApplication(appExePath);
        }

        static void LaunchApplication(string appExePath)
        {
            Process app = new Process();
            app.StartInfo.WorkingDirectory = ApplicationInfo.LauncherDirectory;
            app.StartInfo.CreateNoWindow = true;

            if (appExePath.EndsWith(".dll"))
            {
                app.StartInfo.FileName = "dotnet";
                app.StartInfo.Arguments = $"\"{appExePath}\"";
            }
            else
            {
                app.StartInfo.FileName = appExePath;
            }

            app.Start();
            app.WaitForExit();
        }

        static string GetApplicationExecutablePath()
        {
            foreach (string fileNameCandidate in fileNameCandidates)
            {
                string filePath = Path.Combine(ApplicationInfo.ApplicationDirectory, fileNameCandidate);

                if (File.Exists(filePath))
                {
                    return filePath;
                }
            }

            return null;
        }

        static void EnsureExecutionPermissions(string exePath)
        {
            if (ApplicationInfo.Platform.Contains("linux"))
            {
                EnsureExecutionPermissionsOnLinux(exePath);
            }
        }

        static void EnsureExecutionPermissionsOnLinux(string exePath)
        {
            Process chmod = new Process();
            chmod.StartInfo.FileName = "chmod";
            chmod.StartInfo.Arguments = $"+x \"{exePath}\"";
            chmod.StartInfo.CreateNoWindow = true;

            chmod.Start();
            chmod.WaitForExit();
        }
    }
}
