using ProjectsService.Application.UseCases.Queries.CategoryUseCases.GetAllCategories;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Queries.CategoryUseCases;

public class GetAllCategoriesQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetAllCategoriesQueryHandler>> _loggerMock;
    private readonly GetAllCategoriesQueryHandler _handler;

    public GetAllCategoriesQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetAllCategoriesQueryHandler>>();
        _handler = new GetAllCategoriesQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository).Returns(new Mock<IQueriesRepository<Category>>().Object);
    }

    [Fact]
    public async Task Handle_WhenCategoriesExist_ReturnsPaginatedResult()
    {
        // Arrange
        var pageNo = 2;
        var pageSize = 3;
        var query = new GetAllCategoriesQuery(pageNo, pageSize);
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Category 1", NormalizedName = "CATEGORY_1" },
            new Category { Id = Guid.NewGuid(), Name = "Category 2", NormalizedName = "CATEGORY_2" },
            new Category { Id = Guid.NewGuid(), Name = "Category 3", NormalizedName = "CATEGORY_3" }
        };
        var totalCount = 10;
        var offset = (pageNo - 1) * pageSize;

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.PaginatedListAllAsync(offset, pageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);
        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.CountAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEquivalentTo(categories);
        result.TotalCount.Should().Be(totalCount);
        result.PageNo.Should().Be(pageNo);
        result.PageSize.Should().Be(pageSize);
        _unitOfWorkMock.Verify(u => u.CategoryQueriesRepository.PaginatedListAllAsync(offset, pageSize, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.CategoryQueriesRepository.CountAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting all categories with pagination - Page: {pageNo}, Size: {pageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {categories.Count} categories out of {totalCount}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenNoCategoriesExist_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var pageNo = 1;
        var pageSize = 5;
        var query = new GetAllCategoriesQuery(pageNo, pageSize);
        var categories = new List<Category>();
        var totalCount = 0;
        var offset = (pageNo - 1) * pageSize;

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.PaginatedListAllAsync(offset, pageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);
        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.CountAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(totalCount);
        result.PageNo.Should().Be(pageNo);
        result.PageSize.Should().Be(pageSize);
        _unitOfWorkMock.Verify(u => u.CategoryQueriesRepository.PaginatedListAllAsync(offset, pageSize, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.CategoryQueriesRepository.CountAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting all categories with pagination - Page: {pageNo}, Size: {pageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {categories.Count} categories out of {totalCount}", Times.Once());
    }
}