namespace BiografWeb.Domain;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer";
    public string PasswordHash { get; set; } = string.Empty;
    public string? PasswordSalt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


