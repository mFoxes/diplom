using System.Net.Http.Headers;
using System.Text.Json;
using GrandmaApi.Models.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace GrandmaApi.Notification.MessageServices;

public class MattermostService : IMattermostMessageService
{
    private readonly IOptions<MattermostConfig> _config;
    private readonly HttpClient _client;
    private readonly ILogger<MattermostService> _logger;
    public MattermostService(IOptions<MattermostConfig> config, ILogger<MattermostService> logger, IHttpClientFactory httpFactory)
    {
        _config = config;
        _logger = logger;
        _client = httpFactory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, _config.Value.AuthToken);
    }
    public async Task SendAsync(string username, string subject, string message)
    {
        var configValue = _config.Value;
        var botId = await GetBotIdAsync(configValue.BotIdUrl);
        if (botId == null)
        {
            _logger.LogError("Бот не найден");
            return;
        }
        var userId = await GetUserIdAsync(configValue.UserIdUrl, username);
        if (userId == null)
        {
            _logger.LogError($"Пользователь {username} не найден");
            return;
        }
        var chatId = await GetChatIdAsync(configValue.DirectChatUrl, botId, userId);
        if (chatId == null)
        {
            _logger.LogError($"Чат между ботом и пользователем {username} не найден");
            return;
        }
        await SendMessageAsync(configValue.MessagePostUrl, new MattermostMessage(message, chatId));
    }

    private async Task SendMessageAsync(string uri, MattermostMessage message)
    {
        var content = JsonSerializer.Serialize(message);
        await _client.PostAsync(uri, new StringContent(content));
    }
    private async Task<string> GetBotIdAsync(string url)
    {
        var botIdResponse = await _client.GetAsync(url);
        var botIdContent = await botIdResponse.Content.ReadAsStringAsync();
        var contentDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(botIdContent);
        return contentDictionary.ContainsKey("id") ? contentDictionary["id"].ToString() : null;
    }

    private async Task<string> GetUserIdAsync(string url, string username)
    {
        var content = JsonSerializer.Serialize(new[] { username });
        var userIdResponse = await _client.PostAsync(url, new StringContent(content));
        var userIdContent = await userIdResponse.Content.ReadAsStringAsync();
        var contentDictionary = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(userIdContent);
        
        return contentDictionary.Any() && contentDictionary[0].ContainsKey("id")
            ? contentDictionary[0]["id"].ToString()
            : null;
    }

    private async Task<string> GetChatIdAsync(string url, string botId, string userId)
    {
        var jsonContent = JsonSerializer.Serialize(new[]{botId, userId});
        var chatIdResponse = await _client.PostAsync(url, new StringContent(jsonContent));
        var chatIdContent = await chatIdResponse.Content.ReadAsStringAsync();
        var contentDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(chatIdContent);
        return contentDictionary.ContainsKey("id") ? contentDictionary["id"].ToString() : null;
    }


}