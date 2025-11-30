using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Space.Application.Interfaces;
using Space.Domain.DTO;
using Space.Domain.Models;
using Space.Domain.Responses;
using Space.Infrastructure.Application;
using Space.Infrastructure.Domain.Entities;

namespace Space.Application.Services;

public class MeteoriteService : IMeteoriteService
{
    private readonly IMeteoriteSyncClient _syncClient;
    private readonly ILogger<MeteoriteService> _logger;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    private readonly IMeteoriteRepository _meteoriteRepository;
    private readonly IBaseRepository<Geolocation> _geolocationRepository;
    private readonly IBaseRepository<NameType> _nameTypeRepository;
    private readonly IBaseRepository<RecClass> _recClassRepository;
    private readonly IBaseRepository<GeolocationType> _geoTypeRepository;

    private const int BatchSize = 500;

    public MeteoriteService(
        IMeteoriteRepository meteoriteRepository,
        IBaseRepository<Geolocation> geolocationRepository,
        IBaseRepository<NameType> nameTypeRepository,
        IBaseRepository<RecClass> recClassRepository,
        IBaseRepository<GeolocationType> geoTypeRepository,
        IMeteoriteSyncClient syncClient,
        ILogger<MeteoriteService> logger,
        IMapper mapper,
        IMemoryCache cache)
    {
        _meteoriteRepository = meteoriteRepository;
        _geolocationRepository = geolocationRepository;
        _nameTypeRepository = nameTypeRepository;
        _recClassRepository = recClassRepository;
        _geoTypeRepository = geoTypeRepository;
        _logger = logger;
        _mapper = mapper;
        _syncClient = syncClient;
        _cache = cache;
    }

    public async Task<PaginatedResponse<MeteoriteSummaryDto>> GetSummaryAsync(MeteoriteQueryParams query, CancellationToken cancellationToken = default)
    {
        bool canCache = query.Page == 1
                        && string.IsNullOrEmpty(query.RecClass)
                        && string.IsNullOrEmpty(query.NameContains)
                        && !query.YearFrom.HasValue
                        && !query.YearTo.HasValue
                        && query.PageSize == 50
                        && string.Equals(query.SortField, "Year", StringComparison.OrdinalIgnoreCase)
                        && string.Equals(query.SortOrder, "Asc", StringComparison.OrdinalIgnoreCase);

        const string cacheKey = "MeteoriteSummary_FirstPage_DefaultFilters_Paginated";

        if (canCache && _cache.TryGetValue(cacheKey, out PaginatedResponse<MeteoriteSummaryDto> cached))
        {
            return cached;
        }

        var result = await _meteoriteRepository.GetSummaryAsync(query, cancellationToken);

        if (canCache)
        {
            _cache.Set(cacheKey, result, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) });
        }

        return result;
    }

    public async Task SyncAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Start full sync...");

        var batch = new List<MeteoriteDto>(BatchSize);
        int totalCount = 0;

        await foreach (var dto in _syncClient.GetMeteoritesStreamAsync(cancellationToken))
        {
            batch.Add(dto);

            if (batch.Count >= BatchSize)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ProcessBatchAsync(batch, cancellationToken);
                totalCount += batch.Count;
                _logger.LogInformation("Processed {Count} total records", totalCount);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            await ProcessBatchAsync(batch, cancellationToken);
            totalCount += batch.Count;
            _logger.LogInformation("Processed {Count} total records", totalCount);
        }

        _logger.LogInformation("Full sync finished. Total processed: {Count}", totalCount);
    }

    private async Task ProcessBatchAsync(List<MeteoriteDto> batch, CancellationToken cancellationToken)
    {
        var nameTypes = batch
            .Select(x => x.NameType)
            .Where(v => !string.IsNullOrEmpty(v))
            .Distinct()
            .Select(v => new NameType { Name = v })
            .ToList();

        var recClasses = batch
            .Select(x => x.RecClass)
            .Where(v => !string.IsNullOrEmpty(v))
            .Distinct()
            .Select(v => new RecClass { Name = v })
            .ToList();

        var geoTypes = batch
            .Select(x => x.Geolocation?.Type)
            .Where(v => !string.IsNullOrEmpty(v))
            .Distinct()
            .Select(v => new GeolocationType { Name = v })
            .ToList();

        await _nameTypeRepository.BulkInsertOrUpdateAsync(nameTypes, nameof(NameType.Name), cancellationToken);
        await _recClassRepository.BulkInsertOrUpdateAsync(recClasses, nameof(RecClass.Name), cancellationToken);
        await _geoTypeRepository.BulkInsertOrUpdateAsync(geoTypes, nameof(GeolocationType.Name), cancellationToken);

        var existingNameTypes = (await _nameTypeRepository.GetAllAsync(cancellationToken, asNoTracking: true)).ToDictionary(x => x.Name, x => x);
        var existingRecClasses = (await _recClassRepository.GetAllAsync(cancellationToken, asNoTracking: true)).ToDictionary(x => x.Name, x => x);
        var existingGeoTypes = (await _geoTypeRepository.GetAllAsync(cancellationToken, asNoTracking: true)).ToDictionary(x => x.Name, x => x);

        var meteoriteEntities = new List<Meteorite>(batch.Count);
        var geolocationEntities = new List<Geolocation>(batch.Count);

        foreach (var dtoMeteorite in batch)
        {
            if (!int.TryParse(dtoMeteorite.Id, out var parsedId))
            {
                _logger.LogWarning("Invalid id {Id} for {Name}", dtoMeteorite.Id, dtoMeteorite.Name);
                continue;
            }

            var meteorite = _mapper.Map<Meteorite>(dtoMeteorite);

            meteorite.Id = parsedId;

            if (!string.IsNullOrEmpty(dtoMeteorite.NameType) && existingNameTypes.TryGetValue(dtoMeteorite.NameType, out var nameType)) meteorite.NameTypeId = nameType.Id;
            if (!string.IsNullOrEmpty(dtoMeteorite.RecClass) && existingRecClasses.TryGetValue(dtoMeteorite.RecClass, out var recClass)) meteorite.RecClassId = recClass.Id;


            var geoDto = dtoMeteorite.Geolocation;
            if (geoDto != null && geoDto.Coordinates?.Length == 2)
            {
                var geoTypeName = geoDto.Type;
                existingGeoTypes.TryGetValue(geoTypeName, out var gt);

                geolocationEntities.Add(new Geolocation
                {
                    MeteoriteId = parsedId,
                    GeolocationTypeId = gt?.Id,
                    Longitude = geoDto.Coordinates[0],
                    Latitude = geoDto.Coordinates[1]
                });
            }

            meteoriteEntities.Add(meteorite);
        }

        if (!meteoriteEntities.Any()) return;

        await _meteoriteRepository.BulkInsertOrUpdateAsync(meteoriteEntities, nameof(Meteorite.Id), cancellationToken);
        await _geolocationRepository.BulkInsertOrUpdateAsync(geolocationEntities, nameof(Geolocation.MeteoriteId), cancellationToken);
    }

    public async Task<YearRangeDto> GetYearRangeAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "Meteorite_YearRange";
        if (_cache != null && _cache.TryGetValue(cacheKey, out YearRangeDto cached))
        {
            return cached;
        }

        var (minYear, maxYear) = await _meteoriteRepository.GetYearRangeAsync(cancellationToken);

        var now = DateTime.Now.Year;
        var result = new YearRangeDto
        {
            MinYear = minYear ?? (now - 100),
            MaxYear = maxYear ?? now
        };

        if (_cache != null)
        {
            _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });
        }

        return result;
    }
}
