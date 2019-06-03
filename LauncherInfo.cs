using System.IO;
using System.Reflection;
using System.Runtime;

namespace SteamGiveawaysBot.Launcher
{
    public static class LauncherInfo
    {
        const string PlatformFileName = "platform.txt";

        static string rootDirectory;

        public static string Name = $"{nameof(SteamGiveawaysBot)}.{nameof(SteamGiveawaysBot.Launcher)}";

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
    }
}
