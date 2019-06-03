using System.Collections.Generic;
using System.IO;

using SteamGiveawaysBot.Launcher.Configuration;

namespace SteamGiveawaysBot.Launcher
{
    public sealed class BotConfigurator
    {
        readonly ApplicationSettings settings;

        public BotConfigurator(ApplicationSettings settings)
        {
            this.settings = settings;
        }

        public void ApplyTextReplacements()
        {
            string fileContent = File.ReadAllText(BotInfo.AppSettingsFilePath);
            
            fileContent.Replace("[[BOT_SERVER_URL]]", "http://hori.go.ro:5123");
            fileContent.Replace("[[BOT_USERNAME]]", settings.BotUsername);
            fileContent.Replace("[[BOT_SECRET_KEY]]", settings.BotSharedSecretKey);

            File.WriteAllText(BotInfo.AppSettingsFilePath, fileContent);
        }
    }
}
