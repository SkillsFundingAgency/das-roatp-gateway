using System.Security.Claims;

namespace SFA.DAS.RoatpGateway.Web.Domain
{
    public static class Roles
    {
        public const string RoleClaimType = "http://service/service";

        public const string RoatpGatewayTeam = "GAC";

        public static bool HasValidRole(this ClaimsPrincipal user)
        {
            return user.IsInRole(RoatpGatewayTeam);
        }
    }
}
