using System.Net;
using GrandmaApi.Extensions;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Mediatr.Queries;
using GrandmaApi.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTO;

namespace GrandmaApi.Controllers;

[Authorize(Roles = "Admin")]
[Route("[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DevicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetDevices([FromQuery] int take = 10,
                                          [FromQuery] int skip = 10,
                                          [FromQuery] DeviceOrderBy orderBy = DeviceOrderBy.Name,
                                          [FromQuery] OrderDirections orderDir = OrderDirections.Asc,
                                          [FromQuery(Name = "filter_name")] string? filterName = null,
                                          [FromQuery(Name = "filter_inventoryNumber")] string? filterInventoryNumber = null  )
    {
        var query = new GetDevicesQuery
        {
            Take = take,
            Skip = skip,
            OrderBy = orderBy,
            OrderDirection = orderDir,
            FilterName = filterName,
            FilterInventoryNumber = filterInventoryNumber
        };
        var result = await _mediator.Send(query);
        return this.FormResponse(result);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeleteDevice(Guid id)
    {
        var command = new DeleteDeviceCommand(id);
        var result = await _mediator.Send(command);
        return this.FormResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDevice([FromBody]DeviceDto device)
    {
        if (device == null)
            return BadRequest();
        var command = new CreateDeviceCommand(device);
        var result = await _mediator.Send(command);

        return this.FormResponse(result);
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateDevice([FromBody] DeviceDto device)
    {
        if (device == null)
        {
            return BadRequest();
        }
        
        var command = new UpdateDeviceCommand(device);
        var result = await _mediator.Send(command);

        return this.FormResponse(result);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetDevice(Guid id)
    {
        var query = new GetDeviceByIdQuery(id);
        var result = await _mediator.Send(query);
        return this.FormResponse(result);
    }

    [HttpGet]
    [Route("{id:guid}/history")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHistory(Guid id)
    {
        var query = new GetDeviceHistory(id);
        var result = await _mediator.Send(query);
        return this.FormResponse(result);
    }
}