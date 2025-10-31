using Microsoft.Extensions.Logging;
using Moq;
using StockTracker.ExtractorFunction.Application.Features.GetTrackedCompanies;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.UnitTests.Features.GetTrackedCompanies;

[TestFixture]
public class GetTrackedCompaniesHandlerTests
{
    private Mock<IStockTracker> _mockStockTracker;
    private Mock<ILogger<GetTrackedCompaniesHandler>> _mockLogger;
    private GetTrackedCompaniesHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockStockTracker = new Mock<IStockTracker>();
        _mockLogger = new Mock<ILogger<GetTrackedCompaniesHandler>>();
        _handler = new GetTrackedCompaniesHandler(_mockStockTracker.Object, _mockLogger.Object);
    }

    [Test]
    public async Task Handle_WithEnabledCompanies_ReturnsTrackedCompaniesResponse()
    {
        // Arrange
        var request = new TrackedCompaniesRequest { Enabled = true };
        var persistedCompanies = new List<TrackedCompanyModel>
        {
            new()
            {
                Symbol = "TEST1",
                Name = "Test Company 1",
                Url = "http://test1.com",
                Enabled = true,
                PseudoRowKey = "key1"
            }
        };

        _mockStockTracker.Setup(x => x.GetTrackedCompanies(true))
            .ReturnsAsync(persistedCompanies);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        var companies = result.ToList();
        Assert.That(companies, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(companies[0].Symbol, Is.EqualTo("TEST1"));
            Assert.That(companies[0].Name, Is.EqualTo("Test Company 1"));
            Assert.That(companies[0].Url, Is.EqualTo("http://test1.com"));
            Assert.That(companies[0].Enabled, Is.True);
            Assert.That(companies[0].PseudoRowKey, Is.EqualTo("key1"));
        });
    }

    [Test]
    public async Task Handle_WithNoCompanies_ReturnsEmptyCollection()
    {
        // Arrange
        var request = new TrackedCompaniesRequest { Enabled = true };
        _mockStockTracker.Setup(x => x.GetTrackedCompanies(true))
            .ReturnsAsync(Enumerable.Empty<TrackedCompanyModel>());

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
            new GetTrackedCompaniesHandler(null, _mockLogger.Object));
    }

    [Test]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new GetTrackedCompaniesHandler(_mockStockTracker.Object, null));
    }
}