using System.IO;
using System.Reflection;
using System.Runtime;

namespace SteamGiveawaysBot.Launcher
{
    public static class LauncherInfo
    {
        const string PlatformFileName = "platform.txt";

        static string rootDirectory;

        public static string Name = $"{nameof(SteamGiveawaysBot)}.{nameof(Launcher)}";

        /// <summary>
        /// The application directory.
        /// </summary>
        public static string RootDirectory
        {
            get
            {
                if (rootDirectory == null)
                {
                    rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                }

                return rootDirectory;
            }
        }

        public static string PlaformFilePath => Path.Combine(RootDirectory, PlatformFileName);

        public static string Platform
        {
            get
            {
                string platform = GetPlatformFromDepsFile();

                if (string.IsNullOrWhiteSpace(platform))
                {
                    platform = FileHandler.ReadValue(PlaformFilePath, "linux-x64");
                }

                return platform;
            }
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
