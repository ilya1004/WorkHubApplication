using System.Linq.Expressions;
using ProjectsService.Application.Constants;
using ProjectsService.Application.Exceptions;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationById;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Queries.FreelancerApplicationUseCases;

public class GetFreelancerApplicationByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<GetFreelancerApplicationByIdQueryHandler>> _loggerMock;
    private readonly GetFreelancerApplicationByIdQueryHandler _handler;

    public GetFreelancerApplicationByIdQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<GetFreelancerApplicationByIdQueryHandler>>();
        _handler = new GetFreelancerApplicationByIdQueryHandler(_unitOfWorkMock.Object, _userContextMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository)
            .Returns(new Mock<IQueriesRepository<FreelancerApplication>>().Object);
    }

    [Fact]
    public async Task Handle_WhenApplicationExistsAndUserIsFreelancer_ReturnsApplication()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetFreelancerApplicationByIdQuery(applicationId);
        var application = new FreelancerApplication
        {
            Id = applicationId,
            CreatedAt = DateTime.UtcNow,
            Status = ApplicationStatus.Accepted,
            ProjectId = Guid.NewGuid(),
            FreelancerId = userId,
            Project = new Project { Id = Guid.NewGuid() }
        };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _userContextMock.Setup(u => u.GetUserRole()).Returns(AppRoles.FreelancerRole);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.GetByIdAsync(
            applicationId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<FreelancerApplication, object>>[]>()))
            .ReturnsAsync(application);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(application);
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.GetByIdAsync(
            applicationId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<FreelancerApplication, object>>[]>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting freelancer application by ID: {applicationId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved freelancer application with ID: {applicationId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenApplicationExistsAndUserIsAdmin_ReturnsApplication()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetFreelancerApplicationByIdQuery(applicationId);
        var application = new FreelancerApplication
        {
            Id = applicationId,
            CreatedAt = DateTime.UtcNow,
            Status = ApplicationStatus.Accepted,
            ProjectId = Guid.NewGuid(),
            FreelancerId = Guid.NewGuid(),
            Project = new Project { Id = Guid.NewGuid() }
        };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _userContextMock.Setup(u => u.GetUserRole()).Returns(AppRoles.AdminRole);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.GetByIdAsync(
            applicationId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<FreelancerApplication, object>>[]>()))
            .ReturnsAsync(application);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(application);
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.GetByIdAsync(
            applicationId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<FreelancerApplication, object>>[]>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting freelancer application by ID: {applicationId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved freelancer application with ID: {applicationId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenApplicationNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var query = new GetFreelancerApplicationByIdQuery(applicationId);

        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.GetByIdAsync(
            applicationId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<FreelancerApplication, object>>[]>()))
            .ReturnsAsync((FreelancerApplication?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Freelancer Application with ID '{applicationId}' not found");
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.GetByIdAsync(
            applicationId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<FreelancerApplication, object>>[]>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting freelancer application by ID: {applicationId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Freelancer application with ID {applicationId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenUserNotFreelancerOrAdmin_ThrowsForbiddenException()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetFreelancerApplicationByIdQuery(applicationId);
        var application = new FreelancerApplication
        {
            Id = applicationId,
            CreatedAt = DateTime.UtcNow,
            Status = ApplicationStatus.Accepted,
            ProjectId = Guid.NewGuid(),
            FreelancerId = Guid.NewGuid(),
            Project = new Project { Id = Guid.NewGuid() }
        };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _userContextMock.Setup(u => u.GetUserRole()).Returns(AppRoles.FreelancerRole);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.GetByIdAsync(
            applicationId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<FreelancerApplication, object>>[]>()))
            .ReturnsAsync(application);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>().WithMessage($"You do not have access to Freelancer Application with ID '{applicationId}'");
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.GetByIdAsync(
            applicationId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<FreelancerApplication, object>>[]>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting freelancer application by ID: {applicationId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User {userId} attempted to access application {applicationId} without permission", Times.Once());
    }
}