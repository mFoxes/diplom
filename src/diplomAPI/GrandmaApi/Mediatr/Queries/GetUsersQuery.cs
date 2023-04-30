using GrandmaApi.Models.Enums;
using MediatR;
using DTO;

namespace GrandmaApi.Mediatr.Queries;

public class GetUsersQuery : IRequest<HttpCommandResponse<TableDto<UserTableItemDto>>>
{
    public int Take { get; set; }
    public int Skip { get; set; }
    public UsersOrderBy OrderBy { get; set; }
    public OrderDirections OrderDirection { get; set; }
    public string? FilterName { get; set; }
}