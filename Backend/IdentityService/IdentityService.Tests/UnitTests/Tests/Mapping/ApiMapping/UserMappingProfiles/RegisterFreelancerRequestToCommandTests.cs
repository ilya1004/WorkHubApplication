using IdentityService.API.Contracts.UserContracts;
using IdentityService.API.Mapping.UserMappingProfiles;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.ApiMapping.UserMappingProfiles;

public class RegisterFreelancerRequestToCommandTests
{
    private readonly IMapper _mapper;

    public RegisterFreelancerRequestToCommandTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<RegisterFreelancerRequestToCommand>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapRegisterFreelancerRequestToCommand()
    {
        // Arrange
        var request = new RegisterFreelancerRequest("freelancer", "John", "Doe", "john@example.com", "password123");

        // Act
        var command = _mapper.Map<RegisterFreelancerCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.UserName.Should().Be(request.UserName);
        command.FirstName.Should().Be(request.FirstName);
        command.LastName.Should().Be(request.LastName);
        command.Email.Should().Be(request.Email);
        command.Password.Should().Be(request.Password);
    }
}