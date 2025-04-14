using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.Mapping.PaymentsMappingProfiles;
using Stripe;

namespace PaymentsService.Tests.UnitTests.Tests.Mapping.InfrastructureMapping.PaymentsMappingProfiles;

public class StripePaymentMethodToPaymentMethodModelTests
{
    private readonly IMapper _mapper;

    public StripePaymentMethodToPaymentMethodModelTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<StripePaymentMethodToPaymentMethodModel>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_ValidPaymentMethodWithCard_MapsToPaymentMethodModel()
    {
        // Arrange
        var created = DateTime.UtcNow;
        var paymentMethod = new PaymentMethod
        {
            Id = "pm_123",
            Type = "card",
            Card = new PaymentMethodCard
            {
                Brand = "visa",
                Country = "US",
                ExpMonth = 12,
                ExpYear = 2025,
                Last4 = "4242"
            },
            Created = created
        };

        // Act
        var result = _mapper.Map<PaymentMethodModel>(paymentMethod);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("pm_123");
        result.Type.Should().Be("card");
        result.CreatedAt.Should().Be(created);
        result.Card.Should().NotBeNull();
        result.Card!.Brand.Should().Be("visa");
        result.Card.Country.Should().Be("US");
        result.Card.ExpMonth.Should().Be(12);
        result.Card.ExpYear.Should().Be(2025);
        result.Card.Last4Digits.Should().Be("4242");
    }

    [Fact]
    public void Map_PaymentMethodWithoutCard_MapsToPaymentMethodModelWithNullCard()
    {
        // Arrange
        var created = DateTime.UtcNow;
        var paymentMethod = new PaymentMethod
        {
            Id = "pm_123",
            Type = "card",
            Card = null,
            Created = created
        };

        // Act
        var result = _mapper.Map<PaymentMethodModel>(paymentMethod);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("pm_123");
        result.Type.Should().Be("card");
        result.CreatedAt.Should().Be(created);
        result.Card.Should().BeNull();
    }
}