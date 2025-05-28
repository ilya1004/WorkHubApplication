using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Infrastructure.Interfaces;
using PaymentsService.Infrastructure.Settings;
using Stripe;

namespace PaymentsService.Tests.IntegrationTests.Helpers;

public class StripeIntegrationTestsFixture : IAsyncLifetime
{
    internal WebApplicationFactory<Program> Factory { get; set; }
    public Mock<IEmployersGrpcClient> EmployersGrpcClientMock { get; set; }
    public Mock<IFreelancersGrpcClient> FreelancersGrpcClientMock { get; set; }
    public Mock<IProjectsGrpcClient> ProjectsGrpcClientMock { get; set; }
    public Mock<ITransfersService> TransfersServiceMock { get; set; }
    public Mock<IPaymentsProducerService> PaymentsProducerServiceMock { get; set; }

    public StripeIntegrationTestsFixture()
    {
        EmployersGrpcClientMock = new Mock<IEmployersGrpcClient>();
        FreelancersGrpcClientMock = new Mock<IFreelancersGrpcClient>();
        ProjectsGrpcClientMock = new Mock<IProjectsGrpcClient>();
        TransfersServiceMock = new Mock<ITransfersService>();
        PaymentsProducerServiceMock = new Mock<IPaymentsProducerService>();

        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptorsToRemove = services.Where(d =>
                    d.ServiceType == typeof(IHostedService) ||
                    d.ServiceType == typeof(IEmployersGrpcClient) ||
                    d.ServiceType == typeof(IFreelancersGrpcClient) ||
                    d.ServiceType == typeof(IProjectsGrpcClient) ||
                    d.ServiceType == typeof(ITransfersService) ||
                    d.ServiceType == typeof(IPaymentsProducerService)
                ).ToList();
                foreach (var descriptor in descriptorsToRemove)
                {
                    services.Remove(descriptor);
                }

                services.AddScoped(_ => EmployersGrpcClientMock.Object);
                services.AddScoped(_ => FreelancersGrpcClientMock.Object);
                services.AddScoped(_ => ProjectsGrpcClientMock.Object);
                services.AddScoped(_ => TransfersServiceMock.Object);
                services.AddScoped(_ => PaymentsProducerServiceMock.Object);
            });

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {
                        "StripeSettings:SecretKey",
                        "sk_test_51Qxs52H5FezzbHNwwSV4gO6e4eAVrFWctcoiwHX5o2fDSNgX4gmwnhR5eEhJDQSriCoghMoouEvlkQFmdDFlHBds00LbBi62uj"
                    },
                    {
                        "StripeSettings:PublishableKey",
                        "pk_test_51Qxs52H5FezzbHNwOOPYQiMuf9iwdbxYnZE77RTURd6MqdO3M4aBsJNGmhgtiyZIsv3AXhZhRa5jYMnH0jBQnceL00IVo8QAmZ"
                    },
                    { "GrpcSettings:IdentityServiceAddress", "http://localhost:5001" },
                    { "GrpcSettings:ProjectsServiceAddress", "http://localhost:5002" }
                });
            });
        });
    }

    public Task InitializeAsync()
    {
        var stripeSettings = Factory.Services.GetRequiredService<IOptions<StripeSettings>>().Value;
        StripeConfiguration.ApiKey = stripeSettings.SecretKey;
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await Factory.DisposeAsync();
    }
}