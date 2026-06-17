using PaymentsMatching.API.Entities;

namespace PaymentsMatching.API.Repositories;

public interface IMatchResultRepository
{
    Task<List<MatchResult>> GetAllAsync(string? filter = null);
    Task<MatchResult?> GetByIdAsync(int id);
    Task AddRangeAsync(IEnumerable<MatchResult> results);
    Task UpdateAsync(MatchResult result);
    Task DeleteAllAsync();
}
