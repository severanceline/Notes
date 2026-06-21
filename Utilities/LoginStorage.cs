using System;
using System.IO;
using System.Text.Json;

namespace Noots.Utilities
{
    public static class LoginStorage
    {
        private static string path =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Noots",
                "login.json"
            );

        public static void SaveUser(Guid userId)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            var data = new
            {
                UserId = userId
            };

            File.WriteAllText(path, JsonSerializer.Serialize(data));
        }

        public static Guid? GetSavedUser()
        {
            if (!File.Exists(path))
                return null;

            var json = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<dynamic>(json);

            return Guid.Parse(data.GetProperty("UserId").ToString());
        }

        public static void Clear()
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}