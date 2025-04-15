using IdentityService.API.Contracts.CommonContracts;
using IdentityService.API.Contracts.UserContracts;
using IdentityService.API.Controllers;
using IdentityService.BLL.UseCases.UserUseCases.Commands.ChangePassword;
using IdentityService.BLL.UseCases.UserUseCases.Commands.DeleteUserCommand;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateEmployerProfile;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateFreelancerProfile;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetAllUsers;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetCurrentEmployerUser;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetCurrentFreelancerUser;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetEmployerUserInfoById;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetFreelancerUserInfoById;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Tests.UnitTests.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UsersController _controller;
    private readonly CancellationToken _cancellationToken;

    public UsersControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();
        _controller = new UsersController(_mediatorMock.Object, _mapperMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task RegisterFreelancer_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new RegisterFreelancerRequest("user", "John", "Doe", "john@example.com", "password");
        var command = new RegisterFreelancerCommand("user", "John", "Doe", "john@example.com", "password");
        _mapperMock.Setup(m => m.Map<RegisterFreelancerCommand>(request)).Returns(command);
        _mediatorMock.Setup(m => m.Send(command, _cancellationToken)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RegisterFreelancer(request, _cancellationToken);

        // Assert
        result.Should().BeOfType<StatusCodeResult>()
            .Which.StatusCode.Should().Be(201);
        _mapperMock.Verify(m => m.Map<RegisterFreelancerCommand>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(command, _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task RegisterEmployer_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new RegisterEmployerRequest("company", "Company Inc", "company@example.com", "password");
        var command = new RegisterEmployerCommand("company", "Company Inc", "company@example.com", "password");
        _mapperMock.Setup(m => m.Map<RegisterEmployerCommand>(request)).Returns(command);
        _mediatorMock.Setup(m => m.Send(command, _cancellationToken)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RegisterEmployer(request, _cancellationToken);

        // Assert
        result.Should().BeOfType<StatusCodeResult>()
            .Which.StatusCode.Should().Be(201);
        _mapperMock.Verify(m => m.Map<RegisterEmployerCommand>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(command, _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetAllUsers_ValidRequest_ReturnsOkWithPaginatedResult()
    {
        // Arrange
        var request = new GetPaginatedListRequest(2, 20);
        var paginatedResult = new PaginatedResultModel<AppUser>
        {
            Items = [new AppUser { UserName = "user" }],
            TotalCount = 1,
            PageNo = 2,
            PageSize = 20
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetAllUsersQuery>(q => q.PageNo == 2 && q.PageSize == 20), _cancellationToken))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetAllUsers(request, _cancellationToken);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(paginatedResult);
        _mediatorMock.Verify(m => m.Send(It.Is<GetAllUsersQuery>(q => q.PageNo == 2 && q.PageSize == 20), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetUserById_ValidId_ReturnsOkWithUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new AppUser { Id = userId, UserName = "user" };
        _mediatorMock.Setup(m => m.Send(It.Is<GetUserByIdQuery>(q => q.Id == userId), _cancellationToken))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetUserById(userId, _cancellationToken);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(user);
        _mediatorMock.Verify(m => m.Send(It.Is<GetUserByIdQuery>(q => q.Id == userId), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetFreelancerUserInfoById_ValidId_ReturnsOkWithFreelancerDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var freelancerDto = new FreelancerUserDto { Id = userId.ToString(), UserName = "freelancer" };
        _mediatorMock.Setup(m => m.Send(It.Is<GetFreelancerUserInfoByIdQuery>(q => q.Id == userId), _cancellationToken))
            .ReturnsAsync(freelancerDto);

        // Act
        var result = await _controller.GetFreelancerUserInfoById(userId, _cancellationToken);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(freelancerDto);
        _mediatorMock.Verify(m => m.Send(It.Is<GetFreelancerUserInfoByIdQuery>(q => q.Id == userId), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetEmployerUserInfoById_ValidId_ReturnsOkWithEmployerDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var employerDto = new EmployerUserDto { Id = userId.ToString(), UserName = "employer" };
        _mediatorMock.Setup(m => m.Send(It.Is<GetEmployerUserInfoByIdQuery>(q => q.Id == userId), _cancellationToken))
            .ReturnsAsync(employerDto);

        // Act
        var result = await _controller.GetEmployerUserInfoById(userId, _cancellationToken);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(employerDto);
        _mediatorMock.Verify(m => m.Send(It.Is<GetEmployerUserInfoByIdQuery>(q => q.Id == userId), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetCurrentFreelancerUserInfo_ValidRequest_ReturnsOkWithFreelancerDto()
    {
        // Arrange
        var freelancerDto = new FreelancerUserDto { Id = Guid.NewGuid().ToString(), UserName = "freelancer" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCurrentFreelancerUserQuery>(), _cancellationToken))
            .ReturnsAsync(freelancerDto);

        // Act
        var result = await _controller.GetCurrentFreelancerUserInfo(_cancellationToken);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(freelancerDto);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetCurrentFreelancerUserQuery>(), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetCurrentEmployerUserInfo_ValidRequest_ReturnsOkWithEmployerDto()
    {
        // Arrange
        var employerDto = new EmployerUserDto { Id = Guid.NewGuid().ToString(), UserName = "employer" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCurrentEmployerUserQuery>(), _cancellationToken))
            .ReturnsAsync(employerDto);

        // Act
        var result = await _controller.GetCurrentEmployerUserInfo(_cancellationToken);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(employerDto);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetCurrentEmployerUserQuery>(), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task UpdateFreelancerProfile_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new UpdateFreelancerProfileRequest(new FreelancerProfileDto("John", "Doe", null, null, false), null);
        var command = new UpdateFreelancerProfileCommand(new FreelancerProfileDto("John", "Doe", null, null, false), null, null);
        _mapperMock.Setup(m => m.Map<UpdateFreelancerProfileCommand>(request)).Returns(command);
        _mediatorMock.Setup(m => m.Send(command, _cancellationToken)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateFreelancerProfile(request, _cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mapperMock.Verify(m => m.Map<UpdateFreelancerProfileCommand>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(command, _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task UpdateEmployerProfile_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new UpdateEmployerProfileRequest(new EmployerProfileDto("Company Inc", null, null, false), null);
        var command = new UpdateEmployerProfileCommand(new EmployerProfileDto("Company Inc", null, null, false), null, null);
        _mapperMock.Setup(m => m.Map<UpdateEmployerProfileCommand>(request)).Returns(command);
        _mediatorMock.Setup(m => m.Send(command, _cancellationToken)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateEmployerProfile(request, _cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mapperMock.Verify(m => m.Map<UpdateEmployerProfileCommand>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(command, _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task ChangePassword_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new ChangePasswordRequest("user@example.com", "oldPassword", "newPassword");
        var command = new ChangePasswordCommand("user@example.com", "oldPassword", "newPassword");
        _mapperMock.Setup(m => m.Map<ChangePasswordCommand>(request)).Returns(command);
        _mediatorMock.Setup(m => m.Send(command, _cancellationToken)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ChangePassword(request, _cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mapperMock.Verify(m => m.Map<ChangePasswordCommand>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(command, _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task DeleteUser_ValidId_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteUser(userId, _cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), _cancellationToken), Times.Once());
    }
}