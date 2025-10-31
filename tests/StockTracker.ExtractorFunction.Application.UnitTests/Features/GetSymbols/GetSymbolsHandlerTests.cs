using Microsoft.Extensions.Logging;
using Moq;
using StockTracker.ExtractorFunction.Application.Features.GetSymbols;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.UnitTests.Features.GetSymbols;

[TestFixture]
public class GetSymbolsHandlerTests
{
    private Mock<IStockTracker> _mockStockTracker;
    private Mock<ILogger<GetSymbolsHandler>> _mockLogger;
    private GetSymbolsHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockStockTracker = new Mock<IStockTracker>();
        _mockLogger = new Mock<ILogger<GetSymbolsHandler>>();
        _handler = new GetSymbolsHandler(_mockStockTracker.Object, _mockLogger.Object);
    }

    [Test]
    public async Task Handle_ReturnsSymbols()
    {
        // Arrange
        var expectedSymbols = new[] { "TEST1", "TEST2" };
        var request = new SymbolsRequest();

        _mockStockTracker.Setup(x => x.GetAllSymbolsAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedSymbols);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedSymbols));
        _mockStockTracker.Verify(x => x.GetAllSymbolsAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task Handle_EmptyResult_ReturnsEmptyCollection()
    {
        // Arrange
        var request = new SymbolsRequest();

        _mockStockTracker.Setup(x => x.GetAllSymbolsAsync(It.IsAny<string>()))
            .ReturnsAsync(Array.Empty<string>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Constructor_NullStockTracker_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new GetSymbolsHandler(null, _mockLogger.Object));
    }

    [Test]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new GetSymbolsHandler(_mockStockTracker.Object, null));
    }
}