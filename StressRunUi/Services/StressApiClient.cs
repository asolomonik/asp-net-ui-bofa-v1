using System.Net.Http.Json;
using System.Text.Json;
using StressRunUi.Models;

namespace StressRunUi.Services;

public sealed class StressApiClient(HttpClient httpClient) : IStressApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    public async Task<QueueRunResponse> QueueRunAsync(QueueRunRequest request, CancellationToken ct)
    {
        using var response = await httpClient.PostAsJsonAsync("runs", request, JsonOptions, ct);
        await EnsureSuccessAsync(response, ct);
        return (await response.Content.ReadFromJsonAsync<QueueRunResponse>(JsonOptions, ct))!;
    }

    public async Task<RunStatusResponse> GetStatusAsync(int runId, CancellationToken ct)
    {
        using var response = await httpClient.GetAsync($"runs/{runId}/status", ct);
        await EnsureSuccessAsync(response, ct);
        return (await response.Content.ReadFromJsonAsync<RunStatusResponse>(JsonOptions, ct))!;
    }

    public async Task<RunResultsResponse> GetResultsAsync(int runId, CancellationToken ct)
    {
        using var response = await httpClient.GetAsync($"runs/{runId}/results", ct);
        await EnsureSuccessAsync(response, ct);
        return (await response.Content.ReadFromJsonAsync<RunResultsResponse>(JsonOptions, ct))!;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        ApiProblemDetails? problem = null;
        try
        {
            problem = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(JsonOptions, ct);
        }
        catch (JsonException)
        {
            // Body wasn't a problem-details payload; surface the status code alone.
        }

        throw new StressApiException((int)response.StatusCode, problem);
    }
}
