using StressRunUi.Models;

namespace StressRunUi.Services;

public interface IStressApiClient
{
    Task<QueueRunResponse> QueueRunAsync(QueueRunRequest request, CancellationToken ct);

    Task<RunStatusResponse> GetStatusAsync(int runId, CancellationToken ct);

    Task<RunResultsResponse> GetResultsAsync(int runId, CancellationToken ct);
}
