namespace GrandmaApi.RabbitMqBus;

public static class MessageTypeConverter
{
    public static string FromType(MessageTypes type)
    {
        return type switch
        {
            MessageTypes.DeviceInfo => "device-info",
            MessageTypes.DeviceTook => "device-took",
            MessageTypes.DeviceInfoResult => "device-info-result",
            MessageTypes.DeviceTookResult => "device-took-result",
            MessageTypes.DeviceReturned => "device-returned",
            MessageTypes.DeviceReturnedResult => "device-returned-result",
            MessageTypes.EmbeddingsUpdate => "embeddings-update",
            MessageTypes.EmbeddingsResult => "embeddings-result",
            MessageTypes.RecognizeUser => "recognize-user",
            MessageTypes.RecognizeUserResult => "recognize-user-result",
            _ => "undefined"
        };
    }

    public static MessageTypes FromString(string message)
    {
        return message switch
        {
            "device-info" => MessageTypes.DeviceInfo,
            "device-took" => MessageTypes.DeviceTook,
            "device-info-result" => MessageTypes.DeviceInfoResult,
            "device-took-result" => MessageTypes.DeviceTookResult,
            "device-returned" => MessageTypes.DeviceReturned,
            "device-returned-result" => MessageTypes.DeviceReturnedResult,
            "embeddings-update" => MessageTypes.EmbeddingsUpdate,
            "embeddings-result" => MessageTypes.EmbeddingsResult,
            "recognize-user" => MessageTypes.RecognizeUser,
            "recognize-user-result" => MessageTypes.RecognizeUserResult,
            _ => MessageTypes.Undefined
        };
    }
}