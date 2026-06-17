namespace PaymentsMatching.API.Models;

public class CsvRecord
{
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
}
