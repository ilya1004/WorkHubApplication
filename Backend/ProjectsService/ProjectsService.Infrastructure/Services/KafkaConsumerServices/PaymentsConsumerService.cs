using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ProjectsService.Application.Exceptions;
using ProjectsService.Infrastructure.DTOs;

namespace ProjectsService.Infrastructure.Services.KafkaConsumerServices;

public class PaymentsConsumerService(
    IOptions<KafkaSettings> options,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            GroupId = "payments_group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        
        consumer.Subscribe(options.Value.PaymentIntentSavingTopic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    var dto = JsonSerializer.Deserialize<SavePaymentIntentIdDto>(result.Message.Value);
                    
                    using var scope = serviceScopeFactory.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(
                        Guid.Parse(dto!.ProjectId), stoppingToken);

                    if (project is null)
                    {
                        throw new NotFoundException($"Project with ID '{dto.ProjectId}' not found");
                    }
                    
                    project.PaymentIntentId = dto.PaymentIntentId;

                    await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, stoppingToken);
                    await unitOfWork.SaveAllAsync(stoppingToken);
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