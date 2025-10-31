using Microsoft.Extensions.Configuration;
using Moq;
using StockTracker.CrossCutting.ExceptionHandling.CustomExceptions;
using StockTracker.Infrastructure.Auth.Implementation;

namespace StockTracker.Infrastructure.UnitTests.Auth.Implementation;

[TestFixture]
public class ConsumerAuthFlowTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private ConsumerAuthFlow _authFlow;

    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _authFlow = new ConsumerAuthFlow();
    }

    [Test]
    public void CheckAuthentication_ValidAuthCode_DoesNotThrowException()
    {
        // Arrange
        var authCode = "valid-auth-code";
        _mockConfiguration.Setup(x => x["OpenApi:ApiKey"]).Returns(authCode);

        // Act & Assert
        Assert.DoesNotThrow(() => _authFlow.CheckAuthentication(authCode, _mockConfiguration.Object));
    }

    [Test]
    public void CheckAuthentication_NullAuthCode_ThrowsUnauthorizedException()
    {
        // Arrange
        string authCode = null;

        // Act & Assert
        Assert.Throws<UnauthorizedException>(() => 
            _authFlow.CheckAuthentication(authCode, _mockConfiguration.Object));
    }

    [Test]
    public void CheckAuthentication_EmptyAuthCode_ThrowsUnauthorizedException()
    {
        // Arrange
        var authCode = string.Empty;

        // Act & Assert
        Assert.Throws<UnauthorizedException>(() => 
            _authFlow.CheckAuthentication(authCode, _mockConfiguration.Object));
    }

    [Test]
    public void CheckAuthentication_NoStoredAuthKey_ThrowsUnauthorizedException()
    {
        // Arrange
        var authCode = "valid-auth-code";
        _mockConfiguration.Setup(x => x["OpenApi:ApiKey"]).Returns((string)null);

        // Act & Assert
        Assert.Throws<UnauthorizedException>(() => 
            _authFlow.CheckAuthentication(authCode, _mockConfiguration.Object));
    }

    [Test]
    public void CheckAuthentication_MismatchedAuthCode_ThrowsUnauthorizedException()
    {
        // Arrange
        var authCode = "valid-auth-code";
        _mockConfiguration.Setup(x => x["OpenApi:ApiKey"]).Returns("different-auth-code");

        // Act & Assert
        Assert.Throws<UnauthorizedException>(() => 
            _authFlow.CheckAuthentication(authCode, _mockConfiguration.Object));
    }

    [Test]
    public void CheckAuthentication_CaseInsensitiveMatch_DoesNotThrowException()
    {
        // Arrange
        var authCode = "VALID-AUTH-CODE";
        _mockConfiguration.Setup(x => x["OpenApi:ApiKey"]).Returns("valid-auth-code");

        // Act & Assert
        Assert.DoesNotThrow(() => _authFlow.CheckAuthentication(authCode, _mockConfiguration.Object));
    }
}