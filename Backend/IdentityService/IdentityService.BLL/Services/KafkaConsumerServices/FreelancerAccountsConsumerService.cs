using System.Text.Json;
using Confluent.Kafka;
using IdentityService.BLL.DTOs;
using IdentityService.BLL.Settings;
using IdentityService.DAL.Abstractions.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace IdentityService.BLL.Services.KafkaConsumerServices;

public class FreelancerAccountsConsumerService(
    IOptions<KafkaSettings> options,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private IConsumer<Ignore, string> _consumer = null!;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            GroupId = "accounts_group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _consumer.Subscribe(options.Value.FreelancerAccountIdSavingTopic);

        await Task.Run(() => ConsumeMessagesAsync(stoppingToken), stoppingToken);
    }

    private async Task ConsumeMessagesAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);
                    var dto = JsonSerializer.Deserialize<SaveFreelancerAccountIdDto>(result.Message.Value);
                    
                    if (dto is null)
                    {
                        throw new BadRequestException("Error occurred during message deserialization");
                    }

                    await ProcessMessageAsync(dto, stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    // Логирование ошибки Kafka
                }
                catch (Exception ex)
                {
                    // Логирование ошибки
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Логирование завершения работы
        }
        finally
        {
            _consumer.Close();
        }
    }

    private async Task ProcessMessageAsync(SaveFreelancerAccountIdDto dto, CancellationToken stoppingToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var freelancerProfile = await unitOfWork.FreelancerProfilesRepository.FirstOrDefaultAsync(
            fp => fp.UserId == Guid.Parse(dto.UserId), stoppingToken);

        if (freelancerProfile is null)
        {
            throw new BadRequestException($"Freelancer profile with user ID '{dto.UserId}' not found");
        }

        freelancerProfile.StripeAccountId = dto.FreelancerAccountId;

        await unitOfWork.FreelancerProfilesRepository.UpdateAsync(freelancerProfile, stoppingToken);
        await unitOfWork.SaveAllAsync(stoppingToken);
    }
}