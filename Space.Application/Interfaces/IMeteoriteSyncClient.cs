using Space.Domain.Models;
using System.Runtime.CompilerServices;

namespace Space.Application.Interfaces;

public interface IMeteoriteSyncClient
{
    IAsyncEnumerable<MeteoriteDto> GetMeteoritesStreamAsync([EnumeratorCancellation] CancellationToken cancellationToken = default);
}
