using Microsoft.AspNetCore.Mvc;
using ProjectsService.API.Contracts.CommonContracts;
using ProjectsService.API.Contracts.FreelancerApplicationContracts;
using ProjectsService.API.Controllers;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.AcceptFreelancerApplication;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.CreateFreelancerApplication;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.DeleteFreelancerApplication;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.RejectFreelancerApplication;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetAllFreelancerApplications;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationById;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByFilter;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByProjectId;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetMyFreelancerApplicationsByFilter;

namespace ProjectsService.Tests.UnitTests.Tests.Controllers;

public class FreelancerApplicationsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly FreelancerApplicationsController _controller;

    public FreelancerApplicationsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();
        _controller = new FreelancerApplicationsController(_mediatorMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateFreelancerApplication_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateFreelancerApplicationRequest(Guid.NewGuid());
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.CreateFreelancerApplication(request, cancellationToken);

        // Assert
        result.Should().BeOfType<CreatedResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<CreateFreelancerApplicationCommand>(
            cmd => cmd.ProjectId == request.ProjectId), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetFreelancerApplications_ValidRequest_ReturnsOkWithPaginatedResult()
    {
        // Arrange
        var request = new GetPaginatedListRequest(PageNo: 1, PageSize: 10);
        var cancellationToken = CancellationToken.None;
        var expectedResult = new PaginatedResultModel<FreelancerApplication>
        {
            Items = [new FreelancerApplication { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Status = ApplicationStatus.Pending }],
            TotalCount = 1,
            PageNo = 1,
            PageSize = 10
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetAllFreelancerApplicationsQuery>(
                q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), cancellationToken))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetFreelancerApplications(request, cancellationToken);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
        _mediatorMock.Verify(m => m.Send(It.Is<GetAllFreelancerApplicationsQuery>(
            q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetFreelancerApplicationById_ValidId_ReturnsOkWithApplication()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        var expectedResult = new FreelancerApplication { Id = applicationId, ProjectId = Guid.NewGuid(), Status = ApplicationStatus.Pending };
        _mediatorMock.Setup(m => m.Send(It.Is<GetFreelancerApplicationByIdQuery>(
                q => q.ApplicationId == applicationId), cancellationToken))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetFreelancerApplicationById(applicationId, cancellationToken);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
        _mediatorMock.Verify(m => m.Send(It.Is<GetFreelancerApplicationByIdQuery>(
            q => q.ApplicationId == applicationId), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetFreelancerApplicationsByProjectId_ValidRequest_ReturnsOkWithPaginatedResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new GetPaginatedListRequest(PageNo: 1, PageSize: 10);
        var cancellationToken = CancellationToken.None;
        var expectedResult = new PaginatedResultModel<FreelancerApplication>
        {
            Items = [new FreelancerApplication { Id = Guid.NewGuid(), ProjectId = projectId, Status = ApplicationStatus.Pending }],
            TotalCount = 1,
            PageNo = 1,
            PageSize = 10
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetFreelancerApplicationsByProjectIdQuery>(
                q => q.ProjectId == projectId && q.PageNo == request.PageNo && q.PageSize == request.PageSize), cancellationToken))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetFreelancerApplicationsByProjectId(request, projectId, cancellationToken);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
        _mediatorMock.Verify(m => m.Send(It.Is<GetFreelancerApplicationsByProjectIdQuery>(
            q => q.ProjectId == projectId && q.PageNo == request.PageNo && q.PageSize == request.PageSize), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetFreelancerApplicationsByFilter_ValidRequest_ReturnsOkWithPaginatedResult()
    {
        // Arrange
        var request = new GetFreelancerApplicationsByFilterRequest(
            StartDate: DateTime.UtcNow.AddDays(-10),
            EndDate: DateTime.UtcNow,
            ApplicationStatus: ApplicationStatus.Pending,
            PageNo: 1,
            PageSize: 10);
        var cancellationToken = CancellationToken.None;
        var query = new GetFreelancerApplicationsByFilterQuery(
            request.StartDate,
            request.EndDate,
            request.ApplicationStatus,
            request.PageNo,
            request.PageSize);
        var expectedResult = new PaginatedResultModel<FreelancerApplication>
        {
            Items = [new FreelancerApplication { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Status = ApplicationStatus.Pending }],
            TotalCount = 1,
            PageNo = 1,
            PageSize = 10
        };
        _mapperMock.Setup(m => m.Map<GetFreelancerApplicationsByFilterQuery>(request)).Returns(query);
        _mediatorMock.Setup(m => m.Send(query, cancellationToken)).ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetFreelancerApplications(request, cancellationToken);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
        _mapperMock.Verify(m => m.Map<GetFreelancerApplicationsByFilterQuery>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(query, cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetMyFreelancerApplications_ValidRequest_ReturnsOkWithPaginatedResult()
    {
        // Arrange
        var request = new GetMyFreelancerApplicationsByFilterRequest(
            StartDate: DateTime.UtcNow.AddDays(-10),
            EndDate: DateTime.UtcNow,
            ApplicationStatus: ApplicationStatus.Pending,
            PageNo: 1,
            PageSize: 10);
        var cancellationToken = CancellationToken.None;
        var query = new GetMyFreelancerApplicationsByFilterQuery(
            request.StartDate,
            request.EndDate,
            request.ApplicationStatus,
            request.PageNo,
            request.PageSize);
        var expectedResult = new PaginatedResultModel<FreelancerApplication>
        {
            Items = [new FreelancerApplication { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Status = ApplicationStatus.Pending }],
            TotalCount = 1,
            PageNo = 1,
            PageSize = 10
        };
        _mapperMock.Setup(m => m.Map<GetMyFreelancerApplicationsByFilterQuery>(request)).Returns(query);
        _mediatorMock.Setup(m => m.Send(query, cancellationToken)).ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetMyFreelancerApplications(request, cancellationToken);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
        _mapperMock.Verify(m => m.Map<GetMyFreelancerApplicationsByFilterQuery>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(query, cancellationToken), Times.Once());
    }

    [Fact]
    public async Task AcceptApplication_ValidIds_ReturnsNoContent()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.AcceptApplication(applicationId, projectId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<AcceptFreelancerApplicationCommand>(
            cmd => cmd.ProjectId == projectId && cmd.ApplicationId == applicationId), cancellationToken), Times.Once());
        _mediatorMock.Verify(m => m.Send(It.Is<AcceptFreelancerApplicationCommand>(
            cmd => cmd.ProjectId == projectId && cmd.ApplicationId == applicationId), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task RejectApplication_ValidIds_ReturnsNoContent()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.RejectApplication(applicationId, projectId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<RejectFreelancerApplicationCommand>(
            cmd => cmd.ProjectId == projectId && cmd.ApplicationId == applicationId), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task CancelFreelancerApplication_ValidId_ReturnsNoContent()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.CancelFreelancerApplication(applicationId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<DeleteFreelancerApplicationCommand>(
            cmd => cmd.ApplicationId == applicationId), cancellationToken), Times.Once());
    }
}