using PaymentsMatching.API.DTOs;
using PaymentsMatching.API.Entities;
using PaymentsMatching.API.Models;
using PaymentsMatching.API.Repositories;

namespace PaymentsMatching.API.Services;

public class MatchingService : IMatchingService
{
    private readonly IMatchResultRepository _repository;
    private readonly ICsvParserService _csvParser;
    private readonly ILogger<MatchingService> _logger;

    public MatchingService(
        IMatchResultRepository repository,
        ICsvParserService csvParser,
        ILogger<MatchingService> logger)
    {
        _repository = repository;
        _csvParser  = csvParser;
        _logger     = logger;
    }

    public async Task<MatchSummaryDto> RunMatchingAsync(IFormFile systemFile, IFormFile providerFile)
    {
        var systemRecords   = await _csvParser.ParseAsync(systemFile);
        var providerRecords = await _csvParser.ParseAsync(providerFile);

        await _repository.DeleteAllAsync();

        var systemDict   = systemRecords.ToDictionary(r => BuildKey(r.OrderId, r.Currency));
        var providerDict = providerRecords.ToDictionary(r => BuildKey(r.OrderId, r.Currency));

        var results = new List<MatchResult>();
        var now     = DateTime.UtcNow;

        foreach (var (key, sys) in systemDict)
        {
            if (providerDict.TryGetValue(key, out var prov))
            {
                var status = sys.Amount == prov.Amount
                    ? MatchStatus.MATCHED
                    : MatchStatus.AMOUNTMISMATCH;

                results.Add(CreateResult(sys.OrderId, sys.Currency, sys.Amount, prov.Amount, status, now));
            }
            else
            {
                results.Add(CreateResult(sys.OrderId, sys.Currency, sys.Amount, null, MatchStatus.ONLYSYSTEM, now));
            }
        }

        foreach (var (key, prov) in providerDict)
        {
            if (!systemDict.ContainsKey(key))
            {
                results.Add(CreateResult(prov.OrderId, prov.Currency, null, prov.Amount, MatchStatus.ONLYPROVIDER, now));
            }
        }

        await _repository.AddRangeAsync(results);

        _logger.LogInformation(
            "Matching complete. Total={Total}, Matched={Matched}, Mismatch={Mismatch}, OnlySystem={OnlySystem}, OnlyProvider={OnlyProvider}.",
            results.Count,
            results.Count(r => r.Status == MatchStatus.MATCHED.ToString()),
            results.Count(r => r.Status == MatchStatus.AMOUNTMISMATCH.ToString()),
            results.Count(r => r.Status == MatchStatus.ONLYSYSTEM.ToString()),
            results.Count(r => r.Status == MatchStatus.ONLYPROVIDER.ToString()));

        return BuildSummary(results);
    }

    public async Task<List<MatchResultDto>> GetResultsAsync(string? filter)
    {
        var results = await _repository.GetAllAsync(filter);
        return results.Select(MapToDto).ToList();
    }

    public async Task<MatchSummaryDto> GetSummaryAsync()
    {
        var results = await _repository.GetAllAsync();
        return BuildSummary(results);
    }

    public async Task<MatchResultDto> ResolveAsync(int id, string resolutionSide)
    {
        var upper = resolutionSide.ToUpper();
        if (upper != "SYSTEM" && upper != "PROVIDER")
            throw new ArgumentException("resolutionSide must be SYSTEM or PROVIDER.");

        var result = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Record with id {id} not found.");

        result.IsResolved     = true;
        result.ResolutionSide = upper;
        result.UpdatedDate    = DateTime.UtcNow;

        await _repository.UpdateAsync(result);
        return MapToDto(result);
    }

    private static string BuildKey(string orderId, string currency) =>
        $"{orderId}|{currency}";

    private static MatchResult CreateResult(
        string orderId, string currency,
        decimal? systemAmount, decimal? providerAmount,
        MatchStatus status, DateTime now) => new()
    {
        OrderId        = orderId,
        Currency       = currency,
        SystemAmount   = systemAmount,
        ProviderAmount = providerAmount,
        Status         = status.ToString(),
        IsResolved     = false,
        CreatedDate    = now
    };

    private static MatchSummaryDto BuildSummary(IEnumerable<MatchResult> results)
    {
        var list = results.ToList();
        return new MatchSummaryDto
        {
            Total         = list.Count,
            Matched       = list.Count(r => r.Status == MatchStatus.MATCHED.ToString()),
            OnlySystem    = list.Count(r => r.Status == MatchStatus.ONLYSYSTEM.ToString()),
            OnlyProvider  = list.Count(r => r.Status == MatchStatus.ONLYPROVIDER.ToString()),
            AmountMismatch = list.Count(r => r.Status == MatchStatus.AMOUNTMISMATCH.ToString())
        };
    }

    private static MatchResultDto MapToDto(MatchResult r) => new()
    {
        Id             = r.Id,
        OrderId        = r.OrderId,
        Currency       = r.Currency,
        SystemAmount   = r.SystemAmount,
        ProviderAmount = r.ProviderAmount,
        Status         = r.Status,
        IsResolved     = r.IsResolved,
        ResolutionSide = r.ResolutionSide,
        CreatedDate    = r.CreatedDate,
        UpdatedDate    = r.UpdatedDate
    };
}
