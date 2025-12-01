using Hangfire;
using Space.Application.Interfaces;

namespace Space.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder AddRecurringJobs(this IApplicationBuilder app)
    {
        RecurringJob.AddOrUpdate<IMeteoriteSyncJob>(
            "MeteoriteSyncJob_Daily",
            job => job.RunAsync(),
            Cron.Daily);

        return app;
    }
}
