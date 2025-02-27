using Microsoft.Extensions.Configuration;

namespace ChatService.Domain.Abstractions.DbInitializer;

public interface IDbInitializer
{
    Task InitializeDbAsync(IConfiguration configuration);
}