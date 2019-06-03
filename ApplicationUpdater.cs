using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace SteamGiveawaysBot.Launcher
{
    public static class ApplicationUpdater
    {
        const string LatestVersionFileNameFormat = "steam-giveaways-bot_{0}_{1}.zip";
        const string LatestVersionArchiveUrlFormat = "http://hori.go.ro/sgb/{0}";
        const string LatestVersionStringUrl = "http://hori.go.ro/sgb/version";

        public static void CheckForUpdates()
        {
            Console.WriteLine("Checking for updates...");

            try
            {
                string latestVersion = ApplicationUpdater.GetLatestVersionString();

                if (!Directory.Exists(ApplicationInfo.ApplicationDirectory) ||
                    string.Compare(latestVersion, ApplicationInfo.ApplicationVersion) > 0)
                {
                    ApplicationUpdater.UpdateApplication(latestVersion);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking for updates: {ex.Message}");
            }
        }

        public static string GetLatestVersionString()
        {
            using (WebClient client = new WebClient())
            {
                return client
                    .DownloadString(LatestVersionStringUrl)
                    .Replace(" ", "")
                    .Replace("\r", "")
                    .Replace("\n", "");;
            }
        }

        public static void UpdateApplication(string version)
        {
            string archivePath = Path.Combine(ApplicationInfo.LauncherDirectory, "sgb.zip");
            
            DownloadSgb(archivePath, version, ApplicationInfo.Platform);

            if (Directory.Exists(ApplicationInfo.ApplicationDirectory))
            {
                Directory.Delete(ApplicationInfo.ApplicationDirectory, true);
            }

            Directory.CreateDirectory(ApplicationInfo.ApplicationDirectory);

            ZipFile.ExtractToDirectory(archivePath, ApplicationInfo.ApplicationDirectory);

            File.Delete(archivePath);
            ApplicationInfo.ApplicationVersion = version;
        }

        public static void DownloadSgb(string filePath, string version, string platform)
        {
            string archiveName = string.Format(LatestVersionFileNameFormat, version, platform);
            string url = string.Format(LatestVersionArchiveUrlFormat, archiveName);
            
            Console.WriteLine($"Downloading version {version}...");

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, filePath);
            }
        }
    }
}
