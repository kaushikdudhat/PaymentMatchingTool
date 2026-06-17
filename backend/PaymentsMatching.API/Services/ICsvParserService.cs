using PaymentsMatching.API.Models;

namespace PaymentsMatching.API.Services;

public interface ICsvParserService
{
    Task<List<CsvRecord>> ParseAsync(IFormFile file);
}
