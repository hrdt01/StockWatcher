using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockTracker.Identity.Api.Areas.Identity.Data;
using StockTracker.Identity.Api.Areas.Identity.Models;
using StockTracker.Identity.Api.Areas.Identity.Services;
using StockTracker.Identity.Api.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using StockTracker.CrossCutting.Constants;

namespace StockTracker.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class AuthenticateController : ControllerBase
{
    private readonly ILogger<AuthenticateController> _logger;
    private readonly UserManager<StockTrackerUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly StockTrackerIdentityDbContext _context;
    private readonly IConfiguration _configInstance;

    public AuthenticateController(
        ILogger<AuthenticateController> logger,
        UserManager<StockTrackerUser> userManager,
        ITokenService tokenService,
        StockTrackerIdentityDbContext context,
        IConfiguration configInstance)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(userManager);
        ArgumentNullException.ThrowIfNull(tokenService);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(configInstance);

        _logger = logger;
        _userManager = userManager;
        _tokenService = tokenService;
        _context = context;
        _configInstance = configInstance;
    }


    [HttpPost(ApiConstants.LoginAuthApiEndpointRoute)]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(model.Username);
            if (user == null)
                return BadRequest();
            var checkedPasswordSupplied = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!checkedPasswordSupplied)
                return Unauthorized();

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            // generating access token
            var token = _tokenService.GenerateAccessToken(authClaims);

            string refreshToken = _tokenService.GenerateRefreshToken();
            var tokenInfo = _context.TokenInfos.
                FirstOrDefault(a => a.Username == user.UserName);

            // If tokenInfo is null for the user, create a new one
            if (tokenInfo == null)
            {
                var ti = new TokenInfo
                {
                    Username = user.UserName,
                    RefreshToken = refreshToken,
                    ExpiredAt = DateTime.UtcNow.AddDays(7)
                };
                _context.TokenInfos.Add(ti);
            }
            // Else, update the refresh token and expiration
            else
            {
                tokenInfo.RefreshToken = refreshToken;
                tokenInfo.ExpiredAt = DateTime.UtcNow.AddDays(7);
            }

            await _context.SaveChangesAsync();
            var result = new TokenModel
            {
                AccessToken = token,
                RefreshToken = refreshToken
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize]
    [HttpPost(ApiConstants.TokenCloseToExpireAuthApiEndpointRoute)]
    public async Task<IActionResult> IsCloseToExpire()
    {
        try
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest();
            }
            var suppliedToken = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var isCloseToExpire = _tokenService.IsCloseToExpire(suppliedToken, 5);
            return Ok(isCloseToExpire);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize]
    [HttpPost(ApiConstants.TokenRefreshAuthApiEndpointRoute)]
    public async Task<IActionResult> Refresh(TokenModel tokenModel)
    {
        try
        {
            var suppliedToken = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AccessToken);
            var username = principal.Identity.Name;

            var tokenInfo = _context.TokenInfos.SingleOrDefault(u => u.Username == username);
            if (tokenInfo == null
                || tokenInfo.RefreshToken != tokenModel.RefreshToken
                || tokenInfo.ExpiredAt <= DateTime.UtcNow)
            {
                return BadRequest("Invalid refresh token. Please login again.");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            tokenInfo.RefreshToken = newRefreshToken; // rotating the refresh token
            await _context.SaveChangesAsync();
            var result = new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize]
    [HttpPost(ApiConstants.TokenRevokeAuthApiEndpointRoute)]
    public async Task<IActionResult> Revoke()
    {
        try
        {
            var username = User.Identity.Name;

            var user = _context.TokenInfos.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                return BadRequest();
            }

            user.RefreshToken = string.Empty;
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize]
    [HttpPost(ApiConstants.GetUserInfoAuthApiEndpointRoute)]
    public async Task<IActionResult> GetUserInfo()
    {
        try
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest();
            }
            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var claimsDict = claims.ToDictionary(c => c.Type, c => c.Value);
            claimsDict.Add(ClaimTypes.Role, string.Join(",", roles));

            var userInfo = new UserInfo
            {
                Email = user.Email!,
                Username = user.NormalizedUserName!,
                IsEmailConfirmed = isEmailConfirmed,
                Claims = claimsDict
            };
            return Ok(userInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
}