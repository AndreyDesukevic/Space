using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Space.Api.Auth;
using Space.Api.Extensions;
using Space.Api.Middleware;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.WebHost.ConfigureKestrel(o => o.ListenAnyIP(5000));

builder.Services.AddAppDb(builder.Configuration);
builder.Services.AddAppSerilog(builder.Configuration);
builder.Services.AddAppHangfire(builder.Configuration);
builder.Services.AddAppHttpClients(builder.Configuration);
builder.Services.AddAppServices();
builder.Services.AddAppAutomapper();
builder.Services.AddAppCors();
builder.Services.AddAppSwagger();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddFluentValidation();


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

app.UseCors("AllowFrontend");

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.AddRecurringJobs();

app.Run();

