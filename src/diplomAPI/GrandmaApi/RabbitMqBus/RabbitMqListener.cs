using System.Text;
using DTO;
using GrandmaApi.Localization;
using GrandmaApi.Mediatr;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Mediatr.Queries;
using GrandmaApi.Models.Configs;
using GrandmaApi.Models.MessageModels;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GrandmaApi.RabbitMqBus;

public class RabbitMqListener : IHostedService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IOptions<RabbitMqConfig> _config;
    private readonly IRabbitMqMessageService _messageService;
    private readonly IServiceScope _scopeService;
    private readonly ILogger<RabbitMqListener> _logger;
    private readonly IMediator _mediator;
    private readonly ILocalizationService _localization;
    public RabbitMqListener(IOptions<RabbitMqConfig> config, IServiceScopeFactory serviceFactory, ILoggerFactory loggerFactory)
    {
        _config = config;
        _scopeService = serviceFactory.CreateScope();
        _messageService = _scopeService.ServiceProvider.GetRequiredService<IRabbitMqMessageService>();
        _logger = loggerFactory.CreateLogger<RabbitMqListener>();
        _mediator = _scopeService.ServiceProvider.GetRequiredService<IMediator>();
        _localization = _scopeService.ServiceProvider.GetRequiredService<ILocalizationService>();
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_config.Value.ConnectionUri)
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: config.Value.GrandmaQueue, durable: false, exclusive: false,
            autoDelete: false, arguments: null);
    }
    
    private void DeliverEventHandler(object? o, BasicDeliverEventArgs e)
    {
        var props = e.BasicProperties;
        var type = MessageTypeConverter.FromString(props.Type);
        
        switch (type)
        {
            case MessageTypes.DeviceInfo :
                Process<InventoryNumberCheckMessage, GetDeviceInfoByInventoryNumberQuery, DeviceInfoDto>(o, e, MessageTypes.DeviceInfoResult);
                break;
            case MessageTypes.DeviceTook :
                Process<DeviceTookMessage, BookDeviceCommand, UserHasOverdueBookingDto>(o, e, MessageTypes.DeviceTookResult);
                break;
            case MessageTypes.DeviceReturned :
                Process<InventoryNumberCheckMessage, ReturnDeviceCommand, Unit>(o, e,
                    MessageTypes.DeviceReturnedResult);
                break;
            case MessageTypes.RecognizeUser : 
                Process<RecognizeUserDto, RecognizeUserQuery, UserInfoDto>(o, e, MessageTypes.RecognizeUserResult);
                break;
            case MessageTypes.EmbeddingsResult :
                Process<EmbeddingsResultDto, UpdateEmbeddingsCommand>(o, e);
                break;
            case MessageTypes.Undefined: _logger.LogWarning("Получено неизвестное сообщение");
                break;
        }
    }

    private void Process<TMessage, TCommand, TResponse>(object? o, BasicDeliverEventArgs ea, MessageTypes responseMessageType)
    where TCommand : IRequest<BrokerCommandResponse<TResponse>>
    {
        var response = ExecuteCommand<TMessage, TCommand, TResponse>(o, ea);
        _messageService.SendMessage(response, responseMessageType);
    }
    private void Process<TMessage, TCommand>(object? o, BasicDeliverEventArgs ea) 
        where TCommand : IRequest<BrokerCommandResponse<Unit>>
    {
        ExecuteCommand<TMessage, TCommand, Unit>(o, ea);
    }

    private BrokerCommandResponse<TResponse> ExecuteCommand<TMessage, TCommand, TResponse>(object? o,
        BasicDeliverEventArgs ea)
    where TCommand : IRequest<BrokerCommandResponse<TResponse>>
    {
        var json = Encoding.UTF8.GetString(ea.Body.ToArray());
        TMessage message;
        if (!TryParseJson(json, out message))
        {
            _logger.LogError($"Не удалось десериализовать сообщение типа {typeof(TMessage).Name}");
            var error = new ErrorDto()
            {
                FieldName = "Message",
                Message = _localization.GetString(LocalizationKey.Shared.CannotDeserialize)
            };
            var failed = new BrokerCommandResponse<TResponse>(error);
            return failed;
        }

        var command = (TCommand)Activator.CreateInstance(typeof(TCommand), message);
        var commandName = typeof(TCommand).Name;
        _logger.LogInformation($"Вызов команды {commandName}");
        BrokerCommandResponse<TResponse> response;
        try
        {
            response =  _mediator.Send(command).Result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            var error = new ErrorDto()
            {
                FieldName = "Message",
                Message = _localization.GetString(LocalizationKey.Shared.InternalServerError)
            };
            return new BrokerCommandResponse<TResponse>(error);
        }
        
        _logger.LogInformation($"Вызов {commandName} завершен");
        if (!response.IsSucceed)
        {
            var errors = "";
            if (response.Errors.Any())
            {
                errors = string.Join("\n", response.Errors.Select(e => e.Message));
            }
            _logger.LogWarning($"Команда {commandName} не выполнена. {errors}");
        }
        return response;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += DeliverEventHandler;
        _channel.BasicConsume(queue: _config.Value.GrandmaQueue, autoAck: true, consumer : consumer);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel.Close();
        _connection.Close();
        _scopeService.Dispose();
        return Task.CompletedTask;
    }
    private bool TryParseJson<T>(string json, out T result)
    {
        bool success = true;
        var settings = new JsonSerializerSettings
        {
            Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
            MissingMemberHandling = MissingMemberHandling.Error
        };
        result = JsonConvert.DeserializeObject<T>(json, settings);
        return success;
    }
}