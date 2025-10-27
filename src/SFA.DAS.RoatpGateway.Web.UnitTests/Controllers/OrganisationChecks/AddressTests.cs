using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Controllers;
using SFA.DAS.RoatpGateway.Web.Extensions;
using SFA.DAS.RoatpGateway.Web.Validators.Validation;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers.OrganisationChecks
{
    [TestFixture]
    public class AddressTests : RoatpGatewayControllerTestBase<RoatpGatewayOrganisationChecksController>
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
        public void check_address_request_is_sent()
        {
            var applicationId = Guid.NewGuid();

            _orchestrator.Setup(x => x.GetAddressViewModel(It.IsAny<GetAddressRequest>()))
                .ReturnsAsync(new AddressCheckViewModel())
                .Verifiable("view model not returned");

            var _result = _controller.GetGatewayAddressPage(applicationId).Result;
            _orchestrator.Verify(x => x.GetAddressViewModel(It.IsAny<GetAddressRequest>()), Times.Once());
        }

        [Test]
        public void post_address_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "Address";

            var vm = new AddressCheckViewModel
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            vm.SourcesCheckedOn = DateTime.Now;
            var command = new SubmitGatewayPageAnswerCommand(vm);

            var pageData = JsonConvert.SerializeObject(vm);

            ApplyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, UserId, Username, It.IsAny<string>()));

            var result = _controller.EvaluateAddressPage(command).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),null), Times.Once);
            _orchestrator.Verify(x => x.GetAddressViewModel(It.IsAny<GetAddressRequest>()), Times.Never());
        }


        [Test]
        public void post_address_clarification_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "Address";

            var vm = new AddressCheckViewModel
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                ClarificationAnswer = ClarificationAnswer
            };

            vm.SourcesCheckedOn = DateTime.Now;
            var command = new SubmitGatewayPageAnswerCommand(vm);

            var pageData = JsonConvert.SerializeObject(vm);

            ApplyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, UserId, Username, It.IsAny<string>()));
            GatewayValidator.Setup(v => v.ValidateClarification(command)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            var result = _controller.ClarifyAddressPage(command).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswerPostClarification(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), ClarificationAnswer), Times.Once);
            _orchestrator.Verify(x => x.GetAddressViewModel(It.IsAny<GetAddressRequest>()), Times.Never());
        }

        [Test]
        public void post_address_path_with_errors()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "Address";

            var vm = new AddressCheckViewModel
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()

            };

            vm.ApplicationId = applicationId;
            vm.PageId = vm.PageId;
            vm.SourcesCheckedOn = DateTime.Now;
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

            _orchestrator.Setup(x => x.GetAddressViewModel(It.Is<GetAddressRequest>(y => y.ApplicationId == vm.ApplicationId
                                                                                && y.UserName == Username))).ReturnsAsync(vm);

            var pageData = JsonConvert.SerializeObject(vm);

            ApplyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, UserId, Username, It.IsAny<string>()));

            var result = _controller.EvaluateAddressPage(command).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [Test]
        public void post_address_clarification_path_with_errors()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "Address";

            var vm = new AddressCheckViewModel
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()

            };

            vm.ApplicationId = applicationId;
            vm.PageId = vm.PageId;
            vm.SourcesCheckedOn = DateTime.Now;
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

            _orchestrator.Setup(x => x.GetAddressViewModel(It.Is<GetAddressRequest>(y => y.ApplicationId == vm.ApplicationId
                && y.UserName == Username))).ReturnsAsync(vm);

            var pageData = JsonConvert.SerializeObject(vm);

            ApplyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, UserId, Username, It.IsAny<string>(), ClarificationAnswer));

            var result = _controller.ClarifyAddressPage(command).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

    }
}
