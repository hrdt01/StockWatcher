using System.Security.Claims;

namespace StockTracker.Identity.Api.Areas.Identity.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);
    bool IsCloseToExpire(string accessToken, int minutesBeforeExpire = 5);
}