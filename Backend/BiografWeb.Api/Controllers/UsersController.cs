using BiografWeb.Application.Users;
using BiografWeb.Application.Users.Models;
using BiografWeb.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BiografWeb.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUsersService service) : ControllerBase
{
    private readonly IUsersService _service = service;

    [HttpGet]
    public Task<List<User>> List(CancellationToken ct) => _service.ListAsync(ct);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<User>> Get(Guid id, CancellationToken ct)
        => await _service.GetAsync(id, ct) is { } u ? Ok(u) : NotFound();

    [HttpGet("byEmail")]
    public Task<User?> FindByEmail([FromQuery] string email, CancellationToken ct)
        => _service.FindByEmailAsync(email, ct);

    [HttpPost]
    public Task<User> Create([FromBody] User u, CancellationToken ct) => _service.CreateAsync(u, ct);

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<User>> Update(Guid id, [FromBody] User u, CancellationToken ct)
        => await _service.UpdateAsync(id, u, ct) is { } up ? Ok(up) : NotFound();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();

    [HttpGet("stats")]
    public Task<List<UserStatsDto>> Stats(CancellationToken ct) => _service.GetStatsAsync(ct);

    [HttpGet("{id:guid}/stats")]
    public async Task<ActionResult<UserDetailsStatsDto>> StatsById(Guid id, CancellationToken ct)
        => await _service.GetStatsByIdAsync(id, ct) is { } s ? Ok(s) : NotFound();
}


