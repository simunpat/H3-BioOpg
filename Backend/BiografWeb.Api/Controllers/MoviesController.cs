using BiografWeb.Application.Movies;
using BiografWeb.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BiografWeb.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _service;

    public MoviesController(IMovieService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<Movie>>> List(CancellationToken ct)
        => await _service.ListAsync(ct);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Movie>> Get(Guid id, CancellationToken ct)
        => await _service.GetAsync(id, ct) is { } m ? Ok(m) : NotFound();

    [HttpPost]
    public async Task<ActionResult<Movie>> Create([FromBody] Movie movie, CancellationToken ct)
    {
        var created = await _service.CreateAsync(movie, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Movie>> Update(Guid id, [FromBody] Movie input, CancellationToken ct)
        => await _service.UpdateAsync(id, input, ct) is { } m ? Ok(m) : NotFound();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}


