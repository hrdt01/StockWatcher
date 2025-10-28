using MediatR;

namespace StockTracker.Models.ApiModels;

public class SaveTrackedCompanyRequest : IRequest<bool>
{
    public string Name { get; set; }
    public string Symbol { get; set; }
    public string Url { get; set; }
    public bool Enabled { get; set; }
    public string PseudoRowKey { get; set; }
}