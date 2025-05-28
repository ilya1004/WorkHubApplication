using Employers;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Infrastructure.Mapping.GrpcMappingProfiles;

namespace PaymentsService.Tests.UnitTests.Tests.Mapping.InfrastructureMapping.GrpcMappingProfiles;

public class GetEmployerByIdResponseToEmployerDtoTests
{
    private readonly IMapper _mapper;

    public GetEmployerByIdResponseToEmployerDtoTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GetEmployerByIdResponseToEmployerDto>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_ValidResponseWithCustomerId_MapsToEmployerDto()
    {
        // Arrange
        var response = new GetEmployerByIdResponse
        {
            Id = "emp_123",
            EmployerCustomerId = "cus_456"
        };

        // Act
        var result = _mapper.Map<EmployerDto>(response);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("emp_123");
        result.EmployerCustomerId.Should().Be("cus_456");
    }

    [Fact]
    public void Map_ResponseWithoutCustomerId_MapsToEmployerDtoWithNullCustomerId()
    {
        // Arrange
        var response = new GetEmployerByIdResponse
        {
            Id = "emp_123"
        };

        // Act
        var result = _mapper.Map<EmployerDto>(response);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("emp_123");
        result.EmployerCustomerId.Should().BeEmpty();
    }
}