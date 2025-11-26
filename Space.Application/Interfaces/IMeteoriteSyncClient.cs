using Space.Domain.Models;

namespace Space.Application.Interfaces;

public interface IMeteoriteSyncClient
{
    Task<IEnumerable<MeteoriteDto>> GetMeteoritesAsync(CancellationToken cancellationToken = default);
}
