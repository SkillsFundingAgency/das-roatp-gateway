using SFA.DAS.RoatpGateway.Web.Domain;
using System.Collections.Generic;
using System.Security.Claims;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.MockedObjects
{
    public class MockedUser
    {
        private const string GivenName = "Test";
        private const string Surname = "User";
        private const string Email = "Test.User@example.com";

        public static ClaimsPrincipal Setup(params string[] roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName, GivenName),
                new Claim(ClaimTypes.Surname, Surname),
                new Claim(ClaimTypes.Name, $"{GivenName} {Surname}"),
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.Upn, Email)
            };

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    var rolesClaim = new Claim(Roles.RoleClaimType, role);
                    claims.Add(rolesClaim);
                }
            }

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock", ClaimTypes.Name, Roles.RoleClaimType));
            return user;
        }
    }
}
