using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace IdentityService.BLL.HealthChecks;

public class SmtpHealthCheck(IConfiguration configuration) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var host = configuration["EmailSenderMailHog:Host"]!;
            var port = int.Parse(configuration["EmailSenderMailHog:Port"]!); 

            using var tcpClient = new TcpClient();
            
            await tcpClient.ConnectAsync(host, port, cancellationToken);
            
            return tcpClient.Connected ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("SMTP server error", ex);
        }
    }
}