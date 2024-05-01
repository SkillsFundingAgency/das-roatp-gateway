using System;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.TokenService
{
    public interface ITokenService
    {
        Task<string> GetToken(Uri baseUri);
    }
}
