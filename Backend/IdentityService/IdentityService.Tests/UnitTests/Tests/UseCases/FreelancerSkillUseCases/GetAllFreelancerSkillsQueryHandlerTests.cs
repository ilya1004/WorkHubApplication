using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Queries.GetAllFreelancerSkills;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.FreelancerSkillUseCases;

public class GetAllFreelancerSkillsQueryHandlerTests
{
    private readonly Mock<ILogger<GetAllFreelancerSkillsQueryHandler>> _loggerMock;
    private readonly Mock<IRepository<FreelancerSkill>> _skillsRepositoryMock;
    private readonly GetAllFreelancerSkillsQueryHandler _handler;

    public GetAllFreelancerSkillsQueryHandlerTests()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetAllFreelancerSkillsQueryHandler>>();
        _skillsRepositoryMock = new Mock<IRepository<FreelancerSkill>>();

        unitOfWorkMock.Setup(u => u.FreelancerSkillsRepository).Returns(_skillsRepositoryMock.Object);

        _handler = new GetAllFreelancerSkillsQueryHandler(unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedSkills_WhenDataExists()
    {
        // Arrange
        var pageNo = 2;
        var pageSize = 5;
        var command = new GetAllFreelancerSkillsQuery(pageNo, pageSize);
        var skills = new List<FreelancerSkill>
        {
            new FreelancerSkill { Id = Guid.NewGuid(), Name = "Programming" },
            new FreelancerSkill { Id = Guid.NewGuid(), Name = "Design" }
        };
        var totalCount = 12;

        _skillsRepositoryMock.Setup(r => r.PaginatedListAllAsync((pageNo - 1) * pageSize, pageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(skills);
        _skillsRepositoryMock.Setup(r => r.CountAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(totalCount);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().BeEquivalentTo(new PaginatedResultModel<FreelancerSkill>
        {
            Items = skills,
            TotalCount = totalCount,
            PageNo = pageNo,
            PageSize = pageSize
        });
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {skills.Count} skills out of {totalCount}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPaginatedResult_WhenNoData()
    {
        // Arrange
        var pageNo = 1;
        var pageSize = 10;
        var command = new GetAllFreelancerSkillsQuery(pageNo, pageSize);
        var skills = new List<FreelancerSkill>();
        var totalCount = 0;

        _skillsRepositoryMock.Setup(r => r.PaginatedListAllAsync(0, pageSize, It.IsAny<CancellationToken>())).ReturnsAsync(skills);
        _skillsRepositoryMock.Setup(r => r.CountAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(totalCount);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().BeEquivalentTo(new PaginatedResultModel<FreelancerSkill>
        {
            Items = skills,
            TotalCount = totalCount,
            PageNo = pageNo,
            PageSize = pageSize
        });
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {skills.Count} skills out of {totalCount}", Times.Once());
    }
}