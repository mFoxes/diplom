using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GrandmaApi.Mediatr.Queries;

public record  GetFileQuery(Guid Id) : IRequest<HttpCommandResponse<FileStreamResult>>;