﻿using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Validators;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Validators
{
    [TestFixture]
    public class RoatpRemoveApplicationViewModelValidatorTests
    {
        private RoatpRemoveApplicationViewModel _viewModel;
        private IRoatpRemoveApplicationViewModelValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new RoatpRemoveApplicationViewModelValidator();
        }

        [Test]
        public async Task When_ConfirmApplicationAction_is_Empty()
        {
            _viewModel = new RoatpRemoveApplicationViewModel
            {
                ConfirmApplicationAction = string.Empty
            };

            var result = await _validator.Validate(_viewModel);

            Assert.IsTrue(result.Errors.Any());
            Assert.AreEqual("ConfirmApplicationActionYes", result.Errors.First().Field);
        }

        [Test]
        public async Task When_ConfirmApplicationAction_Is_No()
        {
            _viewModel = new RoatpRemoveApplicationViewModel
            {
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueNo
            };

            var result = await _validator.Validate(_viewModel);

            Assert.IsFalse(result.Errors.Any());
        }

        [Test]
        public async Task When_ConfirmApplicationAction_is_Yes_but_OptionYesText_is_Empty()
        {
            _viewModel = new RoatpRemoveApplicationViewModel
            {
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueYes,
                OptionYesText = string.Empty,
                OptionYesTextExternal = "text"
            };

            var result = await _validator.Validate(_viewModel);

            Assert.IsTrue(result.Errors.Any());
            Assert.AreEqual("OptionYesText", result.Errors.First().Field);
        }

        [Test]
        public async Task When_ConfirmApplicationAction_is_Yes_but_OptionYesTextExternal_is_Empty()
        {
            _viewModel = new RoatpRemoveApplicationViewModel
            {
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueYes,
                OptionYesText = "text",
                OptionYesTextExternal = string.Empty
            };

            var result = await _validator.Validate(_viewModel);

            Assert.IsTrue(result.Errors.Any());
            Assert.AreEqual("OptionYesTextExternal", result.Errors.First().Field);
        }

        [TestCase(150, false)]
        [TestCase(151, true)]
        public async Task When_OptionYesText_is_too_long(int wordCount, bool hasErrorMessage)
        {
            _viewModel = new RoatpRemoveApplicationViewModel
            {
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueYes,
                OptionYesText = string.Join(" ", Enumerable.Range(1, wordCount)),
                OptionYesTextExternal = "text"
            };

            var result = await _validator.Validate(_viewModel);

            Assert.AreEqual(hasErrorMessage, result.Errors.Any());

            if(hasErrorMessage)
            {
                Assert.AreEqual("OptionYesText", result.Errors.First().Field);
            }
        }

        [TestCase(500, false)]
        [TestCase(501, true)]
        public async Task When_OptionYesTextExternal_is_too_long(int wordCount, bool hasErrorMessage)
        {
            _viewModel = new RoatpRemoveApplicationViewModel
            {
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueYes,
                OptionYesText = "text",
                OptionYesTextExternal = string.Join(" ", Enumerable.Range(1, wordCount))
            };

            var result = await _validator.Validate(_viewModel);

            Assert.AreEqual(hasErrorMessage, result.Errors.Any());

            if (hasErrorMessage)
            {
                Assert.AreEqual("OptionYesTextExternal", result.Errors.First().Field);
            }
        }
    }
}
