using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Space.Application.Interfaces;
using Space.Application.Mapping;
using Space.Infrastructure.Database;
using Space.Infrastructure.Extensions;
using Space.Infrastructure.HttpClients;
using Space.Infrastructure.Options;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
    options.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps());
});


builder.Services.AddDbContext<SpaceDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.UseLazyLoadingProxies();
});

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddHangfire((serviceProvider, configuration) =>
{
    configuration.UsePostgreSqlStorage(
        options =>
        {
            options.UseNpgsqlConnection(connectionString);
        },
        new PostgreSqlStorageOptions
        {
            SchemaName = "hangfire",
            PrepareSchemaIfNecessary = true,
            QueuePollInterval = TimeSpan.FromSeconds(5)
        });
});

builder.Services.AddHttpClient("MeteoriteClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddScoped<IMeteoriteSyncClient, MeteoriteSyncClient>();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => (int)msg.StatusCode == 429)
        .WaitAndRetryAsync(
            3,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
        );
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}

builder.Services.AddAutoMapper(cfg => { }, typeof(MeteoriteProfile));
builder.Services.AddRepositories();
builder.Services.AddHangfireServer();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SpaceSync API", Version = "v1" });
});

builder.Services.Configure<MeteorDataOptions>(builder.Configuration.GetSection("MeteorData"));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });

    try
    {
        var swaggerUrl = "https://localhost:5001/swagger/index.html";
        if (OperatingSystem.IsWindows())
        {
            Process.Start(new ProcessStartInfo { FileName = swaggerUrl, UseShellExecute = true });
        }
        else if (OperatingSystem.IsLinux())
        {
            Process.Start("xdg-open", swaggerUrl);
        }
        else if (OperatingSystem.IsMacOS())
        {
            Process.Start("open", swaggerUrl);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Не удалось открыть Swagger автоматически: {ex.Message}");
    }
}


app.UseHangfireDashboard("/hangfire");

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

