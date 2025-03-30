namespace IdentityService.BLL.Settings;

public class AzuriteSettings
{
    public required string ConnectionString { get; set; }
    public required string FilesContainerName { get; set; }
}