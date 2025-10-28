using MediatR;

namespace StockTracker.Models.ApiModels;

public class KpiDatesBySymbolRequest : IRequest<IEnumerable<string>>, IRequestContract
{
    public string Symbol { get; set; }
}