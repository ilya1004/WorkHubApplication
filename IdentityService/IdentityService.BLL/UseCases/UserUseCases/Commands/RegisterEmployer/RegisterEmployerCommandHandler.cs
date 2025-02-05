using MediatR;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

public class RegisterEmployerCommandHandler : IRequestHandler<RegisterEmployerCommand>
{
    public RegisterEmployerCommandHandler()
    {
        
    }
    public Task Handle(RegisterEmployerCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
