using Microsoft.AspNetCore.Mvc;
using StressRunUi.Models;
using StressRunUi.Services;

namespace StressRunUi.Controllers;

[Route("runs")]
public sealed class RunsController(IStressApiClient api, ILogger<RunsController> logger) : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View(new QueueRunFormModel());

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(QueueRunFormModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var queued = await api.QueueRunAsync(new QueueRunRequest(model.ScenarioId!.Value, model.RunDate!.Value), ct);
            return RedirectToAction(nameof(Status), new { id = queued.RunId });
        }
        catch (StressApiException ex) when (ex.StatusCode == 400)
        {
            ApplyApiErrors(ex.Problem);
            return View(model);
        }
        catch (StressApiException ex)
        {
            logger.LogError(ex, "Queueing a run failed with status {StatusCode}", ex.StatusCode);
            ModelState.AddModelError(string.Empty, "Could not queue the run. Please try again.");
            return View(model);
        }
    }

    [HttpGet("{id:int}/status")]
    public IActionResult Status(int id) => View(id);

    [HttpGet("{id:int}/status/data")]
    public async Task<IActionResult> StatusData(int id, CancellationToken ct)
    {
        try
        {
            var status = await api.GetStatusAsync(id, ct);
            return Json(status);
        }
        catch (StressApiException ex) when (ex.StatusCode == 404)
        {
            return NotFound();
        }
    }

    [HttpGet("{id:int}/results")]
    public async Task<IActionResult> Results(int id, CancellationToken ct)
    {
        try
        {
            var results = await api.GetResultsAsync(id, ct);
            return View(results);
        }
        catch (StressApiException ex) when (ex.StatusCode == 404)
        {
            return NotFound();
        }
        catch (StressApiException ex) when (ex.StatusCode == 409)
        {
            return RedirectToAction(nameof(Status), new { id });
        }
    }

    private void ApplyApiErrors(ApiProblemDetails? problem)
    {
        if (problem?.Errors is { Count: > 0 })
        {
            foreach (var (key, messages) in problem.Errors)
            {
                foreach (var message in messages)
                {
                    ModelState.AddModelError(key, message);
                }
            }

            return;
        }

        ModelState.AddModelError(string.Empty, problem?.Title ?? "The request was invalid.");
    }
}
