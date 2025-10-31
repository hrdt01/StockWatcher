using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using Moq;
using StockTracker.CrossCutting.ExceptionHandling.RepositoryExceptions;
using StockTracker.Infrastructure.AzureTable.Implementation;
using System.Net;

namespace StockTracker.Infrastructure.UnitTests.AzureTable.Implementation;

[TestFixture]
public class AzureTableClientTests
{
    private Mock<IOptionsMonitor<AzureTableOptions>> _mockOptions;
    private AzureTableClient _client;
    private readonly string _tableName = "TestTable";

    [SetUp]
    public void Setup()
    {
        _mockOptions = new Mock<IOptionsMonitor<AzureTableOptions>>();
        var options = new AzureTableOptions
        {
            ConnectionString = "UseDevelopmentStorage=true"
        };
        _mockOptions.Setup(x => x.CurrentValue).Returns(options);

        _client = new AzureTableClient(_tableName, _mockOptions.Object);
    }

    [Test]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AzureTableClient(_tableName, null));
    }

    [Test]
    public void HandleException_OtherError_ReturnsOriginalException()
    {
        // Arrange
        var exception = new RequestFailedException((int)HttpStatusCode.InternalServerError, "InternalError");
        var entity = new TableEntity("partition", "row");

        // Act
        var result = _client.HandleException(exception, TableTransactionActionType.Add, entity);

        // Assert
        Assert.That(result, Is.EqualTo(exception));
    }

    [Test]
    public async Task GetByIdAsync_EntityNotFound_ThrowsEntityNotFoundException()
    {
        // Act & Assert
        Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await _client.GetByIdAsync<TableEntity>("partition", "row"));
    }

    [Test]
    public async Task ExistsAsync_EntityNotFound_ReturnsFalse()
    {
        // Act
        var result = await _client.ExistsAsync("partition", "row");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetFromPartitionAsync_ReturnsEmptyList()
    {
        // Act
        var result = await _client.GetFromPartitionAsync<TableEntity>("partition");

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetFromRowAsync_ReturnsEmptyList()
    {
        // Act
        var result = await _client.GetFromRowAsync<TableEntity>("row");

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetFromPartitionRowAsync_EntityNotFound_ThrowsEntityNotFoundException()
    {
        // Act & Assert
        Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await _client.GetFromPartitionRowAsync<TableEntity>("partition", "row"));
    }

    [Test]
    public async Task GetPartitionsAsync_ReturnsEmptyList()
    {
        // Act
        var result = await _client.GetPartitionsAsync<TableEntity>("");

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetByTimestampAsync_ReturnsEmptyList()
    {
        // Act
        var result = await _client.GetByTimestampAsync<TableEntity>(DateTime.Now);

        // Assert
        Assert.That(result, Is.Empty);
    }
}