namespace ProjectsService.Infrastructure.Settings;

public class KafkaSettings
{
    public string BootstrapServers { get; init; }
    public string PaymentIntentSavingTopic { get; init; }
    public string PaymentCancellationTopic { get; init; }
}