using System;
using System.Diagnostics;
using System.IO;
using System.Runtime;

using NuciCLI;

using SteamGiveawaysBot.Launcher.Configuration;

namespace SteamGiveawaysBot.Launcher
{
    class Program
    {
        static string[] fileNameCandidates = {
            $"{nameof(SteamGiveawaysBot)}",
            $"{nameof(SteamGiveawaysBot)}.exe",
            $"{nameof(SteamGiveawaysBot)}.dll" };

        static string[] UsernameOptions = { "-u", "--user", "--username" };

        static string[] SecretKeyOptions = { "-k", "--ssk", "--key", "--secretkey", "--sharedsecretkey" };

        static string[] PlatformOptions = { "-p", "--platform" };
        
        static ApplicationSettings settings;

        static void Main(string[] args)
        {
            settings = GetSettings(args);

            BotUpdater updater = new BotUpdater(settings);
            BotConfigurator configurator = new BotConfigurator(settings);

            updater.CheckForUpdates();
            configurator.ApplyTextReplacements();

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
            if (settings.Platform.Contains("linux"))
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

        static ApplicationSettings GetSettings(string[] args)
        {
            ApplicationSettings settings = new ApplicationSettings();
            settings.BotUsername = CliArgumentsReader.GetOptionValue(args, UsernameOptions);
            settings.BotSharedSecretKey = CliArgumentsReader.GetOptionValue(args, SecretKeyOptions);

            if (CliArgumentsReader.HasOption(args, PlatformOptions))
            {
                settings.Platform = CliArgumentsReader.GetOptionValue(args, PlatformOptions);
            }
            else
            {
                settings.Platform = GetPlatformFromDepsFile();

                if (string.IsNullOrWhiteSpace(settings.Platform))
                {
                    settings.Platform = FileHandler.ReadValue(LauncherInfo.PlaformFilePath, "linux-x64");
                }
            }

            return settings;
        }

        static string GetPlatformFromDepsFile()
        {
            string depsFilePath = Path.Combine(LauncherInfo.RootDirectory, $"{LauncherInfo.Name}.deps.json");
            string line = File.ReadAllLines(depsFilePath)[2];
            string[] split = line.Split('/');

            if (split.Length != 2)
            {
                return null;
            }

            return split[1].Substring(0, split[1].Length - 2);
        }
    }
}
