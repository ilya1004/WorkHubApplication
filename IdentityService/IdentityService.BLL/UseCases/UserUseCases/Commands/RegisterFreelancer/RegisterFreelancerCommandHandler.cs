using IdentityService.BLL.Services.EmailSender;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Constants;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

public class RegisterFreelancerCommandHandler(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IEmailSender emailSender) : IRequestHandler<RegisterFreelancerCommand>
{
    public async Task Handle(RegisterFreelancerCommand request, CancellationToken cancellationToken)
    {
        var userByEmail = await userManager.FindByEmailAsync(request.Email);

        if (userByEmail is not null)
        {
            throw new AlreadyExistsException($"A user with the email '{request.Email}' already exists.");
        }

        var user = mapper.Map<AppUser>(request);

        var role = await roleManager.FindByNameAsync(AppRoles.EmployerRole);

        if (role is null)
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

        var freelancerProfile = mapper.Map<FreelancerProfile>(request);

        freelancerProfile.UserId = user.Id;

        await unitOfWork.FreelancersRepository.AddAsync(freelancerProfile, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

        await emailSender.SendEmailConfirmation(user.Email!, code, cancellationToken);
    }
}
