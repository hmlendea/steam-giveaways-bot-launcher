using System;
using System.Diagnostics;
using System.IO;

using SteamGiveawaysBot.Launcher.Configuration;

namespace SteamGiveawaysBot.Launcher
{
    public sealed class Launcher(ApplicationSettings settings)
    {
        readonly string[] fileNameCandidates = [
            $"{nameof(SteamGiveawaysBot)}",
            $"{nameof(SteamGiveawaysBot)}.exe",
            $"{nameof(SteamGiveawaysBot)}.dll" ];

        readonly ApplicationSettings settings = settings;

        public void LaunchBot()
        {
            string appExePath = GetApplicationExecutablePath();

            if (string.IsNullOrWhiteSpace(appExePath))
            {
                Console.WriteLine("Executable not found");
                return;
            }

            EnsureExecutionPermissions(appExePath);
            RunBot(appExePath);
        }

        string GetApplicationExecutablePath()
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

        public void EnsureExecutionPermissions(string exePath)
        {
            if (settings.Platform.Contains("linux"))
            {
                EnsureExecutionPermissionsOnLinux(exePath);
            }
        }

        public static void EnsureExecutionPermissionsOnLinux(string exePath)
        {
            Process chmod = new();
            chmod.StartInfo.FileName = "chmod";
            chmod.StartInfo.Arguments = $"+x \"{exePath}\"";
            chmod.StartInfo.CreateNoWindow = true;

            chmod.Start();
            chmod.WaitForExit();
        }

        void RunBot(string exePath)
        {
            Process app = new();
            app.StartInfo.WorkingDirectory = LauncherInfo.RootDirectory;
            app.StartInfo.CreateNoWindow = true;

            if (exePath.EndsWith(".dll"))
            {
                app.StartInfo.FileName = "dotnet";
                app.StartInfo.Arguments = $"\"{exePath}\" --username {settings.Username} --ssk {settings.SharedSecretKey}";
            }
            else
            {
                app.StartInfo.FileName = exePath;
                app.StartInfo.Arguments = $"--username {settings.Username} --ssk {settings.SharedSecretKey}";
            }

            app.Start();

            if (settings.Platform.StartsWith("win"))
            {
                return;
            }

            app.WaitForExit();
        }
    }
}
