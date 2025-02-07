using IdentityService.DAL.Constants;
using System.Security.Claims;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

public class RegisterFreelancerCommandHandler : IRequestHandler<RegisterFreelancerCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegisterFreelancerCommandHandler(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(RegisterFreelancerCommand request, CancellationToken cancellationToken)
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

        var result2 = await _userManager.AddToRoleAsync(user, AppRoles.FreelancerRole);

        if (!result2.Succeeded)
        {
            var errors = string.Join("; ", result2.Errors.Select(e => e.Description));
            throw new BadRequestException($"User is not successfully registered. Errors: {errors}");
        }

        var freelancerProfile = _mapper.Map<FreelancerProfile>(request);

        freelancerProfile.UserId = user.Id;

        await _unitOfWork.FreelancersRepository.AddAsync(freelancerProfile, cancellationToken);
        await _unitOfWork.SaveAllAsync(cancellationToken);
    }
}
