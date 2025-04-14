using PaymentsService.API.Contracts.PaymentContracts;
using PaymentsService.API.Mapping.PaymentMappingProfiles;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerMyPaymentsQuery;

namespace PaymentsService.Tests.UnitTests.Tests.Mapping.ApiMapping;

public class GetOperationsRequestToGetEmployerPaymentsQueryTests
{
    private readonly IMapper _mapper;

    public GetOperationsRequestToGetEmployerPaymentsQueryTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GetOperationsRequestToGetEmployerPaymentsQuery>();
        });
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void Map_WithProjectId_MapsCorrectly()
    {
        // Arrange
        var request = new GetOperationsRequest(
            ProjectId: Guid.NewGuid(),
            PageNo: 3,
            PageSize: 25
        );

        // Act
        var result = _mapper.Map<GetEmployerMyPaymentsQuery>(request);

        // Assert
        result.Should().NotBeNull();
        result.ProjectId.Should().Be(request.ProjectId);
        result.PageNo.Should().Be(request.PageNo);
        result.PageSize.Should().Be(request.PageSize);
    }

    [Fact]
    public void Map_WithoutProjectId_MapsCorrectly()
    {
        // Arrange
        var request = new GetOperationsRequest(
            ProjectId: null,
            PageNo: 2,
            PageSize: 5
        );

        // Act
        var result = _mapper.Map<GetEmployerMyPaymentsQuery>(request);

        // Assert
        result.Should().NotBeNull();
        result.ProjectId.Should().BeNull();
        result.PageNo.Should().Be(request.PageNo);
        result.PageSize.Should().Be(request.PageSize);
    }

    [Fact]
    public void Map_WithDefaultValues_MapsCorrectly()
    {
        // Arrange
        var request = new GetOperationsRequest(
            ProjectId: Guid.NewGuid()
        );

        // Act
        var result = _mapper.Map<GetEmployerMyPaymentsQuery>(request);

        // Assert
        result.Should().NotBeNull();
        result.ProjectId.Should().Be(request.ProjectId);
        result.PageNo.Should().Be(1);
        result.PageSize.Should().Be(10);
    }
}