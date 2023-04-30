using System.Security.Claims;
using DTO;
using GrandmaApi.Extensions;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;
using GrandmaApi.Mediatr.Queries;
using GrandmaApi.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrandmaApi.Controllers;

[Authorize]
[Route("[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookingsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPut]
    [Route("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateBooking(Guid id, [FromBody]UpdateBookingDto bookedDevice)
    {
        if (bookedDevice == null)
            return BadRequest();
        
        var command = new UpdateBookingCommand(bookedDevice);
        var result = await _mediator.Send(command);
        return this.FormResponse(result);
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetBookings([FromQuery] int take = 10,
                                                      [FromQuery] int skip = 10,
                                                      [FromQuery] BookingsOrderBy orderBy = BookingsOrderBy.Name,
                                                      [FromQuery] OrderDirections orderDir = OrderDirections.Asc,
                                                      [FromQuery(Name = "filter_inventoryNumber")] string? filterInventoryNumber = null,
                                                      [FromQuery(Name = "filter_name")] string? filterName = null,
                                                      [FromQuery(Name = "filter_takedBy")] string? filterTakedBy = null)
    {
        var query = new GetBookingsQuery()
        {
            Take = take,
            Skip = skip,
            OrderBy = orderBy,
            OrderDirection = orderDir,
            FilterInventoryNumber = filterInventoryNumber,
            FilterName = filterName,
            FilterTakedBy = filterTakedBy
        };
        var result = await _mediator.Send(query);
        return this.FormResponse(result);
    }
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetBooking(Guid id)
    {
        var query = new GetBookingByIdQuery(id);
        var result = await _mediator.Send(query);
        return this.FormResponse(result);
    }

    [HttpGet]
    [Route("overdue")]
    public async Task<IActionResult> Overdue()
    {
        var query = new GetCurrentUserOverdueBookingsCountQuery();
        var result = await _mediator.Send(query);
        return this.FormResponse(result);
    }

    [HttpGet]
    [Route("overdue/list")]
    public async Task<IActionResult> OverdueList()
    {
        var query = new GetUserOverdueBookingsQuery();
        var result = await _mediator.Send(query);
        return this.FormResponse(result);
    }
}