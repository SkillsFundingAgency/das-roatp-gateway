using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Validators;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers
{
    public abstract class RoatpGatewayControllerTestBase<T>
    {
        protected Mock<IRoatpApplicationApiClient> ApplyApiClient;
        protected Mock<IRoatpGatewayPageValidator> GatewayValidator;
        protected Mock<ILogger<T>> Logger;
        protected HttpContext Context;

        protected string UserId => "user id";
        protected string Username => "user name";
        protected string GivenName => "user";
        protected string Surname => "name";


        protected void CoreSetup()
        {
            ApplyApiClient = new Mock<IRoatpApplicationApiClient>();
            GatewayValidator = new Mock<IRoatpGatewayPageValidator>();
            Logger = new Mock<ILogger<T>>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", UserId),
                new Claim(ClaimTypes.GivenName, GivenName),
                new Claim(ClaimTypes.Surname, Surname)
            }));
            
            GatewayValidator.Setup(v => v.Validate(It.IsAny<SubmitGatewayPageAnswerCommand>()))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>()
                }
                );

            Context = new DefaultHttpContext { User = user };


        }
    }
}
