using System.Text.Json;
using Azure.Storage.Queues.Models;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using StockTracker.CrossCutting.Constants;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction;

public class CleanupProcessor
{
    private readonly IMediator _mediator;
    private readonly ILogger<CleanupProcessor> _logger;

    public CleanupProcessor(
        IMediator mediator, 
        ILogger<CleanupProcessor> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Function(nameof(CleanupProcessor))]
    public async Task Run(
        [QueueTrigger(
            queueName: GlobalConstants.CleanupProcessorQueueName,
            Connection = $"{GlobalConstants.AzureQueueRepositorySettingsSectionKeyName}:ConnectionString")]
        QueueMessage message)
    {
        var requestBody = await
            JsonSerializer.DeserializeAsync<CleanupProcessMessageRequest>(message.Body.ToStream());
        var targetDate = requestBody!.CleanupLimitDate;

        _logger.LogInformation($"Starting to process cleaning up deprecated info prior to: {targetDate}");
        var cleanupRequest = new CleanupProcessRequest()
        {
            Symbol = "test",
            LimitDate = targetDate
        };

        var result = await _mediator.Send(cleanupRequest);

        _logger.LogInformation($"Ending to process cleaning up deprecated info prior to: {targetDate} --> {result}");

    }
}