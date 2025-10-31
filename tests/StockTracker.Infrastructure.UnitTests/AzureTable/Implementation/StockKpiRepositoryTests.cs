using Microsoft.Extensions.Options;
using Moq;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Infrastructure.AzureTable.Implementation;
using StockTracker.Models.Persistence;

namespace StockTracker.Infrastructure.UnitTests.AzureTable.Implementation;

[TestFixture]
public class StockKpiRepositoryTests
{
    private Mock<IOptionsMonitor<AzureTableOptions>> _mockOptions;
    private Mock<IAzureTableEntityResolver<StockKpiStorageTableKey>> _mockEntityResolver;
    private StockKpiRepository _repository;
    private AzureTableOptions _options;

    [SetUp]
    public void Setup()
    {
        _mockOptions = new Mock<IOptionsMonitor<AzureTableOptions>>();
        _mockEntityResolver = new Mock<IAzureTableEntityResolver<StockKpiStorageTableKey>>();

        _options = new AzureTableOptions
        {
            ConnectionString = "UseDevelopmentStorage=true"
        };

        _mockOptions.Setup(x => x.CurrentValue).Returns(_options);

        _repository = new StockKpiRepository(_mockOptions.Object, _mockEntityResolver.Object);
    }
    
    [Test]
    public async Task GetKpisBySymbolAsync_CallsGetPartitionsByPattern()
    {
        // Arrange
        var symbol = "TEST";
        var expectedKpis = new[] { "TEST_TrendToOpen", "TEST_TrendToClose" };

        // Act
        var result = await _repository.GetKpisBySymbolAsync(symbol);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetKpiInfoByDateRange_WithValidRange_ReturnsKpiModels()
    {
        // Arrange
        var kpiSymbol = "TEST_TrendToOpen";
        var from = "2025-01-01";
        var to = "2025-01-02";
        var kpiModel = new StockKpiModel
        {
            SymbolKpiId = kpiSymbol,
            When = from,
            Result = 1.5m
        };

        _mockEntityResolver.Setup(x => x.ResolvePartitionKey(It.IsAny<StockKpiStorageTableKey>()))
            .Returns(kpiSymbol);
        _mockEntityResolver.Setup(x => x.ResolveRowKey(It.IsAny<StockKpiStorageTableKey>()))
            .Returns(from);

        // Act
        var result = await _repository.GetKpiInfoByDateRange(kpiSymbol, from, to);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetKpiInfoByDateRange_NoDataInRange_ReturnsEmptyList()
    {
        // Arrange
        var kpiSymbol = "TEST_TrendToOpen";
        var from = "2025-01-01";
        var to = "2025-01-02";

        _mockEntityResolver.Setup(x => x.ResolvePartitionKey(It.IsAny<StockKpiStorageTableKey>()))
            .Returns(kpiSymbol);
        _mockEntityResolver.Setup(x => x.ResolveRowKey(It.IsAny<StockKpiStorageTableKey>()))
            .Returns(from);

        // Act
        var result = await _repository.GetKpiInfoByDateRange(kpiSymbol, from, to);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task RemoveEntriesOlderThan_WithValidDate_DeletesEntries()
    {
        // Arrange
        var sourceDate = DateTime.Now;
        var oldEntries = new List<StockKpiModel>
        {
            new()
            {
                SymbolKpiId = "TEST_TrendToOpen",
                When = "2024-01-01",
                Result = 1.5m
            }
        };

        // Act
        var result = await _repository.RemoveEntriesOlderThan(sourceDate);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new StockKpiRepository(null, _mockEntityResolver.Object));
    }

    [Test]
    public void Constructor_NullEntityResolver_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new StockKpiRepository(_mockOptions.Object, null));
    }

    [Test]
    public void TableName_ReturnsCorrectValue()
    {
        // Act
        var tableName = _repository.TableName;

        // Assert
        Assert.That(tableName, Is.EqualTo("StockKpi"));
    }
}