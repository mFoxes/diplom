using GrandmaApi.Models;
using GrandmaApi.Models.Enums;
using MediatR;
using DTO;

namespace GrandmaApi.Mediatr.Queries;

public class GetDevicesQuery : IRequest<HttpCommandResponse<TableDto<DeviceDto>>>
{
    public int Take { get; set; }
    public int Skip { get; set; }
    public DeviceOrderBy OrderBy { get; set; }
    public OrderDirections OrderDirection { get; set; }
    public string? FilterName { get; set; }
    public string? FilterInventoryNumber { get; set; }
}