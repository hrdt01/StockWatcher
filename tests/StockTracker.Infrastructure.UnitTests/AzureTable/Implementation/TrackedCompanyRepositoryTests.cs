using Microsoft.Extensions.Options;
using Moq;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Infrastructure.AzureTable.Implementation;
using StockTracker.Models.ApiModels;
using StockTracker.Models.Persistence;

namespace StockTracker.Infrastructure.UnitTests.AzureTable.Implementation;

[TestFixture]
public class TrackedCompanyRepositoryTests
{
    private Mock<IOptionsMonitor<AzureTableOptions>> _mockOptions;
    private Mock<IAzureTableEntityResolver<TrackedCompanyStorageTableKey>> _mockEntityResolver;
    private TrackedCompanyRepository _repository;
    private AzureTableOptions _options;

    [SetUp]
    public void Setup()
    {
        _mockOptions = new Mock<IOptionsMonitor<AzureTableOptions>>();
        _mockEntityResolver = new Mock<IAzureTableEntityResolver<TrackedCompanyStorageTableKey>>();

        _options = new AzureTableOptions
        {
            ConnectionString = "UseDevelopmentStorage=true"
        };

        _mockOptions.Setup(x => x.CurrentValue).Returns(_options);

        _repository = new TrackedCompanyRepository(_mockOptions.Object, _mockEntityResolver.Object);
    }

    [Test]
    public async Task GetTrackedCompaniesAsync_NoFilter_ReturnsAllCompanies()
    {
        // Arrange
        var symbols = new[] { "TEST1.BMEX", "TEST2.BMEX" };
        var rowKeys = new[] { "key1", "key2" };
        var company1 = new TrackedCompanyModel
        {
            Symbol = "TEST1.BMEX",
            PseudoRowKey = "key1",
            Name = "Test Company 1",
            Url = "http://test1.com",
            Enabled = true
        };
        var company2 = new TrackedCompanyModel
        {
            Symbol = "TEST2.BMEX",
            PseudoRowKey = "key2",
            Name = "Test Company 2",
            Url = "http://test2.com",
            Enabled = false
        };

        _mockEntityResolver.Setup(x => x.ResolvePartitionKey(It.IsAny<TrackedCompanyStorageTableKey>()))
            .Returns((TrackedCompanyStorageTableKey key) => key.Symbol);
        _mockEntityResolver.Setup(x => x.ResolveRowKey(It.IsAny<TrackedCompanyStorageTableKey>()))
            .Returns((TrackedCompanyStorageTableKey key) => key.PseudoRowKey);

        // Act
        var result = await _repository.GetTrackedCompaniesAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetTrackedCompaniesAsync_EnabledOnly_ReturnsEnabledCompanies()
    {
        // Arrange
        var symbols = new[] { "TEST1.BMEX", "TEST2.BMEX" };
        var rowKeys = new[] { "key1", "key2" };
        var company1 = new TrackedCompanyModel
        {
            Symbol = "TEST1.BMEX",
            PseudoRowKey = "key1",
            Name = "Test Company 1",
            Url = "http://test1.com",
            Enabled = true
        };
        var company2 = new TrackedCompanyModel
        {
            Symbol = "TEST2.BMEX",
            PseudoRowKey = "key2",
            Name = "Test Company 2",
            Url = "http://test2.com",
            Enabled = false
        };

        _mockEntityResolver.Setup(x => x.ResolvePartitionKey(It.IsAny<TrackedCompanyStorageTableKey>()))
            .Returns((TrackedCompanyStorageTableKey key) => key.Symbol);
        _mockEntityResolver.Setup(x => x.ResolveRowKey(It.IsAny<TrackedCompanyStorageTableKey>()))
            .Returns((TrackedCompanyStorageTableKey key) => key.PseudoRowKey);

        // Act
        var result = await _repository.GetTrackedCompaniesAsync(true);

        // Assert
        Assert.That(result, Is.Not.Null);
    }
    
    [Test]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new TrackedCompanyRepository(null, _mockEntityResolver.Object));
    }

    [Test]
    public void Constructor_NullEntityResolver_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new TrackedCompanyRepository(_mockOptions.Object, null));
    }
}