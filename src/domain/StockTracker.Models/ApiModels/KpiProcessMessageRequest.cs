using MediatR;

namespace StockTracker.Models.ApiModels;

public class KpiProcessMessageRequest : IRequest<bool>, IRequestContract
{
    public string Symbol { get; set; }
    public DateTime ProcessDate { get; set; }
}