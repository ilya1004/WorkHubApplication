using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.Mapping.PaymentsMappingProfiles;
using Stripe;

namespace PaymentsService.Tests.UnitTests.Tests.Mapping.InfrastructureMapping.PaymentsMappingProfiles;

public class ChargeToChargeModelTests
{
    private readonly IMapper _mapper;

    public ChargeToChargeModelTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ChargeToChargeModel>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_ValidCharge_MapsToChargeModel()
    {
        // Arrange
        var charge = new Charge
        {
            Id = "ch_123",
            Amount = 1000,
            Currency = "usd",
            Captured = true,
            Status = "succeeded",
            PaymentMethod = "pm_456"
        };

        // Act
        var result = _mapper.Map<ChargeModel>(charge);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("ch_123");
        result.Amount.Should().Be(1000);
        result.Currency.Should().Be("usd");
        result.Captured.Should().BeTrue();
        result.Status.Should().Be("succeeded");
        result.PaymentMethod.Should().Be("pm_456");
    }
}