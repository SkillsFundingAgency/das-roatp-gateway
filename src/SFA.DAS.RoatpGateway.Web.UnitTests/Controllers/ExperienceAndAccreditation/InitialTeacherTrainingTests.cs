using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Controllers;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers.ExperienceAndAccreditation
{
    [TestFixture]
    public class InitialTeacherTrainingTests : RoatpGatewayControllerTestBase<RoatpGatewayExperienceAndAccreditationController>
    {
        private RoatpGatewayExperienceAndAccreditationController _controller;
        private Mock<IGatewayExperienceAndAccreditationOrchestrator> _orchestrator;

        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _orchestrator = new Mock<IGatewayExperienceAndAccreditationOrchestrator>();
            _controller = new RoatpGatewayExperienceAndAccreditationController(ApplyApiClient.Object, GatewayValidator.Object, _orchestrator.Object, Logger.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = Context
            };
        }

        [Test]
        public async Task initial_teacher_training_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedViewModel = new InitialTeacherTrainingViewModel();

            _orchestrator.Setup(x => x.GetInitialTeacherTrainingViewModel(It.Is<GetInitialTeacherTrainingRequest>(y => y.ApplicationId == applicationId && y.UserName == Username))).ReturnsAsync(expectedViewModel);

            var result = await _controller.InitialTeacherTraining(applicationId);
            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task saving_initial_teacher_training_saves_evaluation_result()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.InitialTeacherTraining;

            var vm = new InitialTeacherTrainingViewModel
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

            await _controller.EvaluateInitialTeacherTrainingPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, UserId, Username, vm.OptionPassText));
        }

        [Test]
        public async Task saving_initial_teacher_training_without_required_fields_does_not_save()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.InitialTeacherTraining;

            var vm = new InitialTeacherTrainingViewModel()
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
            _orchestrator.Setup(x => x.GetInitialTeacherTrainingViewModel(It.Is<GetInitialTeacherTrainingRequest>(y => y.ApplicationId == vm.ApplicationId
                                                                                && y.UserName == Username))).ReturnsAsync(vm);

            await _controller.EvaluateInitialTeacherTrainingPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
