namespace GrandmaApi.RabbitMqBus;

public interface IRabbitMqMessageService
{
    void SendMessage<T>(T message, MessageTypes messageType);
}