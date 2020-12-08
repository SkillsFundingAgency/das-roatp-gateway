using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Controllers;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Validators;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers
{
    [TestFixture]
    public class RoatpGatewayControllerBaseTests
    {
        private RoatpGatewayControllerBase<RoatpGatewayControllerBaseTests> _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new RoatpGatewayControllerBase<RoatpGatewayControllerBaseTests>(Mock.Of<IRoatpApplicationApiClient>(), Mock.Of<ILogger<RoatpGatewayControllerBaseTests>>(), Mock.Of<IRoatpGatewayPageValidator>());
        }


        [TestCase(SectionReviewStatus.Pass, "pass", "fail", "in progress", "clarification", "pass", "", "", "")]
        [TestCase(SectionReviewStatus.Fail, "pass", "fail", "in progress", "clarification", "", "fail", "", "")]
        [TestCase(SectionReviewStatus.InProgress, "pass", "fail", "in progress", "clarification", "", "", "in progress", "")]
        [TestCase(SectionReviewStatus.Clarification, "pass", "fail", "in progress", "clarification", "", "", "", "clarification")]
        [TestCase(null, "pass", "fail", "in progress", "clarification", "pass", "fail", "in progress", "clarification")]
        public void check_gateway_options_settings(string status, string optionPassText, string optionFailText, string optionInProgressText, string optionClarificationText, string expectedOptionPassText, string expectedOptionFailText, string expectedOptionInProgressText, string expectedOptionClarificationText)
        {
            var vm = new SubmitGatewayPageAnswerCommand
            {
                Status = status,
                OptionPassText = optionPassText,
                OptionFailText = optionFailText,
                OptionInProgressText = optionInProgressText,
                OptionClarificationText = optionClarificationText
            };

            _controller.SetupGatewayPageOptionTexts(vm);
            Assert.AreEqual(expectedOptionPassText, vm.OptionPassText);
            Assert.AreEqual(expectedOptionFailText, vm.OptionFailText);
            Assert.AreEqual(expectedOptionInProgressText, vm.OptionInProgressText);
            Assert.AreEqual(expectedOptionClarificationText, vm.OptionClarificationText);
        }
    }
}
