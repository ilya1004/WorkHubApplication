using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;
using PaymentsService.Infrastructure.Settings;

namespace PaymentsService.Infrastructure.Services.KafkaProducerServices;

public class AccountsProducerService : IAccountsProducerService
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _employerAccountIdSavingTopic;
    private readonly string _freelancerAccountIdSavingTopic;
    
    public AccountsProducerService(IOptions<KafkaSettings> options)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            AllowAutoCreateTopics = true,
            Acks = Acks.All
        };
        
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();

        _employerAccountIdSavingTopic = options.Value.EmployerAccountIdSavingTopic;
        _freelancerAccountIdSavingTopic = options.Value.FreelancerAccountIdSavingTopic;
    }
    
    public async Task SaveEmployerAccountId(string employerAccountId, CancellationToken cancellationToken)
    {
        try
        {
            await _producer.ProduceAsync(_employerAccountIdSavingTopic, new Message<Null, string>
            {
                Value = employerAccountId
            }, cancellationToken);
        }
        catch (ProduceException<Null, string> ex)
        {
            throw new BadRequestException(
                $"Employer account ID was not successfully saved. Producer exception: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException("Employer account ID was not successfully saved.");
        }
    }
    
    public async Task SaveFreelancerAccountId(string freelancerAccountId, CancellationToken cancellationToken)
    {
        try
        {
            await _producer.ProduceAsync(_freelancerAccountIdSavingTopic, new Message<Null, string>
            {
                Value = freelancerAccountId
            }, cancellationToken);
        }
        catch (ProduceException<Null, string> ex)
        {
            throw new BadRequestException(
                $"Freelancer account ID was not successfully saved. Producer exception: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException("Freelancer account ID was not successfully saved.");
        }
    }
}