using ProjectsService.Application.Exceptions;
using ProjectsService.Application.UseCases.Commands.CategoryUseCases.DeleteCategory;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Commands.CategoryUseCases;

public class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteCategoryCommandHandler>> _loggerMock;
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteCategoryCommandHandler>>();
        _handler = new DeleteCategoryCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository).Returns(new Mock<IQueriesRepository<Category>>().Object);
        _unitOfWorkMock.Setup(u => u.CategoryCommandsRepository).Returns(new Mock<ICommandsRepository<Category>>().Object);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_DeletesCategorySuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);
        var category = new Category { Id = categoryId, Name = "TestCategory" };

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _unitOfWorkMock.Setup(u => u.CategoryCommandsRepository.DeleteAsync(category, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.CategoryCommandsRepository.DeleteAsync(category, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());

        _loggerMock.VerifyLog(LogLevel.Information, $"Starting deletion of category with ID: {categoryId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Deleting category with ID: {categoryId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Saving changes to database", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully deleted category with ID: {categoryId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category with ID '{categoryId}' not found");

        _unitOfWorkMock.Verify(u => u.CategoryCommandsRepository.DeleteAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());

        _loggerMock.VerifyLog(LogLevel.Information, $"Starting deletion of category with ID: {categoryId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Category with ID '{categoryId}' not found", Times.Once());
    }
}