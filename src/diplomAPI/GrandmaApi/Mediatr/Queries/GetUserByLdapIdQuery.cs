using Domain.Enums;
using DTO;
using MediatR;

namespace GrandmaApi.Mediatr.Queries;

public record GetUserByLdapIdQuery() : IRequest<HttpCommandResponse<ContextUserDto>>;