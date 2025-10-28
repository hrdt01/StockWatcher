using Microsoft.Extensions.Configuration;

namespace StockTracker.Infrastructure.Auth.Definition;

public interface IConsumerAuthFlow
{
    /// <summary>
    /// Check if <param name="authCode"></param> is supplied and if auth code is valid against the value stored in <param name="configuration"></param>
    /// </summary>
    /// <param name="authCode"></param>
    /// <param name="configuration"></param>
    void CheckAuthentication(string authCode, IConfiguration configuration);
}