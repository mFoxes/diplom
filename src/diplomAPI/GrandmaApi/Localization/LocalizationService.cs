using Microsoft.Extensions.Options;
using GrandmaApi.Models.Configs;
using System.Text.Json;

namespace GrandmaApi.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly Dictionary<string, string> _localizationDictionary;
        private readonly IOptions<LocalizationConfig> _config;
        public LocalizationService(IOptions<LocalizationConfig> config)
        {
            _config = config;
            var json = File.ReadAllText(config.Value.LocalizationFile);
            _localizationDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }
        public string GetString(string key, params object[] args) => string.Format(_localizationDictionary[key], args);
    }
}