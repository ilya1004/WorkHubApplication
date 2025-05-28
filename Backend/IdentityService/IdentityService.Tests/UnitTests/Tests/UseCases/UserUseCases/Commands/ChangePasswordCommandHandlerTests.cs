using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.UserUseCases.Commands.ChangePassword;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.UserUseCases.Commands;

public class ChangePasswordCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<ILogger<ChangePasswordCommandHandler>> _loggerMock;
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<AppUser>>(
            Mock.Of<IUserStore<AppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        _loggerMock = new Mock<ILogger<ChangePasswordCommandHandler>>();

        _handler = new ChangePasswordCommandHandler(_userManagerMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldChangePassword_WhenValidInput()
    {
        // Arrange
        var command = new ChangePasswordCommand("user@example.com", "OldP@ssw0rd", "NewP@ssw0rd");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email };

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _userManagerMock.Verify(m => m.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully changed password for user {user.Id}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserNotFound()
    {
        // Arrange
        var command = new ChangePasswordCommand("user@example.com", "OldP@ssw0rd", "NewP@ssw0rd");

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync((AppUser)null!);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"User with email '{command.Email}' not found");
        _userManagerMock.Verify(m => m.ChangePasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User with email {command.Email} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenChangePasswordFails()
    {
        // Arrange
        var command = new ChangePasswordCommand("user@example.com", "OldP@ssw0rd", "NewP@ssw0rd");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email };
        var errors = new[] { new IdentityError { Description = "Password too weak" } };

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword))
            .ReturnsAsync(IdentityResult.Failed(errors));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Password is not successfully changed. Errors: Password too weak");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Failed to change password for user {user.Id}: Password too weak", Times.Once());
    }
}