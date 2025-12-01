using Microsoft.EntityFrameworkCore;
using Space.Application.Interfaces;
using Space.Domain.DTO;
using Space.Domain.Responses;
using Space.Infrastructure.Domain.Entities;

namespace Space.Infrastructure.Database;

public class MeteoriteRepository : BaseRepository<Meteorite>, IMeteoriteRepository
{
    public MeteoriteRepository(SpaceDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<PaginatedResponse<MeteoriteSummaryDto>> GetSummaryAsync(MeteoriteQueryParams query, CancellationToken cancellationToken = default)
    {
        var baseQuery = _dbSet.AsNoTracking().AsQueryable();

        if (query.YearFrom.HasValue)
            baseQuery = baseQuery.Where(m => m.Year >= query.YearFrom.Value);

        if (query.YearTo.HasValue)
            baseQuery = baseQuery.Where(m => m.Year <= query.YearTo.Value);

        if (!string.IsNullOrEmpty(query.RecClass))
            baseQuery = baseQuery.Where(m => m.RecClass.Name == query.RecClass);

        if (!string.IsNullOrEmpty(query.NameContains))
            baseQuery = baseQuery.Where(m => EF.Functions.Like(m.Name, $"%{query.NameContains}%"));

        var totalGroups = await baseQuery
            .Select(m => m.Year)
            .Distinct()
            .CountAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalGroups / Math.Max(1, query.PageSize));

        var groupedQuery = baseQuery
            .GroupBy(m => m.Year)
            .Select(g => new MeteoriteSummaryDto
            {
                Year = g.Key,
                Count = g.Count(),
                TotalMass = g.Sum(x => x.Mass ?? 0)
            });

        bool asc = string.Equals(query.SortOrder, "Asc", StringComparison.OrdinalIgnoreCase);

        groupedQuery = query.SortField switch
        {
            "count" => asc ? groupedQuery.OrderBy(g => g.Count) : groupedQuery.OrderByDescending(g => g.Count),
            "totalMass" => asc ? groupedQuery.OrderBy(g => g.TotalMass) : groupedQuery.OrderByDescending(g => g.TotalMass),
            _ => asc ? groupedQuery.OrderBy(g => g.Year) : groupedQuery.OrderByDescending(g => g.Year),
        };

        var items = await groupedQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<MeteoriteSummaryDto>
        {
            Items = items,
            TotalPages = totalPages,
            CurrentPage = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalGroups
        };
    }

    public async Task<(int? MinYear, int? MaxYear)> GetYearRangeAsync(CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Where(m => m.Year.HasValue);

        var minYear = await query.MinAsync(m => (int?)m.Year, cancellationToken);
        var maxYear = await query.MaxAsync(m => (int?)m.Year, cancellationToken);

        return (minYear, maxYear);
    }
}
