using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Polly.Registry;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.MarketStack.Services.Models;
using StockTracker.Models.ApiModels;
using StockTracker.Models.Persistence;
using Assert = NUnit.Framework.Assert;

namespace StockTracker.MarketStack.Services.UnitTests.Contracts.Implementation
{
    [TestFixture]
    public class StockTrackerTests
    {
        private Mock<IHttpClientFactory> _mockClientFactory;
        private Mock<IStockInfoRepository> _mockStockInfoRepository;
        private Mock<IAzureTableEntityResolver<StockInfoStorageTableKey>> _mockTableEntityResolver;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<ITrackedCompanyRepository> _mockTrackedCompanyRepository;
        private Mock<ILogger<MarketStack.Services.Contracts.Implementation.StockTracker>> _mockLogger;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private Mock<ResiliencePipelineProvider<string>> _mockResiliencePipelineProvider;
        private MarketStack.Services.Contracts.Implementation.StockTracker _stockTracker;


        [SetUp]
        public void Setup()
        {
            _mockClientFactory = new Mock<IHttpClientFactory>();
            _mockStockInfoRepository = new Mock<IStockInfoRepository>();
            _mockTableEntityResolver = new Mock<IAzureTableEntityResolver<StockInfoStorageTableKey>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockTrackedCompanyRepository = new Mock<ITrackedCompanyRepository>();
            _mockLogger = new Mock<ILogger<Services.Contracts.Implementation.StockTracker>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockResiliencePipelineProvider = new Mock<ResiliencePipelineProvider<string>>();

            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(x => x.Value).Returns("test-access-key");
            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(configurationSection.Object);

            _stockTracker = new Services.Contracts.Implementation.StockTracker(
                _mockClientFactory.Object,
                _mockStockInfoRepository.Object,
                _mockTableEntityResolver.Object,
                _mockConfiguration.Object,
                _mockTrackedCompanyRepository.Object,
                _mockResiliencePipelineProvider.Object,
                _mockLogger.Object
            );
        }
        
        [Test]
        public async Task ConsolidateEndOfDayInfo_EmptyData_ReturnsFalse()
        {
            // Arrange
            var sourceData = new EndOfDayResponse { data = new List<EndOfDaySymbol>() };
            var symbol = "TEST";
            var targetDate = DateTime.Now;

            // Act
            var result = await _stockTracker.ConsolidateEndOfDayInfo(sourceData, symbol, targetDate);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void BuildEndOfDayEndpointRequest_ValidInput_ReturnsCorrectUrl()
        {
            // Arrange
            var accessKey = "test-key";
            var targetDate = new DateTime(2025, 1, 1);

            // Act
            var result = _stockTracker.BuildEndOfDayEndpointRequest(accessKey, targetDate);

            // Assert
            Assert.That(result, Does.Contain("2025-01-01"));
            Assert.That(result, Does.Contain(accessKey));
        }

        [Test]
        public async Task GetAllSymbolsAsync_SuccessfulResponse_ReturnsSymbols()
        {
            // Arrange
            var expectedSymbols = new[] { "TEST1", "TEST2" };
            _mockStockInfoRepository.Setup(x => x.GetAllSymbolsAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedSymbols);

            // Act
            var result = await _stockTracker.GetAllSymbolsAsync();

            // Assert
            Assert.That(result, Is.EqualTo(expectedSymbols));
        }

        [Test]
        public async Task GetDatesBySymbolAsync_SuccessfulResponse_ReturnsDates()
        {
            // Arrange
            var symbol = "TEST";
            var expectedDates = new[] { "2025-01-01", "2025-01-02" };
            _mockStockInfoRepository.Setup(x => x.GetDatesBySymbolAsync(symbol))
                .ReturnsAsync(expectedDates);

            // Act
            var result = await _stockTracker.GetDatesBySymbolAsync(symbol);

            // Assert
            Assert.That(result, Is.EqualTo(expectedDates));
        }

        [Test]
        public async Task GetStockInfoByDateRange_SuccessfulResponse_ReturnsStockInfo()
        {
            // Arrange
            var symbol = "TEST";
            var from = "2025-01-01";
            var to = "2025-01-02";
            var stockInfoModels = new List<StockInfoModel>
            {
                new()
                {
                    High = Convert.ToDecimal("105.0"),
                    Low = Convert.ToDecimal("95.0"),
                    Open = Convert.ToDecimal("98.0"),
                    Close = Convert.ToDecimal("100.0"),
                    When = "2025-01-01"
                }
            };

            _mockStockInfoRepository.Setup(x => x.GetStockInfoByDateRange(symbol, from, to))
                .ReturnsAsync(stockInfoModels);

            // Act
            var result = await _stockTracker.GetStockInfoByDateRange(symbol, from, to);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TickerSymbol, Is.EqualTo(symbol));
            Assert.That(result.TradeEvents, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task GetTrackedCompanies_SuccessfulResponse_ReturnsCompanies()
        {
            // Arrange
            var expectedCompanies = new List<TrackedCompanyModel>
        {
            new TrackedCompanyModel
            {
                Symbol = "TEST",
                Name = "Test Company",
                Enabled = true
            }
        };

            _mockTrackedCompanyRepository.Setup(x => x.GetTrackedCompaniesAsync(It.IsAny<bool>()))
                .ReturnsAsync(expectedCompanies);

            // Act
            var result = await _stockTracker.GetTrackedCompanies();

            // Assert
            Assert.That(result, Is.EqualTo(expectedCompanies));
        }

        [Test]
        public async Task ExecuteTrackedCompaniesInitialMigration_Success_ReturnsTrue()
        {
            // Arrange
            _mockTrackedCompanyRepository.Setup(x => x.GetAllPartitionsAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<string>());
            _mockTrackedCompanyRepository.Setup(x => x.ExistsAsync(It.IsAny<TrackedCompanyStorageTableKey>()))
                .ReturnsAsync(false);
            _mockTrackedCompanyRepository.Setup(x => x.CreateAsync(It.IsAny<TrackedCompanyModel>()))
                .ReturnsAsync(true);

            // Act
            var result = await _stockTracker.ExecuteTrackedCompaniesInitialMigration();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task SaveTrackedCompany_NewCompany_ReturnsTrue()
        {
            // Arrange
            var company = new TrackedCompanyModel
            {
                Symbol = "TEST",
                PseudoRowKey = Guid.NewGuid().ToString("N")
            };

            _mockTrackedCompanyRepository.Setup(x => x.ExistsAsync(It.IsAny<TrackedCompanyStorageTableKey>()))
                .ReturnsAsync(false);
            _mockTrackedCompanyRepository.Setup(x => x.CreateAsync(It.IsAny<TrackedCompanyModel>()))
                .ReturnsAsync(true);

            // Act
            var result = await _stockTracker.SaveTrackedCompany(company);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CheckExistingCompany_ExistingSymbol_ReturnsTrue()
        {
            // Arrange
            var symbol = "TEST";
            var partitions = new List<string> { symbol };
            _mockTrackedCompanyRepository.Setup(x => x.GetAllPartitionsAsync(string.Empty))
                .ReturnsAsync(partitions);

            // Act
            var result = await _stockTracker.CheckExistingCompany(symbol);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Constructor_NullDependencies_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MarketStack.Services.Contracts.Implementation.StockTracker(
                null,
                _mockStockInfoRepository.Object,
                _mockTableEntityResolver.Object,
                _mockConfiguration.Object,
                _mockTrackedCompanyRepository.Object,
                _mockResiliencePipelineProvider.Object,
                _mockLogger.Object));
        }
    }
}