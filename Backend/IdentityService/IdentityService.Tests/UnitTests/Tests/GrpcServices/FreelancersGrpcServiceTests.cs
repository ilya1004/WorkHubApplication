using Freelancers;
using Grpc.Core;
using IdentityService.API.GrpcServices;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.GrpcServices;

public class FreelancersGrpcServiceTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<FreelancersGrpcService>> _loggerMock;
    private readonly FreelancersGrpcService _service;
    private readonly Mock<ServerCallContext> _contextMock;

    public FreelancersGrpcServiceTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<FreelancersGrpcService>>();
        _service = new FreelancersGrpcService(_mediatorMock.Object, _loggerMock.Object);
        _contextMock = new Mock<ServerCallContext>();
    }

    [Fact]
    public async Task GetFreelancerById_ValidId_ReturnsFreelancerResponse()
    {
        // Arrange
        var freelancerId = Guid.NewGuid();
        var request = new GetFreelancerByIdRequest { Id = freelancerId.ToString() };
        var appUser = new AppUser
        {
            Id = freelancerId,
            FreelancerProfile = new FreelancerProfile { StripeAccountId = "acct_123" }
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetUserByIdQuery>(q => q.Id == freelancerId), CancellationToken.None))
            .ReturnsAsync(appUser);

        // Act
        var response = await _service.GetFreelancerById(request, _contextMock.Object);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(freelancerId.ToString());
        response.StripeAccountId.Should().Be("acct_123");
        _mediatorMock.Verify(m => m.Send(It.Is<GetUserByIdQuery>(q => q.Id == freelancerId), CancellationToken.None), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting freelancer for ID: {freelancerId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully returned freelancer data for {freelancerId}", Times.Once());
    }

    [Fact]
    public async Task GetFreelancerById_NoFreelancerProfile_ReturnsEmptyAccountId()
    {
        // Arrange
        var freelancerId = Guid.NewGuid();
        var request = new GetFreelancerByIdRequest { Id = freelancerId.ToString() };
        var appUser = new AppUser { Id = freelancerId, FreelancerProfile = null };
        _mediatorMock.Setup(m => m.Send(It.Is<GetUserByIdQuery>(q => q.Id == freelancerId), CancellationToken.None))
            .ReturnsAsync(appUser);

        // Act
        var response = await _service.GetFreelancerById(request, _contextMock.Object);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(freelancerId.ToString());
        response.StripeAccountId.Should().BeEmpty();
        _mediatorMock.Verify(m => m.Send(It.Is<GetUserByIdQuery>(q => q.Id == freelancerId), CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task GetFreelancerById_InvalidId_ThrowsRpcException()
    {
        // Arrange
        var request = new GetFreelancerByIdRequest { Id = "invalid-guid" };

        // Act
        Func<Task> act = async () => await _service.GetFreelancerById(request, _contextMock.Object);

        // Assert
        await act.Should().ThrowAsync<FormatException>()
            .WithMessage("Unrecognized Guid format.");
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserByIdQuery>(), CancellationToken.None), Times.Never());
    }
}