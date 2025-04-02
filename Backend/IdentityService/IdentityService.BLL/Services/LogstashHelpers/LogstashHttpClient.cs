using System.Text;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.Http;

namespace IdentityService.BLL.Services.LogstashHelpers;

public class LogstashHttpClient : IHttpClient
{
    private readonly HttpClient _httpClient = new();

    public void Configure(IConfiguration configuration) { }

    public async Task<HttpResponseMessage> PostAsync(
        string requestUri, 
        Stream contentStream, 
        CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(contentStream);
        var rawContent = await reader.ReadToEndAsync(cancellationToken);

        var cleanContent = new StringBuilder();
        foreach (var line in rawContent.Split('\n'))
        {
            var trimmedLine = line.Trim()[1..].Replace("\"", "");
            if (trimmedLine.StartsWith('['))
            {
                cleanContent.AppendLine(trimmedLine);
            }
        }

        var content = new StringContent(
            cleanContent.ToString(),
            Encoding.UTF8,
            "text/plain");

        return await _httpClient.PostAsync(requestUri, content, cancellationToken);
    }

    public void Dispose() => _httpClient.Dispose();
}