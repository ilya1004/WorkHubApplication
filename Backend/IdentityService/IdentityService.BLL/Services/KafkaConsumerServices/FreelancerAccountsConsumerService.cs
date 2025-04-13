using System.Text.Json;
using Confluent.Kafka;
using IdentityService.BLL.DTOs;
using IdentityService.BLL.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityService.BLL.Services.KafkaConsumerServices;

public class FreelancerAccountsConsumerService(
    IOptions<KafkaSettings> options,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<FreelancerAccountsConsumerService> logger) : BackgroundService
{
    private IConsumer<Ignore, string> _consumer = null!;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting FreelancerAccounts consumer service");
        
        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            GroupId = "accounts_group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _consumer.Subscribe(options.Value.FreelancerAccountIdSavingTopic);

        logger.LogInformation("Subscribed to topic: {Topic}", options.Value.FreelancerAccountIdSavingTopic);
        
        await Task.Run(() => ConsumeMessagesAsync(stoppingToken), stoppingToken);
    }

    private async Task ConsumeMessagesAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Starting to consume messages");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);
                    
                    logger.LogInformation("Received message: {Message}", result.Message.Value);
                    
                    var dto = JsonSerializer.Deserialize<SaveFreelancerAccountIdDto>(result.Message.Value);
                    
                    if (dto is null)
                    {
                        logger.LogWarning("Failed to deserialize message: {Message}", result.Message.Value);
                        
                        throw new BadRequestException("Error occurred during message deserialization");
                    }

                    await ProcessMessageAsync(dto, stoppingToken);
                    
                    logger.LogInformation("Successfully processed message for user {UserId}", dto.UserId);
                }
                catch (ConsumeException ex)
                {
                    logger.LogError(ex, "Kafka consume error");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing message");
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Consumer service stopping");
        }
        finally
        {
            _consumer.Close();
            
            logger.LogInformation("Consumer closed");
        }
    }

    private async Task ProcessMessageAsync(SaveFreelancerAccountIdDto dto, CancellationToken stoppingToken)
    {
        logger.LogInformation("Processing freelancer account ID '{AccountId}' for user {UserId}", dto.FreelancerAccountId, dto.UserId);
        
        using var scope = serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var freelancerProfile = await unitOfWork.FreelancerProfilesRepository.FirstOrDefaultAsync(
            fp => fp.UserId == Guid.Parse(dto.UserId), stoppingToken);

        if (freelancerProfile is null)
        {
            logger.LogWarning("Freelancer profile not found for user {UserId}", dto.UserId);
            
            throw new BadRequestException($"Freelancer profile with user ID '{dto.UserId}' not found");
        }

        freelancerProfile.StripeAccountId = dto.FreelancerAccountId;

        await unitOfWork.FreelancerProfilesRepository.UpdateAsync(freelancerProfile, stoppingToken);
        await unitOfWork.SaveAllAsync(stoppingToken);
        
        logger.LogInformation("Successfully updated freelancer profile for user {UserId}", dto.UserId);
    }
}