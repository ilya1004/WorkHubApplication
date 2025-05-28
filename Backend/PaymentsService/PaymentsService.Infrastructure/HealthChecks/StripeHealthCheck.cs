using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PaymentsService.Infrastructure.HealthChecks;

public class StripeHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var service = new BalanceService();
            await service.GetAsync(cancellationToken: cancellationToken);
            
            return HealthCheckResult.Healthy();
        }
        catch (StripeException ex)
        {
            return HealthCheckResult.Unhealthy("Stripe API error", ex);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Failed to connect to Stripe", ex);
        }
    }
}