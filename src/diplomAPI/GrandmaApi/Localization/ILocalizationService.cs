namespace GrandmaApi.Localization;

public interface ILocalizationService
{
    string GetString(string key, params object[] args);
}