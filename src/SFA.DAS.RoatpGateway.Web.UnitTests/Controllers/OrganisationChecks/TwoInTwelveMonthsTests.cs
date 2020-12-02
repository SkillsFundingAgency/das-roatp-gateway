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
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers.OrganisationChecks
{
    [TestFixture]
    public class TwoInTwelveMonthsTests : RoatpGatewayControllerTestBase<RoatpGatewayOrganisationChecksController>
    {
        private RoatpGatewayOrganisationChecksController _controller;
        private Mock<IGatewayOrganisationChecksOrchestrator> _orchestrator;

        private string comment = "test comment";
        private string viewname = "~/Views/Gateway/pages/TwoInTwelveMonths.cshtml";

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
        public async Task check_two_in_twelve_months_request_is_sent()
        {
            var applicationId = Guid.NewGuid();

            var vm = new TwoInTwelveMonthsViewModel
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            _orchestrator.Setup(x => x.GetTwoInTwelveMonthsViewModel(new GetTwoInTwelveMonthsRequest(applicationId, Username))).ReturnsAsync(vm);

            var result = await _controller.GetTwoInTwelveMonthsPage(applicationId);
            var viewResult = result as ViewResult;
            Assert.AreEqual(viewname, viewResult.ViewName);
        }

        [Test]
        public async Task post_two_in_twelve_months_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.TwoInTwelveMonths;

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

            await _controller.EvaluateTwoInTwelveMonthsPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, UserId, Username, comment));
            _orchestrator.Verify(x => x.GetTwoInTwelveMonthsViewModel(new GetTwoInTwelveMonthsRequest(applicationId, Username)), Times.Never());
        }

        [Test]
        public async Task post_two_in_twelve_months_path_with_errors()
        {
            var applicationId = Guid.NewGuid();

            var vm = new TwoInTwelveMonthsViewModel
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

            _orchestrator.Setup(x => x.GetTwoInTwelveMonthsViewModel(It.Is<GetTwoInTwelveMonthsRequest>(y => y.ApplicationId == vm.ApplicationId
                                                                                 && y.UserName == Username))).ReturnsAsync(vm);

            await _controller.EvaluateTwoInTwelveMonthsPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}