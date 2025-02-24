namespace ChatService.Infrastructure.Settings;

public class AzuriteSettings
{
    public required string ConnectionString { get; set; }
    public required string FilesContainerName { get; set; }
}