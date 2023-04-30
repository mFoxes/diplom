using MediatR;
using DTO;
using GrandmaApi.Models.Enums;

namespace GrandmaApi.Mediatr.Queries;

public class GetBookingsQuery : IRequest<HttpCommandResponse<TableDto<BookingDto>>>
{
    public int Take { get; set; }
    public int Skip { get; set; }
    public OrderDirections OrderDirection { get; set; }
    public BookingsOrderBy OrderBy { get; set; }
    public string? FilterInventoryNumber { get; set; }
    public string? FilterName { get; set; }
    public string? FilterTakedBy { get; set; }
}