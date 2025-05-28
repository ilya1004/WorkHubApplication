using Freelancers;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Infrastructure.Mapping.GrpcMappingProfiles;

namespace PaymentsService.Tests.UnitTests.Tests.Mapping.InfrastructureMapping.GrpcMappingProfiles;

public class GetFreelancerByIdResponseToFreelancerDtoTests
{
    private readonly IMapper _mapper;

    public GetFreelancerByIdResponseToFreelancerDtoTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GetFreelancerByIdResponseToFreelancerDto>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_ValidResponseWithStripeAccountId_MapsToFreelancerDto()
    {
        // Arrange
        var response = new GetFreelancerByIdResponse
        {
            Id = "fre_123",
            StripeAccountId = "acct_456"
        };

        // Act
        var result = _mapper.Map<FreelancerDto>(response);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("fre_123");
        result.StripeAccountId.Should().Be("acct_456");
    }

    [Fact]
    public void Map_ResponseWithoutStripeAccountId_MapsToFreelancerDtoWithNullStripeAccountId()
    {
        // Arrange
        var response = new GetFreelancerByIdResponse
        {
            Id = "fre_123"
        };

        // Act
        var result = _mapper.Map<FreelancerDto>(response);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("fre_123");
        result.StripeAccountId.Should().BeEmpty();
    }
}