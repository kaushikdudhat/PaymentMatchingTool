using PaymentsMatching.API.Models;

namespace PaymentsMatching.API.Services;

public class CsvParserService : ICsvParserService
{
    private static readonly HashSet<string> ValidCurrencies = new(StringComparer.OrdinalIgnoreCase)
    {
        "INR", "USD", "EUR", "GBP"
    };

    private readonly ILogger<CsvParserService> _logger;

    public CsvParserService(ILogger<CsvParserService> logger)
    {
        _logger = logger;
    }

    public async Task<List<CsvRecord>> ParseAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("File is empty.");

        var records = new List<CsvRecord>();

        using var reader = new StreamReader(file.OpenReadStream());

        var headerLine = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(headerLine))
            throw new InvalidOperationException($"'{file.FileName}' has no header row.");

        var headers = headerLine.Split(',').Select(h => h.Trim().ToLower()).ToArray();

        int orderIdIdx  = Array.IndexOf(headers, "orderid");
        int amountIdx   = Array.IndexOf(headers, "amount");
        int currencyIdx = Array.IndexOf(headers, "currency");

        if (orderIdIdx == -1 || amountIdx == -1 || currencyIdx == -1)
            throw new InvalidOperationException(
                $"'{file.FileName}' must contain columns: orderId, amount, currency.");

        int lineNumber = 1;
        string? line;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');
            if (parts.Length <= Math.Max(orderIdIdx, Math.Max(amountIdx, currencyIdx)))
                throw new InvalidOperationException(
                    $"'{file.FileName}' line {lineNumber}: insufficient columns.");

            var orderId  = parts[orderIdIdx].Trim();
            var currency = parts[currencyIdx].Trim().ToUpper();

            if (string.IsNullOrEmpty(orderId))
                throw new InvalidOperationException(
                    $"'{file.FileName}' line {lineNumber}: orderId is required.");

            if (!decimal.TryParse(parts[amountIdx].Trim(), out var amount))
                throw new InvalidOperationException(
                    $"'{file.FileName}' line {lineNumber}: amount '{parts[amountIdx].Trim()}' is not numeric.");

            if (!ValidCurrencies.Contains(currency))
                throw new InvalidOperationException(
                    $"'{file.FileName}' line {lineNumber}: currency '{currency}' is invalid. Valid: INR, USD, EUR, GBP.");

            records.Add(new CsvRecord
            {
                OrderId  = orderId,
                Amount   = amount,
                Currency = currency
            });
        }

        _logger.LogInformation("Parsed {Count} records from '{FileName}'.", records.Count, file.FileName);
        return records;
    }
}
