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
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpGateway.Domain.Apply;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers.OrganisationChecks
{
    [TestFixture]
    public class OneInTwelveMonthsTests : RoatpGatewayControllerTestBase<RoatpGatewayOrganisationChecksController>
    {
        private RoatpGatewayOrganisationChecksController _controller;
        private Mock<IGatewayOrganisationChecksOrchestrator> _orchestrator;
        private static string ClarificationAnswer => "Clarification answer";

        private string comment = "test comment";
        private string viewname = "~/Views/Gateway/pages/OneInTwelveMonths.cshtml";

        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _orchestrator = new Mock<IGatewayOrganisationChecksOrchestrator>();
            _controller = new RoatpGatewayOrganisationChecksController(ApplyApiClient.Object, GatewayValidator.Object, _orchestrator.Object, Logger.Object);

            _controller.ControllerContext = MockedControllerContext.Setup();
            UserId = _controller.ControllerContext.HttpContext.User.UserId();
            Username = _controller.ControllerContext.HttpContext.User.UserDisplayName();
        }

        [Test]
        public async Task check_one_in_twelve_months_request_is_sent()
        {
            var applicationId = Guid.NewGuid();

            var vm = new OneInTwelveMonthsViewModel
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            _orchestrator.Setup(x => x.GetOneInTwelveMonthsViewModel(It.IsAny<GetOneInTwelveMonthsRequest>())).ReturnsAsync(vm);
            var result = await _controller.GetOneInTwelveMonthsPage(applicationId);
            var viewResult = result as ViewResult;
            Assert.AreEqual(viewname, viewResult.ViewName);
        }

        [Test]
        public async Task post_one_in_twelve_months_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.OneInTwelveMonths;

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

            await _controller.EvaluateOneInTwelveMonthsPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, UserId, Username, comment,null));
            _orchestrator.Verify(x => x.GetOneInTwelveMonthsViewModel(new GetOneInTwelveMonthsRequest(applicationId, Username)), Times.Never());
        }

        [Test]
        public async Task post_one_in_twelve_months_clarification_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.OneInTwelveMonths;

            var vm = new OneInTwelveMonthsViewModel
            {
                ApplicationId = applicationId,
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                OptionPassText = comment,
                PageId = pageId,
                ClarificationAnswer = ClarificationAnswer
            };
            var command = new SubmitGatewayPageAnswerCommand(vm);
            GatewayValidator.Setup(v => v.ValidateClarification(command)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            await _controller.ClarifyOneInTwelveMonthsPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswerPostClarification(applicationId, pageId, vm.Status, UserId, Username, comment, ClarificationAnswer));
            _orchestrator.Verify(x => x.GetOneInTwelveMonthsViewModel(new GetOneInTwelveMonthsRequest(applicationId, Username)), Times.Never());
        }


        [Test]
        public async Task post_one_in_twelve_months_path_with_errors()
        {
            var applicationId = Guid.NewGuid();

            var vm = new OneInTwelveMonthsViewModel
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

            _orchestrator.Setup(x => x.GetOneInTwelveMonthsViewModel(It.Is<GetOneInTwelveMonthsRequest>(y => y.ApplicationId == vm.ApplicationId
                                                                                 && y.UserName == Username))).ReturnsAsync(vm);

            await _controller.EvaluateOneInTwelveMonthsPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task post_one_in_twelve_months_clarification_path_with_errors()
        {
            var applicationId = Guid.NewGuid();

            var vm = new OneInTwelveMonthsViewModel
            {
                ApplicationId = applicationId,
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                ClarificationAnswer = ClarificationAnswer

            };

            var command = new SubmitGatewayPageAnswerCommand(vm);

            GatewayValidator.Setup(v => v.ValidateClarification(command))
                .ReturnsAsync(new ValidationResponse
                    {
                        Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                    }
                );

            _orchestrator.Setup(x => x.GetOneInTwelveMonthsViewModel(It.Is<GetOneInTwelveMonthsRequest>(y => y.ApplicationId == vm.ApplicationId
                && y.UserName == Username))).ReturnsAsync(vm);

            await _controller.ClarifyOneInTwelveMonthsPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}