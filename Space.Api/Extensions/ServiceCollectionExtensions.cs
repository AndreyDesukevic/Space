using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Space.Application.Interfaces;
using Space.Application.Jobs;
using Space.Application.Mapping;
using Space.Application.Services;
using Space.Infrastructure.Application;
using Space.Infrastructure.Database;
using Space.Infrastructure.Domain.Entities;
using Space.Infrastructure.HttpClients;
using Space.Infrastructure.Options;

namespace Space.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppDb(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");

        services.AddDbContext<SpaceDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.UseLazyLoadingProxies();
        });

        return services;
    }

    public static IServiceCollection AddAppSerilog(this IServiceCollection services, IConfiguration config)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

        return services;
    }

    public static IServiceCollection AddAppHangfire(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("DefaultConnection");

        services.AddHangfire((provider, cfg) =>
        {
            cfg.UsePostgreSqlStorage(o => o.UseNpgsqlConnection(cs),
                new PostgreSqlStorageOptions
                {
                    SchemaName = "hangfire",
                    PrepareSchemaIfNecessary = true
                });
        });

        services.AddHangfireServer();

        return services;
    }

    public static IServiceCollection AddAppHttpClients(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<MeteorDataOptions>(config.GetSection("MeteorData"));

        services.AddHttpClient("MeteoriteClient", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(r => (int)r.StatusCode == 429)
            .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(Math.Pow(2, retry)));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }

    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IMeteoriteSyncClient, MeteoriteSyncClient>();
        services.AddScoped<IMeteoriteService, MeteoriteService>();
        services.AddScoped<IMeteoriteSyncJob, MeteoriteSyncJob>();
        services.AddScoped<IRecClassService, RecClassService>();

        services.AddScoped<IMeteoriteRepository, MeteoriteRepository>();
        services.AddScoped<IBaseRepository<Geolocation>, BaseRepository<Geolocation>>();
        services.AddScoped<IBaseRepository<NameType>, BaseRepository<NameType>>();
        services.AddScoped<IBaseRepository<RecClass>, BaseRepository<RecClass>>();
        services.AddScoped<IBaseRepository<GeolocationType>, BaseRepository<GeolocationType>>();

        return services;
    }

    public static IServiceCollection AddAppAutomapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(MeteoriteProfile), typeof(RecClassProfile));
        return services;
    }

    public static IServiceCollection AddAppCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173", "http://104.248.32.16")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddAppSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "SpaceSync API", Version = "v1" });
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }
}
