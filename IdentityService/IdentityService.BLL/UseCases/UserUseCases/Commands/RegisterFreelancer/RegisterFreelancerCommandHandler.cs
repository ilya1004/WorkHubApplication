using IdentityService.BLL.Services.EmailSender;
using IdentityService.DAL.Constants;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

public class RegisterFreelancerCommandHandler(
    UserManager<AppUser> userManager, 
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

        var result1 = await userManager.CreateAsync(user, request.Password);

        if (!result1.Succeeded)
        {
            var errors = string.Join("; ", result1.Errors.Select(e => e.Description));
            throw new BadRequestException($"User is not successfully registered. Errors: {errors}");
        }

        var result2 = await userManager.AddToRoleAsync(user, AppRoles.FreelancerRole);
        
        if (!result2.Succeeded)
        {
            var errors = string.Join("; ", result2.Errors.Select(e => e.Description));
            throw new BadRequestException($"User is not successfully registered. Errors: {errors}");
        }

        var freelancerProfile = mapper.Map<FreelancerProfile>(request);
        
        freelancerProfile.UserId = user.Id;

        await unitOfWork.FreelancersRepository.AddAsync(freelancerProfile, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

        //var emailVerificationToken = new EmailVerificationToken
        //{
        //    Id = Guid.NewGuid(),
        //    UserId = user.Id,
        //    Code = code,
        //    CreatedAt = DateTime.UtcNow,
        //};

        await emailSender.SendEmailConfirmation(user.Email!, code, cancellationToken);
    }
}
