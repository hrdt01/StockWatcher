using MediatR;

namespace StockTracker.Models.ApiModels;

public class CleanupProcessRequest : IRequest<bool>, IRequestContract
{
    public string Symbol { get; set; }
    public DateTime LimitDate { get; set; }
}