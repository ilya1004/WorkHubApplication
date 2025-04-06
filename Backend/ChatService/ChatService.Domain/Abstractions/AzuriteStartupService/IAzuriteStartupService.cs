namespace ChatService.Domain.Abstractions.AzuriteStartupService;

public interface IAzuriteStartupService
{
    Task CreateContainerIfNotExistAsync();
}