namespace PaymentsMatching.API.DTOs;

public class MatchSummaryDto
{
    public int Total { get; set; }
    public int Matched { get; set; }
    public int OnlySystem { get; set; }
    public int OnlyProvider { get; set; }
    public int AmountMismatch { get; set; }
}
