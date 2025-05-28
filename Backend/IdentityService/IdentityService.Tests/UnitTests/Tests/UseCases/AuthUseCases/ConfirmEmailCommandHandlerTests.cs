using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.AuthUseCases.ConfirmEmail;
using IdentityService.DAL.Abstractions.RedisService;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.AuthUseCases;

public class ConfirmEmailCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<ICachedService> _cachedServiceMock;
    private readonly Mock<ILogger<ConfirmEmailCommandHandler>> _loggerMock;
    private readonly ConfirmEmailCommandHandler _handler;

    public ConfirmEmailCommandHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<AppUser>>(
            Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
        _cachedServiceMock = new Mock<ICachedService>();
        _loggerMock = new Mock<ILogger<ConfirmEmailCommandHandler>>();

        _handler = new ConfirmEmailCommandHandler(_userManagerMock.Object, _cachedServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldConfirmEmail_WhenValidInput()
    {
        // Arrange
        var command = new ConfirmEmailCommand("user@example.com", "123456");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email, EmailConfirmed = false };
        var token = "valid-token";

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _cachedServiceMock.Setup(c => c.GetAsync(command.Code, It.IsAny<CancellationToken>())).ReturnsAsync(token);
        _userManagerMock.Setup(m => m.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Success);
        _cachedServiceMock.Setup(c => c.DeleteAsync(command.Code, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _cachedServiceMock.Verify(c => c.DeleteAsync(command.Code, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Email confirmed successfully for user {user.Id}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenUserNotFound()
    {
        // Arrange
        var command = new ConfirmEmailCommand("user@example.com", "123456");
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync((AppUser)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage($"A user with the email '{command.Email}' not exist.");
        _loggerMock.VerifyLog(LogLevel.Warning, $"User with email {command.Email} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenEmailAlreadyConfirmed()
    {
        // Arrange
        var command = new ConfirmEmailCommand("user@example.com", "123456");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email, EmailConfirmed = true };
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Your email is already confirmed.");
        _loggerMock.VerifyLog(LogLevel.Information, $"Email {command.Email} already confirmed", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenInvalidCode()
    {
        // Arrange
        var command = new ConfirmEmailCommand("user@example.com", "123456");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email, EmailConfirmed = false };
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _cachedServiceMock.Setup(c => c.GetAsync(command.Code, It.IsAny<CancellationToken>())).ReturnsAsync((string)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Invalid verification code.");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Invalid verification code {command.Code}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenInvalidToken()
    {
        // Arrange
        var command = new ConfirmEmailCommand("user@example.com", "123456");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email, EmailConfirmed = false };
        var token = "invalid-token";
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _cachedServiceMock.Setup(c => c.GetAsync(command.Code, It.IsAny<CancellationToken>())).ReturnsAsync(token);
        _userManagerMock.Setup(m => m.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Failed());

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Email confirmation token is invalid");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Invalid email confirmation token for user {user.Id}", Times.Once());
    }
}