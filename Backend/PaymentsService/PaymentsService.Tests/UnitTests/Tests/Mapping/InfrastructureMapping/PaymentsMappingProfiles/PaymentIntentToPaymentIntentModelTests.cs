using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.Mapping.PaymentsMappingProfiles;
using Stripe;

namespace PaymentsService.Tests.UnitTests.Tests.Mapping.InfrastructureMapping.PaymentsMappingProfiles;

public class PaymentIntentToPaymentIntentModelTests
{
    private readonly IMapper _mapper;

    public PaymentIntentToPaymentIntentModelTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PaymentIntentToPaymentIntentModel>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_ValidPaymentIntent_MapsToPaymentIntentModel()
    {
        // Arrange
        var created = DateTime.UtcNow;
        var paymentIntent = new PaymentIntent
        {
            Id = "pi_123",
            Amount = 2000,
            Currency = "eur",
            Status = "requires_payment_method",
            Created = created,
            TransferGroup = "group_789",
            Metadata = new Dictionary<string, string> { { "key", "value" } }
        };

        // Act
        var result = _mapper.Map<PaymentIntentModel>(paymentIntent);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("pi_123");
        result.Amount.Should().Be(2000);
        result.Currency.Should().Be("eur");
        result.Status.Should().Be("requires_payment_method");
        result.Created.Should().Be(created);
        result.TransferGroup.Should().Be("group_789");
        result.Metadata.Should().BeEquivalentTo(new Dictionary<string, string> { { "key", "value" } });
    }

    [Fact]
    public void Map_PaymentIntentWithNullTransferGroup_MapsToEmptyString()
    {
        // Arrange
        var created = DateTime.UtcNow;
        var paymentIntent = new PaymentIntent
        {
            Id = "pi_123",
            Amount = 2000,
            Currency = "eur",
            Status = "requires_payment_method",
            Created = created,
            TransferGroup = null,
            Metadata = new Dictionary<string, string> { { "key", "value" } }
        };

        // Act
        var result = _mapper.Map<PaymentIntentModel>(paymentIntent);

        // Assert
        result.Should().NotBeNull();
        result.TransferGroup.Should().BeEmpty();
    }

    [Fact]
    public void Map_PaymentIntentWithNullMetadata_MapsToEmptyDictionary()
    {
        // Arrange
        var created = DateTime.UtcNow;
        var paymentIntent = new PaymentIntent
        {
            Id = "pi_123",
            Amount = 2000,
            Currency = "eur",
            Status = "requires_payment_method",
            Created = created,
            TransferGroup = "group_789",
            Metadata = null
        };

        // Act
        var result = _mapper.Map<PaymentIntentModel>(paymentIntent);

        // Assert
        result.Should().NotBeNull();
        result.Metadata.Should().BeEmpty();
    }
}