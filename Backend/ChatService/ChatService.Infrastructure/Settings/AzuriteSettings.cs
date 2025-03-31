namespace ChatService.Infrastructure.Settings;

public class AzuriteSettings
{
    public required string ConnectionString { get; init; }
    public required string FilesContainerName { get; init; }
}