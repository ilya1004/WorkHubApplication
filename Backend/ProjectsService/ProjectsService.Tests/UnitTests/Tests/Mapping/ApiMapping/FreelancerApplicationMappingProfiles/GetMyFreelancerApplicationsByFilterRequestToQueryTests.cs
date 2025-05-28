using ProjectsService.API.Contracts.FreelancerApplicationContracts;
using ProjectsService.API.Mapping.FreelancerApplicationMappingProfiles;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetMyFreelancerApplicationsByFilter;

namespace ProjectsService.Tests.UnitTests.Tests.Mapping.ApiMapping.FreelancerApplicationMappingProfiles;

public class GetMyFreelancerApplicationsByFilterRequestToQueryTests
{
    private readonly IMapper _mapper;

    public GetMyFreelancerApplicationsByFilterRequestToQueryTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GetMyFreelancerApplicationsByFilterRequestToQuery>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_RequestWithAllValues_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 12, 31);
        var request = new GetMyFreelancerApplicationsByFilterRequest(
            StartDate: startDate,
            EndDate: endDate,
            ApplicationStatus: ApplicationStatus.Rejected,
            PageNo: 3,
            PageSize: 15);

        // Act
        var query = _mapper.Map<GetMyFreelancerApplicationsByFilterQuery>(request);

        // Assert
        query.Should().NotBeNull();
        query.StartDate.Should().Be(startDate);
        query.EndDate.Should().Be(endDate);
        query.ApplicationStatus.Should().Be(ApplicationStatus.Rejected);
        query.PageNo.Should().Be(3);
        query.PageSize.Should().Be(15);
    }

    [Fact]
    public void Map_RequestWithNullValues_MapsNullPropertiesCorrectly()
    {
        // Arrange
        var request = new GetMyFreelancerApplicationsByFilterRequest(
            StartDate: null,
            EndDate: null,
            ApplicationStatus: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var query = _mapper.Map<GetMyFreelancerApplicationsByFilterQuery>(request);

        // Assert
        query.Should().NotBeNull();
        query.StartDate.Should().BeNull();
        query.EndDate.Should().BeNull();
        query.ApplicationStatus.Should().BeNull();
        query.PageNo.Should().Be(1);
        query.PageSize.Should().Be(10);
    }

    [Fact]
    public void Map_RequestWithDefaultPagination_MapsDefaultValuesCorrectly()
    {
        // Arrange
        var request = new GetMyFreelancerApplicationsByFilterRequest(
            StartDate: new DateTime(2025, 1, 1),
            EndDate: null,
            ApplicationStatus: ApplicationStatus.Accepted);

        // Act
        var query = _mapper.Map<GetMyFreelancerApplicationsByFilterQuery>(request);

        // Assert
        query.Should().NotBeNull();
        query.StartDate.Should().Be(new DateTime(2025, 1, 1));
        query.EndDate.Should().BeNull();
        query.ApplicationStatus.Should().Be(ApplicationStatus.Accepted);
        query.PageNo.Should().Be(1);
        query.PageSize.Should().Be(10);
    }
}