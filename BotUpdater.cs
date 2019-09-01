using System;
using System.IO;
using System.IO.Compression;
using System.Net;

using SteamGiveawaysBot.Launcher.Configuration;

namespace SteamGiveawaysBot.Launcher
{
    public sealed class BotUpdater
    {
        const string LatestVersionFileNameFormat = "steam-giveaways-bot_{0}_{1}.zip";
        const string LatestVersionArchiveUrlFormat = "http://stayback.zapto.org/sgb/{0}";
        const string LatestVersionStringUrl = "http://stayback.zapto.org/sgb/version";

        readonly ApplicationSettings settings;

        public BotUpdater(ApplicationSettings settings)
        {
            this.settings = settings;
        }

        public void CheckForUpdates()
        {
            Console.WriteLine("Checking for updates ...");

            try
            {
                string latestVersion = GetLatestVersionString();

                if (!Directory.Exists(BotInfo.RootDirectory) ||
                    string.Compare(latestVersion, BotInfo.Version) > 0)
                {
                    UpdateApplication(latestVersion);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking for updates: {ex.Message}");
            }
        }

        public void UpdateApplication(string version)
        {
            string archivePath = Path.Combine(LauncherInfo.RootDirectory, "sgb.zip");
            
            DownloadSgb(archivePath, version, settings.Platform);

            if (Directory.Exists(BotInfo.RootDirectory))
            {
                Directory.Delete(BotInfo.RootDirectory, true);
            }

            Directory.CreateDirectory(BotInfo.RootDirectory);

            ZipFile.ExtractToDirectory(archivePath, BotInfo.RootDirectory);

            File.Delete(archivePath);
            BotInfo.Version = version;
        }

        string GetLatestVersionString()
        {
            using (WebClient client = new WebClient())
            {
                string retrievedValue = client
                    .DownloadString(LatestVersionStringUrl)
                    .Replace(" ", "")
                    .Replace("\r", "")
                    .Replace("\n", "");
                
                if (retrievedValue.StartsWith('v'))
                {
                    return retrievedValue.Substring(1);
                }

                return retrievedValue;
            }
        }

        void DownloadSgb(string filePath, string version, string platform)
        {
            string archiveName = string.Format(LatestVersionFileNameFormat, version, platform);
            string url = string.Format(LatestVersionArchiveUrlFormat, archiveName);
            
            Console.WriteLine($"Downloading version {version} ...");

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, filePath);
            }
        }
    }
}
