using FluentValidation.Results;
using FluentValidation;
using MediatR;
using StockTracker.CrossCutting.ExceptionHandling;
using StockTracker.CrossCutting.ExceptionHandling.CustomExceptions;

namespace StockTracker.ExtractorFunction.Application.Features.Behavior;

/// <summary>
/// Class to centralize validations performed by MediatR handlers
/// </summary>
/// <typeparam name="TRequest">Request processed by MediatR handlers</typeparam>
/// <typeparam name="TResponse">Response offered by MediatR handlers</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly bool _hasValidators;
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
        _hasValidators = _validators.Any();
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_hasValidators)
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        ValidationResult[] validationResults =
            await Task.WhenAll(_validators.Select(v =>
                v.ValidateAsync(context, cancellationToken)));

        List<ValidationFailure> failures =
            validationResults.SelectMany(r => r.Errors)
                .Where(f => f != null).ToList();

        if (!failures.Any())
        {
            return await next();
        }
        throw new RequestValidationException("One or more validation errors ocurred", failures.AddValidationFailureToExtensions());
    }
}

