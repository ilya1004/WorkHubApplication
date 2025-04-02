using System.Text.Json;
using Confluent.Kafka;
using IdentityService.BLL.DTOs;
using IdentityService.BLL.Settings;
using IdentityService.DAL.Abstractions.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityService.BLL.Services.KafkaConsumerServices;

public class EmployerAccountsConsumerService(
    IOptions<KafkaSettings> options,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<EmployerAccountsConsumerService> logger) : BackgroundService
{
    private IConsumer<Ignore, string> _consumer = null!;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting EmployerAccounts consumer service");
        
        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            GroupId = "accounts_group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _consumer.Subscribe(options.Value.EmployerAccountIdSavingTopic);

        logger.LogInformation("Subscribed to topic: {Topic}", options.Value.EmployerAccountIdSavingTopic);
        
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
                    
                    var dto = JsonSerializer.Deserialize<SaveEmployerAccountIdDto>(result.Message.Value);
                    
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

    private async Task ProcessMessageAsync(SaveEmployerAccountIdDto dto, CancellationToken stoppingToken)
    {
        logger.LogInformation("Processing employer account ID for user {UserId}", dto.UserId);
        
        using var scope = serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var employerProfile = await unitOfWork.EmployerProfilesRepository.FirstOrDefaultAsync(
            ep => ep.UserId == Guid.Parse(dto.UserId), stoppingToken);

        if (employerProfile is null)
        {
            logger.LogWarning("Employer profile not found for user {UserId}", dto.UserId);
            
            throw new BadRequestException($"Employer profile with user ID '{dto.UserId}' not found");
        }

        employerProfile.StripeCustomerId = dto.EmployerAccountId;

        await unitOfWork.EmployerProfilesRepository.UpdateAsync(employerProfile, stoppingToken);
        await unitOfWork.SaveAllAsync(stoppingToken);
        
        logger.LogInformation("Successfully updated employer profile for user {UserId}", dto.UserId);
    }
}