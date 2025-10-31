using Microsoft.Extensions.Logging;
using Moq;
using StockTracker.ExtractorFunction.Application.Features.GetDatesBySymbol;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.UnitTests.Features.GetDatesBySymbol;

[TestFixture]
public class GetDatesBySymbolHandlerTests
{
    private Mock<IStockTracker> _mockStockTracker;
    private Mock<ILogger<GetDatesBySymbolHandler>> _mockLogger;
    private GetDatesBySymbolHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockStockTracker = new Mock<IStockTracker>();
        _mockLogger = new Mock<ILogger<GetDatesBySymbolHandler>>();
        _handler = new GetDatesBySymbolHandler(_mockStockTracker.Object, _mockLogger.Object);
    }

    [Test]
    public async Task Handle_ValidSymbol_ReturnsDates()
    {
        // Arrange
        var symbol = "TEST";
        var expectedDates = new[] { "2025-01-01", "2025-01-02" };
        var request = new DatesBySymbolRequest { Symbol = symbol };

        _mockStockTracker.Setup(x => x.GetDatesBySymbolAsync(symbol))
            .ReturnsAsync(expectedDates);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedDates));
        _mockStockTracker.Verify(x => x.GetDatesBySymbolAsync(symbol), Times.Once);
    }

    [Test]
    public async Task Handle_NoDataForSymbol_ReturnsEmptyCollection()
    {
        // Arrange
        var symbol = "TEST";
        var request = new DatesBySymbolRequest { Symbol = symbol };

        _mockStockTracker.Setup(x => x.GetDatesBySymbolAsync(symbol))
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
            new GetDatesBySymbolHandler(null, _mockLogger.Object));
    }

    [Test]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new GetDatesBySymbolHandler(_mockStockTracker.Object, null));
    }
}