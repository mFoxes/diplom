using System.Net;
using GrandmaApi.Mediatr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GrandmaApi.Extensions;

public static class ControllerExtensions
{
    public static IActionResult FormResponse<TResponse>(this ControllerBase controller,HttpCommandResponse<TResponse> commandResponse)
    {
        return commandResponse.StatusCode switch
        {
            HttpStatusCode.NotFound => controller.NotFound(),
            HttpStatusCode.BadRequest => controller.BadRequest(commandResponse.ErrorsList),
            HttpStatusCode.Forbidden => new ObjectResult(commandResponse.ErrorsList){StatusCode = 403},
            _ => controller.Ok(commandResponse.Result)
        };
    }
}