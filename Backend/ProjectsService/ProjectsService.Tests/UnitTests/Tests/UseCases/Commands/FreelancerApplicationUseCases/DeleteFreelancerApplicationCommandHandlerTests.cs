using ProjectsService.Application.Constants;
using ProjectsService.Application.Exceptions;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.DeleteFreelancerApplication;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Commands.FreelancerApplicationUseCases;

public class DeleteFreelancerApplicationCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<DeleteFreelancerApplicationCommandHandler>> _loggerMock;
    private readonly DeleteFreelancerApplicationCommandHandler _handler;

    public DeleteFreelancerApplicationCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<DeleteFreelancerApplicationCommandHandler>>();
        _handler = new DeleteFreelancerApplicationCommandHandler(_unitOfWorkMock.Object, _userContextMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository).Returns(new Mock<IQueriesRepository<FreelancerApplication>>().Object);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationCommandsRepository).Returns(new Mock<ICommandsRepository<FreelancerApplication>>().Object);
    }

    [Fact]
    public async Task Handle_WhenUserOwnsApplication_DeletesApplicationSuccessfully()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteFreelancerApplicationCommand(applicationId);
        var application = new FreelancerApplication { Id = applicationId, FreelancerId = userId };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _userContextMock.Setup(u => u.GetUserRole()).Returns(AppRoles.FreelancerRole);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.GetByIdAsync(applicationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(application);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationCommandsRepository.DeleteAsync(It.IsAny<FreelancerApplication>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationCommandsRepository.DeleteAsync(application, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Deleting freelancer application {applicationId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Deleting application {applicationId} by user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully deleted freelancer application {applicationId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenUserIsAdmin_DeletesApplicationSuccessfully()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteFreelancerApplicationCommand(applicationId);
        var application = new FreelancerApplication { Id = applicationId, FreelancerId = Guid.NewGuid() };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _userContextMock.Setup(u => u.GetUserRole()).Returns(AppRoles.AdminRole);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.GetByIdAsync(applicationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(application);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationCommandsRepository.DeleteAsync(It.IsAny<FreelancerApplication>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationCommandsRepository.DeleteAsync(application, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Deleting freelancer application {applicationId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Deleting application {applicationId} by user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully deleted freelancer application {applicationId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenApplicationNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var command = new DeleteFreelancerApplicationCommand(applicationId);

        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.GetByIdAsync(applicationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FreelancerApplication?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Freelancer Application with ID '{applicationId}' not found");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Freelancer application {applicationId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthorized_ThrowsForbiddenException()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteFreelancerApplicationCommand(applicationId);
        var application = new FreelancerApplication { Id = applicationId, FreelancerId = Guid.NewGuid() };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _userContextMock.Setup(u => u.GetUserRole()).Returns(AppRoles.FreelancerRole);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.GetByIdAsync(applicationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(application);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>().WithMessage($"You do not have access to Freelancer Application with ID '{applicationId}'");
        _loggerMock.VerifyLog(LogLevel.Warning, $"User {userId} attempted to delete application {applicationId} without permission", Times.Once());
    }
}