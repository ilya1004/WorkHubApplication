using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PaymentsService.Domain.Abstractions.PaymentsServices;

namespace PaymentsService.Infrastructure.Services.KafkaConsumerServices;

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
        _consumer.Subscribe(options.Value.PaymentCancellationTopic);

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
                    
                    using var scope = serviceScopeFactory.CreateScope();
                    var employerPaymentsService = scope.ServiceProvider.GetRequiredService<IEmployerPaymentsService>();

                    await employerPaymentsService.CancelPaymentForProjectAsync(result.Message.Value, stoppingToken);
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
}