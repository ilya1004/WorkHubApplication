using System.Linq.Expressions;
using IdentityService.BLL.Abstractions.TokenProvider;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.Settings;
using IdentityService.BLL.UseCases.AuthUseCases.LoginUser;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.AuthUseCases;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<SignInManager<AppUser>> _signInManagerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITokenProvider> _tokenProviderMock;
    private readonly Mock<ILogger<LoginUserCommandHandler>> _loggerMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        var userManagerMock = new Mock<UserManager<AppUser>>(
            Mock.Of<IUserStore<AppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        
        _signInManagerMock = new Mock<SignInManager<AppUser>>(
            userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(),
            null!, null!, null!, null!);

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tokenProviderMock = new Mock<ITokenProvider>();
        var optionsMock = new Mock<IOptions<JwtSettings>>();
        _loggerMock = new Mock<ILogger<LoginUserCommandHandler>>();
        _usersRepositoryMock = new Mock<IUsersRepository>();

        optionsMock.Setup(o => o.Value).Returns(new JwtSettings
        {
            RefreshTokenExpiryDays = 7,
            SecretKey = null!,
            Issuer = null!,
            Audience = null!
        });
        _unitOfWorkMock.Setup(u => u.UsersRepository).Returns(_usersRepositoryMock.Object);

        _handler = new LoginUserCommandHandler(
            _signInManagerMock.Object,
            _unitOfWorkMock.Object,
            _tokenProviderMock.Object,
            optionsMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTokens_WhenValidCredentials()
    {
        // Arrange
        var command = new LoginUserCommand("user@example.com", "P@ssw0rd123");
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            EmailConfirmed = true,
            Role = new IdentityRole<Guid> { Name = "User" }
        };
        var accessToken = "access-token";
        var refreshToken = "refresh-token";

        _usersRepositoryMock.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<AppUser, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);
        _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, command.Password, false))
            .ReturnsAsync(SignInResult.Success);
        _tokenProviderMock.Setup(t => t.GenerateAccessToken(user)).Returns(accessToken);
        _tokenProviderMock.Setup(t => t.GenerateRefreshToken()).Returns(refreshToken);
        _usersRepositoryMock.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().BeEquivalentTo(new AuthTokensDto(accessToken, refreshToken));
        user.RefreshToken.Should().Be(refreshToken);
        user.RefreshTokenExpiryTime.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromSeconds(1));
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successful login for user {user.Id}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var command = new LoginUserCommand("user@example.com", "P@ssw0rd123");
        _usersRepositoryMock.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<AppUser, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync((AppUser)null!);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid credentials.");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Invalid login attempt for email {command.Email}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenInvalidPassword()
    {
        // Arrange
        var command = new LoginUserCommand("user@example.com", "P@ssw0rd123");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email, EmailConfirmed = true };
        _usersRepositoryMock.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<AppUser, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);
        _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, command.Password, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid credentials.");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Invalid password for user {user.Id}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenEmailNotConfirmed()
    {
        // Arrange
        var command = new LoginUserCommand("user@example.com", "P@ssw0rd123");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email, EmailConfirmed = false };
        _usersRepositoryMock.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<AppUser, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);
        _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, command.Password, false))
            .ReturnsAsync(SignInResult.Success);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("You need to confirm your email.");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Login attempt for unconfirmed email {command.Email}", Times.Once());
    }
}