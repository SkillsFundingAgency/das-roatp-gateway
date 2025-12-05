using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace SFA.DAS.RoatpGateway.Web;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static void Main(string[] args)
    {
        WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build().Run();
    }
}
