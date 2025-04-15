using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.DeleteEmployerIndustry;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.EmployerIndustryUseCases;

public class DeleteEmployerIndustryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteEmployerIndustryCommandHandler>> _loggerMock;
    private readonly Mock<IRepository<EmployerIndustry>> _industriesRepositoryMock;
    private readonly DeleteEmployerIndustryCommandHandler _handler;

    public DeleteEmployerIndustryCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteEmployerIndustryCommandHandler>>();
        _industriesRepositoryMock = new Mock<IRepository<EmployerIndustry>>();

        _unitOfWorkMock.Setup(u => u.EmployerIndustriesRepository).Returns(_industriesRepositoryMock.Object);

        _handler = new DeleteEmployerIndustryCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteIndustry_WhenIndustryExists()
    {
        // Arrange
        var industryId = Guid.NewGuid();
        var command = new DeleteEmployerIndustryCommand(industryId);
        var industry = new EmployerIndustry { Id = industryId, Name = "Tech" };

        _industriesRepositoryMock.Setup(r => r.GetByIdAsync(industryId, It.IsAny<CancellationToken>())).ReturnsAsync(industry);
        _industriesRepositoryMock.Setup(r => r.DeleteAsync(industry, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _industriesRepositoryMock.Verify(r => r.DeleteAsync(industry, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully deleted industry with ID: {industryId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenIndustryNotFound()
    {
        // Arrange
        var industryId = Guid.NewGuid();
        var command = new DeleteEmployerIndustryCommand(industryId);

        _industriesRepositoryMock.Setup(r => r.GetByIdAsync(industryId, It.IsAny<CancellationToken>())).ReturnsAsync((EmployerIndustry)null!);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Employer Industry with ID '{industryId}' not found");
        _industriesRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<EmployerIndustry>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Employer industry with ID {industryId} not found", Times.Once());
    }
}