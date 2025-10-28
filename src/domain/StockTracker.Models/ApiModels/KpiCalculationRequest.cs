using MediatR;

namespace StockTracker.Models.ApiModels;

public class KpiCalculationRequest : IRequest<bool>, IRequestContract
{
    public string Symbol { get; set; }
    public string CurrentDate { get; set; }

}