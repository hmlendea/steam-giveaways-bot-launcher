using System.Collections.Generic;
using System.IO;

namespace SteamGiveawaysBot.Launcher
{
    public static class ApplicationConfigurator
    {
        const string TextReplacementsFileName = "textreplacements.txt";

        public static void ApplyTextReplacements()
        {
            string appSettingsFilePath = Path.Combine(ApplicationInfo.ApplicationDirectory, "appsettings.json");
            IDictionary<string, string> textReplacements = LoadTextReplacements();

            string fileContent = File.ReadAllText(appSettingsFilePath);

            foreach (string placeholder in textReplacements.Keys)
            {
                fileContent = fileContent.Replace(placeholder, textReplacements[placeholder]);
            }

            File.WriteAllText(appSettingsFilePath, fileContent);
        }

        static IDictionary<string, string> LoadTextReplacements()
        {
            string filePath = Path.Combine(ApplicationInfo.LauncherDirectory, TextReplacementsFileName);

            IDictionary<string, string> textReplacements = new Dictionary<string, string>();

            foreach (string line in File.ReadAllLines(filePath))
            {
                int splitIndex = line.IndexOf('=');
                string placeholder = line.Substring(0, splitIndex);
                string value = line.Substring(splitIndex + 1);

                textReplacements.Add(placeholder, value);
            }

            return textReplacements;
        }
    }
}
