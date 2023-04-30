using DTO;
using GrandmaApi.Models.MessageModels;
using MediatR;

namespace GrandmaApi.Mediatr.Queries;

public record GetDeviceInfoByInventoryNumberQuery(InventoryNumberCheckMessage InventoryNumberMessage) :IRequest<BrokerCommandResponse<DeviceInfoDto>>;