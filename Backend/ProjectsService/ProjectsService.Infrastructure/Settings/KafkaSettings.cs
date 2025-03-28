namespace ProjectsService.Infrastructure.Settings;

public class KafkaSettings
{
    public required string BootstrapServers { get; init; }
    public required string PaymentIntentSavingTopic { get; init; }
    public required string PaymentCancellationTopic { get; init; }
}