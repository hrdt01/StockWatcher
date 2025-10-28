using MediatR;

namespace StockTracker.Models.ApiModels;

public class CleanupProcessMessageRequest : IRequest<bool>, IRequestContract
{
    public string Symbol { get; set; }
    public DateTime CleanupLimitDate { get; set; }
}