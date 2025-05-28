using IdentityService.API.Contracts.CommonContracts;
using IdentityService.API.Controllers;
using IdentityService.API.DTOs;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.CreateEmployerIndustry;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.DeleteEmployerIndustry;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.UpdateEmployerIndustry;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetAllEmployerIndustries;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetEmployerIndustryById;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Tests.UnitTests.Tests.Controllers;

public class EmployerIndustriesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly EmployerIndustriesController _controller;
    private readonly CancellationToken _cancellationToken;

    public EmployerIndustriesControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new EmployerIndustriesController(_mediatorMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var dto = new EmployerIndustryDataDto("Tech");
        _mediatorMock.Setup(m => m.Send(It.Is<CreateEmployerIndustryCommand>(c => c.Name == dto.Name), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(dto, _cancellationToken);

        // Assert
        result.Should().BeOfType<CreatedResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<CreateEmployerIndustryCommand>(c => c.Name == dto.Name), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetById_ValidId_ReturnsOkWithIndustry()
    {
        // Arrange
        var id = Guid.NewGuid();
        var industry = new EmployerIndustry { Id = id, Name = "Tech" };
        _mediatorMock.Setup(m => m.Send(It.Is<GetEmployerIndustryByIdQuery>(q => q.Id == id), _cancellationToken))
            .ReturnsAsync(industry);

        // Act
        var result = await _controller.GetById(id, _cancellationToken);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(industry);
        _mediatorMock.Verify(m => m.Send(It.Is<GetEmployerIndustryByIdQuery>(q => q.Id == id), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetAll_ValidRequest_ReturnsOkWithPaginatedResult()
    {
        // Arrange
        var request = new GetPaginatedListRequest(2, 20);
        var paginatedResult = new PaginatedResultModel<EmployerIndustry>
        {
            Items = [new EmployerIndustry { Name = "Tech" }],
            TotalCount = 1,
            PageNo = 2,
            PageSize = 20
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetAllEmployerIndustriesQuery>(q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), _cancellationToken))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetAll(request, _cancellationToken);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(paginatedResult);
        _mediatorMock.Verify(m => m.Send(It.Is<GetAllEmployerIndustriesQuery>(q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task Update_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new EmployerIndustryDataDto("Updated Tech");
        _mediatorMock.Setup(m => m.Send(It.Is<UpdateEmployerIndustryCommand>(c => c.Id == id && c.Name == dto.Name), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(id, dto, _cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<UpdateEmployerIndustryCommand>(c => c.Id == id && c.Name == dto.Name), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task Delete_ValidId_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.Is<DeleteEmployerIndustryCommand>(c => c.Id == id), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(id, _cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<DeleteEmployerIndustryCommand>(c => c.Id == id), _cancellationToken), Times.Once());
    }
}