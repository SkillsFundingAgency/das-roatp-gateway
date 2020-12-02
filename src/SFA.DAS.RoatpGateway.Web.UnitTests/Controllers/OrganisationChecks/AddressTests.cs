using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Controllers;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers.OrganisationChecks
{
    [TestFixture]
    public class AddressTests : RoatpGatewayControllerTestBase<RoatpGatewayOrganisationChecksController>
    {
        private RoatpGatewayOrganisationChecksController _controller;
        private Mock<IGatewayOrganisationChecksOrchestrator> _orchestrator;

        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _orchestrator = new Mock<IGatewayOrganisationChecksOrchestrator>();
            _controller = new RoatpGatewayOrganisationChecksController(ApplyApiClient.Object, GatewayValidator.Object, _orchestrator.Object, Logger.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = Context
            };
        }

        [Test]
        public void check_address_request_is_sent()
        {
            var applicationId = Guid.NewGuid();

            _orchestrator.Setup(x => x.GetAddressViewModel(new GetAddressRequest(applicationId, Username)))
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

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
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

    }
}
