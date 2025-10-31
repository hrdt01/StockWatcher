using Microsoft.Extensions.Logging;
using Moq;
using StockTracker.ExtractorFunction.Application.Features.KpiProcessMessage;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.UnitTests.Features.KpiProcessMessage;

[TestFixture]
public class KpiProcessMessageHandlerTests
{
    private Mock<IKpiProcessorMessageBroker> _mockMessageBroker;
    private Mock<ILogger<KpiProcessMessageHandler>> _mockLogger;
    private KpiProcessMessageHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockMessageBroker = new Mock<IKpiProcessorMessageBroker>();
        _mockLogger = new Mock<ILogger<KpiProcessMessageHandler>>();
        _handler = new KpiProcessMessageHandler(_mockMessageBroker.Object, _mockLogger.Object);
    }

    [Test]
    public async Task Handle_ValidRequest_ReturnsTrue()
    {
        // Arrange
        var request = new KpiProcessMessageRequest
        {
            Symbol = "TEST",
            ProcessDate = DateTime.Now
        };

        _mockMessageBroker.Setup(x => x.CreateMessageRequestAsync(request))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
        _mockMessageBroker.Verify(x => x.CreateMessageRequestAsync(request), Times.Once);
    }

    [Test]
    public async Task Handle_BrokerFailure_ReturnsFalse()
    {
        // Arrange
        var request = new KpiProcessMessageRequest
        {
            Symbol = "TEST",
            ProcessDate = DateTime.Now
        };

        _mockMessageBroker.Setup(x => x.CreateMessageRequestAsync(request))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Constructor_NullMessageBroker_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new KpiProcessMessageHandler(null, _mockLogger.Object));
    }

    [Test]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new KpiProcessMessageHandler(_mockMessageBroker.Object, null));
    }
}