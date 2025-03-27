using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;
using PaymentsService.Infrastructure.Settings;

namespace PaymentsService.Infrastructure.Services.KafkaProducerServices;

public class AccountsProducerService : IAccountsProducerService
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _accountsTopic;

    public AccountsProducerService(IOptions<KafkaSettings> options)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            AllowAutoCreateTopics = true,
            Acks = Acks.All,
            RetryBackoffMs = 500,   // Задержка перед ретраем
            MessageTimeoutMs = 30000,  // 30 секунд ожидания
            SocketTimeoutMs = 30000,   // 30 секунд ожидания соединения
        };
        
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();

        _accountsTopic = options.Value.AccountsTopic;
    }
    
    public async Task SaveEmployerAccountId(string employerAccountId, CancellationToken cancellationToken)
    {
        try
        {
            await _producer.ProduceAsync(_accountsTopic, new Message<Null, string>
            {
                Value = employerAccountId,
                Headers = [new Header("event_type", Encoding.UTF8.GetBytes($"{nameof(SaveEmployerAccountId)}"))]
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
            await _producer.ProduceAsync(_accountsTopic, new Message<Null, string>
            {
                Value = freelancerAccountId,
                Headers = [new Header("event_type", Encoding.UTF8.GetBytes($"{nameof(SaveFreelancerAccountId)}"))]
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