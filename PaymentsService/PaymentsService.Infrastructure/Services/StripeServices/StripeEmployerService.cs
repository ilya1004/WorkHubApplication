using PaymentsService.Applications.Constants;
using PaymentsService.Domain.Abstractions.EmployerService;


namespace PaymentsService.Infrastructure.Services.StripeServices;

public class StripeEmployerService : IEmployerService
{
    private readonly CustomerService _customerService = new();

    public async Task<string?> CreateEmployerAccountAsync(Guid userId, string companyName, string email)
    {
        var options = new CustomerCreateOptions
        {
            Email = email,
            Name = companyName,
            Metadata = new Dictionary<string, string>
            {
                { "UserId", userId.ToString() },
                { "Role", AppRoles.EmployerRole }
            }
        };

        try
        {
            var customer = await _customerService.CreateAsync(options);
            return customer.Id;
        }
        catch (Exception)
        {
            return null;
        }
    }
}