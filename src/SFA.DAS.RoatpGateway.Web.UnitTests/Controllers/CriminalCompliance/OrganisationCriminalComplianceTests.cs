﻿using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using FluentAssertions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Controllers;
using SFA.DAS.RoatpGateway.Web.Extensions;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers.CriminalCompliance
{
    [TestFixture]
    public class OrganisationCriminalComplianceTests : RoatpGatewayControllerTestBase<RoatpCriminalComplianceChecksController>
    {
        private RoatpCriminalComplianceChecksController _controller;
        private Mock<IGatewayCriminalComplianceChecksOrchestrator> _orchestrator;

        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _orchestrator = new Mock<IGatewayCriminalComplianceChecksOrchestrator>();
            _controller = new RoatpCriminalComplianceChecksController(ApplyApiClient.Object, GatewayValidator.Object, _orchestrator.Object, Logger.Object);

            _controller.ControllerContext = MockedControllerContext.Setup();
            UserId = _controller.ControllerContext.HttpContext.User.UserId();
            Username = _controller.ControllerContext.HttpContext.User.UserDisplayName();
        }

        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.InvestigationPublicBody)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency)]
        public void Organisation_criminal_compliance_check_is_displayed(string gatewayPageId)
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

        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors,null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.InvestigationPublicBody, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.InvestigationPublicBody, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, "clarification answer")]
        public void Organisation_criminal_compliance_check_result_is_saved(string gatewayPageId, string clarificationAnswer)
        {
            var model = new CriminalCompliancePageViewModel
            {
                ApplicationId = Guid.NewGuid(),
                ApplyLegalName = "legal name",
                ComplianceCheckQuestionId = "CC-20",
                ComplianceCheckAnswer = "No",
                OptionPassText = "check passed",
                Status = "Pass",
                PageId = gatewayPageId,
                QuestionText = "Question text",
                Ukprn = "10001234",
                ClarificationAnswer = clarificationAnswer
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

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(model.ApplicationId, gatewayPageId, model.Status, UserId, Username, model.OptionPassText, clarificationAnswer), Times.Once);
        }

        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.InvestigationPublicBody, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.InvestigationPublicBody, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, "clarification answer")]
        public void Organisation_criminal_compliance_check_clarification_result_is_saved(string gatewayPageId, string clarificationAnswer)
        {
            var model = new CriminalCompliancePageViewModel
            {
                ApplicationId = Guid.NewGuid(),
                ApplyLegalName = "legal name",
                ComplianceCheckQuestionId = "CC-20",
                ComplianceCheckAnswer = "No",
                OptionPassText = "check passed",
                Status = "Pass",
                PageId = gatewayPageId,
                QuestionText = "Question text",
                Ukprn = "10001234",
                ClarificationAnswer = clarificationAnswer
            };

            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            var command = new SubmitGatewayPageAnswerCommand(model);

            GatewayValidator.Setup(x => x.ValidateClarification(command)).ReturnsAsync(validationResponse);

            var result = _controller.ClarifyCriminalCompliancePage(command).GetAwaiter().GetResult();

            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("ViewApplication");

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswerPostClarification(model.ApplicationId, gatewayPageId, model.Status, UserId, Username, model.OptionPassText, clarificationAnswer), Times.Once);
        }

        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.InvestigationPublicBody, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, null)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.InvestigationPublicBody, "clarification answer")]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, "clarification answer")]
        public void Organisation_criminal_compliance_check_has_validation_error(string gatewayPageId, string clarificationAnswer)
        {
            var model = new CriminalCompliancePageViewModel
            {
                ApplicationId = Guid.NewGuid(),
                ApplyLegalName = "legal name",
                ComplianceCheckQuestionId = "CC-20",
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

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(model.ApplicationId, gatewayPageId, model.Status, UserId, Username, model.OptionPassText, clarificationAnswer), Times.Never);
        }
    



    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors, null)]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds, null)]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination, null)]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly, null)]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO, null)]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, null)]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister, null)]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation, null)]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, null)]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding, null)]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.InvestigationPublicBody, null)]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, null)]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors, "clarification answer")]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds, "clarification answer")]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination, "clarification answer")]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly, "clarification answer")]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO, "clarification answer")]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, "clarification answer")]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister, "clarification answer")]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation, "clarification answer")]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, "clarification answer")]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding, "clarification answer")]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.InvestigationPublicBody, "clarification answer")]
    [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, "clarification answer")]
    public void Organisation_criminal_compliance_clarification_check_has_validation_error(string gatewayPageId, string clarificationAnswer)
    {
        var model = new CriminalCompliancePageViewModel
        {
            ApplicationId = Guid.NewGuid(),
            ApplyLegalName = "legal name",
            ComplianceCheckQuestionId = "CC-20",
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

        GatewayValidator.Setup(x => x.ValidateClarification(command)).ReturnsAsync(validationResponse);

        _orchestrator.Setup(x => x.GetCriminalComplianceCheckViewModel(It.Is<GetCriminalComplianceCheckRequest>(y => y.ApplicationId == model.ApplicationId
                                                                            && y.UserName == Username))).ReturnsAsync(model);

        var result = _controller.ClarifyCriminalCompliancePage(command).GetAwaiter().GetResult();

        var viewResult = result as ViewResult;
        var viewModel = viewResult.Model as CriminalCompliancePageViewModel;
        viewModel.Should().NotBeNull();
        viewModel.ErrorMessages.Count.Should().BeGreaterThan(0);

        ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(model.ApplicationId, gatewayPageId, model.Status, UserId, Username, model.OptionPassText, clarificationAnswer), Times.Never);
    }
}
}
