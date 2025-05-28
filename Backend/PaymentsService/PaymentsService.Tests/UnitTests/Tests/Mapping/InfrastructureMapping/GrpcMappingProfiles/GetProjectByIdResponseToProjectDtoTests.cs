using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Infrastructure.Mapping.GrpcMappingProfiles;
using Projects;

namespace PaymentsService.Tests.UnitTests.Tests.Mapping.InfrastructureMapping.GrpcMappingProfiles;

public class GetProjectByIdResponseToProjectDtoTests
{
    private readonly IMapper _mapper;

    public GetProjectByIdResponseToProjectDtoTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GetProjectByIdResponseToProjectDto>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_ValidResponseWithAllFields_MapsToProjectDto()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var freelancerId = Guid.NewGuid();
        var response = new GetProjectByIdResponse
        {
            Id = projectId.ToString(),
            BudgetInCents = 10000,
            FreelancerId = freelancerId.ToString(),
            PaymentIntentId = "pi_789"
        };

        // Act
        var result = _mapper.Map<ProjectDto>(response);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(projectId);
        result.BudgetInCents.Should().Be(10000);
        result.FreelancerId.Should().Be(freelancerId);
        result.PaymentIntentId.Should().Be("pi_789");
    }

    [Fact]
    public void Map_ResponseWithEmptyFreelancerId_MapsToProjectDtoWithNullFreelancerId()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var response = new GetProjectByIdResponse
        {
            Id = projectId.ToString(),
            BudgetInCents = 10000,
            FreelancerId = "",
            PaymentIntentId = "pi_789"
        };

        // Act
        var result = _mapper.Map<ProjectDto>(response);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(projectId);
        result.BudgetInCents.Should().Be(10000);
        result.FreelancerId.Should().BeNull();
        result.PaymentIntentId.Should().Be("pi_789");
    }

    [Fact]
    public void Map_ResponseWithoutOptionalFields_MapsToProjectDtoWithNullFields()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var response = new GetProjectByIdResponse
        {
            Id = projectId.ToString(),
            BudgetInCents = 10000
        };

        // Act
        var result = _mapper.Map<ProjectDto>(response);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(projectId);
        result.BudgetInCents.Should().Be(10000);
        result.FreelancerId.Should().BeNull();
        result.PaymentIntentId.Should().BeEmpty();
    }
}