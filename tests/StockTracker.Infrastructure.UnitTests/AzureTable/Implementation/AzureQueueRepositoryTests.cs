using Microsoft.Extensions.Options;
using Moq;
using StockTracker.Infrastructure.AzureTable.Implementation;
using StockTracker.Models.ApiModels;

namespace StockTracker.Infrastructure.UnitTests.AzureTable.Implementation;

[TestFixture]
public class AzureQueueRepositoryTests
{
    private class TestRequest : IRequestContract
    {
        public string Id { get; set; }

        /// <inheritdoc />
        public string Symbol { get; set; }
    }

    private class TestQueueRepository : AzureQueueRepository<TestRequest>
    {
        public TestQueueRepository(IOptionsMonitor<AzureQueueOptions> options) : base(options)
        {
        }

        public override string QueueName => "testqueue";
    }

    private Mock<IOptionsMonitor<AzureQueueOptions>> _mockOptions;
    private TestQueueRepository _repository;

    [SetUp]
    public void Setup()
    {
        _mockOptions = new Mock<IOptionsMonitor<AzureQueueOptions>>();
        var options = new AzureQueueOptions
        {
            ConnectionString = "UseDevelopmentStorage=true"
        };
        _mockOptions.Setup(x => x.CurrentValue).Returns(options);

        _repository = new TestQueueRepository(_mockOptions.Object);
    }

    [Test]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TestQueueRepository(null));
    }

    [Test]
    public async Task InsertMessageAsync_WithValidEntity_ReturnsTrue()
    {
        // Arrange
        var entity = new TestRequest { Id = "test" };

        // Act
        var result = await _repository.InsertMessageAsync(entity);

        // Assert
        Assert.That(result, Is.True);
    }
}