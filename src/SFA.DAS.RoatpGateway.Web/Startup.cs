using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Primitives;
using Polly;
using Polly.Extensions.Http;
using Refit;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Api.Common.Interfaces;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.ModelBinders;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Settings;
using SFA.DAS.RoatpGateway.Web.StartupExtensions;
using SFA.DAS.RoatpGateway.Web.Validators;

namespace SFA.DAS.RoatpGateway.Web;

[ExcludeFromCodeCoverage]
public class Startup
{
    private const string Culture = "en-GB";

    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public IWebConfiguration ApplicationConfiguration { get; set; }

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        _env = env;

        var config = new ConfigurationBuilder()
            .AddConfiguration(configuration);

        config.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = configuration["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            }
        );

        _configuration = config.Build();
        ApplicationConfiguration = _configuration.Get<WebConfiguration>();
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {

        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => false; // Default is true, make it false
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddAndConfigureDfESignInAuthentication(_configuration,
            "SFA.DAS.AdminService.Web.Auth",
            typeof(CustomServiceRole),
            ClientName.RoatpServiceAdmin,
            "/SignOut",
            "");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(Culture);
            options.SupportedCultures = new List<CultureInfo> { new CultureInfo(Culture) };
            options.RequestCultureProviders.Clear();
        });

        services.AddMvc(options =>
        {
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            options.ModelBinderProviders.Insert(0, new StringTrimmingModelBinderProvider());
        });

        services.AddSession(opt => { opt.IdleTimeout = TimeSpan.FromHours(1); });

        services.AddCache(ApplicationConfiguration, _env);
        services.AddDataProtection(ApplicationConfiguration, _env);

        services.AddAntiforgery(options => options.Cookie = new CookieBuilder() { Name = ".RoatpGateway.Staff.AntiForgery", HttpOnly = false });

        services.AddHealthChecks();

        services.AddLogging(builder =>
        {
            builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
            builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
        });

        services.AddApplicationInsightsTelemetry();

        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        ConfigureRoatpRegisterApiClient(services, _configuration);
        ConfigureApplyApiClients(services, _configuration);
        ConfigureDependencyInjection(services);

#if DEBUG
        services.AddControllersWithViews().AddControllersAsServices().AddRazorRuntimeCompilation();
