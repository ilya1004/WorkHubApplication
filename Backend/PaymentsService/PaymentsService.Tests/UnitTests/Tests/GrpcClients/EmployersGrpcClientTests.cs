using Employers;
using Grpc.Core;
using PaymentsService.Application.Exceptions;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Infrastructure.GrpcClients;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.GrpcClients;

public class EmployersGrpcClientTests
{
    private readonly Mock<Employers.Employers.EmployersClient> _clientMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<EmployersGrpcClient>> _loggerMock;
    private readonly EmployersGrpcClient _sut;

    public EmployersGrpcClientTests()
    {
        _clientMock = new Mock<Employers.Employers.EmployersClient>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<EmployersGrpcClient>>();
        _sut = new EmployersGrpcClient(_mapperMock.Object, _clientMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetEmployerByIdAsync_ValidId_ReturnsEmployerDto()
    {
        // Arrange
        var employerId = "emp123";
        var response = new GetEmployerByIdResponse();
        var expectedDto = new EmployerDto();
        _clientMock.Setup(c => c.GetEmployerByIdAsync(
                It.Is<GetEmployerByIdRequest>(r => r.Id == employerId), null, null, It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetEmployerByIdResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }
            ));

        _mapperMock.Setup(m => m.Map<EmployerDto>(response))
            .Returns(expectedDto);

        // Act
        var result = await _sut.GetEmployerByIdAsync(employerId, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        _loggerMock.VerifyLog(LogLevel.Information, $"Requesting employer with ID {employerId} from gRPC service", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully received employer with ID {employerId} from gRPC service", Times.Once());
    }

    [Fact]
    public async Task GetEmployerByIdAsync_NullResponse_ThrowsNotFoundException()
    {
        // Arrange
        var employerId = "emp123";
        _clientMock.Setup(c => c.GetEmployerByIdAsync(
                It.Is<GetEmployerByIdRequest>(r => r.Id == employerId),
                null,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetEmployerByIdResponse>(
                Task.FromResult<GetEmployerByIdResponse>(null!),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }
            ));

        // Act
        Func<Task> act = async () => await _sut.GetEmployerByIdAsync(employerId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Employer by user ID '{employerId}' not found.");

        _loggerMock.VerifyLog(LogLevel.Information, $"Requesting employer with ID {employerId} from gRPC service", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Employer not found for user {employerId}", Times.Once());
    }
}