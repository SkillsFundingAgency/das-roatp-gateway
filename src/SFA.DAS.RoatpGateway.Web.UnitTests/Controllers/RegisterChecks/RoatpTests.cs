﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Validation;
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
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers.RegisterChecks
{
    [TestFixture]
    public class RoatpTests
    {
        private RoatpGatewayRegisterChecksController _controller;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpGatewayPageValidator> _gatewayValidator;
        private Mock<IGatewayRegisterChecksOrchestrator> _orchestrator;
        private Mock<ILogger<RoatpGatewayRegisterChecksController>> _logger;
        private static string ClarificationAnswer => "Clarification answer";

        private string userId = "user123";
        private string username = "john smith";
        private string givenName = "john";
        private string surname = "smith";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _gatewayValidator = new Mock<IRoatpGatewayPageValidator>();
            _logger = new Mock<ILogger<RoatpGatewayRegisterChecksController>>();
            _orchestrator = new Mock<IGatewayRegisterChecksOrchestrator>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
             {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", userId),
                new Claim(ClaimTypes.GivenName, givenName),
                new Claim(ClaimTypes.Surname, surname)
             }));

            _gatewayValidator.Setup(v => v.Validate(It.IsAny<SubmitGatewayPageAnswerCommand>()))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>()
                });

            _controller = new RoatpGatewayRegisterChecksController(_applyApiClient.Object, _gatewayValidator.Object, _orchestrator.Object, _logger.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task check_roatp_request_is_sent()
        {
            var applicationId = Guid.NewGuid();

            _orchestrator.Setup(x => x.GetRoatpViewModel(It.IsAny<GetRoatpRequest>()))
                .ReturnsAsync(new RoatpPageViewModel())
                .Verifiable("view model not returned");

            var _result = await _controller.GetGatewayRoatpPage(applicationId);
            _orchestrator.Verify(x => x.GetRoatpViewModel(It.Is<GetRoatpRequest>(y => y.ApplicationId == applicationId && y.UserName == username)), Times.Once());
        }

        [Test]
        public async Task saving_roatp_happy_path_saves_evaluation_result()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.Roatp;

            var vm = new RoatpPageViewModel
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

            var result = await _controller.EvaluateRoatpPage(command);

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, userId, username, vm.OptionPassText,null), Times.Once);
        }

        [Test]
        public async Task saving_roatp_clarification_happy_path_saves_evaluation_result()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.Roatp;

            var vm = new RoatpPageViewModel
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

            var result = await _controller.ClarifyRoatpPage(command);

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswerPostClarification(applicationId, pageId, vm.Status, userId, username, vm.OptionPassText, ClarificationAnswer), Times.Once);
        }

        [Test]
        public async Task saving_roatp_without_required_fields_does_not_save()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.Roatp;

            var vm = new RoatpPageViewModel
            {
                ApplicationId = applicationId,
                PageId = pageId,
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                OptionFailText = null
            };

            var command = new SubmitGatewayPageAnswerCommand(vm);

            _gatewayValidator.Setup(v => v.Validate(command))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                });

            _orchestrator.Setup(x => x.GetRoatpViewModel(It.Is<GetRoatpRequest>(y => y.ApplicationId == vm.ApplicationId
                                                                                && y.UserName == username))).ReturnsAsync(vm);

            var result = await _controller.EvaluateRoatpPage(command);

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, userId, username, vm.OptionPassText), Times.Never);
        }

        [Test]
        public async Task clarifying_roatp_without_required_fields_does_not_save()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.Roatp;

            var vm = new RoatpPageViewModel
            {
                ApplicationId = applicationId,
                PageId = pageId,
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                OptionFailText = null,
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
                });

            _orchestrator.Setup(x => x.GetRoatpViewModel(It.Is<GetRoatpRequest>(y => y.ApplicationId == vm.ApplicationId
                && y.UserName == username))).ReturnsAsync(vm);

            var result = await _controller.ClarifyRoatpPage(command);

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, userId, username, vm.OptionPassText, ClarificationAnswer), Times.Never);
        }
    }
}
