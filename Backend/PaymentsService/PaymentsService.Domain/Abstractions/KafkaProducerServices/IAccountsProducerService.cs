namespace PaymentsService.Domain.Abstractions.KafkaProducerServices;

public interface IAccountsProducerService
{
    Task SaveEmployerAccountIdAsync(string userEmployerId, string employerAccountId,
        CancellationToken cancellationToken);
    Task SaveFreelancerAccountIdAsync(string userFreelancerId, string freelancerAccountId, 
        CancellationToken cancellationToken);
}
