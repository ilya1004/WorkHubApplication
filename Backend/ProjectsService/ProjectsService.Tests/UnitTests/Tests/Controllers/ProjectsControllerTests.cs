using Microsoft.AspNetCore.Mvc;
using ProjectsService.API.Contracts.CommonContracts;
using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.API.Controllers;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.CancelProject;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.CreateProject;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.DeleteProject;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceRequest;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceStatus;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateProject;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetAllProjects;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectById;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByEmployerFilter;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFilter;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFreelancerFilter;

namespace ProjectsService.Tests.UnitTests.Tests.Controllers;

public class ProjectsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ProjectsController _controller;

    public ProjectsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();
        _controller = new ProjectsController(_mediatorMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateProject_ValidRequest_ReturnsOkWithProjectId()
    {
        // Arrange
        var request = new CreateProjectRequest(
            new ProjectDto("Test Project", "Description", 1000, Guid.NewGuid()),
            new LifecycleDto(DateTime.UtcNow, DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(20), DateTime.UtcNow.AddDays(30)));
        var projectId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        _mediatorMock.Setup(m => m.Send(It.Is<CreateProjectCommand>(cmd => 
            cmd.Project.Title == request.Project.Title && 
            cmd.Lifecycle.ApplicationsStartDate == request.Lifecycle.ApplicationsStartDate), cancellationToken))
            .ReturnsAsync(projectId);

        // Act
        var result = await _controller.CreateProject(request, cancellationToken);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { projectId = projectId.ToString() });
        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateProjectCommand>(), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetProjectById_ValidId_ReturnsOkWithProject()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        var expectedProject = new Project { Id = projectId, Title = "Test Project" };
        _mediatorMock.Setup(m => m.Send(It.Is<GetProjectByIdQuery>(q => q.Id == projectId), cancellationToken))
            .ReturnsAsync(expectedProject);

        // Act
        var result = await _controller.GetProjectById(projectId, cancellationToken);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedProject);
        _mediatorMock.Verify(m => m.Send(It.Is<GetProjectByIdQuery>(q => q.Id == projectId), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetProjectsByFilter_ValidRequest_ReturnsOkWithPaginatedResult()
    {
        // Arrange
        var request = new GetProjectsByFilterRequest(
            Title: "Test",
            BudgetFrom: 500,
            BudgetTo: 1500,
            CategoryId: Guid.NewGuid(),
            EmployerId: Guid.NewGuid(),
            ProjectStatus: ProjectStatus.Published,
            PageNo: 1,
            PageSize: 10);
        var cancellationToken = CancellationToken.None;
        var query = new GetProjectsByFilterQuery(
            request.Title,
            request.BudgetFrom,
            request.BudgetTo,
            request.CategoryId,
            request.EmployerId,
            request.ProjectStatus,
            request.PageNo,
            request.PageSize);
        var expectedResult = new PaginatedResultModel<Project>
        {
            Items = [new Project { Id = Guid.NewGuid(), Title = "Test Project" }],
            TotalCount = 1,
            PageNo = 1,
            PageSize = 10
        };
        _mapperMock.Setup(m => m.Map<GetProjectsByFilterQuery>(request)).Returns(query);
        _mediatorMock.Setup(m => m.Send(query, cancellationToken)).ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetProjectsByFilter(request, cancellationToken);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
        _mapperMock.Verify(m => m.Map<GetProjectsByFilterQuery>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(query, cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetMyProjectsByFreelancerFilter_ValidRequest_ReturnsOkWithPaginatedResult()
    {
        // Arrange
        var request = new GetProjectsByFreelancerFilterRequest(
            ProjectStatus: ProjectStatus.InProgress,
            EmployerId: Guid.NewGuid(),
            PageNo: 1,
            PageSize: 10);
        var cancellationToken = CancellationToken.None;
        var query = new GetProjectsByFreelancerFilterQuery(
            request.ProjectStatus,
            request.EmployerId,
            request.PageNo,
            request.PageSize);
        var expectedResult = new PaginatedResultModel<Project>
        {
            Items = [new Project { Id = Guid.NewGuid(), Title = "Freelancer Project" }],
            TotalCount = 1,
            PageNo = 1,
            PageSize = 10
        };
        _mapperMock.Setup(m => m.Map<GetProjectsByFreelancerFilterQuery>(request)).Returns(query);
        _mediatorMock.Setup(m => m.Send(query, cancellationToken)).ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetMyProjectsByFreelancerFilter(request, cancellationToken);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
        _mapperMock.Verify(m => m.Map<GetProjectsByFreelancerFilterQuery>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(query, cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetMyProjectsByEmployerFilter_ValidRequest_ReturnsOkWithPaginatedResult()
    {
        // Arrange
        var request = new GetProjectsByEmployerFilterRequest(
            UpdatedAtStartDate: DateTime.UtcNow.AddDays(-10),
            UpdatedAtEndDate: DateTime.UtcNow,
            ProjectStatus: ProjectStatus.Completed,
            AcceptanceRequestedAndNotConfirmed: true,
            PageNo: 1,
            PageSize: 10);
        var cancellationToken = CancellationToken.None;
        var query = new GetProjectsByEmployerFilterQuery(
            request.UpdatedAtStartDate,
            request.UpdatedAtEndDate,
            request.ProjectStatus,
            request.AcceptanceRequestedAndNotConfirmed,
            request.PageNo,
            request.PageSize);
        var expectedResult = new PaginatedResultModel<Project>
        {
            Items = [new Project { Id = Guid.NewGuid(), Title = "Employer Project" }],
            TotalCount = 1,
            PageNo = 1,
            PageSize = 10
        };
        _mapperMock.Setup(m => m.Map<GetProjectsByEmployerFilterQuery>(request)).Returns(query);
        _mediatorMock.Setup(m => m.Send(query, cancellationToken)).ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetMyProjectsByEmployerFilter(request, cancellationToken);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
        _mapperMock.Verify(m => m.Map<GetProjectsByEmployerFilterQuery>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(query, cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetAllProjects_ValidRequest_ReturnsOkWithPaginatedResult()
    {
        // Arrange
        var request = new GetPaginatedListRequest(PageNo: 1, PageSize: 10);
        var cancellationToken = CancellationToken.None;
        var expectedResult = new PaginatedResultModel<Project>
        {
            Items = [new Project { Id = Guid.NewGuid(), Title = "Project" }],
            TotalCount = 1,
            PageNo = 1,
            PageSize = 10
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetAllProjectsQuery>(
                q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), cancellationToken))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetAllProjects(request, cancellationToken);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
        _mediatorMock.Verify(m => m.Send(It.Is<GetAllProjectsQuery>(
            q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task UpdateProjectData_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new UpdateProjectRequest(
            new UpdateProjectDto("Updated Project", "New Description", 2000, Guid.NewGuid()),
            new LifecycleDto(DateTime.UtcNow, DateTime.UtcNow.AddDays(15), DateTime.UtcNow.AddDays(25), DateTime.UtcNow.AddDays(35)));
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.UpdateProjectData(projectId, request, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<UpdateProjectCommand>(cmd => 
            cmd.ProjectId == projectId && 
            cmd.Project.Title == request.Project.Title && 
            cmd.Lifecycle.WorkDeadline == request.Lifecycle.WorkDeadline), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task UpdateProjectStatus_ValidId_ReturnsNoContent()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.UpdateProjectStatus(projectId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<CancelProjectCommand>(cmd => cmd.ProjectId == projectId), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task UpdateAcceptanceRequest_ValidId_ReturnsNoContent()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.UpdateAcceptanceRequest(projectId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<UpdateAcceptanceRequestCommand>(
            cmd => cmd.ProjectId == projectId), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task UpdateAcceptanceStatus_ValidIdAndStatus_ReturnsNoContent()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var status = true;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.UpdateAcceptanceStatus(projectId, status, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<UpdateAcceptanceStatusCommand>(
            cmd => cmd.ProjectId == projectId && cmd.IsAcceptanceConfirmed == status), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task DeleteProject_ValidId_ReturnsNoContent()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.DeleteProject(projectId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<DeleteProjectCommand>(
            cmd => cmd.ProjectId == projectId), cancellationToken), Times.Once());
    }
}