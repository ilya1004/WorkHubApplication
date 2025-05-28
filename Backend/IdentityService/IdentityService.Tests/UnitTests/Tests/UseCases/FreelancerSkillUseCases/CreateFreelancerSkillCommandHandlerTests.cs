using System.Linq.Expressions;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.CreateFreelancerSkill;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.FreelancerSkillUseCases;

public class CreateFreelancerSkillCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<CreateFreelancerSkillCommandHandler>> _loggerMock;
    private readonly Mock<IRepository<FreelancerSkill>> _skillsRepositoryMock;
    private readonly CreateFreelancerSkillCommandHandler _handler;

    public CreateFreelancerSkillCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CreateFreelancerSkillCommandHandler>>();
        _skillsRepositoryMock = new Mock<IRepository<FreelancerSkill>>();

        _unitOfWorkMock.Setup(u => u.FreelancerSkillsRepository).Returns(_skillsRepositoryMock.Object);

        _handler = new CreateFreelancerSkillCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateSkill_WhenNameIsUnique()
    {
        // Arrange
        var command = new CreateFreelancerSkillCommand("Programming");
        var newSkill = new FreelancerSkill { Id = Guid.NewGuid(), Name = command.Name };

        _skillsRepositoryMock.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<FreelancerSkill, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FreelancerSkill)null!);
        _mapperMock.Setup(m => m.Map<FreelancerSkill>(command)).Returns(newSkill);
        _skillsRepositoryMock.Setup(r => r.AddAsync(newSkill, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _skillsRepositoryMock.Verify(r => r.AddAsync(newSkill, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully created freelancer skill with ID: {newSkill.Id}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenNameExists()
    {
        // Arrange
        var command = new CreateFreelancerSkillCommand("Programming");
        var existingSkill = new FreelancerSkill { Id = Guid.NewGuid(), Name = command.Name };

        _skillsRepositoryMock.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<FreelancerSkill, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSkill);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage($"Freelancer skill with name '{command.Name}' already exists.");
        _skillsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FreelancerSkill>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Freelancer skill with name {command.Name} already exists", Times.Once());
    }
}