#endif
    }

    private void ConfigureApplyApiClients(IServiceCollection services, IConfiguration configuration)
    {
        var acceptHeaderName = "Accept";
        var acceptHeaderValue = "application/json";
        var handlerLifeTime = TimeSpan.FromMinutes(5);

        services.AddHttpClient<IRoatpApplicationApiClient, RoatpApplicationApiClient>(config =>
        {
            config.BaseAddress = new Uri(ApplicationConfiguration.ApplyApiAuthentication.ApiBaseAddress);
            config.DefaultRequestHeaders.Add(acceptHeaderName, acceptHeaderValue);
        })
        .AddHttpMessageHandler(() => new InnerApiAuthenticationHeaderHandler(new AzureClientCredentialHelper(configuration), ApplicationConfiguration.ApplyApiAuthentication.Identifier))
        .SetHandlerLifetime(handlerLifeTime)
        .AddPolicyHandler(GetRetryPolicy());

        services.AddHttpClient<IRoatpOrganisationSummaryApiClient, RoatpOrganisationSummaryApiClient>(config =>
        {
            config.BaseAddress = new Uri(ApplicationConfiguration.ApplyApiAuthentication.ApiBaseAddress);
            config.DefaultRequestHeaders.Add(acceptHeaderName, acceptHeaderValue);
        })
        .AddHttpMessageHandler(() => new InnerApiAuthenticationHeaderHandler(new AzureClientCredentialHelper(configuration), ApplicationConfiguration.ApplyApiAuthentication.Identifier))
        .SetHandlerLifetime(handlerLifeTime)
        .AddPolicyHandler(GetRetryPolicy());

        services.AddHttpClient<IRoatpExperienceAndAccreditationApiClient, RoatpExperienceAndAccreditationApiClient>(config =>
        {
            config.BaseAddress = new Uri(ApplicationConfiguration.ApplyApiAuthentication.ApiBaseAddress);
            config.DefaultRequestHeaders.Add(acceptHeaderName, acceptHeaderValue);
        })
        .AddHttpMessageHandler(() => new InnerApiAuthenticationHeaderHandler(new AzureClientCredentialHelper(configuration), ApplicationConfiguration.ApplyApiAuthentication.Identifier))
       .SetHandlerLifetime(handlerLifeTime)
       .AddPolicyHandler(GetRetryPolicy());

        services.AddHttpClient<IRoatpGatewayCriminalComplianceChecksApiClient, RoatpGatewayCriminalComplianceChecksApiClient>(config =>
        {
            config.BaseAddress = new Uri(ApplicationConfiguration.ApplyApiAuthentication.ApiBaseAddress);
            config.DefaultRequestHeaders.Add(acceptHeaderName, acceptHeaderValue);
        })
        .AddHttpMessageHandler(() => new InnerApiAuthenticationHeaderHandler(new AzureClientCredentialHelper(configuration), ApplicationConfiguration.ApplyApiAuthentication.Identifier))
       .SetHandlerLifetime(handlerLifeTime)
       .AddPolicyHandler(GetRetryPolicy());
    }

    private void ConfigureRoatpRegisterApiClient(IServiceCollection services, IConfiguration configuration)
    {
        services.AddRefitClient<IRoatpRegisterApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(ApplicationConfiguration.RoatpRegisterApiAuthentication.ApiBaseAddress))
            .AddHttpMessageHandler(() => new InnerApiAuthenticationHeaderHandler(new AzureClientCredentialHelper(configuration), ApplicationConfiguration.RoatpRegisterApiAuthentication.Identifier));
    }

    private void ConfigureDependencyInjection(IServiceCollection services)
    {
        services.AddTransient(x => ApplicationConfiguration);

        services.AddTransient<IGatewayOverviewOrchestrator, GatewayOverviewOrchestrator>();
        services.AddTransient<IGatewayOrganisationChecksOrchestrator, GatewayOrganisationChecksOrchestrator>();
        services.AddTransient<IGatewaySectionsNotRequiredService, GatewaySectionsNotRequiredService>();
        services.AddTransient<IGatewayExperienceAndAccreditationOrchestrator, GatewayExperienceAndAccreditationOrchestrator>();
        services.AddTransient<IPeopleInControlOrchestrator, PeopleInControlOrchestrator>();
        services.AddTransient<IGatewayRegisterChecksOrchestrator, GatewayRegisterChecksOrchestrator>();
        services.AddTransient<IGatewayCriminalComplianceChecksOrchestrator, GatewayCriminalComplianceChecksOrchestrator>();
        services.AddTransient<IRoatpGatewayPageValidator, RoatpGatewayPageValidator>();
        services.AddTransient<IRoatpGatewayApplicationViewModelValidator, RoatpGatewayApplicationViewModelValidator>();
        services.AddTransient<IRoatpSearchTermValidator, RoatpSearchTermValidator>();
        services.AddTransient<IGatewayApplicationActionsOrchestrator, GatewayApplicationActionsOrchestrator>();
        services.AddTransient<IRoatpWithdrawApplicationViewModelValidator, RoatpWithdrawApplicationViewModelValidator>();
        services.AddTransient<IRoatpRemoveApplicationViewModelValidator, RoatpRemoveApplicationViewModelValidator>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseCookiePolicy();
        app.UseRouting();
        app.UseSession();
        app.UseRequestLocalization();
        app.UseStatusCodePagesWithReExecute("/ErrorPage/{0}");
        app.Use(async (context, next) =>
        {
            // security headers
            context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = "none";
            context.Response.Headers.ContentSecurityPolicy = "default-src 'self'; img-src 'self' *.azureedge.net *.google-analytics.com; script-src 'self' 'unsafe-inline' *.azureedge.net *.googletagmanager.com *.google-analytics.com *.googleapis.com; style-src-elem 'self' *.azureedge.net; style-src 'self' *.azureedge.net; font-src 'self' *.azureedge.net data:;";
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
            await next();
        });
        app.Use(async (context, next) =>
        {
            if (!context.Response.Headers.ContainsKey("X-Permitted-Cross-Domain-Policies"))
            {
                context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", new StringValues("none"));
            }
            await next();
        });
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHealthChecks("/health");
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                "default",
                "{controller=Home}/{action=Index}/{id?}");
        });
    }

    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                retryAttempt)));
    }
}

public class InnerApiAuthenticationHeaderHandler : DelegatingHandler
{
    private readonly IAzureClientCredentialHelper _azureClientCredentialHelper;
    private readonly string _apiIdentifier;

    public InnerApiAuthenticationHeaderHandler(IAzureClientCredentialHelper azureClientCredentialHelper, string apiIdentifier)
    {
        _azureClientCredentialHelper = azureClientCredentialHelper;
        _apiIdentifier = apiIdentifier;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("X-Version", "1.0");
        if (!string.IsNullOrEmpty(_apiIdentifier))
        {
            var accessToken = await _azureClientCredentialHelper.GetAccessTokenAsync(_apiIdentifier);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
    }
}
