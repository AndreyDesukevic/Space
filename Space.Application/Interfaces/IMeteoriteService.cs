using Space.Domain.DTO;
using Space.Domain.Responses;

namespace Space.Application.Interfaces;

public interface IMeteoriteService
{
    Task SyncAsync(CancellationToken cancellationToken = default);
    Task<PaginatedResponse<MeteoriteSummaryDto>> GetSummaryAsync(MeteoriteQueryParams query, CancellationToken cancellationToken = default);
    Task<YearRangeDto> GetYearRangeAsync(CancellationToken cancellationToken = default);
}
