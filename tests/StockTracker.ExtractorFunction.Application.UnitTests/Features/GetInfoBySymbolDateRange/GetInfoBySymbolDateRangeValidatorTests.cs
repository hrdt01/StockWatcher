namespace StockTracker.ExtractorFunction.Application.UnitTests.Features.GetInfoBySymbolDateRange
{
    using NUnit.Framework;
    using StockTracker.ExtractorFunction.Application.Features.GetInfoBySymbolDateRange;

    [TestFixture]
    public class GetInfoBySymbolDateRangeValidatorTests
    {
        private GetInfoBySymbolDateRangeValidator _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new GetInfoBySymbolDateRangeValidator();
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new GetInfoBySymbolDateRangeValidator();

            // Assert
            Assert.That(instance, Is.Not.Null);
        }
    }
}