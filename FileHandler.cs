using System.IO;
using System.Reflection;
using System.Runtime;

namespace SteamGiveawaysBot.Launcher
{
    public static class FileHandler
    {
        public static string ReadValue(string path)
            => ReadValue(path, null);

        public static string ReadValue(string path, object defaultValue)
        {
            string fileContent = ReadContent(path);
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
                return defaultValue.ToString();
            }

            return firstValue;
        }

        public static string ReadContent(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            return null;
        }

        public static void WriteContent(string path, string content)
        {
            File.WriteAllText(path, content);
        }
    }
}
