using Microsoft.EntityFrameworkCore;
using PaymentsMatching.API.Data;
using PaymentsMatching.API.Entities;

namespace PaymentsMatching.API.Repositories;

public class MatchResultRepository : IMatchResultRepository
{
    private readonly AppDbContext _context;

    public MatchResultRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MatchResult>> GetAllAsync(string? filter = null)
    {
        var query = _context.MatchResults.AsQueryable();

        query = filter?.ToLower() switch
        {
            "resolved"   => query.Where(r => r.IsResolved),
            "unresolved" => query.Where(r => !r.IsResolved),
            _            => query
        };

        return await query.OrderByDescending(r => r.CreatedDate).ToListAsync();
    }

    public async Task<MatchResult?> GetByIdAsync(int id)
    {
        return await _context.MatchResults.FindAsync(id);
    }

    public async Task AddRangeAsync(IEnumerable<MatchResult> results)
    {
        await _context.MatchResults.AddRangeAsync(results);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MatchResult result)
    {
        _context.MatchResults.Update(result);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllAsync()
    {
        await _context.MatchResults.ExecuteDeleteAsync();
    }
}
