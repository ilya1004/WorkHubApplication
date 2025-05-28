using System.Linq.Expressions;
using ProjectsService.Application.Constants;
using ProjectsService.Application.Exceptions;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByProjectId;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Queries.FreelancerApplicationUseCases;

public class GetFreelancerApplicationsByProjectIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<GetFreelancerApplicationsByProjectIdQueryHandler>> _loggerMock;
    private readonly GetFreelancerApplicationsByProjectIdQueryHandler _handler;

    public GetFreelancerApplicationsByProjectIdQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<GetFreelancerApplicationsByProjectIdQueryHandler>>();
        _handler = new GetFreelancerApplicationsByProjectIdQueryHandler(_unitOfWorkMock.Object, _userContextMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository)
            .Returns(new Mock<IQueriesRepository<FreelancerApplication>>().Object);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository)
            .Returns(new Mock<IQueriesRepository<Project>>().Object);
    }

    [Fact]
    public async Task Handle_ValidRequestAsEmployer_ReturnsPaginatedResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetFreelancerApplicationsByProjectIdQuery(projectId, 1, 10);

        var project = new Project { Id = projectId, EmployerId = userId };
        var applications = new List<FreelancerApplication>
        {
            new() { Id = Guid.NewGuid(), ProjectId = projectId },
            new() { Id = Guid.NewGuid(), ProjectId = projectId }
        };
        var totalCount = 25;

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _userContextMock.Setup(u => u.GetUserRole()).Returns(AppRoles.EmployerRole);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.PaginatedListAsync(
            It.IsAny<Expression<Func<FreelancerApplication, bool>>>(), 0, query.PageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applications);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.CountAsync(
            It.IsAny<Expression<Func<FreelancerApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEquivalentTo(applications);
        result.TotalCount.Should().Be(totalCount);
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);

        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.PaginatedListAsync(
            It.IsAny<Expression<Func<FreelancerApplication, bool>>>(), 0, query.PageSize, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.CountAsync(
            It.IsAny<Expression<Func<FreelancerApplication, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting freelancer applications for project ID: {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {applications.Count} applications for project {projectId} out of {totalCount}", Times.Once());
    }

    [Fact]
    public async Task Handle_ValidRequestAsAdmin_ReturnsPaginatedResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetFreelancerApplicationsByProjectIdQuery(projectId, 1, 10);

        var project = new Project { Id = projectId, EmployerId = Guid.NewGuid() };
        var applications = new List<FreelancerApplication>
        {
            new() { Id = Guid.NewGuid(), ProjectId = projectId }
        };
        var totalCount = 15;

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _userContextMock.Setup(u => u.GetUserRole()).Returns(AppRoles.AdminRole);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.PaginatedListAsync(
            It.IsAny<Expression<Func<FreelancerApplication, bool>>>(), 0, query.PageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applications);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.CountAsync(
            It.IsAny<Expression<Func<FreelancerApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEquivalentTo(applications);
        result.TotalCount.Should().Be(totalCount);
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);

        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {applications.Count} applications for project {projectId} out of {totalCount}", Times.Once());
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var query = new GetFreelancerApplicationsByProjectIdQuery(projectId, 1, 10);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Project with ID '{projectId}' not found");

        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Project with ID {projectId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_UnauthorizedUser_ThrowsForbiddenException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetFreelancerApplicationsByProjectIdQuery(projectId, 1, 10);

        var project = new Project { Id = projectId, EmployerId = Guid.NewGuid() };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _userContextMock.Setup(u => u.GetUserRole()).Returns(AppRoles.FreelancerRole);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage($"You do not have access to Project with ID '{projectId}'");

        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User {userId} attempted to access project {projectId} applications without permission", Times.Once());
    }
}