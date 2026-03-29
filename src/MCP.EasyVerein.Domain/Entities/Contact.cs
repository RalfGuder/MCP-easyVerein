namespace MCP.EasyVerein.Domain.Entities;

public class Contact
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Company { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
