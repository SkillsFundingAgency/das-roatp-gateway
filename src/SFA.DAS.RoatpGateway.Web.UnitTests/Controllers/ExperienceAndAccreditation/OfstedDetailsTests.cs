using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Controllers;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers.ExperienceAndAccreditation
{
    [TestFixture]
    public class OfstedDetailsTests : RoatpGatewayControllerTestBase<RoatpGatewayExperienceAndAccreditationController>
    {
        private RoatpGatewayExperienceAndAccreditationController _controller;
        private Mock<IGatewayExperienceAndAccreditationOrchestrator> _orchestrator;
        private static string ClarificationAnswer => "Clarification answer";

        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _orchestrator = new Mock<IGatewayExperienceAndAccreditationOrchestrator>();
            _controller = new RoatpGatewayExperienceAndAccreditationController(ApplyApiClient.Object, GatewayValidator.Object, _orchestrator.Object, Logger.Object);

            _controller.ControllerContext = MockedControllerContext.Setup();
            UserId = _controller.ControllerContext.HttpContext.User.UserId();
            Username = _controller.ControllerContext.HttpContext.User.UserDisplayName();
        }

        [Test]
        public async Task ofsted_details_are_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedViewModel = new OfstedDetailsViewModel();

            _orchestrator.Setup(x => x.GetOfstedDetailsViewModel(It.Is<GetOfstedDetailsRequest>(y => y.ApplicationId == applicationId && y.UserName == Username))).ReturnsAsync(expectedViewModel);

            var result = await _controller.OfstedDetails(applicationId);
            Assert.That(result.Model, Is.SameAs(expectedViewModel));
        }

        [Test]
        public async Task saving_ofsted_details_saves_evaluation_result()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.Ofsted;

            var vm = new OfstedDetailsViewModel
            {
                ApplicationId = applicationId,
                PageId = pageId,
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                OptionPassText = "Some pass text"
            };

            var command = new SubmitGatewayPageAnswerCommand(vm);

            GatewayValidator.Setup(v => v.Validate(command)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            await _controller.EvaluateOfstedDetailsPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, UserId, Username, vm.OptionPassText,null));
        }


        [Test]
        public async Task clarifying_ofsted_details_saves_evaluation_result()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.Ofsted;

            var vm = new OfstedDetailsViewModel
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

            await _controller.ClarifyOfstedDetailsPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswerPostClarification(applicationId, pageId, vm.Status, UserId, Username, vm.OptionPassText, ClarificationAnswer));
        }

        [Test]
        public async Task saving_ofsted_details_without_required_fields_does_not_save()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.Ofsted;

            var vm = new OfstedDetailsViewModel()
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                ApplicationId = applicationId,
                PageId = pageId,
                ClarificationAnswer = ClarificationAnswer
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

            _orchestrator.Setup(x => x.GetOfstedDetailsViewModel(It.Is<GetOfstedDetailsRequest>(y => y.ApplicationId == vm.ApplicationId
                                                                                && y.UserName == Username))).ReturnsAsync(vm);

            await _controller.EvaluateOfstedDetailsPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [Test]
        public async Task clarifying_ofsted_details_without_required_fields_does_not_save()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.Ofsted;

            var vm = new OfstedDetailsViewModel()
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

            _orchestrator.Setup(x => x.GetOfstedDetailsViewModel(It.Is<GetOfstedDetailsRequest>(y => y.ApplicationId == vm.ApplicationId
                && y.UserName == Username))).ReturnsAsync(vm);

            await _controller.ClarifyOfstedDetailsPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
