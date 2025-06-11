using System;
using System.IO;
using System.Net;
using System.Threading;

using NuciCLI;

using SteamGiveawaysBot.Launcher.Configuration;

namespace SteamGiveawaysBot.Launcher
{
    class Program
    {
        static readonly string[] UsernameOptions = ["-u", "--user", "--username"];
        static readonly string[] SecretKeyOptions = ["-k", "--ssk", "--key", "--secretkey", "--sharedsecretkey"];
        static readonly string[] PlatformOptions = ["-p", "--platform"];

        static TimeSpan ConnectionWaitTime => TimeSpan.FromSeconds(30);

        static void Main(string[] args)
        {
            WaitForInternet();

            ApplicationSettings settings = GetSettings(args);
            Launcher launcher = new(settings);
            BotUpdater updater = new(settings);

            updater.CheckForUpdates();
            launcher.LaunchBot();
        }

        static ApplicationSettings GetSettings(string[] args)
        {
            ApplicationSettings settings = new();
            settings.Username = CliArgumentsReader.GetOptionValue(args, UsernameOptions);
            settings.SharedSecretKey = CliArgumentsReader.GetOptionValue(args, SecretKeyOptions);

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

        static void WaitForInternet()
        {
            bool isConnected = false;

            while (!isConnected)
            {
                try
                {
                    using (WebClient client = new())
                    using (client.OpenRead("http://clients3.google.com/generate_204"))
                    {
                        isConnected = true;
                        return;
                    }
                }
                catch
                {
                    isConnected = false;
                }

                Thread.Sleep((int)ConnectionWaitTime.TotalMilliseconds);
            }
        }
    }
}
