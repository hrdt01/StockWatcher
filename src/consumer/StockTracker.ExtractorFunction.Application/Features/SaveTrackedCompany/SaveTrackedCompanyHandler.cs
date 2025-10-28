using MediatR;
using Microsoft.Extensions.Logging;
using StockTracker.CrossCutting.Utils;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.SaveTrackedCompany;

public class SaveTrackedCompanyHandler : IRequestHandler<SaveTrackedCompanyRequest, bool>
{
    private readonly IStockTracker _stockTracker;
    private readonly IKpiProcessorMessageBroker _kpiProcessorMessageBroker;
    private readonly ILogger<SaveTrackedCompanyHandler> _logger;
    private readonly IPublisher _publisher;
    public SaveTrackedCompanyHandler(
        IStockTracker stockTracker,
        IKpiProcessorMessageBroker kpiProcessorMessageBroker,
        ILogger<SaveTrackedCompanyHandler> logger, IPublisher publisher)
    {
        _stockTracker = stockTracker;
        _kpiProcessorMessageBroker = kpiProcessorMessageBroker;
        _logger = logger;
        _publisher = publisher;
    }
    public async Task<bool> Handle(SaveTrackedCompanyRequest request, CancellationToken cancellationToken)
    {
        bool trackedCompanyAlreadyExist = await _stockTracker.CheckExistingCompany(request.Symbol);
        var modelToSave = new TrackedCompanyModel()
        {
            Enabled = request.Enabled,
            Name = request.Name,
            PseudoRowKey = request.PseudoRowKey,
            Symbol = request.Symbol,
            Url = request.Url
        };

        var result = await _stockTracker.SaveTrackedCompany(modelToSave);
        if (result && !trackedCompanyAlreadyExist)
        {
            await GetPreviousWeekStockInfo(request.Symbol);
        }

        await _publisher.Publish(new SaveTrackedCompanyEvent(), cancellationToken);

        return result;
    }

    private async Task GetPreviousWeekStockInfo(string tickerSymbol)
    {

        var previousWeekDays = DateTime.UtcNow.GetPreviousWeek();
        foreach (var day in previousWeekDays)
        {
            var info = await _stockTracker.GetEndOfDayBySymbol(tickerSymbol, day);
            var persistedInfo = await _stockTracker.ConsolidateEndOfDayInfo(info, tickerSymbol, day);
            if (persistedInfo)
            {
                var processorRequest = new KpiProcessMessageRequest
                { Symbol = tickerSymbol, ProcessDate = day };

                _ = await _kpiProcessorMessageBroker.CreateMessageRequestAsync(processorRequest);
            }
        }
    }
}