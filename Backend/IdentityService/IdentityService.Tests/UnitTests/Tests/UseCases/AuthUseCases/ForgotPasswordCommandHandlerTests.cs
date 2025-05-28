using IdentityService.BLL.Abstractions.EmailSender;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.AuthUseCases.ForgotPassword;
using IdentityService.DAL.Abstractions.RedisService;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.AuthUseCases;

public class ForgotPasswordCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<ICachedService> _cachedServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<ForgotPasswordCommandHandler>> _loggerMock;
    private readonly ForgotPasswordCommandHandler _handler;

    public ForgotPasswordCommandHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<AppUser>>(
            Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
        _emailSenderMock = new Mock<IEmailSender>();
        _cachedServiceMock = new Mock<ICachedService>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<ForgotPasswordCommandHandler>>();

        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.Setup(s => s.Value).Returns("24");
        _configurationMock.Setup(c => c.GetSection("IdentityTokenExpirationTimeInHours")).Returns(configSectionMock.Object);

        _handler = new ForgotPasswordCommandHandler(
            _userManagerMock.Object,
            _emailSenderMock.Object,
            _cachedServiceMock.Object,
            _configurationMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldSendResetEmail_WhenValidInput()
    {
        // Arrange
        var command = new ForgotPasswordCommand("user@example.com", "https://reset.url");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email };
        var token = "reset-token";
        var code = "123456";

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(token);
        _cachedServiceMock.SetupSequence(c => c.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _cachedServiceMock.Setup(c => c.SetAsync(code, token, TimeSpan.FromHours(24), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _emailSenderMock.Setup(e => e.SendPasswordReset(user.Email, $"{command.ResetUrl}?email={user.Email}&code={code}", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _cachedServiceMock.Verify(c => c.SetAsync(It.IsAny<string>(), token, TimeSpan.FromHours(24), It.IsAny<CancellationToken>()), Times.Once());
        _emailSenderMock.Verify(e => e.SendPasswordReset(user.Email, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Password reset email sent to {user.Email}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserNotFound()
    {
        // Arrange
        var command = new ForgotPasswordCommand("user@example.com", "https://reset.url");
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync((AppUser)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User with this email does not exist.");
        _loggerMock.VerifyLog(LogLevel.Warning, $"User with email {command.Email} not found", Times.Once());
    }
}