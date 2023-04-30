using DTO;
using GrandmaApi.Models.MessageModels;
using MediatR;

namespace GrandmaApi.Mediatr.Commands;

public record BookDeviceCommand(DeviceTookMessage DeviceTook) : IRequest<BrokerCommandResponse<UserHasOverdueBookingDto>>;