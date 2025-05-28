using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.UpdateEmployerIndustry;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.EmployerIndustryUseCases;

public class UpdateEmployerIndustryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<UpdateEmployerIndustryCommandHandler>> _loggerMock;
    private readonly Mock<IRepository<EmployerIndustry>> _industriesRepositoryMock;
    private readonly UpdateEmployerIndustryCommandHandler _handler;

    public UpdateEmployerIndustryCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<UpdateEmployerIndustryCommandHandler>>();
        _industriesRepositoryMock = new Mock<IRepository<EmployerIndustry>>();

        _unitOfWorkMock.Setup(u => u.EmployerIndustriesRepository).Returns(_industriesRepositoryMock.Object);

        _handler = new UpdateEmployerIndustryCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateIndustry_WhenIndustryExists()
    {
        // Arrange
        var industryId = Guid.NewGuid();
        var command = new UpdateEmployerIndustryCommand(industryId, "Updated Tech");
        var industry = new EmployerIndustry { Id = industryId, Name = "Tech" };

        _industriesRepositoryMock.Setup(r => r.GetByIdAsync(industryId, It.IsAny<CancellationToken>())).ReturnsAsync(industry);
        _mapperMock.Setup(m => m.Map(command, industry)).Returns(industry);
        _industriesRepositoryMock.Setup(r => r.UpdateAsync(industry, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _industriesRepositoryMock.Verify(r => r.UpdateAsync(industry, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully updated industry with ID: {industryId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenIndustryNotFound()
    {
        // Arrange
        var industryId = Guid.NewGuid();
        var command = new UpdateEmployerIndustryCommand(industryId, "Updated Tech");

        _industriesRepositoryMock.Setup(r => r.GetByIdAsync(industryId, It.IsAny<CancellationToken>())).ReturnsAsync((EmployerIndustry)null!);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Employer industry with ID '{industryId}' not found");
        _industriesRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<EmployerIndustry>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Employer industry with ID {industryId} not found", Times.Once());
    }
}