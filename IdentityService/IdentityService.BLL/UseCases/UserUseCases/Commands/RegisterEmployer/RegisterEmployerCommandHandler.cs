using IdentityService.BLL.Services.EmailSender;
using IdentityService.DAL.Constants;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

public class RegisterEmployerCommandHandler(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IEmailSender emailSender) : IRequestHandler<RegisterEmployerCommand>
{
    public async Task Handle(RegisterEmployerCommand request, CancellationToken cancellationToken)
    {
        var userByEmail = await userManager.FindByEmailAsync(request.Email);

        if (userByEmail is not null)
        {
            throw new AlreadyExistsException($"A user with the email '{request.Email}' already exists.");
        }
        
        var user = mapper.Map<AppUser>(request);

        var role = await roleManager.FindByNameAsync(AppRoles.EmployerRole);

        if (role == null)
        {
            throw new BadRequestException($"User is not successfully registered. User Role is not successfully find");
        }

        user.RoleId = role.Id;

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new BadRequestException($"User is not successfully registered. Errors: {errors}");
        }

        var employerProfile = mapper.Map<EmployerProfile>(request);

        employerProfile.UserId = user.Id;

        await unitOfWork.EmployersRepository.AddAsync(employerProfile, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

        await emailSender.SendEmailConfirmation(user.Email!, code, cancellationToken);
    }
}
