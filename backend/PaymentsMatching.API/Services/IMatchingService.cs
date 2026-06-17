using PaymentsMatching.API.DTOs;

namespace PaymentsMatching.API.Services;

public interface IMatchingService
{
    Task<MatchSummaryDto> RunMatchingAsync(IFormFile systemFile, IFormFile providerFile);
    Task<List<MatchResultDto>> GetResultsAsync(string? filter);
    Task<MatchSummaryDto> GetSummaryAsync();
    Task<MatchResultDto> ResolveAsync(int id, string resolutionSide);
}
