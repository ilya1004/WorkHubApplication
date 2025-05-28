using FluentValidation.TestHelper;
using ProjectsService.API.Contracts.FreelancerApplicationContracts;
using ProjectsService.API.Validators.FreelancerApplicationValidators;

namespace ProjectsService.Tests.UnitTests.Tests.Validators.FreelancerApplicationValidators;

public class CreateFreelancerApplicationRequestValidatorTests
{
    private readonly CreateFreelancerApplicationRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidProjectId_Succeeds()
    {
        // Arrange
        var request = new CreateFreelancerApplicationRequest(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}