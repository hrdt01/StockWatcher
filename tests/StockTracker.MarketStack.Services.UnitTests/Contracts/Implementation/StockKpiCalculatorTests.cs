using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.MarketStack.Services.Contracts.Implementation;
using StockTracker.Models.KpiCalculation;
using StockTracker.Models.Persistence;
using System.Collections.ObjectModel;

namespace StockTracker.MarketStack.Services.UnitTests.Contracts.Implementation;

[TestFixture]
public class StockKpiCalculatorTests
{
    private Mock<IStockInfoRepository> _mockStockInfoRepository;
    private Mock<IAzureTableEntityResolver<StockInfoStorageTableKey>> _mockTableEntityResolver;
    private Mock<IStockKpiRepository> _mockStockKpiRepository;
    private Mock<ILogger<StockKpiCalculator>> _mockLogger;
    private StockKpiCalculator _calculator;

    [SetUp]
    public void Setup()
    {
        _mockStockInfoRepository = new Mock<IStockInfoRepository>();
        _mockTableEntityResolver = new Mock<IAzureTableEntityResolver<StockInfoStorageTableKey>>();
        _mockStockKpiRepository = new Mock<IStockKpiRepository>();
        _mockLogger = new Mock<ILogger<StockKpiCalculator>>();

        _calculator = new StockKpiCalculator(
            _mockStockInfoRepository.Object,
            _mockTableEntityResolver.Object,
            _mockStockKpiRepository.Object,
            _mockLogger.Object
        );
    }

    [Test]
    public async Task CalculateKpis_WithValidData_ReturnsTrue()
    {
        // Arrange
        var symbol = "TEST";
        var currentDate = "2025-01-02";
        var previousDate = "2025-01-01";

        var currentInfo = new StockInfoModel
        {
            Open = Convert.ToDecimal("100"),
            Close = Convert.ToDecimal("110"),
            High = Convert.ToDecimal("115"),
            Low = Convert.ToDecimal("95")
        };

        var previousInfo = new StockInfoModel
        {
            Open = Convert.ToDecimal("95"),
            Close = Convert.ToDecimal("105"),
            High = Convert.ToDecimal("110"),
            Low = Convert.ToDecimal("90")
        };

        _mockStockInfoRepository.Setup(x => x.GetByIdAsync(It.IsAny<StockInfoStorageTableKey>()))
            .ReturnsAsync((StockInfoStorageTableKey key) =>
                key.When == currentDate ? currentInfo : previousInfo);

        _mockStockKpiRepository.Setup(x => x.CreateAsync(It.IsAny<StockKpiModel>()))
            .ReturnsAsync(true);

        // Act
        var result = await _calculator.CalculateKpis(symbol, currentDate, previousDate);

        // Assert
        Assert.That(result, Is.True);
        _mockStockKpiRepository.Verify(x => x.CreateAsync(It.IsAny<StockKpiModel>()), Times.Exactly(6));
    }

    [Test]
    public async Task GetPersistedKpiNamesBySymbol_ValidSymbol_ReturnsKpiDictionary()
    {
        // Arrange
        var symbol = "TEST";
        var persistedKpis = new[]
        {
            "TEST_ToOpen",
            "TEST_ToClose",
            "TEST_FromApertura"
        };

        _mockStockKpiRepository.Setup(x => x.GetKpisBySymbolAsync(symbol))
            .ReturnsAsync(persistedKpis);

        // Act
        var result = await _calculator.GetPersistedKpiNamesBySymbol(symbol);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ReadOnlyDictionary<string, string>>());
        Assert.That(result.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task GetPersistedKpiBySymbolByDateRange_ValidInput_ReturnsKpiModels()
    {
        // Arrange
        var kpiSymbol = "TEST_TrendToOpen";
        var from = "2025-01-01";
        var to = "2025-01-02";
        var expectedModels = new List<StockKpiModel>
        {
            new() { SymbolKpiId = kpiSymbol, When = from, Result = Convert.ToDecimal("1.5") }
        };

        _mockStockKpiRepository.Setup(x => x.GetKpiInfoByDateRange(kpiSymbol, from, to))
            .ReturnsAsync(expectedModels);

        // Act
        var result = await _calculator.GetPersistedKpiBySymbolByDateRange(kpiSymbol, from, to);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedModels));
    }

    [Test]
    public async Task GetKpiDatesBySymbol_ValidSymbol_ReturnsDates()
    {
        // Arrange
        var symbol = "TEST";
        var kpiId = "TEST_TrendToOpen";
        var expectedDates = new[] { "2025-01-01", "2025-01-02" };

        _mockStockKpiRepository.Setup(x => x.GetKpisBySymbolAsync(symbol))
            .ReturnsAsync(new[] { kpiId });
        _mockStockKpiRepository.Setup(x => x.GetRowKeysByPartitionKeyAsync(kpiId))
            .ReturnsAsync(expectedDates);

        // Act
        var result = await _calculator.GetKpiDatesBySymbol(symbol);

        // Assert
        Assert.That(result, Is.EqualTo(expectedDates));
    }

    [Test]
    public async Task GetKpiDatesBySymbol_NoKpisFound_ReturnsEmptyCollection()
    {
        // Arrange
        var symbol = "TEST";
        _mockStockKpiRepository.Setup(x => x.GetKpisBySymbolAsync(symbol))
            .ReturnsAsync(Array.Empty<string>());

        // Act
        var result = await _calculator.GetKpiDatesBySymbol(symbol);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task CleanupDeprecatedInfo_SuccessfulCleanup_ReturnsTrue()
    {
        // Arrange
        var sourceDate = DateTime.Now;
        _mockStockInfoRepository.Setup(x => x.RemoveEntriesOlderThan(sourceDate))
            .ReturnsAsync(true);
        _mockStockKpiRepository.Setup(x => x.RemoveEntriesOlderThan(sourceDate))
            .ReturnsAsync(true);

        // Act
        var result = await _calculator.CleanupDeprecatedInfo(sourceDate);

        // Assert
        Assert.That(result, Is.True);
        _mockStockInfoRepository.Verify(x => x.RemoveEntriesOlderThan(sourceDate), Times.Once);
        _mockStockKpiRepository.Verify(x => x.RemoveEntriesOlderThan(sourceDate), Times.Once);
    }

    [Test]
    public async Task CleanupDeprecatedInfo_PartialFailure_ReturnsFalse()
    {
        // Arrange
        var sourceDate = DateTime.Now;
        _mockStockKpiRepository.Setup(x => x.RemoveEntriesOlderThan(sourceDate))
            .ReturnsAsync(false);

        // Act
        var result = await _calculator.CleanupDeprecatedInfo(sourceDate);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CalculateKpis_RepositoryFailure_ThrowsException()
    {
        // Arrange
        var symbol = "TEST";
        var currentDate = "2025-01-02";
        var previousDate = "2025-01-01";

        _mockStockInfoRepository.Setup(x => x.GetByIdAsync(It.IsAny<StockInfoStorageTableKey>()))
            .ThrowsAsync(new Exception("Repository error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () =>
            await _calculator.CalculateKpis(symbol, currentDate, previousDate));
    }

    [Test]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Arrange & Act
        var calculator = new StockKpiCalculator(
            _mockStockInfoRepository.Object,
            _mockTableEntityResolver.Object,
            _mockStockKpiRepository.Object,
            _mockLogger.Object
        );

        // Assert
        Assert.That(calculator, Is.Not.Null);
    }
}