using MediatR;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.IniitalMigration;

public class InitialMigrationHandler : IRequestHandler<InitialMigrationRequest, bool>
{
    private readonly IStockTracker _stockTracker;

    public InitialMigrationHandler(
        IStockTracker stockTracker)
    {
        _stockTracker = stockTracker;
    }
    public async Task<bool> Handle(InitialMigrationRequest request, CancellationToken cancellationToken)
    {
        return await _stockTracker.ExecuteTrackedCompaniesInitialMigration();
    }
}