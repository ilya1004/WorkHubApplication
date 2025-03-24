namespace IdentityService.BLL.Abstractions.AzuriteStartupService;

public interface IAzuriteStartupService
{
    Task CreateContainerIfNotExistAsync();
}