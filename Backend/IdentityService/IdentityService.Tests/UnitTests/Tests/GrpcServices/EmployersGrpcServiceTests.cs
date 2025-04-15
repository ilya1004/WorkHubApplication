using Employers;
using Grpc.Core;
using IdentityService.API.GrpcServices;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.GrpcServices;

public class EmployersGrpcServiceTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<EmployersGrpcService>> _loggerMock;
    private readonly EmployersGrpcService _service;
    private readonly Mock<ServerCallContext> _contextMock;

    public EmployersGrpcServiceTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<EmployersGrpcService>>();
        _service = new EmployersGrpcService(_mediatorMock.Object, _loggerMock.Object);
        _contextMock = new Mock<ServerCallContext>();
    }

    [Fact]
    public async Task GetEmployerById_ValidId_ReturnsEmployerResponse()
    {
        // Arrange
        var employerId = Guid.NewGuid();
        var request = new GetEmployerByIdRequest { Id = employerId.ToString() };
        var appUser = new AppUser
        {
            Id = employerId,
            EmployerProfile = new EmployerProfile { StripeCustomerId = "cust_123" }
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetUserByIdQuery>(q => q.Id == employerId), CancellationToken.None))
            .ReturnsAsync(appUser);

        // Act
        var response = await _service.GetEmployerById(request, _contextMock.Object);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(employerId.ToString());
        response.EmployerCustomerId.Should().Be("cust_123");
        _mediatorMock.Verify(m => m.Send(It.Is<GetUserByIdQuery>(q => q.Id == employerId), CancellationToken.None), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting employer for ID: {employerId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully returned employer data for {employerId}", Times.Once());
    }

    [Fact]
    public async Task GetEmployerById_NoEmployerProfile_ReturnsEmptyCustomerId()
    {
        // Arrange
        var employerId = Guid.NewGuid();
        var request = new GetEmployerByIdRequest { Id = employerId.ToString() };
        var appUser = new AppUser { Id = employerId, EmployerProfile = null };
        _mediatorMock.Setup(m => m.Send(It.Is<GetUserByIdQuery>(q => q.Id == employerId), CancellationToken.None))
            .ReturnsAsync(appUser);

        // Act
        var response = await _service.GetEmployerById(request, _contextMock.Object);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(employerId.ToString());
        response.EmployerCustomerId.Should().BeEmpty();
        _mediatorMock.Verify(m => m.Send(It.Is<GetUserByIdQuery>(q => q.Id == employerId), CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task GetEmployerById_InvalidId_ThrowsRpcException()
    {
        // Arrange
        var request = new GetEmployerByIdRequest { Id = "invalid-guid" };

        // Act
        Func<Task> act = async () => await _service.GetEmployerById(request, _contextMock.Object);

        // Assert
        await act.Should().ThrowAsync<FormatException>()
            .WithMessage("Unrecognized Guid format.");
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserByIdQuery>(), CancellationToken.None), Times.Never());
    }
}