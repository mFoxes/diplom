using GrandmaApi.Models.MessageModels;
using MediatR;

namespace GrandmaApi.Mediatr.Commands;

public record ReturnDeviceCommand(InventoryNumberCheckMessage InventoryNumberMessage) : IRequest<BrokerCommandResponse<Unit>>;