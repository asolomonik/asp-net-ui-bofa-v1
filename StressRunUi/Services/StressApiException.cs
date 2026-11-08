using StressRunUi.Models;

namespace StressRunUi.Services;

public sealed class StressApiException(int statusCode, ApiProblemDetails? problem)
    : Exception(problem?.Title ?? $"Stress API returned status {statusCode}.")
{
    public int StatusCode { get; } = statusCode;

    public ApiProblemDetails? Problem { get; } = problem;
}
