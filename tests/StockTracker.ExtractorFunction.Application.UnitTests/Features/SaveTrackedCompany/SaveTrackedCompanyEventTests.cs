namespace StockTracker.ExtractorFunction.Application.UnitTests.Features.SaveTrackedCompany
{
    using NUnit.Framework;
    using StockTracker.ExtractorFunction.Application.Features.SaveTrackedCompany;

    [TestFixture]
    public class SaveTrackedCompanyEventTests
    {
        private SaveTrackedCompanyEvent _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new SaveTrackedCompanyEvent();
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new SaveTrackedCompanyEvent();

            // Assert
            Assert.That(instance, Is.Not.Null);
        }

    }
}