namespace MCP.EasyVerein.Domain.Entities;

public class Invoice
{
    public long Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public long? MemberId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsPaid { get; set; }
}
