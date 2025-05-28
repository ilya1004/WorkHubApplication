using IdentityService.BLL.Abstractions.UserContext;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.AuthUseCases.LogoutUser;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.AuthUseCases;

public class LogoutUserCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<LogoutUserCommandHandler>> _loggerMock;
    private readonly LogoutUserCommandHandler _handler;

    public LogoutUserCommandHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<AppUser>>(
            Mock.Of<IUserStore<AppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<LogoutUserCommandHandler>>();

        _handler = new LogoutUserCommandHandler(_userManagerMock.Object, _userContextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldLogoutUser_WhenUserExistsAndHasTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new AppUser { Id = userId, RefreshToken = "refresh-token", RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1) };
        _userContextMock.Setup(c => c.GetUserId()).Returns(userId);
        _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        var act = async () => await _handler.Handle(new LogoutUserCommand(), CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        user.RefreshToken.Should().BeNull();
        user.RefreshTokenExpiryTime.Should().BeNull();
        _userManagerMock.Verify(m => m.UpdateAsync(user), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"User with ID '{userId}' logged out successfully", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userContextMock.Setup(c => c.GetUserId()).Returns(userId);
        _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync((AppUser)null!);

        // Act
        var act = async () => await _handler.Handle(new LogoutUserCommand(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"User with ID '{userId}' not found");
        _userManagerMock.Verify(m => m.UpdateAsync(It.IsAny<AppUser>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User with ID '{userId}' not found during logout", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldDoNothing_WhenUserAlreadyLoggedOut()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new AppUser { Id = userId, RefreshToken = null, RefreshTokenExpiryTime = null };
        _userContextMock.Setup(c => c.GetUserId()).Returns(userId);
        _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);

        // Act
        var act = async () => await _handler.Handle(new LogoutUserCommand(), CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _userManagerMock.Verify(m => m.UpdateAsync(It.IsAny<AppUser>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Information, $"User with ID '{userId}' has already logout", Times.Once());
    }
}