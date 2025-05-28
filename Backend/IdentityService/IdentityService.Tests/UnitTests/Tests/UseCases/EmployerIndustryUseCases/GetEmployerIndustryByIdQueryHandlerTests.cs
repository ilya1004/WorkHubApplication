using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetEmployerIndustryById;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.EmployerIndustryUseCases;

public class GetEmployerIndustryByIdQueryHandlerTests
{
    private readonly Mock<ILogger<GetEmployerIndustryByIdQueryHandler>> _loggerMock;
    private readonly Mock<IRepository<EmployerIndustry>> _industriesRepositoryMock;
    private readonly GetEmployerIndustryByIdQueryHandler _handler;

    public GetEmployerIndustryByIdQueryHandlerTests()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetEmployerIndustryByIdQueryHandler>>();
        _industriesRepositoryMock = new Mock<IRepository<EmployerIndustry>>();

        unitOfWorkMock.Setup(u => u.EmployerIndustriesRepository).Returns(_industriesRepositoryMock.Object);

        _handler = new GetEmployerIndustryByIdQueryHandler(unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnIndustry_WhenIndustryExists()
    {
        // Arrange
        var industryId = Guid.NewGuid();
        var command = new GetEmployerIndustryByIdQuery(industryId);
        var industry = new EmployerIndustry { Id = industryId, Name = "Tech" };

        _industriesRepositoryMock.Setup(r => r.GetByIdAsync(industryId, It.IsAny<CancellationToken>())).ReturnsAsync(industry);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().Be(industry);
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved industry with ID: {industryId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenIndustryNotFound()
    {
        // Arrange
        var industryId = Guid.NewGuid();
        var command = new GetEmployerIndustryByIdQuery(industryId);

        _industriesRepositoryMock.Setup(r => r.GetByIdAsync(industryId, It.IsAny<CancellationToken>())).ReturnsAsync((EmployerIndustry)null!);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Employer Industry with ID '{industryId}' not found");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Employer industry with ID {industryId} not found", Times.Once());
    }
}