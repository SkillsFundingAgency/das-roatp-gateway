﻿using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Validators;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Validators
{
    [TestFixture]
    public class RoatpWithdrawApplicationViewModelValidatorTests
    {
        private RoatpWithdrawApplicationViewModel _viewModel;
        private IRoatpWithdrawApplicationViewModelValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new RoatpWithdrawApplicationViewModelValidator();
        }

        [Test]
        public async Task When_ConfirmApplicationAction_is_Empty()
        {
            _viewModel = new RoatpWithdrawApplicationViewModel
            {
                ConfirmApplicationAction = string.Empty
            };

            var result = await _validator.Validate(_viewModel);

            Assert.That(result.Errors.Any());
            Assert.That("ConfirmApplicationActionYes", Is.EqualTo(result.Errors.First().Field));
        }

        [Test]
        public async Task When_ConfirmApplicationAction_Is_No()
        {
            _viewModel = new RoatpWithdrawApplicationViewModel
            {
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueNo
            };

            var result = await _validator.Validate(_viewModel);

            Assert.That(result.Errors.Any(), Is.False);
        }

        [Test]
        public async Task When_ConfirmApplicationAction_is_Yes_but_OptionYesText_is_Empty()
        {
            _viewModel = new RoatpWithdrawApplicationViewModel
            {
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueYes,
                OptionYesText = string.Empty
            };

            var result = await _validator.Validate(_viewModel);

            Assert.That(result.Errors.Any(), Is.True);
            Assert.That("OptionYesText", Is.EqualTo(result.Errors.First().Field));
        }

        [TestCase(150, false)]
        [TestCase(151, true)]
        public async Task When_OptionYesText_is_too_long(int wordCount, bool hasErrorMessage)
        {
            _viewModel = new RoatpWithdrawApplicationViewModel
            {
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueYes,
                OptionYesText = string.Join(" ", Enumerable.Range(1, wordCount))
            };

            var result = await _validator.Validate(_viewModel);

            Assert.That(hasErrorMessage, Is.EqualTo(result.Errors.Any()));

            if (hasErrorMessage)
            {
                Assert.That("OptionYesText", Is.EqualTo(result.Errors.First().Field));
            }
        }
    }
}
