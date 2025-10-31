using Microsoft.Extensions.Logging;
using Moq;
using StockTracker.ExtractorFunction.Application.Features.KpiProcessMessage;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.UnitTests.Features.CleanupProcess;

[TestFixture]
public class CleanupProcessHandlerTests
{
    private Mock<IStockKpiCalculator> _mockKpiCalculator;
    private Mock<ILogger<CleanupProcessHandler>> _mockLogger;
    private CleanupProcessHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockKpiCalculator = new Mock<IStockKpiCalculator>();
        _mockLogger = new Mock<ILogger<CleanupProcessHandler>>();
        _handler = new CleanupProcessHandler(_mockKpiCalculator.Object, _mockLogger.Object);
    }

    [Test]
    public async Task Handle_ValidRequest_ReturnsTrue()
    {
        // Arrange
        var request = new CleanupProcessRequest
        {
            Symbol = "TEST",
            LimitDate = DateTime.Now.AddMonths(-3)
        };

        _mockKpiCalculator.Setup(x => x.CleanupDeprecatedInfo(request.LimitDate))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
        _mockKpiCalculator.Verify(x => x.CleanupDeprecatedInfo(request.LimitDate), Times.Once);
    }

    [Test]
    public async Task Handle_CleanupFails_ReturnsFalse()
    {
        // Arrange
        var request = new CleanupProcessRequest
        {
            Symbol = "TEST",
            LimitDate = DateTime.Now.AddMonths(-3)
        };

        _mockKpiCalculator.Setup(x => x.CleanupDeprecatedInfo(request.LimitDate))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task Handle_CalculatorThrowsException_PropagatesException()
    {
        // Arrange
        var request = new CleanupProcessRequest
        {
            Symbol = "TEST",
            LimitDate = DateTime.Now.AddMonths(-3)
        };

        _mockKpiCalculator.Setup(x => x.CleanupDeprecatedInfo(request.LimitDate))
            .ThrowsAsync(new Exception("Cleanup error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => 
            await _handler.Handle(request, CancellationToken.None));
    }

    [Test]
    public void Constructor_NullCalculator_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new CleanupProcessHandler(null, _mockLogger.Object));
    }

    [Test]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new CleanupProcessHandler(_mockKpiCalculator.Object, null));
    }
}