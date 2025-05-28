using System.Linq.Expressions;
using ProjectsService.Application.Exceptions;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.CreateFreelancerApplication;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Commands.FreelancerApplicationUseCases;

public class CreateFreelancerApplicationCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<CreateFreelancerApplicationCommandHandler>> _loggerMock;
    private readonly CreateFreelancerApplicationCommandHandler _handler;

    public CreateFreelancerApplicationCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<CreateFreelancerApplicationCommandHandler>>();
        _handler = new CreateFreelancerApplicationCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _userContextMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository).Returns(new Mock<IQueriesRepository<FreelancerApplication>>().Object);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationCommandsRepository).Returns(new Mock<ICommandsRepository<FreelancerApplication>>().Object);
    }

    [Fact]
    public async Task Handle_WhenNoExistingApplication_CreatesApplicationSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateFreelancerApplicationCommand(projectId);
        var newApplication = new FreelancerApplication { Id = Guid.NewGuid(), ProjectId = projectId, FreelancerId = userId };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<FreelancerApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FreelancerApplication?)null);
        _mapperMock.Setup(m => m.Map<FreelancerApplication>(command)).Returns(newApplication);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationCommandsRepository.AddAsync(It.IsAny<FreelancerApplication>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        newApplication.FreelancerId.Should().Be(userId);
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationCommandsRepository.AddAsync(newApplication, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"User {userId} creating application for project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Adding new freelancer application for project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully created freelancer application for project {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenApplicationExists_ThrowsAlreadyExistsException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateFreelancerApplicationCommand(projectId);
        var existingApplication = new FreelancerApplication { Id = Guid.NewGuid(), ProjectId = projectId, FreelancerId = userId };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<FreelancerApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingApplication);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AlreadyExistsException>().WithMessage($"Freelancer application to the project with ID '{projectId}' already exists.");
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationCommandsRepository.AddAsync(It.IsAny<FreelancerApplication>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Information, $"User {userId} creating application for project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User {userId} already has application for project {projectId}", Times.Once());
    }
}