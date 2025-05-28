using System.Linq.Expressions;
using System.Security.Claims;
using IdentityService.BLL.Abstractions.TokenProvider;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.Settings;
using IdentityService.BLL.UseCases.AuthUseCases.RefreshToken;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.AuthUseCases;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<ITokenProvider> _tokenProviderMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<RefreshTokenCommandHandler>> _loggerMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _tokenProviderMock = new Mock<ITokenProvider>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        var optionsMock = new Mock<IOptions<JwtSettings>>();
        _loggerMock = new Mock<ILogger<RefreshTokenCommandHandler>>();
        _usersRepositoryMock = new Mock<IUsersRepository>();

        optionsMock.Setup(o => o.Value).Returns(new JwtSettings
        {
            RefreshTokenExpiryDays = 7,
            SecretKey = null!,
            Issuer = null!,
            Audience = null!
        });
        _unitOfWorkMock.Setup(u => u.UsersRepository).Returns(_usersRepositoryMock.Object);

        _handler = new RefreshTokenCommandHandler(
            _tokenProviderMock.Object,
            _unitOfWorkMock.Object,
            optionsMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldRefreshTokens_WhenValidInput()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new RefreshTokenCommand("access-token", "refresh-token");
        var user = new AppUser
        {
            Id = userId,
            Email = "user@example.com",
            RefreshToken = command.RefreshToken,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1),
            Role = new IdentityRole<Guid> { Name = "User" }
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));
        var newAccessToken = "new-access-token";
        var newRefreshToken = "new-refresh-token";

        _tokenProviderMock.Setup(t => t.GetPrincipalFromExpiredToken(command.AccessToken)).Returns(principal);
        _usersRepositoryMock.Setup(r => r.GetByIdAsync(userId, false, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);
        _tokenProviderMock.Setup(t => t.GenerateAccessToken(user)).Returns(newAccessToken);
        _tokenProviderMock.Setup(t => t.GenerateRefreshToken()).Returns(newRefreshToken);
        _usersRepositoryMock.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().BeEquivalentTo(new AuthTokensDto(newAccessToken, newRefreshToken));
        user.RefreshToken.Should().Be(newRefreshToken);
        user.RefreshTokenExpiryTime.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromSeconds(1));
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Tokens refreshed successfully for user {user.Id}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new RefreshTokenCommand("access-token", "refresh-token");
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));

        _tokenProviderMock.Setup(t => t.GetPrincipalFromExpiredToken(command.AccessToken)).Returns(principal);
        _usersRepositoryMock.Setup(r => r.GetByIdAsync(userId, false, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync((AppUser)null!);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid refresh token");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Invalid refresh token for user {userId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenRefreshTokenInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new RefreshTokenCommand("access-token", "refresh-token");
        var user = new AppUser
        {
            Id = userId,
            RefreshToken = "different-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));

        _tokenProviderMock.Setup(t => t.GetPrincipalFromExpiredToken(command.AccessToken)).Returns(principal);
        _usersRepositoryMock.Setup(r => r.GetByIdAsync(userId, false, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid refresh token");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Invalid refresh token for user {userId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenRefreshTokenExpired()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new RefreshTokenCommand("access-token", "refresh-token");
        var user = new AppUser
        {
            Id = userId,
            RefreshToken = command.RefreshToken,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));

        _tokenProviderMock.Setup(t => t.GetPrincipalFromExpiredToken(command.AccessToken)).Returns(principal);
        _usersRepositoryMock.Setup(r => r.GetByIdAsync(userId, false, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid refresh token");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Invalid refresh token for user {userId}", Times.Once());
    }
}