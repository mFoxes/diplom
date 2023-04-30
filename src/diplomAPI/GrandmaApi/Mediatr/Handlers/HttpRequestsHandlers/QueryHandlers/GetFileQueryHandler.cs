using System.Net;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Queries;
using GrandmaApi.Models.Configs;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Singularis.Internal.Domain.Specifications;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;

public class GetFileQueryHandler : IRequestHandler<GetFileQuery, HttpCommandResponse<FileStreamResult>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IOptions<ImagesConfig> _config;
    private readonly ILogger<GetFileQueryHandler> _logger;

    public GetFileQueryHandler(IGrandmaRepository grandmaRepository, IOptions<ImagesConfig> config, ILogger<GetFileQueryHandler> logger)
    {
        _logger = logger;
        _grandmaRepository = grandmaRepository;
        _config = config;
    }

    public async Task<HttpCommandResponse<FileStreamResult>> Handle(GetFileQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Requested file with id: {request.Id}");
        var file = await _grandmaRepository.GetAsync(new NotDeleted<FileModel, Guid>(request.Id));
        if (file == null)
        {
            _logger.LogWarning("File not found");
            return new HttpCommandResponse<FileStreamResult>(null)
            {
                StatusCode = HttpStatusCode.NotFound
            };
        }
        _logger.LogDebug($"Found file with name {file.StorredFileName}");
        var path = Path.Combine(_config.Value.Path, file.StorredFileName);
        Stream stream = File.OpenRead(path);
        var result = new FileStreamResult(stream, MimeTypes.GetMimeType(Path.GetFileName(file.StorredFileName)));
        return new HttpCommandResponse<FileStreamResult>(result);
    }
}