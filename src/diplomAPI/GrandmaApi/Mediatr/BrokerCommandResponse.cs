using DTO;
using MediatR;

namespace GrandmaApi.Mediatr;

public class BrokerCommandResponse
{
    public bool IsSucceed { get; set; }
    public ICollection<ErrorDto> Errors { get; set; }
    public BrokerCommandResponse()
    {
        Errors = new List<ErrorDto>();
        IsSucceed = true;
    }
}

public class BrokerCommandResponse <TResponse> : BrokerCommandResponse
{
    public TResponse Result { get; set; }
    public BrokerCommandResponse() : base()
    {
        
    }
    public BrokerCommandResponse(ErrorDto error)
    {
        Errors = new List<ErrorDto>();
        Errors.Add(error);
        IsSucceed = false;
    }
    public BrokerCommandResponse(TResponse result)
    {
        Result = result;
    }
}