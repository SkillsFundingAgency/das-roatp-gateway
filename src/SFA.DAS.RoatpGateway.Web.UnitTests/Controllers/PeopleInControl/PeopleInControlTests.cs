using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Controllers;
using SFA.DAS.RoatpGateway.Web.Extensions;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Validators.Validation;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers.PeopleInControl
{

    [TestFixture]
    public class PeopleInControlTests : RoatpGatewayControllerTestBase<RoatpGatewayPeopleInControlController>
    {
        private RoatpGatewayPeopleInControlController _controller;
        private Mock<IPeopleInControlOrchestrator> _orchestrator;
        private Mock<ILogger<RoatpGatewayPeopleInControlController>> _logger;
        private readonly Guid _applicationId = Guid.NewGuid();

        public GetPeopleInControlRequest Request;
        public PeopleInControlPageViewModel ViewModel;

        [SetUp]
        public void Setup()
        {
            CoreSetup();
            _logger = new Mock<ILogger<RoatpGatewayPeopleInControlController>>();
            _orchestrator = new Mock<IPeopleInControlOrchestrator>();
            ViewModel = new PeopleInControlPageViewModel { ApplicationId = _applicationId };

            Request = new GetPeopleInControlRequest(_applicationId, UserId, Username);
            _orchestrator.Setup(x =>
                    x.GetPeopleInControlViewModel(It.IsAny<GetPeopleInControlRequest>()))
                .ReturnsAsync(ViewModel)
                .Verifiable("view model not returned");

            _controller = new RoatpGatewayPeopleInControlController(ApplyApiClient.Object, _logger.Object, GatewayValidator.Object, _orchestrator.Object);

            _controller.ControllerContext = MockedControllerContext.Setup();
            UserId = _controller.ControllerContext.HttpContext.User.UserId();
            Username = _controller.ControllerContext.HttpContext.User.UserDisplayName();
        }

        [Test]
        public void check_people_in_control_request_is_sent_and_viewmodel_returned()
        {
            var result = (ViewResult)_controller.GetGatewayPeopleInControlPage(_applicationId).Result;
            var resultModel = (PeopleInControlPageViewModel)result.Model;
            Assert.That(resultModel.ApplicationId, Is.EqualTo(_applicationId));
        }

        [Test]
        public void post_people_in_control_happy_path()
        {
            var command = new SubmitGatewayPageAnswerCommand
            {
                Status = SectionReviewStatus.Pass,
                ApplicationId = ViewModel.ApplicationId,
                SequenceNumber = ViewModel.SequenceNumber,
                PageId = ViewModel.PageId
            };

            var result = (RedirectToActionResult)_controller.EvaluatePeopleInControlPage(command).Result;

            GatewayValidator.Verify(x => x.Validate(command), Times.Once);
            Assert.That(result.ActionName, Is.EqualTo("ViewApplication"));
            Assert.That(result.ControllerName, Is.EqualTo("RoatpGateway"));
        }

        [Test]
        public void post_people_in_control_path_with_errors()
        {
            var vm = ViewModel;

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

            vm.PageId = GatewayPageIds.PeopleInControlRisk;
            vm.SourcesCheckedOn = DateTime.Now;

            _orchestrator.Setup(x => x.GetPeopleInControlViewModel(Request))
                .ReturnsAsync(vm)
                .Verifiable("view model not returned");

            var result = (ViewResult)_controller.EvaluatePeopleInControlPage(command).Result;
            var resultModel = (PeopleInControlPageViewModel)result.Model;

            GatewayValidator.Verify(x => x.Validate(command), Times.Once);
            Assert.That(resultModel.ErrorMessages.Count, Is.EqualTo(1));
        }
    }

}
