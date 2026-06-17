using Microsoft.AspNetCore.Mvc;
using PaymentsMatching.API.DTOs;
using PaymentsMatching.API.Services;

namespace PaymentsMatching.API.Controllers;

[ApiController]
[Route("api/matching")]
[Produces("application/json")]
public class MatchingController : ControllerBase
{
    private readonly IMatchingService _matchingService;
    private readonly ILogger<MatchingController> _logger;

    public MatchingController(IMatchingService matchingService, ILogger<MatchingController> logger)
    {
        _matchingService = matchingService;
        _logger          = logger;
    }

    /// <summary>Upload both CSV files and run the matching process.</summary>
    [HttpPost("run")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<MatchSummaryDto>> RunMatching(
        IFormFile systemFile,
        IFormFile providerFile)
    {
        if (systemFile == null || systemFile.Length == 0)
            return BadRequest(new { error = "systemFile is required." });

        if (providerFile == null || providerFile.Length == 0)
            return BadRequest(new { error = "providerFile is required." });

        _logger.LogInformation("Running matching: systemFile={S}, providerFile={P}",
            systemFile.FileName, providerFile.FileName);

        var summary = await _matchingService.RunMatchingAsync(systemFile, providerFile);
        return Ok(summary);
    }

    /// <summary>Retrieve all match results with optional filter (all/resolved/unresolved).</summary>
    [HttpGet("results")]
    public async Task<ActionResult<List<MatchResultDto>>> GetResults([FromQuery] string filter = "all")
    {
        var results = await _matchingService.GetResultsAsync(filter);
        return Ok(results);
    }

    /// <summary>Retrieve summary counts for current match results.</summary>
    [HttpGet("summary")]
    public async Task<ActionResult<MatchSummaryDto>> GetSummary()
    {
        var summary = await _matchingService.GetSummaryAsync();
        return Ok(summary);
    }

    /// <summary>Resolve a mismatch by selecting which side (SYSTEM or PROVIDER) is correct.</summary>
    [HttpPut("{id}/resolve")]
    public async Task<ActionResult<MatchResultDto>> Resolve(int id, [FromBody] ResolveRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.ResolutionSide))
            return BadRequest(new { error = "resolutionSide is required." });

        var result = await _matchingService.ResolveAsync(id, request.ResolutionSide);
        return Ok(result);
    }
}
