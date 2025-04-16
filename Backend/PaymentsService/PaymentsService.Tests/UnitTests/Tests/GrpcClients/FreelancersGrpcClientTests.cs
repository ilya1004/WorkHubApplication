using Freelancers;
using Grpc.Core;
using PaymentsService.Application.Exceptions;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Infrastructure.GrpcClients;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.GrpcClients;

public class FreelancersGrpcClientTests
{
    private readonly Mock<Freelancers.Freelancers.FreelancersClient> _clientMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<FreelancersGrpcClient>> _loggerMock;
    private readonly FreelancersGrpcClient _sut;

    public FreelancersGrpcClientTests()
    {
        _clientMock = new Mock<Freelancers.Freelancers.FreelancersClient>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<FreelancersGrpcClient>>();
        _sut = new FreelancersGrpcClient(_mapperMock.Object, _clientMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetFreelancerByIdAsync_ValidId_ReturnsFreelancerDto()
    {
        // Arrange
        var freelancerId = "free123";
        var response = new GetFreelancerByIdResponse();
        var expectedDto = new FreelancerDto();

        _clientMock.Setup(c => c.GetFreelancerByIdAsync(
                It.Is<GetFreelancerByIdRequest>(r => r.Id == freelancerId),
                null,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetFreelancerByIdResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }
            ));

        _mapperMock.Setup(m => m.Map<FreelancerDto>(response))
            .Returns(expectedDto);

        // Act
        var result = await _sut.GetFreelancerByIdAsync(freelancerId, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        _loggerMock.VerifyLog(LogLevel.Information, $"Requesting freelancer with ID {freelancerId} from gRPC service", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully received freelancer with ID {freelancerId} from gRPC service", Times.Once());
    }

    [Fact]
    public async Task GetFreelancerByIdAsync_NullResponse_ThrowsNotFoundException()
    {
        // Arrange
        var freelancerId = "free123";

        _clientMock.Setup(c => c.GetFreelancerByIdAsync(
                It.Is<GetFreelancerByIdRequest>(r => r.Id == freelancerId),
                null,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetFreelancerByIdResponse>(
                Task.FromResult<GetFreelancerByIdResponse>(null!),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }
            ));

        // Act
        Func<Task> act = async () => await _sut.GetFreelancerByIdAsync(freelancerId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Freelancer by user ID '{freelancerId}' not found.");

        _loggerMock.VerifyLog(LogLevel.Information, $"Requesting freelancer with ID {freelancerId} from gRPC service", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Freelancer not found for user {freelancerId}", Times.Once());
    }
}