using IdentityService.API.Contracts.AuthContracts;
using IdentityService.API.Controllers;
using IdentityService.BLL.UseCases.AuthUseCases.ConfirmEmail;
using IdentityService.BLL.UseCases.AuthUseCases.ForgotPassword;
using IdentityService.BLL.UseCases.AuthUseCases.LoginUser;
using IdentityService.BLL.UseCases.AuthUseCases.LogoutUser;
using IdentityService.BLL.UseCases.AuthUseCases.RefreshToken;
using IdentityService.BLL.UseCases.AuthUseCases.ResendEmailConfirmation;
using IdentityService.BLL.UseCases.AuthUseCases.ResetPassword;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Tests.UnitTests.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AuthController _controller;
    private readonly CancellationToken _cancellationToken;

    public AuthControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();
        _controller = new AuthController(_mediatorMock.Object, _mapperMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task Login_ValidRequest_ReturnsOkWithAuthTokens()
    {
        // Arrange
        var request = new LoginUserRequest("test@example.com", "password123");
        var command = new LoginUserCommand("test@example.com", "password123");
        var authTokens = new AuthTokensDto("access-token", "refresh-token");

        _mapperMock.Setup(m => m.Map<LoginUserCommand>(request)).Returns(command);
        _mediatorMock.Setup(m => m.Send(command, _cancellationToken)).ReturnsAsync(authTokens);

        // Act
        var result = await _controller.Login(request, _cancellationToken);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(authTokens);
    }

    [Fact]
    public async Task Logout_AuthorizedUser_ReturnsNoContent()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<LogoutUserCommand>(), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Logout(_cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.IsAny<LogoutUserCommand>(), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task RefreshToken_ValidRequest_ReturnsOkWithAuthTokens()
    {
        // Arrange
        var request = new RefreshTokenRequest("old-access-token", "old-refresh-token");
        var command = new RefreshTokenCommand("old-access-token", "old-refresh-token");
        var authTokens = new AuthTokensDto("new-access-token", "new-refresh-token");

        _mapperMock.Setup(m => m.Map<RefreshTokenCommand>(request)).Returns(command);
        _mediatorMock.Setup(m => m.Send(command, _cancellationToken)).ReturnsAsync(authTokens);

        // Act
        var result = await _controller.RefreshToken(request, _cancellationToken);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(authTokens);
        _mapperMock.Verify(m => m.Map<RefreshTokenCommand>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(command, _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task ConfirmEmail_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new ConfirmEmailRequest("test@example.com", "123456");
        var command = new ConfirmEmailCommand("test@example.com", "123456");

        _mapperMock.Setup(m => m.Map<ConfirmEmailCommand>(request)).Returns(command);
        _mediatorMock.Setup(m => m.Send(command, _cancellationToken)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ConfirmEmail(request, _cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mapperMock.Verify(m => m.Map<ConfirmEmailCommand>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(command, _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task ResendConfirmationEmail_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new ResendEmailConfirmationRequest("test@example.com");
        var command = new ResendEmailConfirmationCommand("test@example.com");

        _mapperMock.Setup(m => m.Map<ResendEmailConfirmationCommand>(request)).Returns(command);
        _mediatorMock.Setup(m => m.Send(command, _cancellationToken)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ResendConfirmationEmail(request, _cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mapperMock.Verify(m => m.Map<ResendEmailConfirmationCommand>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(command, _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task ForgotPassword_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new ForgotPasswordRequest("test@example.com", "http://reset-url");
        var command = new ForgotPasswordCommand("test@example.com", "http://reset-url");

        _mapperMock.Setup(m => m.Map<ForgotPasswordCommand>(request)).Returns(command);
        _mediatorMock.Setup(m => m.Send(command, _cancellationToken)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ForgotPassword(request, _cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mapperMock.Verify(m => m.Map<ForgotPasswordCommand>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(command, _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task ResetPassword_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new ResetPasswordRequest("test@example.com", "new-password", "123456");
        var command = new ResetPasswordCommand("test@example.com", "new-password", "123456");

        _mapperMock.Setup(m => m.Map<ResetPasswordCommand>(request)).Returns(command);
        _mediatorMock.Setup(m => m.Send(command, _cancellationToken)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ResetPassword(request, _cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mapperMock.Verify(m => m.Map<ResetPasswordCommand>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(command, _cancellationToken), Times.Once());
    }
}