using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Queries.GetFreelancerSkillById;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.FreelancerSkillUseCases;

public class GetFreelancerSkillByIdQueryHandlerTests
{
    private readonly Mock<ILogger<GetFreelancerSkillByIdQueryHandler>> _loggerMock;
    private readonly Mock<IRepository<FreelancerSkill>> _skillsRepositoryMock;
    private readonly GetFreelancerSkillByIdQueryHandler _handler;

    public GetFreelancerSkillByIdQueryHandlerTests()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetFreelancerSkillByIdQueryHandler>>();
        _skillsRepositoryMock = new Mock<IRepository<FreelancerSkill>>();

        unitOfWorkMock.Setup(u => u.FreelancerSkillsRepository).Returns(_skillsRepositoryMock.Object);

        _handler = new GetFreelancerSkillByIdQueryHandler(unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSkill_WhenSkillExists()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var command = new GetFreelancerSkillByIdQuery(skillId);
        var skill = new FreelancerSkill { Id = skillId, Name = "Programming" };

        _skillsRepositoryMock.Setup(r => r.GetByIdAsync(skillId, It.IsAny<CancellationToken>())).ReturnsAsync(skill);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().Be(skill);
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved freelancer skill with ID: {skillId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenSkillNotFound()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var command = new GetFreelancerSkillByIdQuery(skillId);

        _skillsRepositoryMock.Setup(r => r.GetByIdAsync(skillId, It.IsAny<CancellationToken>())).ReturnsAsync((FreelancerSkill)null!);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Freelancer Skill with ID '{skillId}' not found");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Freelancer skill with ID {skillId} not found", Times.Once());
    }
}