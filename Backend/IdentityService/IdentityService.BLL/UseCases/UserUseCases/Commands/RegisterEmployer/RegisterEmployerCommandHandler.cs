using IdentityService.BLL.Abstractions.EmailSender;
using IdentityService.DAL.Abstractions.RedisService;
using IdentityService.DAL.Constants;
using Microsoft.Extensions.Configuration;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

public class RegisterEmployerCommandHandler(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IEmailSender emailSender,
    ICachedService cachedService,
    IConfiguration configuration,
    ILogger<RegisterEmployerCommandHandler> logger) : IRequestHandler<RegisterEmployerCommand>
{
    public async Task Handle(RegisterEmployerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering new employer with email: {Email}", request.Email);

        var userByEmail = await userManager.FindByEmailAsync(request.Email);

        if (userByEmail is not null)
        {
            logger.LogWarning("User with email {Email} already exists", request.Email);
            
            throw new AlreadyExistsException($"A user with the email '{request.Email}' already exists.");
        }

        var user = mapper.Map<AppUser>(request);

        var role = await roleManager.FindByNameAsync(AppRoles.EmployerRole);

        if (role is null)
        {
            logger.LogError("Employer role not found");
            
            throw new BadRequestException("User is not successfully registered. User Role is not successfully find");
        }

        user.RoleId = role.Id;

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            
            logger.LogError("Failed to create employer: {Errors}", errors);
            
            throw new BadRequestException($"User is not successfully registered. Errors: {errors}");
        }

        logger.LogInformation("Creating employer profile for user {UserId}", user.Id);
        
        var employerProfile = mapper.Map<EmployerProfile>(request);
        employerProfile.UserId = user.Id;

        await unitOfWork.EmployerProfilesRepository.AddAsync(employerProfile, cancellationToken);
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

        logger.LogInformation("Successfully registered employer with ID: {UserId}", user.Id);
    }
}