namespace StressRunUi.Models;

public sealed record QueueRunRequest(int ScenarioId, DateOnly RunDate);

public sealed record QueueRunResponse(int RunId, string Status);

public sealed record RunStatusResponse(int RunId, string Status, int PercentComplete, string? ErrorMessage);

public sealed record PortfolioLossDto(int PortfolioId, decimal Loss);

public sealed record RunResultsResponse(int RunId, int ScenarioId, DateOnly RunDate, List<PortfolioLossDto> Results);

public sealed record ApiProblemDetails(string? Title, int? Status, Dictionary<string, string[]>? Errors, string? RunStatus);
