using BiografWeb.Application.Auditoriums;
using BiografWeb.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BiografWeb.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditoriumsController(IAuditoriumService service) : ControllerBase
{
    private readonly IAuditoriumService _service = service;

    [HttpGet]
    public Task<List<Auditorium>> List(CancellationToken ct) => _service.ListAsync(ct);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Auditorium>> Get(Guid id, CancellationToken ct)
        => await _service.GetAsync(id, ct) is { } a ? Ok(a) : NotFound();

    [HttpPost]
    public async Task<ActionResult<Auditorium>> Create([FromBody] Auditorium a, CancellationToken ct)
    {
        var created = await _service.CreateAsync(a, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Auditorium>> Update(Guid id, [FromBody] Auditorium a, CancellationToken ct)
        => await _service.UpdateAsync(id, a, ct) is { } up ? Ok(up) : NotFound();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}


