using IdentityService.API.Contracts.CommonContracts;
using IdentityService.API.Controllers;
using IdentityService.API.DTOs;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.CreateFreelancerSkill;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.DeleteFreelancerSkill;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.UpdateFreelancerSkill;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Queries.GetAllFreelancerSkills;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Queries.GetFreelancerSkillById;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Tests.UnitTests.Tests.Controllers;

public class FreelancerSkillsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly FreelancerSkillsController _controller;
    private readonly CancellationToken _cancellationToken;

    public FreelancerSkillsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new FreelancerSkillsController(_mediatorMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var dto = new FreelancerSkillDataDto("C#");
        _mediatorMock.Setup(m => m.Send(It.Is<CreateFreelancerSkillCommand>(c => c.Name == dto.Name), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(dto, _cancellationToken);

        // Assert
        result.Should().BeOfType<CreatedResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<CreateFreelancerSkillCommand>(c => c.Name == dto.Name), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetById_ValidId_ReturnsOkWithSkill()
    {
        // Arrange
        var id = Guid.NewGuid();
        var skill = new FreelancerSkill { Id = id, Name = "C#" };
        _mediatorMock.Setup(m => m.Send(It.Is<GetFreelancerSkillByIdQuery>(q => q.Id == id), _cancellationToken))
            .ReturnsAsync(skill);

        // Act
        var result = await _controller.GetById(id, _cancellationToken);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(skill);
        _mediatorMock.Verify(m => m.Send(It.Is<GetFreelancerSkillByIdQuery>(q => q.Id == id), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetAll_ValidRequest_ReturnsOkWithPaginatedResult()
    {
        // Arrange
        var request = new GetPaginatedListRequest(2, 20);
        var paginatedResult = new PaginatedResultModel<FreelancerSkill>
        {
            Items = [new FreelancerSkill { Name = "C#" }],
            TotalCount = 1,
            PageNo = 2,
            PageSize = 20
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetAllFreelancerSkillsQuery>(q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), _cancellationToken))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetAll(request, _cancellationToken);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(paginatedResult);
        _mediatorMock.Verify(m => m.Send(It.Is<GetAllFreelancerSkillsQuery>(q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task Update_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new FreelancerSkillDataDto("Updated C#");
        _mediatorMock.Setup(m => m.Send(It.Is<UpdateFreelancerSkillCommand>(c => c.Id == id && c.Name == dto.Name), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(id, dto, _cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<UpdateFreelancerSkillCommand>(c => c.Id == id && c.Name == dto.Name), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task Delete_ValidId_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.Is<DeleteFreelancerSkillCommand>(c => c.Id == id), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(id, _cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<DeleteFreelancerSkillCommand>(c => c.Id == id), _cancellationToken), Times.Once());
    }
}