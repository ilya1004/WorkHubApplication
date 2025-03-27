namespace PaymentsService.Infrastructure.Settings;

public class StripeSettings
{
    public required string SecretKey { get; init; }
    public required string PublishableKey { get; init; }
}