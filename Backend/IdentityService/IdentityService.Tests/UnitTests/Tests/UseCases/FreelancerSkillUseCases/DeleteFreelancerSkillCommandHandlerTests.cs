using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.DeleteFreelancerSkill;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.FreelancerSkillUseCases;

public class DeleteFreelancerSkillCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteFreelancerSkillCommandHandler>> _loggerMock;
    private readonly Mock<IRepository<FreelancerSkill>> _skillsRepositoryMock;
    private readonly DeleteFreelancerSkillCommandHandler _handler;

    public DeleteFreelancerSkillCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteFreelancerSkillCommandHandler>>();
        _skillsRepositoryMock = new Mock<IRepository<FreelancerSkill>>();

        _unitOfWorkMock.Setup(u => u.FreelancerSkillsRepository).Returns(_skillsRepositoryMock.Object);

        _handler = new DeleteFreelancerSkillCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteSkill_WhenSkillExists()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var command = new DeleteFreelancerSkillCommand(skillId);
        var skill = new FreelancerSkill { Id = skillId, Name = "Programming" };

        _skillsRepositoryMock.Setup(r => r.GetByIdAsync(skillId, It.IsAny<CancellationToken>())).ReturnsAsync(skill);
        _skillsRepositoryMock.Setup(r => r.DeleteAsync(skill, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _skillsRepositoryMock.Verify(r => r.DeleteAsync(skill, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully deleted freelancer skill with ID: {skillId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenSkillNotFound()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var command = new DeleteFreelancerSkillCommand(skillId);

        _skillsRepositoryMock.Setup(r => r.GetByIdAsync(skillId, It.IsAny<CancellationToken>())).ReturnsAsync((FreelancerSkill)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Freelancer skill with ID '{skillId}' not found");
        _skillsRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<FreelancerSkill>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Freelancer skill with ID {skillId} not found", Times.Once());
    }
}