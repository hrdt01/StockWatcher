using Microsoft.Extensions.Logging;
using Moq;
using StockTracker.ExtractorFunction.Application.Features.GetKpisBySymbolDateRange;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;
using StockTracker.Models.Persistence;

namespace StockTracker.ExtractorFunction.Application.UnitTests.Features.GetKpisBySymbolDateRange;

[TestFixture]
public class GetKpisBySymbolDateRangeHandlerTests
{
    private Mock<IStockKpiCalculator> _mockStockKpiCalculator;
    private Mock<ILogger<GetKpisBySymbolDateRangeHandler>> _mockLogger;
    private GetKpisBySymbolDateRangeHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockStockKpiCalculator = new Mock<IStockKpiCalculator>();
        _mockLogger = new Mock<ILogger<GetKpisBySymbolDateRangeHandler>>();
        _handler = new GetKpisBySymbolDateRangeHandler(_mockStockKpiCalculator.Object, _mockLogger.Object);
    }

    [Test]
    public async Task Handle_ValidRequest_ReturnsStockKpiResponse()
    {
        // Arrange
        var request = new KpisBySymbolDateRangeRequest 
        { 
            SymbolKpi = "TEST_TrendToOpen",
            From = "2025-01-01",
            To = "2025-01-02"
        };

        var persistedKpis = new List<StockKpiModel>
        {
            new()
            {
                SymbolKpiId = request.SymbolKpi,
                When = "2025-01-01",
                Result = 1.5m
            },
            new()
            {
                SymbolKpiId = request.SymbolKpi,
                When = "2025-01-02",
                Result = 2.0m
            }
        };

        _mockStockKpiCalculator.Setup(x => x.GetPersistedKpiBySymbolByDateRange(
                request.SymbolKpi, request.From, request.To))
            .ReturnsAsync(persistedKpis);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SymbolKpi, Is.EqualTo(request.SymbolKpi));
            Assert.That(result.KpiValues, Is.Not.Empty);
            Assert.That(result.KpiValues.Count(), Is.EqualTo(2));
        });
    }

    [Test]
    public async Task Handle_NoData_ReturnsEmptyKpiValues()
    {
        // Arrange
        var request = new KpisBySymbolDateRangeRequest 
        { 
            SymbolKpi = "TEST_TrendToOpen",
            From = "2025-01-01",
            To = "2025-01-02"
        };

        _mockStockKpiCalculator.Setup(x => x.GetPersistedKpiBySymbolByDateRange(
                request.SymbolKpi, request.From, request.To))
            .ReturnsAsync(Array.Empty<StockKpiModel>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SymbolKpi, Is.EqualTo(request.SymbolKpi));
            Assert.That(result.KpiValues, Is.Empty);
        });
    }

    [Test]
    public void Constructor_NullCalculator_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new GetKpisBySymbolDateRangeHandler(null, _mockLogger.Object));
    }

    [Test]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new GetKpisBySymbolDateRangeHandler(_mockStockKpiCalculator.Object, null));
    }
}