namespace GrandmaApi.RabbitMqBus;

public enum MessageTypes
{
    DeviceInfo,
    DeviceTook,
    DeviceInfoResult,
    DeviceTookResult,
    DeviceReturned,
    DeviceReturnedResult,
    EmbeddingsUpdate,
    EmbeddingsResult,
    RecognizeUser,
    RecognizeUserResult,
    Undefined
}