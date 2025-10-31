using Microsoft.Extensions.Logging;
using Moq;
using StockTracker.ExtractorFunction.Application.Features.KpiProcessMessage;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.UnitTests.Features.CleanupProcessMessage;

[TestFixture]
public class CleanupProcessMessageHandlerTests
{
    private Mock<ICleanupProcessorMessageBroker> _mockMessageBroker;
    private Mock<ILogger<CleanupProcessMessageHandler>> _mockLogger;
    private CleanupProcessMessageHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockMessageBroker = new Mock<ICleanupProcessorMessageBroker>();
        _mockLogger = new Mock<ILogger<CleanupProcessMessageHandler>>();
        _handler = new CleanupProcessMessageHandler(_mockMessageBroker.Object, _mockLogger.Object);
    }

    [Test]
    public async Task Handle_ValidRequest_ReturnsTrue()
    {
        // Arrange
        var request = new CleanupProcessMessageRequest
        {
            Symbol = "TEST",
            CleanupLimitDate = DateTime.Now.AddMonths(-3)
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
        var request = new CleanupProcessMessageRequest
        {
            Symbol = "TEST",
            CleanupLimitDate = DateTime.Now.AddMonths(-3)
        };

        _mockMessageBroker.Setup(x => x.CreateMessageRequestAsync(request))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task Handle_BrokerThrowsException_PropagatesException()
    {
        // Arrange
        var request = new CleanupProcessMessageRequest
        {
            Symbol = "TEST",
            CleanupLimitDate = DateTime.Now.AddMonths(-3)
        };

        _mockMessageBroker.Setup(x => x.CreateMessageRequestAsync(request))
            .ThrowsAsync(new Exception("Broker error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => 
            await _handler.Handle(request, CancellationToken.None));
    }

    [Test]
    public void Constructor_NullMessageBroker_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new CleanupProcessMessageHandler(null, _mockLogger.Object));
    }

    [Test]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new CleanupProcessMessageHandler(_mockMessageBroker.Object, null));
    }
}