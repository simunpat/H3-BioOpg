namespace BiografWeb.Domain;

public class Auditorium
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Rows { get; set; }
    public int Cols { get; set; }
}


