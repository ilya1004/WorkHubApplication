using IdentityService.BLL.Abstractions.EmailSender;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.AuthUseCases.ResendEmailConfirmation;
using IdentityService.DAL.Abstractions.RedisService;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.AuthUseCases;

public class ResendEmailConfirmationCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<ICachedService> _cachedServiceMock;
    private readonly Mock<ILogger<ResendEmailConfirmationCommandHandler>> _loggerMock;
    private readonly ResendEmailConfirmationCommandHandler _handler;

    public ResendEmailConfirmationCommandHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<AppUser>>(
            Mock.Of<IUserStore<AppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        _emailSenderMock = new Mock<IEmailSender>();
        _cachedServiceMock = new Mock<ICachedService>();
        var configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<ResendEmailConfirmationCommandHandler>>();

        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.Setup(s => s.Value).Returns("24");
        configurationMock.Setup(c => c.GetSection("IdentityTokenExpirationTimeInHours")).Returns(configSectionMock.Object);

        _handler = new ResendEmailConfirmationCommandHandler(
            _userManagerMock.Object,
            _emailSenderMock.Object,
            _cachedServiceMock.Object,
            configurationMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldSendConfirmationEmail_WhenValidInput()
    {
        // Arrange
        var command = new ResendEmailConfirmationCommand("user@example.com");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email, EmailConfirmed = false };
        var token = "confirmation-token";
        var code = "123456";

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GenerateEmailConfirmationTokenAsync(user)).ReturnsAsync(token);
        _cachedServiceMock.SetupSequence(c => c.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _cachedServiceMock.Setup(c => c.SetAsync(code, token, TimeSpan.FromHours(24), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _emailSenderMock.Setup(e => e.SendEmailConfirmation(user.Email, code, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _cachedServiceMock.Verify(c => c.SetAsync(It.IsAny<string>(), token, TimeSpan.FromHours(24), It.IsAny<CancellationToken>()), Times.Once());
        _emailSenderMock.Verify(e => e.SendEmailConfirmation(user.Email, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Confirmation email sent to {user.Email}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenUserNotFound()
    {
        // Arrange
        var command = new ResendEmailConfirmationCommand("user@example.com");
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync((AppUser)null!);

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
        var command = new ResendEmailConfirmationCommand("user@example.com");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email, EmailConfirmed = true };
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Your email is already confirmed.");
        _loggerMock.VerifyLog(LogLevel.Information, $"Email {command.Email} already confirmed", Times.Once());
    }
}