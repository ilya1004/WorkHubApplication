using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.API.Mapping.ProjectMappingProfiles;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByEmployerFilter;

namespace ProjectsService.Tests.UnitTests.Tests.Mapping.ApiMapping.ProjectMappingProfiles;

public class GetProjectsByEmployerFilterRequestToQueryTests
{
    private readonly IMapper _mapper;

    public GetProjectsByEmployerFilterRequestToQueryTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GetProjectsByEmployerFilterRequestToQuery>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_RequestWithAllValues_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 12, 31);
        var request = new GetProjectsByEmployerFilterRequest(
            UpdatedAtStartDate: startDate,
            UpdatedAtEndDate: endDate,
            ProjectStatus: ProjectStatus.InProgress,
            AcceptanceRequestedAndNotConfirmed: true,
            PageNo: 2,
            PageSize: 20);

        // Act
        var query = _mapper.Map<GetProjectsByEmployerFilterQuery>(request);

        // Assert
        query.Should().NotBeNull();
        query.UpdatedAtStartDate.Should().Be(startDate);
        query.UpdatedAtEndDate.Should().Be(endDate);
        query.ProjectStatus.Should().Be(ProjectStatus.InProgress);
        query.AcceptanceRequestedAndNotConfirmed.Should().BeTrue();
        query.PageNo.Should().Be(2);
        query.PageSize.Should().Be(20);
    }

    [Fact]
    public void Map_RequestWithNullValues_MapsNullPropertiesCorrectly()
    {
        // Arrange
        var request = new GetProjectsByEmployerFilterRequest(
            UpdatedAtStartDate: null,
            UpdatedAtEndDate: null,
            ProjectStatus: null,
            AcceptanceRequestedAndNotConfirmed: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var query = _mapper.Map<GetProjectsByEmployerFilterQuery>(request);

        // Assert
        query.Should().NotBeNull();
        query.UpdatedAtStartDate.Should().BeNull();
        query.UpdatedAtEndDate.Should().BeNull();
        query.ProjectStatus.Should().BeNull();
        query.AcceptanceRequestedAndNotConfirmed.Should().BeNull();
        query.PageNo.Should().Be(1);
        query.PageSize.Should().Be(10);
    }

    [Fact]
    public void Map_RequestWithDefaultPagination_MapsDefaultValuesCorrectly()
    {
        // Arrange
        var request = new GetProjectsByEmployerFilterRequest(
            UpdatedAtStartDate: new DateTime(2025, 1, 1),
            UpdatedAtEndDate: null,
            ProjectStatus: ProjectStatus.Completed,
            AcceptanceRequestedAndNotConfirmed: false);

        // Act
        var query = _mapper.Map<GetProjectsByEmployerFilterQuery>(request);

        // Assert
        query.Should().NotBeNull();
        query.UpdatedAtStartDate.Should().Be(new DateTime(2025, 1, 1));
        query.UpdatedAtEndDate.Should().BeNull();
        query.ProjectStatus.Should().Be(ProjectStatus.Completed);
        query.AcceptanceRequestedAndNotConfirmed.Should().BeFalse();
        query.PageNo.Should().Be(1);
        query.PageSize.Should().Be(10);
    }
}