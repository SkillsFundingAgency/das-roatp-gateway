using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Web.ModelBinders;
using IValueProvider = Microsoft.AspNetCore.Mvc.ModelBinding.IValueProvider;
using ModelStateDictionary = Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;
using ValueProviderResult = Microsoft.AspNetCore.Mvc.ModelBinding.ValueProviderResult;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.ModelBinders
{
    [TestFixture]
    public class StringTrimmingModelBinderTests
    {
        private StringTrimmingModelBinder _binder;

        [SetUp]
        public void Arrange()
        {
            _binder = new StringTrimmingModelBinder();
        }

        [TestCase(null, null, false)]
        [TestCase("", "", true)]
        [TestCase("  ", "", true)]
        [TestCase("  test", "test", true)]
        [TestCase("test  ", "test", true)]
        [TestCase("  test  ", "test", true)]
        [TestCase("  test1 test2", "test1 test2", true)]
        [TestCase("test1 test2  ", "test1 test2", true)]
        [TestCase("  test1 test2  ", "test1 test2", true)]
        public async Task Value_Bind_Correctly(string value, string expectedValue, bool success)
        {
            var context = new Mock<DefaultModelBindingContext>();
            var valueProvider = new Mock<IValueProvider>();
            var modelStateDictionary = new ModelStateDictionary();

            var valueProviderResult = new ValueProviderResult(value);

            context.Setup(x => x.ModelName).Returns(nameof(value));
            context.Setup(x => x.ModelType).Returns(typeof(string));
            context.Setup(x => x.Model).Returns(value);
            valueProvider.Setup(x => x.GetValue(nameof(value))).Returns(valueProviderResult);
            context.Setup(x => x.ValueProvider).Returns(valueProvider.Object);
            context.Setup(x => x.ModelState).Returns(modelStateDictionary);

            var result = new ModelBindingResult();

            context.SetupSet(p => p.Result = It.IsAny<ModelBindingResult>()).Callback<ModelBindingResult>(r => result = r);

            await _binder.BindModelAsync(context.Object);

            ModelBindingResult expectedResult = success ? ModelBindingResult.Success(expectedValue) : ModelBindingResult.Failed();

            Assert.That(expectedResult, Is.EqualTo(result));
        }
    }
}
