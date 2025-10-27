using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpGateway.Web.Extensions;

namespace SFA.DAS.RoatpGateway.Web.StartupExtensions
{
    public static class DependencyInjection
    {
        public static void ConfigureDependencyInjection(IServiceCollection services)
        {
            ClaimsIdentityExtensions.Logger = services.BuildServiceProvider().GetService<ILogger<ClaimsPrincipal>>();
        }
    }
}
