using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace SFA.DAS.RoatpGateway.Web.Extensions
{
    public static class SecurityHeadersExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            app.Use((Func<HttpContext, Func<Task>, Task>)(async (context, next) =>
            {
                context.Response.Headers["X-Frame-Options"] = (StringValues)"SAMEORIGIN";
                context.Response.Headers["X-XSS-Protection"] = (StringValues)"1; mode=block";
                context.Response.Headers["X-Content-Type-Options"] = (StringValues)"nosniff";
                context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = (StringValues)"none";
                context.Response.Headers["Content-Security-Policy"] = (StringValues)"default-src 'self'; img-src 'self' *.azureedge.net *.google-analytics.com; script-src 'self' 'unsafe-inline' *.azureedge.net *.googletagmanager.com *.google-analytics.com *.googleapis.com; style-src-elem 'self' *.azureedge.net; style-src 'self' *.azureedge.net; font-src 'self' *.azureedge.net data:;";
                context.Response.Headers["Referrer-Policy"] = (StringValues)"strict-origin-when-cross-origin";
                context.Response.Headers["Cache-Control"] = (StringValues)"no-cache, no-store, must-revalidate";
                context.Response.Headers["Pragma"] = (StringValues)"no-cache";
                await next();
            }));
            return app;
        }
    }
}
