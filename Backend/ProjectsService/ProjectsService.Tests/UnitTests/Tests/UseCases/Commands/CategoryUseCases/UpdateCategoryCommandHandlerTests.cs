using ProjectsService.Application.Exceptions;
using ProjectsService.Application.UseCases.Commands.CategoryUseCases.UpdateCategory;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Commands.CategoryUseCases;

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<UpdateCategoryCommandHandler>> _loggerMock;
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<UpdateCategoryCommandHandler>>();
        _handler = new UpdateCategoryCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository).Returns(new Mock<IQueriesRepository<Category>>().Object);
        _unitOfWorkMock.Setup(u => u.CategoryCommandsRepository).Returns(new Mock<ICommandsRepository<Category>>().Object);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_UpdatesCategorySuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand(categoryId, "UpdatedCategory");
        var category = new Category { Id = categoryId, Name = "TestCategory" };

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _mapperMock.Setup(m => m.Map(command, category))
            .Callback<UpdateCategoryCommand, Category>((cmd, cat) => cat.Name = cmd.Name);

        _unitOfWorkMock.Setup(u => u.CategoryCommandsRepository.UpdateAsync(category, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.CategoryCommandsRepository.UpdateAsync(category, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());

        _loggerMock.VerifyLog(LogLevel.Information, $"Starting update of category with ID: {categoryId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Updating category with ID: {categoryId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Saving changes to database", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully updated category with ID: {categoryId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand(categoryId, "UpdatedCategory");

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category with ID '{categoryId}' not found");

        _unitOfWorkMock.Verify(u => u.CategoryCommandsRepository.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());

        _loggerMock.VerifyLog(LogLevel.Information, $"Starting update of category with ID: {categoryId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Category with ID '{categoryId}' not found", Times.Once());
    }
}