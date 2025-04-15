using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.AuthUseCases.ResetPassword;
using IdentityService.DAL.Abstractions.RedisService;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.AuthUseCases;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<ICachedService> _cachedServiceMock;
    private readonly Mock<ILogger<ResetPasswordCommandHandler>> _loggerMock;
    private readonly ResetPasswordCommandHandler _handler;

    public ResetPasswordCommandHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<AppUser>>(
            Mock.Of<IUserStore<AppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        _cachedServiceMock = new Mock<ICachedService>();
        _loggerMock = new Mock<ILogger<ResetPasswordCommandHandler>>();

        _handler = new ResetPasswordCommandHandler(_userManagerMock.Object, _cachedServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldResetPassword_WhenValidInput()
    {
        // Arrange
        var command = new ResetPasswordCommand("user@example.com", "NewP@ssw0rd123", "123456");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email };
        var token = "reset-token";

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _cachedServiceMock.Setup(c => c.GetAsync(command.Code, It.IsAny<CancellationToken>())).ReturnsAsync(token);
        _userManagerMock.Setup(m => m.ResetPasswordAsync(user, token, command.NewPassword)).ReturnsAsync(IdentityResult.Success);
        _cachedServiceMock.Setup(c => c.DeleteAsync(command.Code, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _cachedServiceMock.Verify(c => c.DeleteAsync(command.Code, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Password reset successfully for user {user.Id}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserNotFound()
    {
        // Arrange
        var command = new ResetPasswordCommand("user@example.com", "NewP@ssw0rd123", "123456");
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync((AppUser)null!);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User with this email does not exist.");
        _loggerMock.VerifyLog(LogLevel.Warning, $"User with email {command.Email} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenInvalidCode()
    {
        // Arrange
        var command = new ResetPasswordCommand("user@example.com", "NewP@ssw0rd123", "123456");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email };
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _cachedServiceMock.Setup(c => c.GetAsync(command.Code, It.IsAny<CancellationToken>())).ReturnsAsync((string)null!);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Invalid resetting password code.");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Invalid reset code {command.Code}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenResetFails()
    {
        // Arrange
        var command = new ResetPasswordCommand("user@example.com", "NewP@ssw0rd123", "123456");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email };
        var token = "reset-token";
        var errors = new[] { new IdentityError { Description = "Password too weak" } };
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _cachedServiceMock.Setup(c => c.GetAsync(command.Code, It.IsAny<CancellationToken>())).ReturnsAsync(token);
        _userManagerMock.Setup(m => m.ResetPasswordAsync(user, token, command.NewPassword))
            .ReturnsAsync(IdentityResult.Failed(errors));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Password is not successfully changed. Errors: Password too weak");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Password reset failed for user {user.Id}: Password too weak", Times.Once());
    }
}