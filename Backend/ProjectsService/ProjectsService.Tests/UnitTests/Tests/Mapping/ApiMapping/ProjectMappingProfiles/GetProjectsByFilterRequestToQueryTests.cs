using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.API.Mapping.ProjectMappingProfiles;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFilter;

namespace ProjectsService.Tests.UnitTests.Tests.Mapping.ApiMapping.ProjectMappingProfiles;

public class GetProjectsByFilterRequestToQueryTests
{
    private readonly IMapper _mapper;

    public GetProjectsByFilterRequestToQueryTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GetProjectsByFilterRequestToQuery>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_RequestWithAllValues_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var employerId = Guid.NewGuid();
        var request = new GetProjectsByFilterRequest(
            Title: "Test Project",
            BudgetFrom: 1000.50m,
            BudgetTo: 5000.75m,
            CategoryId: categoryId,
            EmployerId: employerId,
            ProjectStatus: ProjectStatus.InProgress,
            PageNo: 3,
            PageSize: 15);

        // Act
        var query = _mapper.Map<GetProjectsByFilterQuery>(request);

        // Assert
        query.Should().NotBeNull();
        query.Title.Should().Be("Test Project");
        query.BudgetFrom.Should().Be(1000.50m);
        query.BudgetTo.Should().Be(5000.75m);
        query.CategoryId.Should().Be(categoryId);
        query.EmployerId.Should().Be(employerId);
        query.ProjectStatus.Should().Be(ProjectStatus.InProgress);
        query.PageNo.Should().Be(3);
        query.PageSize.Should().Be(15);
    }

    [Fact]
    public void Map_RequestWithNullValues_MapsNullPropertiesCorrectly()
    {
        // Arrange
        var request = new GetProjectsByFilterRequest(
            Title: null,
            BudgetFrom: null,
            BudgetTo: null,
            CategoryId: null,
            EmployerId: null,
            ProjectStatus: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var query = _mapper.Map<GetProjectsByFilterQuery>(request);

        // Assert
        query.Should().NotBeNull();
        query.Title.Should().BeNull();
        query.BudgetFrom.Should().BeNull();
        query.BudgetTo.Should().BeNull();
        query.CategoryId.Should().BeNull();
        query.EmployerId.Should().BeNull();
        query.ProjectStatus.Should().BeNull();
        query.PageNo.Should().Be(1);
        query.PageSize.Should().Be(10);
    }

    [Fact]
    public void Map_RequestWithDefaultPagination_MapsDefaultValuesCorrectly()
    {
        // Arrange
        var request = new GetProjectsByFilterRequest(
            Title: "Sample Project",
            BudgetFrom: 2000m,
            BudgetTo: null,
            CategoryId: Guid.NewGuid(),
            EmployerId: null,
            ProjectStatus: ProjectStatus.InProgress);

        // Act
        var query = _mapper.Map<GetProjectsByFilterQuery>(request);

        // Assert
        query.Should().NotBeNull();
        query.Title.Should().Be("Sample Project");
        query.BudgetFrom.Should().Be(2000m);
        query.BudgetTo.Should().BeNull();
        query.CategoryId.Should().NotBeEmpty();
        query.EmployerId.Should().BeNull();
        query.ProjectStatus.Should().Be(ProjectStatus.InProgress);
        query.PageNo.Should().Be(1);
        query.PageSize.Should().Be(10);
    }
}