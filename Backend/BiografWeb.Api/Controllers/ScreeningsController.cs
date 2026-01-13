using BiografWeb.Application.Screenings;
using BiografWeb.Application.Screenings.Models;
using BiografWeb.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BiografWeb.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScreeningsController(IScreeningsService service) : ControllerBase
{
    private readonly IScreeningsService _service = service;

    [HttpGet]
    public Task<List<Screening>> List([FromQuery] Guid? movieId, CancellationToken ct)
        => _service.ListAsync(movieId, ct);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Screening>> Get(Guid id, CancellationToken ct)
        => await _service.GetAsync(id, ct) is { } s ? Ok(s) : NotFound();

    [HttpPost]
    public Task<Screening> Create([FromBody] Screening s, CancellationToken ct) => _service.CreateAsync(s, ct);

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Screening>> Update(Guid id, [FromBody] Screening s, CancellationToken ct)
        => await _service.UpdateAsync(id, s, ct) is { } up ? Ok(up) : NotFound();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();

    [HttpGet("stats")]
    public Task<List<ScreeningStatsDto>> Stats(CancellationToken ct)
        => _service.GetStatsAsync(ct);

    [HttpGet("{id:guid}/stats")]
    public async Task<ActionResult<ScreeningDetailsStatsDto>> StatsById(Guid id, CancellationToken ct)
        => await _service.GetStatsByIdAsync(id, ct) is { } s ? Ok(s) : NotFound();
}


