using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.RoatpGateway.Web.Settings;
using StackExchange.Redis;


namespace SFA.DAS.RoatpGateway.Web.StartupExtensions
{
    public static class DataProtectionStartupExtensions
    {
        public static IServiceCollection AddDataProtection(this IServiceCollection services, IWebConfiguration configuration, IHostingEnvironment environment)
        {
            if (!environment.IsDevelopment())
            {
                var redisConnectionString = configuration.SessionRedisConnectionString;
                var dataProtectionKeysDatabase = configuration.DataProtectionKeysDatabase;

                var redis = ConnectionMultiplexer.Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

                services.AddDataProtection()
                    .SetApplicationName("das-roatp-gateway")
                    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
            }
            return services;
        }
    }
}
