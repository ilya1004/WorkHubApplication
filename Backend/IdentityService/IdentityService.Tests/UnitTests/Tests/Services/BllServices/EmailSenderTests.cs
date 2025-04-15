using FluentEmail.Core;
using FluentEmail.Core.Models;
using IdentityService.BLL.Services.EmailSender;

namespace IdentityService.Tests.UnitTests.Tests.Services.BllServices;

public class EmailSenderTests
{
    private readonly Mock<IFluentEmail> _fluentEmailMock;
    private readonly EmailSender _service;

    public EmailSenderTests()
    {
        _fluentEmailMock = new Mock<IFluentEmail>();
        _service = new EmailSender(_fluentEmailMock.Object);
    }

    [Fact]
    public async Task SendEmailConfirmation_ShouldSendEmail_WithCorrectParameters()
    {
        // Arrange
        var userEmail = "user@example.com";
        var code = "123456";
        var emailMock = new Mock<IFluentEmail>();
        _fluentEmailMock.Setup(f => f.To(userEmail)).Returns(emailMock.Object);
        emailMock.Setup(e => e.Subject("Email verification from WorkHubApplication")).Returns(emailMock.Object);
        emailMock.Setup(e => e.Body($"Your verification code is <div>{code}</div>", true)).Returns(emailMock.Object);
        emailMock.Setup(e => e.SendAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Mock.Of<SendResponse>());

        // Act
        await _service.SendEmailConfirmation(userEmail, code, CancellationToken.None);

        // Assert
        emailMock.Verify(e => e.Subject("Email verification from WorkHubApplication"), Times.Once());
        emailMock.Verify(e => e.Body($"Your verification code is <div>{code}</div>", true), Times.Once());
        emailMock.Verify(e => e.SendAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task SendPasswordReset_ShouldSendEmail_WithCorrectParameters()
    {
        // Arrange
        var userEmail = "user@example.com";
        var resetUrl = "https://reset.url";
        var emailMock = new Mock<IFluentEmail>();
        _fluentEmailMock.Setup(f => f.To(userEmail)).Returns(emailMock.Object);
        emailMock.Setup(e => e.Subject("Reset password from WorkHubApplication")).Returns(emailMock.Object);
        emailMock.Setup(e => e.Body($"Click the link to reset your password: <a href='{resetUrl}'>Reset!</a>", true)).Returns(emailMock.Object);
        emailMock.Setup(e => e.SendAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Mock.Of<SendResponse>());

        // Act
        await _service.SendPasswordReset(userEmail, resetUrl, CancellationToken.None);

        // Assert
        emailMock.Verify(e => e.Subject("Reset password from WorkHubApplication"), Times.Once());
        emailMock.Verify(e => e.Body($"Click the link to reset your password: <a href='{resetUrl}'>Reset!</a>", true), Times.Once());
        emailMock.Verify(e => e.SendAsync(It.IsAny<CancellationToken>()), Times.Once());
    }
}