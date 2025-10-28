using FluentValidation.Results;

namespace StockTracker.CrossCutting.ExceptionHandling;

public static class AddExtensionsToProblemDetails
{
    public static Dictionary<string, List<string>> AddValidationFailureToExtensions(this List<ValidationFailure> errors)
    {
        return errors
            .GroupBy(key => key.PropertyName)
            .ToDictionary(x => x.Key,
                x => x.Select(y => y.ErrorMessage)
                    .ToList());
    }
}