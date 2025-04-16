using Grpc.Core;
using PaymentsService.Application.Exceptions;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Infrastructure.GrpcClients;
using PaymentsService.Tests.UnitTests.Extensions;
using Projects;

namespace PaymentsService.Tests.UnitTests.Tests.GrpcClients;

public class ProjectsGrpcClientTests
{
    private readonly Mock<Projects.Projects.ProjectsClient> _clientMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<ProjectsGrpcClient>> _loggerMock;
    private readonly ProjectsGrpcClient _sut;

    public ProjectsGrpcClientTests()
    {
        _clientMock = new Mock<Projects.Projects.ProjectsClient>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<ProjectsGrpcClient>>();
        _sut = new ProjectsGrpcClient(_clientMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ValidId_ReturnsProjectDto()
    {
        // Arrange
        var projectId = "proj123";
        var response = new GetProjectByIdResponse();
        var expectedDto = new ProjectDto();

        _clientMock.Setup(c => c.GetProjectByIdAsync(
                It.Is<GetProjectByIdRequest>(r => r.Id == projectId),
                null,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetProjectByIdResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }
            ));

        _mapperMock.Setup(m => m.Map<ProjectDto>(response))
            .Returns(expectedDto);

        // Act
        var result = await _sut.GetProjectByIdAsync(projectId, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        _loggerMock.VerifyLog(LogLevel.Information, $"Requesting project with ID {projectId} from gRPC service", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully received project with ID {projectId} from gRPC service", Times.Once());
    }

    [Fact]
    public async Task GetProjectByIdAsync_NullResponse_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = "proj123";

        _clientMock.Setup(c => c.GetProjectByIdAsync(
                It.Is<GetProjectByIdRequest>(r => r.Id == projectId),
                null,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetProjectByIdResponse>(
                Task.FromResult<GetProjectByIdResponse>(null!),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }
            ));

        // Act
        Func<Task> act = async () => await _sut.GetProjectByIdAsync(projectId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Project with ID '{projectId}' not found.");

        _loggerMock.VerifyLog(LogLevel.Information, $"Requesting project with ID {projectId} from gRPC service", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Project with ID '{projectId}' not found ", Times.Once());
    }
}