using Grpc.Core;
using Projects;
using ProjectsService.API.GrpcServices;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectById;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.GrpcServices;

public class ProjectsGrpcServiceTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ProjectsGrpcService>> _loggerMock;
    private readonly ProjectsGrpcService _service;
    private readonly Mock<ServerCallContext> _serverCallContext;

    public ProjectsGrpcServiceTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<ProjectsGrpcService>>();
        _service = new ProjectsGrpcService(_mediatorMock.Object, _loggerMock.Object);
        _serverCallContext = new Mock<ServerCallContext>();
    }

    [Fact]
    public async Task GetProjectById_ValidRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new GetProjectByIdRequest { Id = projectId.ToString() };
        var project = new Project
        {
            Id = projectId,
            Budget = 1000.50m,
            FreelancerId = Guid.NewGuid(),
            PaymentIntentId = "pi_12345"
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetProjectByIdQuery>(q => q.Id == projectId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var response = await _service.GetProjectById(request, _serverCallContext.Object);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(projectId.ToString());
        response.BudgetInCents.Should().Be(100050);
        response.FreelancerId.Should().Be(project.FreelancerId.ToString());
        response.PaymentIntentId.Should().Be(project.PaymentIntentId);

        _mediatorMock.Verify(m => m.Send(It.Is<GetProjectByIdQuery>(q => q.Id == projectId), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting project by ID: {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully returned project data for {projectId}", Times.Once());
    }

    [Fact]
    public async Task GetProjectById_NullFreelancerIdAndPaymentIntentId_ReturnsEmptyStrings()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new GetProjectByIdRequest { Id = projectId.ToString() };
        var project = new Project
        {
            Id = projectId,
            Budget = 500.25m,
            FreelancerId = null,
            PaymentIntentId = null
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetProjectByIdQuery>(q => q.Id == projectId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var response = await _service.GetProjectById(request, _serverCallContext.Object);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(projectId.ToString());
        response.BudgetInCents.Should().Be(50025);
        response.FreelancerId.Should().BeEmpty();
        response.PaymentIntentId.Should().BeEmpty();

        _mediatorMock.Verify(m => m.Send(It.Is<GetProjectByIdQuery>(q => q.Id == projectId), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting project by ID: {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully returned project data for {projectId}", Times.Once());
    }
}