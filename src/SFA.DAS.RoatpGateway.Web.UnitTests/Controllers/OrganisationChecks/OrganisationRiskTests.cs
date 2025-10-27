using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Controllers;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Validators;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.RoatpGateway.Web.Validators.Validation;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers.OrganisationChecks
{
    [TestFixture]
    public class OrganisationRiskTests
    {
        private RoatpGatewayOrganisationChecksController _controller;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpGatewayPageValidator> _gatewayValidator;
        private Mock<IGatewayOrganisationChecksOrchestrator> _orchestrator;
        private Mock<ILogger<RoatpGatewayOrganisationChecksController>> _logger;
        private static string ClarificationAnswer => "Clarification answer";

        private string userId = "user123";
        private string username = "john smith";
        private string givenName = "john";
        private string surname = "smith";
        private string comment = "test comment";
        private string viewname = "~/Views/Gateway/pages/OrganisationRisk.cshtml";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _gatewayValidator = new Mock<IRoatpGatewayPageValidator>();
            _logger = new Mock<ILogger<RoatpGatewayOrganisationChecksController>>();
            _orchestrator = new Mock<IGatewayOrganisationChecksOrchestrator>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
             {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", userId),
                new Claim(ClaimTypes.GivenName, givenName),
                new Claim(ClaimTypes.Surname, surname)
             }));

            var context = new DefaultHttpContext { User = user };
            _gatewayValidator.Setup(v => v.Validate(It.IsAny<SubmitGatewayPageAnswerCommand>()))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>()
                }
                );

            _controller = new RoatpGatewayOrganisationChecksController(_applyApiClient.Object, _gatewayValidator.Object, _orchestrator.Object, _logger.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
        }

        [Test]
        public void check_organisation_risk_request_is_sent()
        {
            var applicationId = Guid.NewGuid();

            var vm = new OrganisationRiskViewModel
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            _orchestrator.Setup(x => x.GetOrganisationRiskViewModel(It.IsAny<GetOrganisationRiskRequest>()))
                .ReturnsAsync(vm);

            var result = _controller.GetOrganisationRiskPage(applicationId).Result;
            var viewResult = result as ViewResult;
            Assert.That(viewResult.ViewName, Is.EqualTo(viewname));
        }

        [Test]
        public async Task post_organisation_risk_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.OrganisationRisk;

            var vm = new OrganisationRiskViewModel
            {
                ApplicationId = applicationId,
                PageId = pageId,
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                OptionPassText = "Some pass text"
            };
            var command = new SubmitGatewayPageAnswerCommand(vm);

            _gatewayValidator.Setup(v => v.Validate(command)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            await _controller.EvaluateOrganisationRiskPage(command);

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, userId, username, vm.OptionPassText, null));
        }


        [Test]
        public async Task post_organisation_risk_clarification_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.OrganisationRisk;

            var vm = new OrganisationRiskViewModel
            {
                ApplicationId = applicationId,
                PageId = pageId,
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                OptionPassText = "Some pass text",
                ClarificationAnswer = ClarificationAnswer
            };
            var command = new SubmitGatewayPageAnswerCommand(vm);

            _gatewayValidator.Setup(v => v.ValidateClarification(command)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            await _controller.ClarifyOrganisationRiskPage(command);

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswerPostClarification(applicationId, pageId, vm.Status, userId, username, vm.OptionPassText, ClarificationAnswer));
        }

        [Test]
        public void post_organisation_risk_path_with_errors()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.OrganisationRisk;

            var vm = new OrganisationRiskViewModel
            {
                ApplicationId = applicationId,
                PageId = pageId,
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()

            };

            var command = new SubmitGatewayPageAnswerCommand(vm);

            _gatewayValidator.Setup(v => v.Validate(command))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                }
                );


            _orchestrator.Setup(x => x.GetOrganisationRiskViewModel(It.Is<GetOrganisationRiskRequest>(y => y.ApplicationId == vm.ApplicationId
                                                                                && y.UserName == username))).ReturnsAsync(vm);

            _applyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, userId, username, comment));

            var result = _controller.EvaluateOrganisationRiskPage(command).Result;

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, userId, username, comment), Times.Never);
        }

        [Test]
        public void post_organisation_risk_clarification_path_with_errors()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.OrganisationRisk;

            var vm = new OrganisationRiskViewModel
            {
                ApplicationId = applicationId,
                PageId = pageId,
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                ClarificationAnswer = ClarificationAnswer
            };

            var command = new SubmitGatewayPageAnswerCommand(vm);

            _gatewayValidator.Setup(v => v.ValidateClarification(command))
                .ReturnsAsync(new ValidationResponse
                    {
                        Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                    }
                );


            _orchestrator.Setup(x => x.GetOrganisationRiskViewModel(It.Is<GetOrganisationRiskRequest>(y => y.ApplicationId == vm.ApplicationId
                && y.UserName == username))).ReturnsAsync(vm);

            _applyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, userId, username, comment));

            var result = _controller.ClarifyOrganisationRiskPage(command).Result;

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, userId, username, comment), Times.Never);
        }
    }
}