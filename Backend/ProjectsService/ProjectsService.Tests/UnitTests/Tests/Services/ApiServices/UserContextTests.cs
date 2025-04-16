using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProjectsService.API.Services;
using ProjectsService.Application.Exceptions;

namespace ProjectsService.Tests.UnitTests.Tests.Services.ApiServices;

public class UserContextTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly UserContext _userContext;

    public UserContextTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _userContext = new UserContext(_httpContextAccessorMock.Object);
    }

    [Fact]
    public void GetUserId_ValidUserIdClaim_ReturnsGuid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _userContext.GetUserId();

        // Assert
        result.Should().Be(userId);
    }

    [Fact]
    public void GetUserId_MissingUserIdClaim_ThrowsUnauthorizedException()
    {
        // Arrange
        var claims = new Claim[] { };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        Action act = () => _userContext.GetUserId();

        // Assert
        act.Should().Throw<UnauthorizedException>()
            .WithMessage("You are not authorized to access this resource.");
    }

    [Fact]
    public void GetUserId_NullHttpContext_ThrowsUnauthorizedException()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null!);

        // Act
        Action act = () => _userContext.GetUserId();

        // Assert
        act.Should().Throw<UnauthorizedException>()
            .WithMessage("You are not authorized to access this resource.");
    }

    [Fact]
    public void GetUserRole_ValidRoleClaim_ReturnsRole()
    {
        // Arrange
        var role = "Freelancer";
        var claims = new[]
        {
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _userContext.GetUserRole();

        // Assert
        result.Should().Be(role);
    }

    [Fact]
    public void GetUserRole_MissingRoleClaim_ThrowsUnauthorizedException()
    {
        // Arrange
        var claims = new Claim[] { };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        Action act = () => _userContext.GetUserRole();

        // Assert
        act.Should().Throw<UnauthorizedException>()
            .WithMessage("You are not authorized to access this resource.");
    }

    [Fact]
    public void GetUserRole_NullHttpContext_ThrowsUnauthorizedException()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null!);

        // Act
        Action act = () => _userContext.GetUserRole();

        // Assert
        act.Should().Throw<UnauthorizedException>()
            .WithMessage("You are not authorized to access this resource.");
    }
}