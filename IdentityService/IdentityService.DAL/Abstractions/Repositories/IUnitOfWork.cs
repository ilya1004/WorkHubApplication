using IdentityService.DAL.Abstractions.Repositories;

namespace IdentityService.DAL.Abstractions.Data;

public interface IUnitOfWork
{
    public IEmployersRepository EmployersRepository { get; }
    public IFreelancersRepository FreelancersRepository { get; }
    public Task SaveAllAsync(CancellationToken cancellationToken);
}
