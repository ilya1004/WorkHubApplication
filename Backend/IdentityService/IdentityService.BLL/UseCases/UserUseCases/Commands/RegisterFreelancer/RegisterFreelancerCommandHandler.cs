using IdentityService.BLL.Abstractions.EmailSender;
using IdentityService.DAL.Abstractions.RedisService;
using IdentityService.DAL.Constants;
using Microsoft.Extensions.Configuration;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

public class RegisterFreelancerCommandHandler(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IEmailSender emailSender,
    ICachedService cachedService,
    IConfiguration configuration,
    ILogger<RegisterFreelancerCommandHandler> logger) : IRequestHandler<RegisterFreelancerCommand>
{
    public async Task Handle(RegisterFreelancerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering new freelancer with email: {Email}", request.Email);

        var userByEmail = await userManager.FindByEmailAsync(request.Email);

        if (userByEmail is not null)
        {
            logger.LogWarning("User with email {Email} already exists", request.Email);
            
            throw new AlreadyExistsException($"A user with the email '{request.Email}' already exists.");
        }

        var user = mapper.Map<AppUser>(request);

        var role = await roleManager.FindByNameAsync(AppRoles.FreelancerRole);

        if (role is null)
        {
            logger.LogError("Freelancer role not found");
            
            throw new BadRequestException($"User is not successfully registered. User Role is not successfully find");
        }

        user.RoleId = role.Id;

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            
            logger.LogError("Failed to create freelancer: {Errors}", errors);
            
            throw new BadRequestException($"User is not successfully registered. Errors: {errors}");
        }

        logger.LogInformation("Creating freelancer profile for user {UserId}", user.Id);
        
        var freelancerProfile = mapper.Map<FreelancerProfile>(request);
        freelancerProfile.UserId = user.Id;

        await unitOfWork.FreelancerProfilesRepository.AddAsync(freelancerProfile, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        logger.LogInformation("Generating email confirmation token for user {UserId}", user.Id);
        
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        string code;
        var random = new Random();
        do
        {
            code = random.Next(100000, 999999).ToString();
        } 
        while (await cachedService.ExistsAsync(code, cancellationToken));

        logger.LogInformation("Storing confirmation code {Code} in cache", code);
        
        await cachedService.SetAsync(code, token, TimeSpan.FromHours(
            int.Parse(configuration.GetRequiredSection("IdentityTokenExpirationTimeInHours").Value!)), cancellationToken);

        logger.LogInformation("Sending confirmation email to {Email}", user.Email);
        
        await emailSender.SendEmailConfirmation(user.Email!, code, cancellationToken);

        logger.LogInformation("Successfully registered freelancer with ID: {UserId}", user.Id);
    }
}