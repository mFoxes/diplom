using DTO;
using MediatR;

namespace GrandmaApi.Mediatr.Queries;

public record RecognizeUserQuery(RecognizeUserDto RecognizeUserDto) : IRequest<BrokerCommandResponse<UserInfoDto>>;