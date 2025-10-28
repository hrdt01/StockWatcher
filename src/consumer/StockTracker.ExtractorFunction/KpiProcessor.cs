using Azure.Storage.Queues.Models;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using StockTracker.CrossCutting.Constants;
using StockTracker.CrossCutting.Utils;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction
{
    public class KpiProcessor
    {
        private readonly IMediator _mediator;
        private readonly ILogger<KpiProcessor> _logger;

        public KpiProcessor(
            IMediator mediator,
            ILogger<KpiProcessor> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        [Function(nameof(KpiProcessor))]
        public async Task Run(
            [QueueTrigger(
                queueName: GlobalConstants.KpiProcessorQueueName, 
                Connection = $"{GlobalConstants.AzureQueueRepositorySettingsSectionKeyName}:ConnectionString")]
            QueueMessage message)
        {
            var requestBody = await
                System.Text.Json.JsonSerializer.DeserializeAsync<KpiProcessMessageRequest>(message.Body.ToStream());
            var symbol = requestBody.Symbol;
            var targetDate = requestBody.ProcessDate.ToRowKeyFormat();

            _logger.LogInformation($"Starting to process Kpi calculation for: {symbol} and date: {targetDate}");
            
            var kpiCalculationRequest = new KpiCalculationRequest()
            {
                CurrentDate = targetDate,
                Symbol = symbol
            };

            var result = await _mediator.Send(kpiCalculationRequest);

            _logger.LogInformation($"Ending to process Kpi calculation for: {symbol} and date: {targetDate} --> {result}");
        }
    }
}
