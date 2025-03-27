namespace PaymentsService.Domain.Abstractions.KafkaProducerServices;

public interface IAccountsProducerService
{
    Task SaveEmployerAccountId(string employerAccountId, CancellationToken cancellationToken);
    Task SaveFreelancerAccountId(string freelancerAccountId, CancellationToken cancellationToken);
}
