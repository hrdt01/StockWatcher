using Microsoft.Extensions.Options;
using Moq;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Infrastructure.AzureTable.Implementation;
using StockTracker.Models.Persistence;

namespace StockTracker.Infrastructure.UnitTests.AzureTable.Implementation;

[TestFixture]
public class StockInfoRepositoryTests
{
    private Mock<IOptionsMonitor<AzureTableOptions>> _mockOptions;
    private Mock<IAzureTableEntityResolver<StockInfoStorageTableKey>> _mockEntityResolver;
    private StockInfoRepository _repository;
    private AzureTableOptions _options;

    [SetUp]
    public void Setup()
    {
        _mockOptions = new Mock<IOptionsMonitor<AzureTableOptions>>();
        _mockEntityResolver = new Mock<IAzureTableEntityResolver<StockInfoStorageTableKey>>();

        _options = new AzureTableOptions
        {
            ConnectionString = "UseDevelopmentStorage=true"
        };

        _mockOptions.Setup(x => x.CurrentValue).Returns(_options);

        _repository = new StockInfoRepository(_mockOptions.Object, _mockEntityResolver.Object);
    }

    [Test]
    public async Task ExistsAsync_WithValidModel_CallsBaseExists()
    {
        // Arrange
        var model = new StockInfoModel
        {
            Symbol = "TEST",
            When = "2025-01-01",
            Open = 100,
            Close = 110,
            High = 115,
            Low = 95
        };

        _mockEntityResolver.Setup(x => x.ResolvePartitionKey(It.IsAny<StockInfoStorageTableKey>()))
            .Returns("TEST");
        _mockEntityResolver.Setup(x => x.ResolveRowKey(It.IsAny<StockInfoStorageTableKey>()))
            .Returns("2025-01-01");

        // Act
        await _repository.ExistsAsync(model);

        // Assert
        _mockEntityResolver.Verify(x => x.ResolvePartitionKey(It.IsAny<StockInfoStorageTableKey>()), Times.Once);
        _mockEntityResolver.Verify(x => x.ResolveRowKey(It.IsAny<StockInfoStorageTableKey>()), Times.Once);
    }

    [Test]
    public async Task GetAllSymbolsAsync_CallsGetAllPartitions()
    {
        // Arrange
        var searchPattern = "BMEX";
        var expectedSymbols = new[] { "TEST1.BMEX", "TEST2.BMEX" };

        // Act
        var result = await _repository.GetAllSymbolsAsync(searchPattern);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetDatesBySymbolAsync_CallsGetRowKeys()
    {
        // Arrange
        var symbol = "TEST.BMEX";
        var expectedDates = new[] { "2025-01-01", "2025-01-02" };

        // Act
        var result = await _repository.GetDatesBySymbolAsync(symbol);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetStockInfoByDateRange_WithValidRange_ReturnsStockInfo()
    {
        // Arrange
        var symbol = "TEST";
        var from = "2025-01-01";
        var to = "2025-01-02";

        var stockInfo = new StockInfoModel
        {
            Symbol = symbol,
            When = from,
            Open = 100,
            Close = 110,
            High = 115,
            Low = 95
        };

        _mockEntityResolver.Setup(x => x.ResolvePartitionKey(It.IsAny<StockInfoStorageTableKey>()))
            .Returns(symbol);
        _mockEntityResolver.Setup(x => x.ResolveRowKey(It.IsAny<StockInfoStorageTableKey>()))
            .Returns(from);

        // Act
        var result = await _repository.GetStockInfoByDateRange(symbol, from, to);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task RemoveEntriesOlderThan_WithValidDate_DeletesOldEntries()
    {
        // Arrange
        var sourceDate = DateTime.Now;
        var oldEntries = new List<StockInfoModel>
        {
            new()
            {
                Symbol = "TEST",
                When = "2024-01-01",
                Open = 100,
                Close = 110,
                High = 115,
                Low = 95
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
            new StockInfoRepository(null, _mockEntityResolver.Object));
    }

    [Test]
    public void Constructor_NullEntityResolver_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new StockInfoRepository(_mockOptions.Object, null));
    }
}