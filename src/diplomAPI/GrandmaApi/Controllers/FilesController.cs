using System.Net;
using GrandmaApi.Database;
using GrandmaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTO;
using GrandmaApi.Extensions;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Mediatr.Queries;
using MediatR;

namespace GrandmaApi.Controllers;

[Route("[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetFile(Guid id)
    {
        var query = new GetFileQuery(id);
        var result = await _mediator.Send(query);
        return result.StatusCode == HttpStatusCode.OK ? result.Result : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> SaveFile()
    {
        var formFile = Request.Form.Files.GetFile("image");
        var command = new CreateFileCommand(formFile);
        var result = await _mediator.Send(command);
        return this.FormResponse(result);
    }
}