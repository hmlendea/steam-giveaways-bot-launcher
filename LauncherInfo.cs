using System.IO;
using System.Reflection;
using System.Runtime;

namespace SteamGiveawaysBot.Launcher
{
    public static class LauncherInfo
    {
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
    }
}
