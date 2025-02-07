using IdentityService.BLL.Services.EmailSender;
using IdentityService.BLL.Services.EmailVerificationCodeService;
using IdentityService.DAL.Constants;
using IdentityService.DAL.Models;
using IdentityService.DAL.Services.TokenCacheService;
using System.Security.Claims;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

public class RegisterEmployerCommandHandler(
    UserManager<AppUser> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IEmailSender emailSender,
    ITokenCacheService tokenCacheService,
    IEmailVerificationCodeProvider emailVerificationCodeProvider) : IRequestHandler<RegisterEmployerCommand>
{
    public async Task Handle(RegisterEmployerCommand request, CancellationToken cancellationToken)
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

        var result2 = await userManager.AddToRoleAsync(user, AppRoles.EmployerRole);

        if (!result2.Succeeded)
        {
            var errors = string.Join("; ", result2.Errors.Select(e => e.Description));
            throw new BadRequestException($"User is not successfully registered. Errors: {errors}");
        }

        var employerProfile = mapper.Map<EmployerProfile>(request);

        employerProfile.UserId = user.Id;

        await unitOfWork.EmployersRepository.AddAsync(employerProfile, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var emailVerificationToken = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = code,
            CreatedAt = DateTime.UtcNow,
        };

        await tokenCacheService.SaveTokenAsync(emailVerificationToken);

        await emailSender.SendEmailConfirmation(user.Email!, code, cancellationToken);
    }
}
