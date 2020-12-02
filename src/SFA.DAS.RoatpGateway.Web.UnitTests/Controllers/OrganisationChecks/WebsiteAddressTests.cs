using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Controllers;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers.OrganisationChecks
{
    [TestFixture]
    public class WebsiteAddressTests : RoatpGatewayControllerTestBase<RoatpGatewayOrganisationChecksController>
    {
        private RoatpGatewayOrganisationChecksController _controller;
        private Mock<IGatewayOrganisationChecksOrchestrator> _orchestrator;

        private string comment = "test comment";
        private string viewname = "~/Views/Gateway/pages/Website.cshtml";

        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _orchestrator = new Mock<IGatewayOrganisationChecksOrchestrator>();
            _controller = new RoatpGatewayOrganisationChecksController(ApplyApiClient.Object, GatewayValidator.Object, _orchestrator.Object, Logger.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = Context
            };
        }

        [Test]
        public void check_website_request_is_sent()
        {
            var applicationId = Guid.NewGuid();

            var vm = new WebsiteViewModel
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            _orchestrator.Setup(x => x.GetWebsiteViewModel(new GetWebsiteRequest(applicationId, Username))).ReturnsAsync(vm);

            var result = _controller.GetWebsitePage(applicationId).Result;
            var viewResult = result as ViewResult;
            Assert.AreEqual(viewname, viewResult.ViewName);
        }

        [Test]
        public void post_website_address_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.WebsiteAddress;

            var vm = new WebsiteViewModel
            {
                ApplicationId = applicationId,
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                OptionPassText = comment,
                PageId = pageId
            };
            var command = new SubmitGatewayPageAnswerCommand(vm);

            var result = _controller.EvaluateWebsitePage(command).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, UserId, Username, comment));
            _orchestrator.Verify(x => x.GetWebsiteViewModel(new GetWebsiteRequest(applicationId, Username)), Times.Never());
        }

        [Test]
        public void post_website_address_path_with_errors()
        {
            var applicationId = Guid.NewGuid();

            var vm = new WebsiteViewModel
            {
                ApplicationId = applicationId,
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()

            };

            var command = new SubmitGatewayPageAnswerCommand(vm);

            GatewayValidator.Setup(v => v.Validate(command))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                }
                );

            _orchestrator.Setup(x => x.GetWebsiteViewModel(It.Is<GetWebsiteRequest>(y => y.ApplicationId == vm.ApplicationId
                                                                                 && y.UserName == Username))).ReturnsAsync(vm);

            var result = _controller.EvaluateWebsitePage(command).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}