using System.Text.Json.Serialization;

namespace GrandmaApi.Notification.MessageServices;

public class MattermostMessage
{
    [JsonPropertyName("message")] 
    public string Message { get; }

    [JsonPropertyName("channel_id")] 
    public string ChannelId { get; }

    public MattermostMessage(string message, string channelId)
    {
        Message = message;
        ChannelId = channelId;
    }
}