using Confluent.Kafka;
using IdentityService.BLL.Settings;
using IdentityService.DAL.Abstractions.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace IdentityService.BLL.Services.KafkaConsumerServices;

public class AccountsConsumerService(
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
        
        consumer.Subscribe([
            options.Value.EmployerAccountIdSavingTopic,
            options.Value.FreelancerAccountIdSavingTopic
        ]);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    using var scope = serviceScopeFactory.CreateScope();
                    if (result.Topic == options.Value.EmployerAccountIdSavingTopic)
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        
                        unitOfWork.EmployerProfilesRepository.
                    }

                    // using var scope = serviceScopeFactory.CreateScope();
                    var employerPaymentsService = scope.ServiceProvider.GetRequiredService<IEmployerPaymentsService>();

                    await employerPaymentsService.CancelPaymentForProjectAsync(result.Message.Value, stoppingToken);
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