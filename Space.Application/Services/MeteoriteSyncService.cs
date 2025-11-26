using AutoMapper;
using Microsoft.Extensions.Logging;
using Space.Application.Interfaces;
using Space.Domain.Models;
using Space.Infrastructure.Application;
using Space.Infrastructure.Domain.Entities;

namespace Space.Application.Services;

public class MeteoriteSyncService : IMeteoriteSyncService
{
    private readonly IMeteoriteSyncClient _syncClient;
    private readonly IBaseRepository<Meteorite> _meteoriteRepo;
    private readonly IBaseRepository<GeoLocation> _geoRepo;
    private readonly IBaseRepository<NameType> _nameTypeRepo;
    private readonly IBaseRepository<RecClass> _recClassRepo;
    private readonly IBaseRepository<GeoType> _geoTypeRepo;
    private readonly ILogger<MeteoriteSyncService> _logger;
    private readonly IMapper _mapper;

    public MeteoriteSyncService(
        IBaseRepository<Meteorite> meteoriteRepo,
        IBaseRepository<GeoLocation> geoRepo,
        IBaseRepository<NameType> nameTypeRepo,
        IBaseRepository<RecClass> recClassRepo,
        IBaseRepository<GeoType> geoTypeRepo,
        IMapper mapper)
    {
        _meteoriteRepo = meteoriteRepo;
        _geoRepo = geoRepo;
        _nameTypeRepo = nameTypeRepo;
        _recClassRepo = recClassRepo;
        _geoTypeRepo = geoTypeRepo;
        _mapper = mapper;
    }

    public Task SyncMeteoritesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
