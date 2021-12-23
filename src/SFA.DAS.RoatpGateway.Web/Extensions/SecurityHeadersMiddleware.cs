using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace SFA.DAS.RoatpGateway.Web.Extensions
{
    public sealed class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", new StringValues("none"));
            return _next(context);
        }
    }
}
