using PaymentsService.Domain.Abstractions.Repositories;
using PaymentsService.Infrastructure.Data;

namespace PaymentsService.Infrastructure.Repositories;

public class AppUnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly Lazy<IRepository<Payment>> _paymentsRepository = 
        new(() => new AppRepository<Payment>(context));

    private readonly Lazy<IRepository<Transaction>> _transactionsRepository = 
        new(() => new AppRepository<Transaction>(context));
    
    public IRepository<Payment> PaymentsRepository => _paymentsRepository.Value;
    public IRepository<Transaction> TransactionsRepository => _transactionsRepository.Value;

    public async Task SaveAllAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}