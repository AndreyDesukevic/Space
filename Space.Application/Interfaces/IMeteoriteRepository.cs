using Space.Domain.DTO;
using Space.Domain.Responses;
using Space.Infrastructure.Application;
using Space.Infrastructure.Domain.Entities;

namespace Space.Application.Interfaces;

public interface IMeteoriteRepository : IBaseRepository<Meteorite>
{
    Task<PaginatedResponse<MeteoriteSummaryDto>> GetSummaryAsync(MeteoriteQueryParams query, CancellationToken cancellationToken = default);
    Task<(int? MinYear, int? MaxYear)> GetYearRangeAsync(CancellationToken cancellationToken = default);
}
