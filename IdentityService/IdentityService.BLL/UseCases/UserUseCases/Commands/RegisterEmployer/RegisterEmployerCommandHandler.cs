using IdentityService.DAL.Constants;
using System.Security.Claims;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

public class RegisterEmployerCommandHandler : IRequestHandler<RegisterEmployerCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegisterEmployerCommandHandler(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(RegisterEmployerCommand request, CancellationToken cancellationToken)
    {
        var userByEmail = await _userManager.FindByEmailAsync(request.Email);

        if (userByEmail is not null)
        {
            throw new AlreadyExistsException($"A user with the email '{request.Email}' already exists.");
        }
        
        var user = _mapper.Map<AppUser>(request);

        var result1 = await _userManager.CreateAsync(user, request.Password);

        if (!result1.Succeeded)
        {
            var errors = string.Join("; ", result1.Errors.Select(e => e.Description));
            throw new BadRequestException($"User is not successfully registered. Errors: {errors}");
        }

        var result2 = await _userManager.AddToRoleAsync(user, AppRoles.EmployerRole);

        if (!result2.Succeeded)
        {
            var errors = string.Join("; ", result2.Errors.Select(e => e.Description));
            throw new BadRequestException($"User is not successfully registered. Errors: {errors}");
        }

        var employerProfile = _mapper.Map<EmployerProfile>(request);

        employerProfile.UserId = user.Id;

        await _unitOfWork.EmployersRepository.AddAsync(employerProfile, cancellationToken);
        await _unitOfWork.SaveAllAsync(cancellationToken);

        var token = _userManager.GenerateEmailConfirmationTokenAsync(user);

    }
}
