using System.Linq.Expressions;
using ProjectsService.Application.Exceptions;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.CancelProject;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Domain.Abstractions.KafkaProducerServices;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Commands.ProjectUseCases;

public class CancelProjectCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<IPaymentsProducerService> _paymentsProducerServiceMock;
    private readonly Mock<ILogger<CancelProjectCommandHandler>> _loggerMock;
    private readonly CancelProjectCommandHandler _handler;

    public CancelProjectCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userContextMock = new Mock<IUserContext>();
        _paymentsProducerServiceMock = new Mock<IPaymentsProducerService>();
        _loggerMock = new Mock<ILogger<CancelProjectCommandHandler>>();
        _handler = new CancelProjectCommandHandler(_unitOfWorkMock.Object, _userContextMock.Object, _paymentsProducerServiceMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository).Returns(new Mock<IQueriesRepository<Project>>().Object);
        _unitOfWorkMock.Setup(u => u.ProjectCommandsRepository).Returns(new Mock<ICommandsRepository<Project>>().Object);
    }

    [Fact]
    public async Task Handle_WhenProjectExistsAndUserIsEmployer_CancelsProjectSuccessfully_WithPaymentIntent()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var paymentIntentId = Guid.NewGuid().ToString();
        var command = new CancelProjectCommand(projectId);
        var project = new Project { Id = projectId, EmployerId = userId, Lifecycle = new Lifecycle { Status = ProjectStatus.AcceptingApplications }, PaymentIntentId = paymentIntentId };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.ProjectCommandsRepository.UpdateAsync(project, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _paymentsProducerServiceMock.Setup(p => p.CancelPaymentAsync(paymentIntentId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        project.Lifecycle.Status.Should().Be(ProjectStatus.Cancelled);
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(project, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _paymentsProducerServiceMock.Verify(p => p.CancelPaymentAsync(paymentIntentId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Starting cancellation of project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Cancelling project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Sending payment cancellation for project {projectId}, payment intent {paymentIntentId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully cancelled project {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenProjectExistsAndUserIsEmployer_CancelsProjectSuccessfully_WithoutPaymentIntent()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CancelProjectCommand(projectId);
        var project = new Project { Id = projectId, EmployerId = userId, Lifecycle = new Lifecycle { Status = ProjectStatus.AcceptingApplications }, PaymentIntentId = null };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.ProjectCommandsRepository.UpdateAsync(project, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        project.Lifecycle.Status.Should().Be(ProjectStatus.Cancelled);
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(project, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _paymentsProducerServiceMock.Verify(p => p.CancelPaymentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Information, $"Starting cancellation of project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Cancelling project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully cancelled project {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command = new CancelProjectCommand(projectId);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync((Project?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Project with ID '{projectId}' not found");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _paymentsProducerServiceMock.Verify(p => p.CancelPaymentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Project {projectId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenUserNotEmployer_ThrowsForbiddenException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CancelProjectCommand(projectId);
        var project = new Project { Id = projectId, EmployerId = Guid.NewGuid(), Lifecycle = new Lifecycle { Status = ProjectStatus.AcceptingApplications } };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>().WithMessage($"You do not have access to project with ID '{projectId}'");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _paymentsProducerServiceMock.Verify(p => p.CancelPaymentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User {userId} attempted to cancel project {projectId} without permission", Times.Once());
    }
}