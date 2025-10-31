namespace StockTracker.ExtractorFunction.Application.UnitTests.Features.GetKpisBySymbol
{
    using NUnit.Framework;
    using StockTracker.ExtractorFunction.Application.Features.GetKpisBySymbol;

    [TestFixture]
    public class GetKpisBySymbolValidatorTests
    {
        private GetKpisBySymbolValidator _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new GetKpisBySymbolValidator();
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new GetKpisBySymbolValidator();

            // Assert
            Assert.That(instance, Is.Not.Null);
        }
    }
}