using System.IO;
using System.Reflection;
using System.Runtime;

namespace SteamGiveawaysBot.Launcher
{
    public static class BotInfo
    {
        const string AppSettingsFileName = "appsettings.json";
        const string VersionFileName = "version.txt";
        const string RootDirectoryName = "sgb_app";

        public static string Name = nameof(SteamGiveawaysBot);

        public static string RootDirectory => Path.Combine(LauncherInfo.RootDirectory, RootDirectoryName);

        public static string AppSettingsFilePath => Path.Combine(RootDirectory, AppSettingsFileName);

        public static string VersionFilePath => Path.Combine(LauncherInfo.RootDirectory, VersionFileName);

        public static string Version
        {
            get => FileHandler.ReadValue(VersionFilePath);
            set => FileHandler.WriteContent(VersionFilePath, value);
        }
    }
}
