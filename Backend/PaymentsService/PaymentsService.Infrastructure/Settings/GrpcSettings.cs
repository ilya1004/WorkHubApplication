namespace PaymentsService.Infrastructure.Settings;

public class GrpcSettings
{
    public required string ProjectsServiceAddress { get; init; }
    public required string IdentityServiceAddress { get; init; }
}