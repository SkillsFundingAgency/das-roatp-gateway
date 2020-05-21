using System;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.TokenService
{
    public interface ITokenService
    {
        string GetToken(Uri baseUri);
    }
}
