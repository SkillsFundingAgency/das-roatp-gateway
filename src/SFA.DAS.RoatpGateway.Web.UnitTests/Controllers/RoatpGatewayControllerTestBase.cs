using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Infrastructure.Validation;
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
        public string UserId { get; protected set; }
        public string Username { get; protected set; }

        protected void CoreSetup()
        {
            ApplyApiClient = new Mock<IRoatpApplicationApiClient>();
            GatewayValidator = new Mock<IRoatpGatewayPageValidator>();
            Logger = new Mock<ILogger<T>>();

            GatewayValidator.Setup(v => v.Validate(It.IsAny<SubmitGatewayPageAnswerCommand>()))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>()
                }
                );
        }
    }
}
