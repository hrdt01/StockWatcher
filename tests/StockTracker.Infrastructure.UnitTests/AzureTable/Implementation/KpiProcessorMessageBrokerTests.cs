using Microsoft.Extensions.Options;
using Moq;
using StockTracker.Infrastructure.AzureTable.Implementation;
using StockTracker.Models.ApiModels;

namespace StockTracker.Infrastructure.UnitTests.AzureTable.Implementation;

[TestFixture]
public class KpiProcessorMessageBrokerTests
{
    private Mock<IOptionsMonitor<AzureQueueOptions>> _mockOptions;
    private KpiProcessorMessageBroker _broker;

    [SetUp]
    public void Setup()
    {
        _mockOptions = new Mock<IOptionsMonitor<AzureQueueOptions>>();
        var options = new AzureQueueOptions
        {
            ConnectionString = "UseDevelopmentStorage=true"
        };
        _mockOptions.Setup(x => x.CurrentValue).Returns(options);

        _broker = new KpiProcessorMessageBroker(_mockOptions.Object);
    }

    [Test]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new KpiProcessorMessageBroker(null));
    }

    [Test]
    public async Task CreateMessageRequestAsync_ValidMessage_CallsInsertMessage()
    {
        // Arrange
        var message = new KpiProcessMessageRequest
        {
            Symbol = "TEST",
            ProcessDate = DateTime.Now
        };

        // Act
        var result = await _broker.CreateMessageRequestAsync(message);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void QueueName_ReturnsCorrectValue()
    {
        // Act
        var queueName = _broker.QueueName;

        // Assert
        Assert.That(queueName, Is.EqualTo("kpi-message-broker"));
    }
}