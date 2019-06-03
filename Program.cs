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
            BotUpdater.CheckForUpdates();
            BotConfigurator.ApplyTextReplacements();

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
            app.StartInfo.WorkingDirectory = LauncherInfo.RootDirectory;
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
                string exePath = Path.Combine(BotInfo.RootDirectory, fileNameCandidate);

                if (File.Exists(exePath))
                {
                    return exePath;
                }
            }

            return null;
        }

        static void EnsureExecutionPermissions(string exePath)
        {
            if (BotInfo.Platform.Contains("linux"))
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
