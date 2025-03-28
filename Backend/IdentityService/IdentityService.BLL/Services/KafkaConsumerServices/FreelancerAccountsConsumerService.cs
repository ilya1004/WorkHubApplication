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
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            GroupId = "accounts_group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        
        consumer.Subscribe(options.Value.FreelancerAccountIdSavingTopic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    using var scope = serviceScopeFactory.CreateScope();
                    var dto = JsonSerializer.Deserialize<SaveFreelancerAccountIdDto>(result.Message.Value);

                    if (dto is null)
                    {
                        throw new BadRequestException("Some error is occured in Message deserialization");
                    }
                        
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var freelancerProfile = await unitOfWork.FreelancerProfilesRepository.FirstOrDefaultAsync(
                        fp => fp.UserId == Guid.Parse(dto.UserId), stoppingToken);

                    if (freelancerProfile is null)
                    {
                        throw new BadRequestException($"Freelancer profile with user ID '{dto.UserId}' not found");
                    }

                    freelancerProfile.StripeAccountId = dto.FreelancerAccountId;
                    Console.WriteLine(dto.FreelancerAccountId);       
                    // await unitOfWork.FreelancerProfilesRepository.UpdateAsync(freelancerProfile, stoppingToken);
                    // await unitOfWork.SaveAllAsync(stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    // logging error
                }
            }
        }
        catch (OperationCanceledException ex)
        {
            // logging error
        }
        finally
        {
            consumer.Close();
        }
    }
}