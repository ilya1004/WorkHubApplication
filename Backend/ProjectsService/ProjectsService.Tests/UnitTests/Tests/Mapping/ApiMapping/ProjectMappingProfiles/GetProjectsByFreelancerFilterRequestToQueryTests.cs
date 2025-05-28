using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.API.Mapping.ProjectMappingProfiles;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFreelancerFilter;

namespace ProjectsService.Tests.UnitTests.Tests.Mapping.ApiMapping.ProjectMappingProfiles;

public class GetProjectsByFreelancerFilterRequestToQueryTests
{
    private readonly IMapper _mapper;

    public GetProjectsByFreelancerFilterRequestToQueryTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GetProjectsByFreelancerFilterRequestToQuery>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_RequestWithAllValues_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var employerId = Guid.NewGuid();
        var request = new GetProjectsByFreelancerFilterRequest(
            ProjectStatus: ProjectStatus.Completed,
            EmployerId: employerId,
            PageNo: 4,
            PageSize: 25);

        // Act
        var query = _mapper.Map<GetProjectsByFreelancerFilterQuery>(request);

        // Assert
        query.Should().NotBeNull();
        query.ProjectStatus.Should().Be(ProjectStatus.Completed);
        query.EmployerId.Should().Be(employerId);
        query.PageNo.Should().Be(4);
        query.PageSize.Should().Be(25);
    }

    [Fact]
    public void Map_RequestWithNullValues_MapsNullPropertiesCorrectly()
    {
        // Arrange
        var request = new GetProjectsByFreelancerFilterRequest(
            ProjectStatus: null,
            EmployerId: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var query = _mapper.Map<GetProjectsByFreelancerFilterQuery>(request);

        // Assert
        query.Should().NotBeNull();
        query.ProjectStatus.Should().BeNull();
        query.EmployerId.Should().BeNull();
        query.PageNo.Should().Be(1);
        query.PageSize.Should().Be(10);
    }

    [Fact]
    public void Map_RequestWithDefaultPagination_MapsDefaultValuesCorrectly()
    {
        // Arrange
        var request = new GetProjectsByFreelancerFilterRequest(
            ProjectStatus: ProjectStatus.AcceptingApplications,
            EmployerId: Guid.NewGuid());

        // Act
        var query = _mapper.Map<GetProjectsByFreelancerFilterQuery>(request);

        // Assert
        query.Should().NotBeNull();
        query.ProjectStatus.Should().Be(ProjectStatus.AcceptingApplications);
        query.EmployerId.Should().NotBeEmpty();
        query.PageNo.Should().Be(1);
        query.PageSize.Should().Be(10);
    }
}