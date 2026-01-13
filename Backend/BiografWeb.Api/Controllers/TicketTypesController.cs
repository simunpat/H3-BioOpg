using BiografWeb.Application.TicketTypes;
using BiografWeb.Application.TicketTypes.Models;
using BiografWeb.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BiografWeb.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketTypesController(ITicketTypesService service) : ControllerBase
{
    private readonly ITicketTypesService _service = service;

    [HttpGet]
    public Task<List<TicketType>> List(CancellationToken ct) => _service.ListAsync(ct);

    [HttpPost]
    public Task<TicketType> Create([FromBody] TicketType tt, CancellationToken ct) => _service.CreateAsync(tt, ct);

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TicketType>> Update(Guid id, [FromBody] TicketType tt, CancellationToken ct)
        => await _service.UpdateAsync(id, tt, ct) is { } up ? Ok(up) : NotFound();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();

    [HttpGet("stats/inuse")]
    public Task<List<TicketTypeInUseDto>> InUseStats(CancellationToken ct) => _service.GetInUseCountsAsync(ct);

    [HttpGet("stats/revenue")]
    public Task<List<TicketTypeRevenueDto>> RevenueStats(CancellationToken ct) => _service.GetRevenueAsync(ct);
}


