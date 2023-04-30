using DTO;
using GrandmaApi.Database;
using Domain.Models;
using GrandmaApi.Localization;
using Singularis.Internal.Domain.Specifications;
using GrandmaApi.Mediatr.Queries;
using GrandmaApi.Models.Configs;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Linq;

namespace GrandmaApi.Mediatr.Handlers.RabbitMqHandlers;

public class RecognizeUserQueryHandler : IRequestHandler<RecognizeUserQuery, BrokerCommandResponse<UserInfoDto>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IOptions<ThresholdConfig> _thresholdConfig;
    private readonly ILogger<RecognizeUserQueryHandler> _logger;
    private readonly ILocalizationService _localizationService;

    public RecognizeUserQueryHandler(IGrandmaRepository grandmaRepository, IOptions<ThresholdConfig> thresholdConfig, ILogger<RecognizeUserQueryHandler> logger, ILocalizationService localizationService)
    {
        _logger = logger;
        _localizationService = localizationService;
        _grandmaRepository = grandmaRepository;
        _thresholdConfig = thresholdConfig;
    }

    public async Task<BrokerCommandResponse<UserInfoDto>> Handle(RecognizeUserQuery request, CancellationToken cancellationToken)
    {
        var users = await _grandmaRepository.ListAsync(new NotDeleted<MongoUser, Guid>());
        var requestEmbeddings = request.RecognizeUserDto.Embeddings;
        var requestNorm = Math.Sqrt(requestEmbeddings.Sum(f => f * f));

        var usersLength = users.Where(u => u.Embeddings != null).Select(u => new
        {
            UserId = u.Id,
            Name = u.CommonName,
            Length = u.Embeddings.Zip(requestEmbeddings, (e1, e2) => e1 * e2).Sum() /
                     (Math.Sqrt(u.Embeddings.Sum(f => f * f)) * requestNorm)
        }).MaxBy(ul => ul.Length);
        
        if (usersLength?.Length > _thresholdConfig.Value.Threshold)
        {
            _logger.LogDebug($"Rocognized user: {usersLength.Name}, with length: {usersLength.Length}");
            return new BrokerCommandResponse<UserInfoDto>(new UserInfoDto()
            {
                Name = usersLength.Name,
                UserId = usersLength.UserId
            });
        }
        _logger.LogDebug("User not recognized");
        var error = new ErrorDto()
        {
            FieldName = nameof(RecognizeUserQuery.RecognizeUserDto.Embeddings),
            Message = _localizationService.GetString(LocalizationKey.User.NotRecognized)
        };
        return new BrokerCommandResponse<UserInfoDto>(error);
    }
}