using System.IO;
using System.Text.Json;
using ToDo_App.Models;

namespace ToDo_App.Helpers
{
    public static class AppSecretsHelpers
    {
        private static readonly string _secretsFilePath = @"C:\Users\chris\AppData\Roaming\Microsoft\UserSecrets\2be339be-6e65-4a52-ac05-4aec9522b3f0\secrets.json";

        public static AppSecrets AppSecrets()
        {
            string jsonString = File.ReadAllText(_secretsFilePath);
            return JsonSerializer.Deserialize<AppSecrets>(jsonString)!;
        }
    }
}