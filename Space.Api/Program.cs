using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Space.Api.Auth;
using Space.Application.Interfaces;
using Space.Application.Jobs;
using Space.Application.Mapping;
using Space.Application.Services;
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
builder.Services.AddScoped<IMeteoriteService, MeteoriteService>();
builder.Services.AddScoped<IMeteoriteSyncJob, MeteoriteSyncJob>();
builder.Services.AddScoped<IRecClassService, RecClassService>();

builder.Services.AddCors(options =>
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
builder.Services.AddMemoryCache();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SpaceSync API", Version = "v1" });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.Configure<MeteorDataOptions>(builder.Configuration.GetSection("MeteorData"));


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = "swagger";
});

if (app.Environment.IsDevelopment())
{
    try
    {
        var swaggerUrl = "http://localhost:5000/swagger/index.html";
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

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new AllowAllDashboardAuthorization() }
});

RecurringJob.AddOrUpdate<IMeteoriteSyncJob>(
    "MeteoriteSyncJob_Daily",
    job => job.RunAsync(),
    Cron.Daily);

app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();

