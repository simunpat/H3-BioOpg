using BiografWeb.Application.Bookings;
using BiografWeb.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BiografWeb.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(IBookingsService service) : ControllerBase
{
    private readonly IBookingsService _service = service;

    [HttpGet]
    public Task<List<Booking>> List([FromQuery] Guid? screeningId, [FromQuery] Guid? userId, CancellationToken ct)
        => _service.ListAsync(screeningId, userId, ct);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Booking>> Get(Guid id, CancellationToken ct)
        => await _service.GetAsync(id, ct) is { } b ? Ok(b) : NotFound();

    [HttpPost]
    public Task<Booking> Create([FromBody] Booking b, CancellationToken ct) => _service.CreateAsync(b, ct);

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Booking>> Update(Guid id, [FromBody] Booking b, CancellationToken ct)
        => await _service.UpdateAsync(id, b, ct) is { } up ? Ok(up) : NotFound();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}


