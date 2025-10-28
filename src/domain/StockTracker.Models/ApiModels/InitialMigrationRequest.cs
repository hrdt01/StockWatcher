using MediatR;

namespace StockTracker.Models.ApiModels;

public class InitialMigrationRequest : IRequest<bool>
{ }