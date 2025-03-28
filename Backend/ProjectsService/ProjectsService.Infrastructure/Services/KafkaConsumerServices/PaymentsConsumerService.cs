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
    private IConsumer<Ignore, string> _consumer = null!;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            GroupId = "payments_group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        
        _consumer.Subscribe(options.Value.PaymentIntentSavingTopic);

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
                    var dto = JsonSerializer.Deserialize<SavePaymentIntentIdDto>(result.Message.Value);
                    
                    if (dto is null)
                    {
                        throw new BadRequestException("Error occurred during message deserialization");
                    }

                    await ProcessMessageAsync(dto, stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    // Логирование ConsumeException
                }
                catch (Exception ex)
                {
                    // Логирование Exception
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Логирование OperationCanceledException
        }
        finally
        {
            _consumer.Close();
        }
    }

    private async Task ProcessMessageAsync(SavePaymentIntentIdDto dto, CancellationToken stoppingToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(
            Guid.Parse(dto.ProjectId), stoppingToken);

        if (project is null)
        {
            throw new NotFoundException($"Project with ID '{dto.ProjectId}' not found");
        }

        project.PaymentIntentId = dto.PaymentIntentId;

        await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, stoppingToken);
        await unitOfWork.SaveAllAsync(stoppingToken);
    }
}