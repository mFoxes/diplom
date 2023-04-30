using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Models.Configs;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.CommandHandlers;

public class CreateFileCommandHandler : IRequestHandler<CreateFileCommand, HttpCommandResponse<FileDto>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IOptions<ImagesConfig> _config;
    private readonly ILogger<CreateFileCommandHandler> _logger;

    public CreateFileCommandHandler(IGrandmaRepository grandmaRepository, IOptions<ImagesConfig> config, ILogger<CreateFileCommandHandler> logger)
    {
        _grandmaRepository = grandmaRepository;
        _config = config;
        _logger = logger;
    }

    public async Task<HttpCommandResponse<FileDto>> Handle(CreateFileCommand request, CancellationToken cancellationToken)
    {
        var formFile = request.File;
        var requestStream = formFile.OpenReadStream();
        var configValue = _config.Value;
        var guid = Guid.NewGuid();
        var filename = Path.GetFileNameWithoutExtension(formFile.FileName);
        var extension = Path.GetExtension(formFile.FileName);
        var newFileName = $"{filename}-{guid}{extension}";
        
        await _grandmaRepository.SaveAsync(new FileModel
        {
            Id = guid,
            StorredFileName = newFileName
        });
        if (!Directory.Exists(configValue.Path))
        {
            Directory.CreateDirectory(configValue.Path);
        }
        await using (var stream = File.Create(Path.Combine(configValue.Path, newFileName)))
        {
            await requestStream.CopyToAsync(stream);
        }
        _logger.LogDebug($"Created file {newFileName} with id {guid}");
        return new HttpCommandResponse<FileDto>(new FileDto() { File = guid });
    }
}