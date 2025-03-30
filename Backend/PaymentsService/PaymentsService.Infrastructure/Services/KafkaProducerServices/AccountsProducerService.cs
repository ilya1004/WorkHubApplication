using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;

namespace PaymentsService.Infrastructure.Services.KafkaProducerServices;

public class AccountsProducerService : IAccountsProducerService
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _employerAccountIdSavingTopic;
    private readonly string _freelancerAccountIdSavingTopic;
    private readonly ILogger<AccountsProducerService> _logger;
    
    public AccountsProducerService(
        IOptions<KafkaSettings> options,
        ILogger<AccountsProducerService> logger)
    {
        _logger = logger;
        
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            AllowAutoCreateTopics = true,
            Acks = Acks.All
        };
        
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        
        _logger.LogInformation("Kafka producer initialized");

        _employerAccountIdSavingTopic = options.Value.EmployerAccountIdSavingTopic;
        _freelancerAccountIdSavingTopic = options.Value.FreelancerAccountIdSavingTopic;
        
        _logger.LogInformation("Using topics: Employer={EmployerTopic}, Freelancer={FreelancerTopic}", 
            _employerAccountIdSavingTopic, _freelancerAccountIdSavingTopic);
    }
    
    public async Task SaveEmployerAccountIdAsync(string userEmployerId, string employerAccountId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Saving employer account ID {AccountId} for user {UserId}", 
            employerAccountId, userEmployerId);
            
        try
        {
            var dto = new SaveEmployerAccountIdDto
            {
                UserId = userEmployerId,
                EmployerAccountId = employerAccountId
            };

            var jsonData = JsonSerializer.Serialize(dto);
            
            var result = await _producer.ProduceAsync(_employerAccountIdSavingTopic, new Message<Null, string>
            {
                Value = jsonData
            }, cancellationToken);

            _logger.LogInformation("Successfully saved employer account ID {AccountId} for user {UserId}. " +
                                   "Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
                employerAccountId, userEmployerId, result.Topic, result.Partition, result.Offset);
        }
        catch (ProduceException<Null, string> ex)
        {
            _logger.LogError(ex, "Failed to save employer account ID {AccountId}. Kafka error: {Error}", 
                employerAccountId, ex.Error.Reason);
            
            throw new BadRequestException($"Employer account ID was not successfully saved. Producer exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save employer account ID {AccountId}", employerAccountId);
            
            throw new BadRequestException("Employer account ID was not successfully saved.");
        }
    }
    
    public async Task SaveFreelancerAccountIdAsync(string userFreelancerId, string freelancerAccountId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Saving freelancer account ID {AccountId} for user {UserId}", 
            freelancerAccountId, userFreelancerId);
            
        try
        {
            var dto = new SaveFreelancerAccountIdDto
            {
                UserId = userFreelancerId,
                FreelancerAccountId = freelancerAccountId
            };

            var jsonData = JsonSerializer.Serialize(dto);
            
            var result = await _producer.ProduceAsync(_freelancerAccountIdSavingTopic, new Message<Null, string>
            {
                Value = jsonData
            }, cancellationToken);

            _logger.LogInformation("Successfully saved freelancer account ID {AccountId} for user {UserId}. " +
                                   "Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
                freelancerAccountId, userFreelancerId, result.Topic, result.Partition, result.Offset);
        }
        catch (ProduceException<Null, string> ex)
        {
            _logger.LogError(ex, "Failed to save freelancer account ID {AccountId}. Kafka error: {Error}", 
                freelancerAccountId, ex.Error.Reason);
            
            throw new BadRequestException($"Freelancer account ID was not successfully saved. Producer exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save freelancer account ID {AccountId}", freelancerAccountId);
            
            throw new BadRequestException("Freelancer account ID was not successfully saved.");
        }
    }
}