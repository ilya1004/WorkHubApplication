using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityService.BLL.Services.TokenProvider;
using IdentityService.BLL.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Tests.UnitTests.Tests.Services.BllServices;

public class TokenProviderTests
{
    private readonly TokenProvider _service;
    private readonly JwtSettings _jwtSettings;

    public TokenProviderTests()
    {
        var loggerMock = new Mock<ILogger<TokenProvider>>();
        var optionsMock = new Mock<IOptions<JwtSettings>>();
        _jwtSettings = new JwtSettings
        {
            SecretKey = "this_is_a_very_long_secret_key_1234567890",
            Issuer = "test_issuer",
            Audience = "test_audience",
            AccessTokenExpiryMinutes = 30
        };
        optionsMock.Setup(o => o.Value).Returns(_jwtSettings);

        _service = new TokenProvider(optionsMock.Object, loggerMock.Object);
    }

    [Fact]
    public void GenerateAccessToken_ShouldReturnValidJwtToken()
    {
        // Arrange
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            Role = new IdentityRole<Guid> { Name = "User" }
        };

        // Act
        var token = _service.GenerateAccessToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == user.Email);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == user.Role.Name);
        jwtToken.Issuer.Should().Be(_jwtSettings.Issuer);
        jwtToken.Audiences.Should().Contain(_jwtSettings.Audience); 
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnRandomBase64String()
    {
        // Act
        var token1 = _service.GenerateRefreshToken();
        var token2 = _service.GenerateRefreshToken();

        // Assert
        token1.Should().NotBeNullOrEmpty();
        token2.Should().NotBeNullOrEmpty();
        token1.Should().NotBe(token2);
        Convert.TryFromBase64String(token1, new byte[64], out _).Should().BeTrue();
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ShouldReturnPrincipal_WhenTokenIsValid()
    {
        // Arrange
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            Role = new IdentityRole<Guid> { Name = "User" }
        };
        var token = GenerateTestToken(user, DateTime.UtcNow.AddMinutes(-10));

        // Act
        var principal = _service.GetPrincipalFromExpiredToken(token);

        // Assert
        principal.Should().NotBeNull();
        principal.FindFirst(ClaimTypes.NameIdentifier)!.Value.Should().Be(user.Id.ToString());
        principal.FindFirst(ClaimTypes.Email)!.Value.Should().Be(user.Email);
        principal.FindFirst(ClaimTypes.Role)!.Value.Should().Be(user.Role.Name);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ShouldThrowUnauthorizedException_WhenTokenIsInvalid()
    {
        // Arrange
        var token = "invalid_token";

        // Act
        var act = () => _service.GetPrincipalFromExpiredToken(token);

        // Assert
        act.Should().Throw<SecurityTokenMalformedException>();
    }

    private string GenerateTestToken(AppUser user, DateTime expires)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, user.Role.Name!)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}