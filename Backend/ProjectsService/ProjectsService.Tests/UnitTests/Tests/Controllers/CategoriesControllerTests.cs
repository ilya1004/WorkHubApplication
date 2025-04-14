using Microsoft.AspNetCore.Mvc;
using ProjectsService.API.Contracts.CommonContracts;
using ProjectsService.API.Controllers;
using ProjectsService.Application.UseCases.Commands.CategoryUseCases.CreateCategory;
using ProjectsService.Application.UseCases.Commands.CategoryUseCases.DeleteCategory;
using ProjectsService.Application.UseCases.Commands.CategoryUseCases.UpdateCategory;
using ProjectsService.Application.UseCases.Queries.CategoryUseCases.GetAllCategories;
using ProjectsService.Application.UseCases.Queries.CategoryUseCases.GetCategoryById;

namespace ProjectsService.Tests.UnitTests.Tests.Controllers;

public class CategoriesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CategoriesController _controller;

    public CategoriesControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new CategoriesController(_mediatorMock.Object);
    }

    [Fact]
    public async Task CreateCategory_ValidInput_ReturnsCreated()
    {
        // Arrange
        var categoryDto = new CategoryDto("TestCategory");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.CreateCategory(categoryDto, cancellationToken);

        // Assert
        result.Should().BeOfType<CreatedResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<CreateCategoryCommand>(
            cmd => cmd.Name == categoryDto.Name), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetCategoryById_ValidId_ReturnsOkWithCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        var expectedCategory = new Category { Name = "TestCategory" };
        
        _mediatorMock.Setup(m => m.Send(It.Is<GetCategoryByIdQuery>(q => q.Id == categoryId), cancellationToken))
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _controller.GetCategoryById(categoryId, cancellationToken);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedCategory);
        _mediatorMock.Verify(m => m.Send(It.Is<GetCategoryByIdQuery>(q => q.Id == categoryId), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetAllCategories_ValidRequest_ReturnsOkWithCategories()
    {
        // Arrange
        var request = new GetPaginatedListRequest { PageNo = 1, PageSize = 10 };
        var cancellationToken = CancellationToken.None;
        var expectedCategories = new PaginatedResultModel<Category>
        {
            Items = [
                new Category { Name = "Category1" },
                new Category { Name = "Category2" }
            ],
            PageNo = 1,
            PageSize = 10
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetAllCategoriesQuery>(
                q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), cancellationToken))
            .ReturnsAsync(expectedCategories);

        // Act
        var result = await _controller.GetAllCategories(request, cancellationToken);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedCategories);
        _mediatorMock.Verify(m => m.Send(It.Is<GetAllCategoriesQuery>(
            q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task UpdateCategory_ValidInput_ReturnsNoContent()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categoryDto = new CategoryDto("UpdatedCategory");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.UpdateCategory(categoryId, categoryDto, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<UpdateCategoryCommand>(
            cmd => cmd.Id == categoryId && cmd.Name == categoryDto.Name), cancellationToken), Times.Once());
    }

    [Fact]
    public async Task DeleteCategory_ValidId_ReturnsNoContent()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.DeleteCategory(categoryId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<DeleteCategoryCommand>(cmd => cmd.Id == categoryId), cancellationToken), Times.Once());
    }
}