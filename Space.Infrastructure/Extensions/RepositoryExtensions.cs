using Microsoft.Extensions.DependencyInjection;
using Space.Infrastructure.Application;
using Space.Infrastructure.Database;
using Space.Infrastructure.Domain.Entities;

namespace Space.Infrastructure.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBaseRepository<Meteorite>, BaseRepository<Meteorite>>();
        services.AddScoped<IBaseRepository<GeoLocation>, BaseRepository<GeoLocation>>();
        services.AddScoped<IBaseRepository<NameType>, BaseRepository<NameType>>();
        services.AddScoped<IBaseRepository<RecClass>, BaseRepository<RecClass>>();
        services.AddScoped<IBaseRepository<GeoType>, BaseRepository<GeoType>>();

        return services;
    }
}
