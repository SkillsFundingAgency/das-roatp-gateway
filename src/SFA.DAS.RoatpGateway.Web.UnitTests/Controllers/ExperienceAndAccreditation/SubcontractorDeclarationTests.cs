using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Controllers;
using SFA.DAS.RoatpGateway.Web.Extensions;
using SFA.DAS.RoatpGateway.Web.Infrastructure.Validation;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers.ExperienceAndAccreditation
{
    [TestFixture]
    public class
        SubcontractorDeclarationTests : RoatpGatewayControllerTestBase<RoatpGatewayExperienceAndAccreditationController>
    {
        private RoatpGatewayExperienceAndAccreditationController _controller;
        private Mock<IGatewayExperienceAndAccreditationOrchestrator> _orchestrator;
        private static string ClarificationAnswer => "Clarification answer";

        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _orchestrator = new Mock<IGatewayExperienceAndAccreditationOrchestrator>();
            _controller = new RoatpGatewayExperienceAndAccreditationController(ApplyApiClient.Object,
                GatewayValidator.Object, _orchestrator.Object, Logger.Object);

            _controller.ControllerContext = MockedControllerContext.Setup();
            UserId = _controller.ControllerContext.HttpContext.User.UserId();
            Username = _controller.ControllerContext.HttpContext.User.UserDisplayName();
        }

        [Test]
        public async Task subcontractor_declaration_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedViewModel = new SubcontractorDeclarationViewModel();

            _orchestrator
                .Setup(x => x.GetSubcontractorDeclarationViewModel(
                    It.Is<GetSubcontractorDeclarationRequest>(y =>
                        y.ApplicationId == applicationId && y.UserName == Username))).ReturnsAsync(expectedViewModel);

            var result = await _controller.SubcontractorDeclaration(applicationId);
            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task subcontractor_contract_file_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedContractFile = new FileStreamResult(new MemoryStream(), "application/pdf");

            _orchestrator
                .Setup(x => x.GetSubcontractorDeclarationContractFile(
                    It.Is<GetSubcontractorDeclarationContractFileRequest>(y => y.ApplicationId == applicationId)))
                .ReturnsAsync(expectedContractFile);

            var result = await _controller.SubcontractorDeclarationContractFile(applicationId);
            Assert.AreSame(expectedContractFile, result);
        }



        [Test]
        public async Task subcontractor_contract_file_clarification_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var fileName = "something.pdf";
            var expectedContractFile = new FileStreamResult(new MemoryStream(), "application/pdf");

            _orchestrator
                .Setup(x => x.GetSubcontractorDeclarationContractFileClarification(
                    It.Is<GetSubcontractorDeclarationContractFileClarificationRequest>(y => y.ApplicationId == applicationId && y.FileName == fileName)))
                .ReturnsAsync(expectedContractFile);

            var result = await _controller.SubcontractorDeclarationContractFileClarification(applicationId, fileName);
            Assert.AreSame(expectedContractFile, result);
        }


        [Test]
        public async Task saving_subcontractor_declaration_saves_evaluation_result()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.SubcontractorDeclaration;

            var vm = new SubcontractorDeclarationViewModel
            {
                ApplicationId = applicationId,
                PageId = pageId,
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                OptionPassText = "Some pass text"
            };

            var command = new SubmitGatewayPageAnswerCommand(vm);

            GatewayValidator.Setup(v => v.Validate(command)).ReturnsAsync(new ValidationResponse
            { Errors = new List<ValidationErrorDetail>() });

            await _controller.EvaluateSubcontractorDeclarationPage(command);

            ApplyApiClient.Verify(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, UserId, Username, vm.OptionPassText, null));
        }
        [Test]
        public async Task Clarifying_subcontractor_declaration_saves_evaluation_result()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.SubcontractorDeclaration;

            var vm = new SubcontractorDeclarationViewModel
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

            GatewayValidator.Setup(v => v.ValidateClarification(command)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            await _controller.ClarifySubcontractorDeclarationPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswerPostClarification(applicationId, pageId, vm.Status, UserId, Username, vm.OptionPassText, ClarificationAnswer));
        }


        [Test]
        public async Task saving_subcontractor_declaration_without_required_fields_does_not_save()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.SubcontractorDeclaration;

            var vm = new SubcontractorDeclarationViewModel
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                ApplicationId = applicationId,
                PageId = pageId
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

            _orchestrator.Setup(x => x.GetSubcontractorDeclarationViewModel(It.Is<GetSubcontractorDeclarationRequest>(y => y.ApplicationId == vm.ApplicationId
                                                                                && y.UserName == Username))).ReturnsAsync(vm);

            await _controller.EvaluateSubcontractorDeclarationPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task clarifying_subcontractor_declaration_without_required_fields_does_not_save()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.SubcontractorDeclaration;

            var vm = new SubcontractorDeclarationViewModel
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                ApplicationId = applicationId,
                PageId = pageId,
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

            _orchestrator.Setup(x => x.GetSubcontractorDeclarationViewModel(It.Is<GetSubcontractorDeclarationRequest>(y => y.ApplicationId == vm.ApplicationId
                && y.UserName == Username))).ReturnsAsync(vm);

            await _controller.ClarifySubcontractorDeclarationPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

    }
}


