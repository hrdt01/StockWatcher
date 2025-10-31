namespace StockTracker.Identity.Api.IntegrationTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using StockTracker.CrossCutting.Constants;
    using StockTracker.Identity.Api.Areas.Identity.Data;
    using StockTracker.Identity.Api.Areas.Identity.Models;
    using StockTracker.Identity.Api.Areas.Identity.Services;
    using StockTracker.Identity.Api.Controllers;
    using StockTracker.Identity.Api.Data;
    using StockTracker.Identity.Api.IntegrationTests.Helpers;
    using Xunit;

    public class AuthenticateControllerTests : IClassFixture<IntegrationTestWebApplicationFactory<Program>>
    {
        private readonly AuthenticateController _testClass;
        private readonly Mock<ILogger<AuthenticateController>> _logger;
        private UserManager<StockTrackerUser> _userManager;
        private readonly Mock<ITokenService> _tokenService;
        private StockTrackerIdentityDbContext _context;
        private readonly Mock<IConfiguration> _configInstance;

        private readonly IntegrationTestWebApplicationFactory<Program> _factory;

        public AuthenticateControllerTests(IntegrationTestWebApplicationFactory<Program> factory)
        {
            _factory = factory;

            _logger = new Mock<ILogger<AuthenticateController>>();
            _userManager = new UserManager<StockTrackerUser>(new Mock<IUserStore<StockTrackerUser>>().Object, new Mock<IOptions<IdentityOptions>>().Object, new Mock<IPasswordHasher<StockTrackerUser>>().Object, new[] { new Mock<IUserValidator<StockTrackerUser>>().Object, new Mock<IUserValidator<StockTrackerUser>>().Object, new Mock<IUserValidator<StockTrackerUser>>().Object }, new[] { new Mock<IPasswordValidator<StockTrackerUser>>().Object, new Mock<IPasswordValidator<StockTrackerUser>>().Object, new Mock<IPasswordValidator<StockTrackerUser>>().Object }, new Mock<ILookupNormalizer>().Object, new IdentityErrorDescriber(), new Mock<IServiceProvider>().Object, new Mock<ILogger<UserManager<StockTrackerUser>>>().Object);
            _tokenService = new Mock<ITokenService>();
            _context = new StockTrackerIdentityDbContext(new DbContextOptions<StockTrackerIdentityDbContext>());
            _configInstance = new Mock<IConfiguration>();
            _testClass = new AuthenticateController(_logger.Object, _userManager, _tokenService.Object, _context);
        }

        private void EnsureDatabaseCreated()
        {
            using var scope = _factory.Services.CreateScope();
            var scopedService = scope.ServiceProvider;
            var db = scopedService.GetRequiredService<StockTrackerIdentityDbContext>();

            db.Database.EnsureCreated();
        }

        [Fact]
        public void CanConstruct()
        {
            // Act
            var instance = new AuthenticateController(_logger.Object, _userManager, _tokenService.Object, _context);

            // Assert
            instance.Should().NotBeNull();
        }

        [Fact]
        public void CannotConstructWithNullLogger()
        {
            FluentActions.Invoking(() => new AuthenticateController(default(ILogger<AuthenticateController>), _userManager, _tokenService.Object, _context)).Should().Throw<ArgumentNullException>().WithParameterName("logger");
        }

        [Fact]
        public void CannotConstructWithNullUserManager()
        {
            FluentActions.Invoking(() => new AuthenticateController(_logger.Object, default(UserManager<StockTrackerUser>), _tokenService.Object, _context)).Should().Throw<ArgumentNullException>().WithParameterName("userManager");
        }

        [Fact]
        public void CannotConstructWithNullTokenService()
        {
            FluentActions.Invoking(() => new AuthenticateController(_logger.Object, _userManager, default(ITokenService), _context)).Should().Throw<ArgumentNullException>().WithParameterName("tokenService");
        }

        [Fact]
        public void CannotConstructWithNullContext()
        {
            FluentActions.Invoking(() => new AuthenticateController(_logger.Object, _userManager, _tokenService.Object, default(StockTrackerIdentityDbContext))).Should().Throw<ArgumentNullException>().WithParameterName("context");
        }
        
        [Fact]
        public async Task Login_WithInvalidUser_ReturnsBadRequest()
        {
            // Arrange
            EnsureDatabaseCreated();

            var model = new LoginModel
            {
                Username = "TestValue2061244347",
                Password = "TestValue1112916957"
            };
            var request = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.LoginAuthApiEndpointRoute}")
            {
                Content = JsonContent.Create(model)
            };
            // Act
            var response = await _factory.HttpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_WithValidUserAndWrongPassword_ReturnsUnauthorized()
        {
            // Arrange
            EnsureDatabaseCreated();

            var model = new LoginModel
            {
                Username = "testApiViewer@test.com",
                Password = "TestValue1112916957"
            };
            var request = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.LoginAuthApiEndpointRoute}")
            {
                Content = JsonContent.Create(model)
            };
            // Act
            var response = await _factory.HttpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_WithValidUser_ReturnsSuccess()
        {
            // Arrange
            EnsureDatabaseCreated();

            var request = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.LoginAuthApiEndpointRoute}")
            {
                Content = JsonContent.Create(new LoginModel
                {
                    Username = "testApiViewer@test.com",
                    Password = "!123Password"
                })
            };

            // Act
            var response = await _factory.HttpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TokenModel>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result?.AccessToken.Should().NotBeNull();
            result?.RefreshToken.Should().NotBeNull();
        }

        [Fact]
        public async Task GetUserInfo_WithValidViewerUser_ReturnsSuccess()
        {
            // Arrange
            EnsureDatabaseCreated();

            var requestToken = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.LoginAuthApiEndpointRoute}")
            {
                Content = JsonContent.Create(new LoginModel
                {
                    Username = "testApiViewer@test.com",
                    Password = "!123Password"
                })
            };
            var responseToken = await _factory.HttpClient.SendAsync(requestToken);

            responseToken.EnsureSuccessStatusCode();
            var tokenResult = await responseToken.Content.ReadFromJsonAsync<TokenModel>();

            var requestUserInfo = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.GetUserInfoAuthApiEndpointRoute}");
            _factory.HttpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResult?.AccessToken);
            
            // Act

            var response = await _factory.HttpClient.SendAsync(requestUserInfo);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<UserInfo>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result?.Email.Should().Be("testApiViewer@test.com");
            result?.IsEmailConfirmed.Should().Be(true);
            result?.Claims.Should().NotBeEmpty();
            result?.Claims.First().Value.Should().Be(GlobalConstants.UserRoleViewer);
        }

        [Fact]
        public async Task GetUserInfo_WithValidAdminUser_ReturnsSuccess()
        {
            // Arrange
            EnsureDatabaseCreated();

            var requestToken = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.LoginAuthApiEndpointRoute}")
            {
                Content = JsonContent.Create(new LoginModel
                {
                    Username = "testApiAdmin@test.com",
                    Password = "!123Password"
                })
            };
            var responseToken = await _factory.HttpClient.SendAsync(requestToken);

            responseToken.EnsureSuccessStatusCode();
            var tokenResult = await responseToken.Content.ReadFromJsonAsync<TokenModel>();

            var requestUserInfo = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.GetUserInfoAuthApiEndpointRoute}");
            _factory.HttpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResult?.AccessToken);

            // Act

            var response = await _factory.HttpClient.SendAsync(requestUserInfo);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<UserInfo>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result?.Email.Should().Be("testApiAdmin@test.com");
            result?.IsEmailConfirmed.Should().Be(true);
            result?.Claims.Should().NotBeEmpty();
            result?.Claims.First().Value.Should().Be(GlobalConstants.UserRoleAdmin);
        }

        [Fact]
        public async Task GetUserInfo_WithNoValidUser_ReturnsFail()
        {
            // Arrange
            EnsureDatabaseCreated();

            var requestUserInfo = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.GetUserInfoAuthApiEndpointRoute}");
            _factory.HttpClient.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _factory.HttpClient.SendAsync(requestUserInfo);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        
        [Fact]
        public async Task Refresh_WithValidUser_ReturnsSuccess()
        {
            // Arrange
            EnsureDatabaseCreated();

            var requestToken = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.LoginAuthApiEndpointRoute}")
            {
                Content = JsonContent.Create(new LoginModel
                {
                    Username = "testApiViewer@test.com",
                    Password = "!123Password"
                })
            };
            var responseToken = await _factory.HttpClient.SendAsync(requestToken);

            responseToken.EnsureSuccessStatusCode();
            var tokenResult = await responseToken.Content.ReadFromJsonAsync<TokenModel>();

            var requestRefresh = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.TokenRefreshAuthApiEndpointRoute}")
            {
                Content = JsonContent.Create(tokenResult)
            };

            _factory.HttpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResult?.AccessToken);

            // Act
            var response = await _factory.HttpClient.SendAsync(requestRefresh);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TokenModel>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result?.AccessToken.Should().NotBe(tokenResult.AccessToken);
            result?.RefreshToken.Should().NotBe(tokenResult.RefreshToken);
        }
        
        [Fact]
        public async Task Refresh_WithNoValidUser_ReturnsFail()
        {
            // Arrange
            EnsureDatabaseCreated();

            var requestRefresh = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.TokenRefreshAuthApiEndpointRoute}");

            _factory.HttpClient.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _factory.HttpClient.SendAsync(requestRefresh);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        
        [Fact]
        public async Task Revoke_WithValidUser_ReturnsSuccess()
        {
            // Arrange
            EnsureDatabaseCreated();

            var requestToken = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.LoginAuthApiEndpointRoute}")
            {
                Content = JsonContent.Create(new LoginModel
                {
                    Username = "testApiViewer@test.com",
                    Password = "!123Password"
                })
            };
            var responseToken = await _factory.HttpClient.SendAsync(requestToken);

            responseToken.EnsureSuccessStatusCode();
            var tokenResult = await responseToken.Content.ReadFromJsonAsync<TokenModel>();

            var requestRefresh = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.TokenRevokeAuthApiEndpointRoute}")
            {
                Content = JsonContent.Create(tokenResult)
            };

            _factory.HttpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResult?.AccessToken);

            // Act
            var response = await _factory.HttpClient.SendAsync(requestRefresh);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<bool>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().Be(true);
        }

        [Fact]
        public async Task Revoke_WithNoValidUser_ReturnsFail()
        {
            // Arrange
            EnsureDatabaseCreated();

            var requestRefresh = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.TokenRevokeAuthApiEndpointRoute}");

            _factory.HttpClient.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _factory.HttpClient.SendAsync(requestRefresh);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        
        [Fact]
        public async Task IsCloseToExpire_WithValidUser_ReturnsSuccess()
        {
            // Arrange
            EnsureDatabaseCreated();

            var requestToken = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.LoginAuthApiEndpointRoute}")
            {
                Content = JsonContent.Create(new LoginModel
                {
                    Username = "testApiViewer@test.com",
                    Password = "!123Password"
                })
            };
            var responseToken = await _factory.HttpClient.SendAsync(requestToken);

            responseToken.EnsureSuccessStatusCode();
            var tokenResult = await responseToken.Content.ReadFromJsonAsync<TokenModel>();

            var requestRefresh = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.TokenCloseToExpireAuthApiEndpointRoute}")
            {
                Content = JsonContent.Create(tokenResult)
            };

            _factory.HttpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResult?.AccessToken);

            // Act
            var response = await _factory.HttpClient.SendAsync(requestRefresh);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<bool>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsCloseToExpire_WithNoValidUser_ReturnsFail()
        {
            // Arrange
            EnsureDatabaseCreated();

            var requestRefresh = new HttpRequestMessage(HttpMethod.Post,
                $"api/Authenticate/{ApiConstants.TokenCloseToExpireAuthApiEndpointRoute}");

            _factory.HttpClient.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _factory.HttpClient.SendAsync(requestRefresh);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}