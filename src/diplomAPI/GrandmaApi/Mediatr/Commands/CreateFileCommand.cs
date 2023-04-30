using DTO;
using MediatR;

namespace GrandmaApi.Mediatr.Commands;

public record CreateFileCommand(IFormFile File) : IRequest<HttpCommandResponse<FileDto>>;