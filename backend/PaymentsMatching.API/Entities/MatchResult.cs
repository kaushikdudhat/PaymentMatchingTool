namespace PaymentsMatching.API.Entities;

public class MatchResult
{
    public int Id { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal? SystemAmount { get; set; }
    public decimal? ProviderAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
    public string? ResolutionSide { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
