using MediatR;

namespace StockTracker.Models.ApiModels.Contracts.Definition;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}