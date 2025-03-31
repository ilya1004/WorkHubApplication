namespace IdentityService.BLL.Settings;

public class AzuriteSettings
{
    public required string ConnectionString { get; init; }
    public required string ImagesContainerName { get; init; }
}