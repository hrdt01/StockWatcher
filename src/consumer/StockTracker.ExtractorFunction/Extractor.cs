using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StockTracker.CrossCutting.Constants;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction
{
    public class Extractor
    {
        private readonly IConfiguration _configuration;
        private readonly IStockTracker _stockTracker;
        private readonly IKpiProcessorMessageBroker _kpiProcessorMessageBroker;
        private readonly ICleanupProcessorMessageBroker _cleanupProcessorMessageBroker;
        private readonly ILogger<Extractor> _logger;
        private IEnumerable<string> symbolsToQuery;
        private readonly int _deprecatedInfoAntiquityInMonths = -3;
        public Extractor(
            IConfiguration configuration,
            IStockTracker stockTracker,
            IKpiProcessorMessageBroker kpiProcessorMessageBroker,
            ICleanupProcessorMessageBroker cleanupProcessorMessageBroker,
            ILoggerFactory loggerFactory
            )
        {
            _configuration = configuration;
            _stockTracker = stockTracker;
            _kpiProcessorMessageBroker = kpiProcessorMessageBroker;
            _cleanupProcessorMessageBroker = cleanupProcessorMessageBroker;
            _logger = loggerFactory.CreateLogger<Extractor>();
        }

        [Function("Extractor")]
        public async Task Run([TimerTrigger("0 0 8 * * 2-6")] TimerInfo myTimer)
        {
            _logger.LogInformation($"Executing Extractor function at: {DateTime.UtcNow}");

            symbolsToQuery = await GetTickersToTrack();
            var delayToApply = _configuration.GetValue<int>(ExtractorFunctionConstants.QueryDelaySettingName);
            _logger.LogInformation($"Ready to process these symbols: {string.Join(",", symbolsToQuery)}");
            foreach (var symbol in symbolsToQuery)
            {
                _logger.LogInformation($"Starting to process symbol: {symbol} at: {DateTime.UtcNow.Date}");

                var dateToProcess = DateTime.UtcNow.AddDays(delayToApply);
                var result = await ProcessSymbol(symbol, dateToProcess);
                _logger.LogInformation($"Success processing {symbol} at: {DateTime.UtcNow.Date} --> {result}");
                if (!result) continue;

                var toBroker = await SendRequestToMessageBroker(symbol, dateToProcess);
                _logger.LogInformation($"Message sent to broker: {toBroker}");
            }

            var cleanup = _configuration.GetValue<bool>(ExtractorFunctionConstants.CleanupDeprecatedInfo);
            if (cleanup)
            {
                var toCleanupBroker = 
                    await SendRequestToCleanupMessageBroker(DateTime.UtcNow.AddMonths(_deprecatedInfoAntiquityInMonths));
                _logger.LogInformation($"Message sent to cleanup broker: {toCleanupBroker}");
            }

            _logger.LogInformation($"Extractor function executed at: {DateTime.UtcNow}");
        }

        private async Task<bool> SendRequestToMessageBroker(string symbol, DateTime dateToProcess)
        {
            var processorRequest = new KpiProcessMessageRequest()
                { Symbol = symbol, ProcessDate = dateToProcess };

            return await _kpiProcessorMessageBroker.CreateMessageRequestAsync(processorRequest);
        }
        private async Task<bool> SendRequestToCleanupMessageBroker(DateTime dateToProcess)
        {
            var processorRequest = new CleanupProcessMessageRequest()
                { Symbol = string.Empty, CleanupLimitDate = dateToProcess };

            return await _cleanupProcessorMessageBroker.CreateMessageRequestAsync(processorRequest);
        }

        private async Task<bool> ProcessSymbol(string symbol, DateTime targetTime)
        {
            var info = await _stockTracker.GetEndOfDayBySymbol(symbol, targetTime);
            var result = await _stockTracker.ConsolidateEndOfDayInfo(info, symbol, targetTime);
            return result;
        }

        private async Task<IEnumerable<string>> GetTickersToTrack()
        {
            var result = await _stockTracker.GetTrackedCompanies(true);
            return result.Select(company => company.Symbol);
        }
    }
}
