using System.Linq.Expressions;
using ProjectsService.Application.Exceptions;
using ProjectsService.Application.UseCases.Commands.CategoryUseCases.CreateCategory;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Commands.CategoryUseCases;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<CreateCategoryCommandHandler>> _loggerMock;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CreateCategoryCommandHandler>>();
        _handler = new CreateCategoryCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository).Returns(new Mock<IQueriesRepository<Category>>().Object);
        _unitOfWorkMock.Setup(u => u.CategoryCommandsRepository).Returns(new Mock<ICommandsRepository<Category>>().Object);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_CreatesCategorySuccessfully()
    {
        // Arrange
        var command = new CreateCategoryCommand("TestCategory");
        var newCategory = new Category { Id = Guid.NewGuid(), Name = command.Name };

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        _mapperMock.Setup(m => m.Map<Category>(command)).Returns(newCategory);

        _unitOfWorkMock.Setup(u => u.CategoryCommandsRepository.AddAsync(newCategory, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.CategoryCommandsRepository.AddAsync(newCategory, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());

        _loggerMock.VerifyLog(LogLevel.Information, $"Starting creation of category with name: {command.Name}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Adding new category with ID: {newCategory.Id}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Saving changes to database", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully created category with ID: {newCategory.Id}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ThrowsAlreadyExistsException()
    {
        // Arrange
        var command = new CreateCategoryCommand("TestCategory");
        var existingCategory = new Category { Id = Guid.NewGuid(), Name = command.Name };

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AlreadyExistsException>()
            .WithMessage($"Category with name '{command.Name}' already exists.");

        _unitOfWorkMock.Verify(u => u.CategoryCommandsRepository.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());

        _loggerMock.VerifyLog(LogLevel.Information, $"Starting creation of category with name: {command.Name}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Category with name '{command.Name}' already exists", Times.Once());
    }
}