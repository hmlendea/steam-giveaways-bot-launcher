using System.IO;
using System.Reflection;
using System.Runtime;

namespace SteamGiveawaysBot.Launcher
{
    public static class BotInfo
    {
        const string AppSettingsFileName = "appsettings.json";
        const string VersionFileName = "version.txt";
        const string PlatformFileName = "platform.txt";
        const string SgbDirectoryName = "sgb_app";

        public static string Name = nameof(SteamGiveawaysBot);

        public static string RootDirectory => Path.Combine(LauncherInfo.RootDirectory, SgbDirectoryName);

        public static string Version
        {
            get => GetFileValue(VersionFileName);
            set => WriteFileContent(VersionFileName, value);
        }

        public static string Platform
        {
            get
            {
                string platform = GetPlatformFromDepsFile();

                if (string.IsNullOrWhiteSpace(platform))
                {
                    platform = GetFileValue(PlatformFileName, "linux-x64");
                }

                return platform;
            }
        }

        public static string AppSettingsFilePath => Path.Combine(RootDirectory, AppSettingsFileName);

        static string GetFileValue(string fileName)
            => GetFileValue(fileName, null);

        static string GetFileValue(string fileName, string defaultValue)
        {
            string fileContent = GetFileContent(fileName);
            string firstValue = string.Empty;

            foreach (char c in fileContent ?? string.Empty)
            {
                if (char.IsWhiteSpace(c) ||
                    c == '\r' ||
                    c == '\n')
                {
                    break;
                }

                firstValue += c;
            }

            if (string.IsNullOrWhiteSpace(firstValue))
            {
                return defaultValue;
            }

            return firstValue;
        }

        static string GetFileContent(string fileName)
        {
            string filePath = Path.Combine(RootDirectory, fileName);

            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }

            return null;
        }

        static void WriteFileContent(string fileName, string fileContent)
        {
            string filePath = Path.Combine(RootDirectory, fileName);
            File.WriteAllText(filePath, fileContent);
        }

        static string GetPlatformFromDepsFile()
        {
            string depsFilePath = Path.Combine(RootDirectory, $"{Name}.deps.json");
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
