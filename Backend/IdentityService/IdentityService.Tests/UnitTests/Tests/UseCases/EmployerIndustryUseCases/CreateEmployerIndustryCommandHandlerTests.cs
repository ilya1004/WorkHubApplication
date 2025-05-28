using System.Linq.Expressions;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.CreateEmployerIndustry;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.EmployerIndustryUseCases;

public class CreateEmployerIndustryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<CreateEmployerIndustryCommandHandler>> _loggerMock;
    private readonly Mock<IRepository<EmployerIndustry>> _industriesRepositoryMock;
    private readonly CreateEmployerIndustryCommandHandler _handler;

    public CreateEmployerIndustryCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CreateEmployerIndustryCommandHandler>>();
        _industriesRepositoryMock = new Mock<IRepository<EmployerIndustry>>();

        _unitOfWorkMock.Setup(u => u.EmployerIndustriesRepository).Returns(_industriesRepositoryMock.Object);

        _handler = new CreateEmployerIndustryCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateIndustry_WhenNameIsUnique()
    {
        // Arrange
        var command = new CreateEmployerIndustryCommand("Tech");
        var newIndustry = new EmployerIndustry { Id = Guid.NewGuid(), Name = command.Name };

        _industriesRepositoryMock.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<EmployerIndustry, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmployerIndustry)null!);
        _mapperMock.Setup(m => m.Map<EmployerIndustry>(command)).Returns(newIndustry);
        _industriesRepositoryMock.Setup(r => r.AddAsync(newIndustry, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _industriesRepositoryMock.Verify(r => r.AddAsync(newIndustry, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully created new industry with ID: {newIndustry.Id}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenNameExists()
    {
        // Arrange
        var command = new CreateEmployerIndustryCommand("Tech");
        var existingIndustry = new EmployerIndustry { Id = Guid.NewGuid(), Name = command.Name };

        _industriesRepositoryMock.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<EmployerIndustry, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingIndustry);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage($"Industry with the name '{command.Name}' already exists.");
        _industriesRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmployerIndustry>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Industry with name {command.Name} already exists", Times.Once());
    }
}