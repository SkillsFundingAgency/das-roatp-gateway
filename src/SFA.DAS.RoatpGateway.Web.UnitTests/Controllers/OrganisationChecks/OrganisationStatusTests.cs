using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Controllers;
using SFA.DAS.RoatpGateway.Web.Extensions;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Validators.Validation;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers.OrganisationChecks
{
    [TestFixture]
    public class OrganisationStatusTests : RoatpGatewayControllerTestBase<RoatpGatewayOrganisationChecksController>
    {
        private RoatpGatewayOrganisationChecksController _controller;
        private Mock<IGatewayOrganisationChecksOrchestrator> _orchestrator;
        private static string ClarificationAnswer => "Clarification answer";

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
        public void check_organisation_status_request_is_called()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-10";

            _orchestrator.Setup(x => x.GetOrganisationStatusViewModel(It.IsAny<GetOrganisationStatusRequest>()))
                .ReturnsAsync(new OrganisationStatusViewModel())
                .Verifiable("view model not returned");

            var _result = _controller.GetOrganisationStatusPage(applicationId, pageId).Result;
            _orchestrator.Verify(x => x.GetOrganisationStatusViewModel(It.IsAny<GetOrganisationStatusRequest>()), Times.Once());
        }

        [Test]
        public void post_organisation_status_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-30";

            var viewModel = new OrganisationStatusViewModel()
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            viewModel.SourcesCheckedOn = DateTime.Now;
            var command = new SubmitGatewayPageAnswerCommand(viewModel);

            ApplyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, viewModel.Status, UserId, Username, It.IsAny<string>()));

            var result = _controller.EvaluateOrganisationStatus(command).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),null), Times.Once);
            _orchestrator.Verify(x => x.GetOrganisationStatusViewModel(It.IsAny<GetOrganisationStatusRequest>()), Times.Never());
        }

        [Test]
        public void post_organisation_status_clarification_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-30";

            var viewModel = new OrganisationStatusViewModel()
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                ClarificationAnswer = ClarificationAnswer
            };

            viewModel.SourcesCheckedOn = DateTime.Now;
            var command = new SubmitGatewayPageAnswerCommand(viewModel);
            GatewayValidator.Setup(v => v.ValidateClarification(command)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            ApplyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, viewModel.Status, UserId, Username, It.IsAny<string>()));

            var result = _controller.ClarifyOrganisationStatus(command).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswerPostClarification(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), ClarificationAnswer), Times.Once);
            _orchestrator.Verify(x => x.GetOrganisationStatusViewModel(It.IsAny<GetOrganisationStatusRequest>()), Times.Never());
        }


        [Test]
        public void post_organisation_status_path_with_errors()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-20";

            var viewModel = new OrganisationStatusViewModel()
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()

            };

            viewModel.ApplicationId = applicationId;
            viewModel.PageId = viewModel.PageId;
            viewModel.SourcesCheckedOn = DateTime.Now;
            var command = new SubmitGatewayPageAnswerCommand(viewModel);

            GatewayValidator.Setup(v => v.Validate(command))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                }
                );

            _orchestrator.Setup(x => x.GetOrganisationStatusViewModel(It.IsAny<GetOrganisationStatusRequest>()))
                .ReturnsAsync(viewModel)
                .Verifiable("view model not returned");

            ApplyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, viewModel.Status, UserId, Username, It.IsAny<string>()));

            var result = _controller.EvaluateOrganisationStatus(command).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _orchestrator.Verify(x => x.GetTradingNameViewModel(It.IsAny<GetTradingNameRequest>()), Times.Never());
        }


        [Test]
        public void post_organisation_status_clarification_path_with_errors()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-20";

            var viewModel = new OrganisationStatusViewModel()
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                ClarificationAnswer = ClarificationAnswer
            };

            viewModel.ApplicationId = applicationId;
            viewModel.PageId = viewModel.PageId;
            viewModel.SourcesCheckedOn = DateTime.Now;
            var command = new SubmitGatewayPageAnswerCommand(viewModel);

            GatewayValidator.Setup(v => v.ValidateClarification(command))
                .ReturnsAsync(new ValidationResponse
                    {
                        Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                    }
                );

            _orchestrator.Setup(x => x.GetOrganisationStatusViewModel(It.IsAny<GetOrganisationStatusRequest>()))
                .ReturnsAsync(viewModel)
                .Verifiable("view model not returned");

            ApplyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, viewModel.Status, UserId, Username, It.IsAny<string>()));

            var result = _controller.ClarifyOrganisationStatus(command).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _orchestrator.Verify(x => x.GetTradingNameViewModel(It.IsAny<GetTradingNameRequest>()), Times.Never());
        }
    }
}