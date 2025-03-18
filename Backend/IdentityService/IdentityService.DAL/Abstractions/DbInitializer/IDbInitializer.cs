namespace IdentityService.DAL.Abstractions.DbInitializer;

public interface IDbInitializer
{
    Task InitializeDb();
}