using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.RoatpGateway.Web.Settings;

namespace SFA.DAS.RoatpGateway.Web.StartupExtensions
{
    public static class CacheStartupExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services, IWebConfiguration configuration, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    var redisConnectionString = configuration.SessionRedisConnectionString;
                    var sessionCachingDatabase = configuration.SessionCachingDatabase;
                    options.Configuration = $"{redisConnectionString},{sessionCachingDatabase}";
                });
            }

            return services;
        }
    }
}
