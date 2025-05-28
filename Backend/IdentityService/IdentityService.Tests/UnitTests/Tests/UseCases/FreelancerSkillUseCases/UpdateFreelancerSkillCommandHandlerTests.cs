using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.UpdateFreelancerSkill;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.FreelancerSkillUseCases;

public class UpdateFreelancerSkillCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<UpdateFreelancerSkillCommandHandler>> _loggerMock;
    private readonly Mock<IRepository<FreelancerSkill>> _skillsRepositoryMock;
    private readonly UpdateFreelancerSkillCommandHandler _handler;

    public UpdateFreelancerSkillCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<UpdateFreelancerSkillCommandHandler>>();
        _skillsRepositoryMock = new Mock<IRepository<FreelancerSkill>>();

        _unitOfWorkMock.Setup(u => u.FreelancerSkillsRepository).Returns(_skillsRepositoryMock.Object);

        _handler = new UpdateFreelancerSkillCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateSkill_WhenSkillExists()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var command = new UpdateFreelancerSkillCommand(skillId, "Updated Programming");
        var skill = new FreelancerSkill { Id = skillId, Name = "Programming" };

        _skillsRepositoryMock.Setup(r => r.GetByIdAsync(skillId, It.IsAny<CancellationToken>())).ReturnsAsync(skill);
        _mapperMock.Setup(m => m.Map(command, skill)).Returns(skill);
        _skillsRepositoryMock.Setup(r => r.UpdateAsync(skill, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _skillsRepositoryMock.Verify(r => r.UpdateAsync(skill, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully updated freelancer skill with ID: {skillId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenSkillNotFound()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var command = new UpdateFreelancerSkillCommand(skillId, "Updated Programming");

        _skillsRepositoryMock.Setup(r => r.GetByIdAsync(skillId, It.IsAny<CancellationToken>())).ReturnsAsync((FreelancerSkill)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Freelancer skill with ID '{skillId}' not found");
        _skillsRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<FreelancerSkill>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Freelancer skill with ID {skillId} not found", Times.Once());
    }
}