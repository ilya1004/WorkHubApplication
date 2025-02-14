namespace ProjectsService.Domain.Abstractions.Data;

public interface IUnitOfWork
{
    
    
    public Task SaveAllAsync(CancellationToken cancellationToken);
}