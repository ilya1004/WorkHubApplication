namespace PaymentsService.Infrastructure.Settings;

public class KafkaSettings
{
    public required string BootstrapServers { get; init; }
    public required string AccountsTopic { get; init; }
    public required string PaymentsTopic { get; init; }
}