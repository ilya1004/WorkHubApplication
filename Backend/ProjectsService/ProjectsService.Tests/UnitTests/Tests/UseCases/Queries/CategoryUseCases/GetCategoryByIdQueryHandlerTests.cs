using ProjectsService.Application.Exceptions;
using ProjectsService.Application.UseCases.Queries.CategoryUseCases.GetCategoryById;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Queries.CategoryUseCases;

public class GetCategoryByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetCategoryByIdQueryHandler>> _loggerMock;
    private readonly GetCategoryByIdQueryHandler _handler;

    public GetCategoryByIdQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetCategoryByIdQueryHandler>>();
        _handler = new GetCategoryByIdQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository).Returns(new Mock<IQueriesRepository<Category>>().Object);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ReturnsCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var query = new GetCategoryByIdQuery(categoryId);
        var category = new Category
        {
            Id = categoryId,
            Name = "Test Category",
            NormalizedName = "TEST_CATEGORY",
            Projects = new List<Project>()
        };

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(category);
        _unitOfWorkMock.Verify(u => u.CategoryQueriesRepository.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting category by ID: {categoryId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved category with ID: {categoryId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var query = new GetCategoryByIdQuery(categoryId);

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Category with ID '{categoryId}' not found");
        _unitOfWorkMock.Verify(u => u.CategoryQueriesRepository.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting category by ID: {categoryId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Category with ID {categoryId} not found", Times.Once());
    }
}