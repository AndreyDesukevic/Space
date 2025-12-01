using Hangfire.Dashboard;

namespace Space.Api.Auth;

public class AllowAllDashboardAuthorization : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}
