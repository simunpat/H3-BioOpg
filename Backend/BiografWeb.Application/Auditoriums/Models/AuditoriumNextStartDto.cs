namespace BiografWeb.Application.Auditoriums.Models;

public sealed class AuditoriumNextStartDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? NextStartTime { get; set; }
}

