﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.RoatpGateway.Web.Settings;
using StackExchange.Redis;


namespace SFA.DAS.RoatpGateway.Web.StartupExtensions
{
    public static class DataProtectionStartupExtensions
    {
        public static IServiceCollection AddDataProtection(this IServiceCollection services, IWebConfiguration configuration, IHostEnvironment environment)
        {
            if (!environment.IsDevelopment())
            {
                var redisConnectionString = configuration.SessionRedisConnectionString;
                var dataProtectionKeysDatabase = configuration.DataProtectionKeysDatabase;

                var redis = ConnectionMultiplexer.Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

                services.AddDataProtection()
                    .SetApplicationName("das-admin-service-web")
                    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
            }
            return services;
        }
    }
}
