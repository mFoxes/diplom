using System.Net;
using DTO;

namespace GrandmaApi.Mediatr;

public class HttpCommandResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public ErrorListDto ErrorsList { get; set; } = new ()
    {
        Errors = new List<ErrorDto>()
    };
    public HttpCommandResponse()
    {
        StatusCode = HttpStatusCode.OK;
    }

}

public class HttpCommandResponse <TResponse> : HttpCommandResponse
{
    public TResponse Result { get; set; }
    public HttpCommandResponse() : base()
    {
        
    }
    public HttpCommandResponse(TResponse result)
    {
        Result = result;
    }

    public HttpCommandResponse(HttpStatusCode statusCode, ErrorDto error)
    {
        StatusCode = statusCode;
        ErrorsList.Errors.Add(error);
    }
    public HttpCommandResponse(HttpStatusCode statusCode, List<ErrorDto> errors)
    {
        StatusCode = statusCode;
        ErrorsList.Errors = errors;
    }
}