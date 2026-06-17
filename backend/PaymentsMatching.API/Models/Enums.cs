namespace PaymentsMatching.API.Models;

public enum MatchStatus
{
    MATCHED,
    ONLYSYSTEM,
    ONLYPROVIDER,
    AMOUNTMISMATCH
}

public enum ResolutionSide
{
    NONE,
    SYSTEM,
    PROVIDER
}
