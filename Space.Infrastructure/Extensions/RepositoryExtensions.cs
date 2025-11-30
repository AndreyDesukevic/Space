using Microsoft.Extensions.DependencyInjection;
using Space.Application.Interfaces;
using Space.Infrastructure.Application;
using Space.Infrastructure.Database;
using Space.Infrastructure.Domain.Entities;

namespace Space.Infrastructure.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IMeteoriteRepository, MeteoriteRepository>();
        services.AddScoped<IBaseRepository<Geolocation>, BaseRepository<Geolocation>>();
        services.AddScoped<IBaseRepository<NameType>, BaseRepository<NameType>>();
        services.AddScoped<IBaseRepository<RecClass>, BaseRepository<RecClass>>();
        services.AddScoped<IBaseRepository<GeolocationType>, BaseRepository<GeolocationType>>();

        return services;
    }
}
