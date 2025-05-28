using System.Linq.Expressions;
using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.UserUseCases.Commands.DeleteUserCommand;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.UserUseCases.Commands;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<ILogger<DeleteUserCommandHandler>> _loggerMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _blobServiceMock = new Mock<IBlobService>();
        _loggerMock = new Mock<ILogger<DeleteUserCommandHandler>>();
        _usersRepositoryMock = new Mock<IUsersRepository>();

        _unitOfWorkMock.Setup(u => u.UsersRepository).Returns(_usersRepositoryMock.Object);

        _handler = new DeleteUserCommandHandler(_unitOfWorkMock.Object, _blobServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteUser_WhenUserExistsWithImage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var command = new DeleteUserCommand(userId);
        var user = new AppUser { Id = userId, ImageUrl = imageId.ToString() };

        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId, false, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);
        _blobServiceMock.Setup(b => b.DeleteAsync(imageId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _usersRepositoryMock.Setup(r => r.DeleteAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _blobServiceMock.Verify(b => b.DeleteAsync(imageId, It.IsAny<CancellationToken>()), Times.Once());
        _usersRepositoryMock.Verify(r => r.DeleteAsync(user, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully deleted user with ID: {userId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldDeleteUser_WhenUserExistsWithoutImage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand(userId);
        var user = new AppUser { Id = userId, ImageUrl = null };

        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId, false, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);
        _usersRepositoryMock.Setup(r => r.DeleteAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _blobServiceMock.Verify(b => b.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        _usersRepositoryMock.Verify(r => r.DeleteAsync(user, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully deleted user with ID: {userId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand(userId);

        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId, false, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync((AppUser)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"User with ID '{userId}' not found");
        _blobServiceMock.Verify(b => b.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        _usersRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<AppUser>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User with ID {userId} not found", Times.Once());
    }
}