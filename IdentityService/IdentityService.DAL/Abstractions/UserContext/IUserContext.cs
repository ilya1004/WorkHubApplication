namespace IdentityService.DAL.Abstractions.UserContext;

public interface IUserContext
{
    Guid GetUserId();
    string GetUserRole();
}