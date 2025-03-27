using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Infrastructure.Settings;

namespace PaymentsService.Infrastructure.Services.KafkaConsumerServices;

public class PaymentsConsumerService(
    IOptions<KafkaSettings> options,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService 
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        
        consumer.Subscribe(options.Value.PaymentsTopic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    using var scope = serviceScopeFactory.CreateScope();
                    var employerPaymentsService = scope.ServiceProvider.GetRequiredService<IEmployerPaymentsService>();

                    await employerPaymentsService.CancelPaymentForProjectAsync(result.Message.Value, stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    // logging error
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }
}