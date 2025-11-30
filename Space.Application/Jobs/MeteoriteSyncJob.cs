using Hangfire;
using Microsoft.Extensions.Logging;
using Space.Application.Interfaces;

namespace Space.Application.Jobs;

public class MeteoriteSyncJob : IMeteoriteSyncJob
{
    private readonly IMeteoriteService _meteoriteService;
    private readonly ILogger<MeteoriteSyncJob> _logger;

    public MeteoriteSyncJob(IMeteoriteService meteoriteService, ILogger<MeteoriteSyncJob> logger)
    {
        _meteoriteService = meteoriteService;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task RunAsync()
    {
        _logger.LogInformation("Hangfire job started at {Time}", DateTime.UtcNow);

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        try
        {
            cts.CancelAfter(TimeSpan.FromHours(1));


            await _meteoriteService.SyncAsync(token);

            _logger.LogInformation("Hangfire job finished successfully at {Time}", DateTime.UtcNow);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Hangfire job was canceled at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hangfire job failed at {Time}", DateTime.UtcNow);
            throw;
        }
    }
}
