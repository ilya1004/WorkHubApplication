using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.Mapping.PaymentsMappingProfiles;
using Stripe;

namespace PaymentsService.Tests.UnitTests.Tests.Mapping.InfrastructureMapping.PaymentsMappingProfiles;

public class TransferToTransferModelTests
{
    private readonly IMapper _mapper;

    public TransferToTransferModelTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TransferToTransferModel>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_ValidTransfer_MapsToTransferModel()
    {
        // Arrange
        var transfer = new Transfer
        {
            Id = "tr_123",
            Amount = 500,
            Currency = "usd",
            TransferGroup = "group_456",
            Metadata = new Dictionary<string, string> { { "key", "value" } }
        };

        // Act
        var result = _mapper.Map<TransferModel>(transfer);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("tr_123");
        result.Amount.Should().Be(500);
        result.Currency.Should().Be("usd");
        result.TransferGroup.Should().Be("group_456");
        result.Metadata.Should().BeEquivalentTo(new Dictionary<string, string> { { "key", "value" } });
    }

    [Fact]
    public void Map_TransferWithNullMetadata_MapsToTransferModelWithNullMetadata()
    {
        // Arrange
        var transfer = new Transfer
        {
            Id = "tr_123",
            Amount = 500,
            Currency = "usd",
            TransferGroup = "group_456",
            Metadata = null
        };

        // Act
        var result = _mapper.Map<TransferModel>(transfer);

        // Assert
        result.Should().NotBeNull();
        result.Metadata.Should().BeEmpty();
    }
}