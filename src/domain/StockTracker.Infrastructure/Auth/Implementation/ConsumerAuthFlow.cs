using Microsoft.Extensions.Configuration;
using StockTracker.CrossCutting.ExceptionHandling.CustomExceptions;
using StockTracker.Infrastructure.Auth.Definition;

namespace StockTracker.Infrastructure.Auth.Implementation;

public class ConsumerAuthFlow : IConsumerAuthFlow
{

    /// <inheritdoc />
    public void CheckAuthentication(string authCode, IConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(authCode))
            throw new UnauthorizedException("No auth key provided");

        var storedAuthKey = configuration["OpenApi:ApiKey"];

        if (string.IsNullOrWhiteSpace(storedAuthKey))
            throw new UnauthorizedException("No auth key provided");

        if(!storedAuthKey.Equals(authCode, StringComparison.InvariantCultureIgnoreCase))
            throw new UnauthorizedException("No auth key provided");
    }
}