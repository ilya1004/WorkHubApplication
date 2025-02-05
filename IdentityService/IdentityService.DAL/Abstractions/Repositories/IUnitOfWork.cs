namespace IdentityService.DAL.Abstractions.Data;

public interface IUnitOfWork
{
    public Task SaveAllAsync(CancellationToken cancellationToken);
}
