namespace IdentityService.BLL.Settings;

public class KafkaSettings
{
    public required string BootstrapServers { get; init; }
    public required string EmployerAccountIdSavingTopic { get; init; }
    public required string FreelancerAccountIdSavingTopic { get; init; }
}