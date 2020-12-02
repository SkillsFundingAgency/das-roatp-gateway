using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using FluentAssertions;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Controllers;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers.CriminalCompliance
{
    [TestFixture]
    public class PeopleInControlCriminalComplianceTests : RoatpGatewayControllerTestBase<RoatpCriminalComplianceChecksController>
    {
        private RoatpCriminalComplianceChecksController _controller;
        private Mock<IGatewayCriminalComplianceChecksOrchestrator> _orchestrator;

        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _orchestrator = new Mock<IGatewayCriminalComplianceChecksOrchestrator>();
            _controller = new RoatpCriminalComplianceChecksController(ApplyApiClient.Object, ContextAccessor.Object, GatewayValidator.Object, _orchestrator.Object, Logger.Object);
        }

        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.UnspentCriminalConvictions)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.FailedToRepayFunds)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.FraudIrregularities)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.OngoingInvestigation)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.ContractTerminated)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.WithdrawnFromContract)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.BreachedPayments)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.RegisterOfRemovedTrustees)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.Bankrupt)]
        public void PeopleInControl_criminal_compliance_check_is_displayed(string gatewayPageId)
        {
            var applicationId = Guid.NewGuid();

            _orchestrator.Setup(x => x.GetCriminalComplianceCheckViewModel(It.IsAny<GetCriminalComplianceCheckRequest>()))
                .ReturnsAsync(new CriminalCompliancePageViewModel())
                .Verifiable("view model not returned");

            var result = _controller.GetCriminalCompliancePage(applicationId, gatewayPageId).GetAwaiter().GetResult();
            _orchestrator.Verify(x => x.GetCriminalComplianceCheckViewModel(It.IsAny<GetCriminalComplianceCheckRequest>()), Times.Once());

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var viewModel = viewResult.Model as CriminalCompliancePageViewModel;
            viewModel.Should().NotBeNull();
        }

        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.UnspentCriminalConvictions)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.FailedToRepayFunds)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.FraudIrregularities)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.OngoingInvestigation)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.ContractTerminated)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.WithdrawnFromContract)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.BreachedPayments)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.RegisterOfRemovedTrustees)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.Bankrupt)]
        public void PeopleInControl_criminal_compliance_check_result_is_saved(string gatewayPageId)
        {
            var model = new CriminalCompliancePageViewModel
            {
                ApplicationId = Guid.NewGuid(),
                ApplyLegalName = "legal name",
                ComplianceCheckQuestionId = "CC-40",
                ComplianceCheckAnswer = "No",
                OptionPassText = "check passed",
                Status = "Pass",
                PageId = gatewayPageId,
                QuestionText = "Question text",
                Ukprn = "10001234",
            };

            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            var command = new SubmitGatewayPageAnswerCommand(model);

            GatewayValidator.Setup(x => x.Validate(command)).ReturnsAsync(validationResponse);

            var result = _controller.EvaluateCriminalCompliancePage(command).GetAwaiter().GetResult();

            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("ViewApplication");

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(model.ApplicationId, gatewayPageId, model.Status, UserId, Username, model.OptionPassText), Times.Once);
        }

        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.UnspentCriminalConvictions)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.FailedToRepayFunds)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.FraudIrregularities)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.OngoingInvestigation)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.ContractTerminated)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.WithdrawnFromContract)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.BreachedPayments)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.RegisterOfRemovedTrustees)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.Bankrupt)]
        public void PeopleInControl_criminal_compliance_check_has_validation_error(string gatewayPageId)
        {
            var model = new CriminalCompliancePageViewModel
            {
                ApplicationId = Guid.NewGuid(),
                ApplyLegalName = "legal name",
                ComplianceCheckQuestionId = "CC-40",
                ComplianceCheckAnswer = "No",
                OptionFailText = null,
                Status = "Fail",
                PageId = gatewayPageId,
                QuestionText = "Question text",
                Ukprn = "10001234"
            };

            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
                {
                    new ValidationErrorDetail
                    {
                        ErrorMessage = "Comments are mandatory",
                        Field = "OptionFailText"
                    }
                }
            };

            var command = new SubmitGatewayPageAnswerCommand(model);

            GatewayValidator.Setup(x => x.Validate(command)).ReturnsAsync(validationResponse);

            _orchestrator.Setup(x => x.GetCriminalComplianceCheckViewModel(It.Is<GetCriminalComplianceCheckRequest>(y => y.ApplicationId == model.ApplicationId
                                                                                && y.UserName == Username))).ReturnsAsync(model);

            var result = _controller.EvaluateCriminalCompliancePage(command).GetAwaiter().GetResult();

            var viewResult = result as ViewResult;
            var viewModel = viewResult.Model as CriminalCompliancePageViewModel;
            viewModel.Should().NotBeNull();
            viewModel.ErrorMessages.Count.Should().BeGreaterThan(0);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(model.ApplicationId, gatewayPageId, model.Status, UserId, Username, model.OptionPassText), Times.Never);
        }
    }
}
