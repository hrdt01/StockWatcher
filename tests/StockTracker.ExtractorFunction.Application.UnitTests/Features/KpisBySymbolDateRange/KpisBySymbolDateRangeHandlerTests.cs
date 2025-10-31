using Microsoft.Extensions.Logging;
using Moq;
using StockTracker.ExtractorFunction.Application.Features.KpisBySymbolDateRange;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.UnitTests.Features.KpisBySymbolDateRange;

[TestFixture]
public class KpisBySymbolDateRangeHandlerTests
{
    private Mock<IStockKpiCalculator> _mockKpiCalculator;
    private Mock<IStockTracker> _mockStockTracker;
    private Mock<ILogger<KpisBySymbolDateRangeHandler>> _mockLogger;
    private KpisBySymbolDateRangeHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockKpiCalculator = new Mock<IStockKpiCalculator>();
        _mockStockTracker = new Mock<IStockTracker>();
        _mockLogger = new Mock<ILogger<KpisBySymbolDateRangeHandler>>();
        _handler = new KpisBySymbolDateRangeHandler(
            _mockKpiCalculator.Object,
            _mockStockTracker.Object,
            _mockLogger.Object);
    }

    [Test]
    public async Task Handle_WithValidDates_ReturnsTrue()
    {
        // Arrange
        var request = new KpiCalculationRequest
        {
            Symbol = "TEST",
            CurrentDate = "2025-01-02"
        };

        var dates = new[] { "2025-01-01", "2025-01-02", "2025-01-03" };

        _mockStockTracker.Setup(x => x.GetDatesBySymbolAsync(request.Symbol))
            .ReturnsAsync(dates);

        _mockKpiCalculator.Setup(x => x.CalculateKpis(request.Symbol, request.CurrentDate, "2025-01-01"))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
        _mockKpiCalculator.Verify(
            x => x.CalculateKpis(request.Symbol, request.CurrentDate, "2025-01-01"), 
            Times.Once);
    }

    [Test]
    public async Task Handle_DateNotFound_ReturnsFalse()
    {
        // Arrange
        var request = new KpiCalculationRequest
        {
            Symbol = "TEST",
            CurrentDate = "2025-01-04"
        };

        var dates = new[] { "2025-01-01", "2025-01-02", "2025-01-03" };

        _mockStockTracker.Setup(x => x.GetDatesBySymbolAsync(request.Symbol))
            .ReturnsAsync(dates);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task Handle_NoDates_ReturnsFalse()
    {
        // Arrange
        var request = new KpiCalculationRequest
        {
            Symbol = "TEST",
            CurrentDate = "2025-01-01"
        };

        _mockStockTracker.Setup(x => x.GetDatesBySymbolAsync(request.Symbol))
            .ReturnsAsync(Array.Empty<string>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Constructor_NullKpiCalculator_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new KpisBySymbolDateRangeHandler(
                null, 
                _mockStockTracker.Object, 
                _mockLogger.Object));
    }

    [Test]
    public void Constructor_NullStockTracker_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new KpisBySymbolDateRangeHandler(
                _mockKpiCalculator.Object, 
                null, 
                _mockLogger.Object));
    }

    [Test]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new KpisBySymbolDateRangeHandler(
                _mockKpiCalculator.Object, 
                _mockStockTracker.Object, 
                null));
    }
}