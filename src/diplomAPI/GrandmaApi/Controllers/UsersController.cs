using System.Security.Claims;
using Domain.Enums;
using GrandmaApi.Extensions;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Mediatr.Queries;
using GrandmaApi.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTO;

namespace GrandmaApi.Controllers;

[Route("[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("current")]
    public async Task<IActionResult> CurrentUser()
    {
        var query = new GetUserByLdapIdQuery();
        var result = await _mediator.Send(query);
        return this.FormResponse(result);
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers( [FromQuery] int take = 10,
                                                            [FromQuery] int skip = 10,
                                                            [FromQuery] UsersOrderBy orderBy = UsersOrderBy.Name,
                                                            [FromQuery] OrderDirections orderDir = OrderDirections.Asc,
                                                            [FromQuery(Name = "filter_name")] string? filterName = null)
    {
        var command = new GetUsersQuery
        {
            Take = take,
            Skip = skip,
            OrderBy = orderBy,
            OrderDirection = orderDir,
            FilterName = filterName
        };
        var result = await _mediator.Send(command);
        return this.FormResponse(result);
    }
    [HttpGet]
    [Route("usernames")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsernames()
    {
        var context = ControllerContext.HttpContext;
        var query = new GetUsernamesQuery();
        var result = await _mediator.Send(query); 
        return this.FormResponse(result);
    }

    [HttpGet]
    [Route("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query);

        return this.FormResponse(result);
    }
    
    [HttpPost("sync")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Synchronize()
    {
        var command = new SyncronizeUsersCommand();
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserCardDto user)
    {
        if (user == null)
            return BadRequest();
        user.Id = id;
        var command = new UpdateUserCommand(user);
        var result = await _mediator.Send(command);

        return this.FormResponse(result);
    }
}