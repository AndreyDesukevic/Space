namespace Space.Application.Interfaces;

public interface IMeteoriteSyncService
{
    Task SyncMeteoritesAsync(CancellationToken cancellationToken = default);
}
