namespace PaymentsService.Domain.Abstractions.UserContext;

public interface IUserContext
{
    Guid GetUserId();
    string GetUserRole();
    string GetUserEmail();
}