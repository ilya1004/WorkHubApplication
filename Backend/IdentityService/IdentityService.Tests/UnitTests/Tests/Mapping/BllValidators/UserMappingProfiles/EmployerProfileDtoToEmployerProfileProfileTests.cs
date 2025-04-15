using IdentityService.BLL.Mapping.UserMappingProfiles;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.BllValidators.UserMappingProfiles;

public class EmployerProfileDtoToEmployerProfileProfileTests
{
    private readonly IMapper _mapper;

    public EmployerProfileDtoToEmployerProfileProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<EmployerProfileDtoToEmployerProfileProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapEmployerProfileDtoToEmployerProfile()
    {
        // Arrange
        var dto = new EmployerProfileDto("Company Inc", "About company", Guid.NewGuid(), true);

        // Act
        var employerProfile = _mapper.Map<EmployerProfile>(dto);

        // Assert
        employerProfile.Should().NotBeNull();
        employerProfile.CompanyName.Should().Be(dto.CompanyName);
        employerProfile.About.Should().Be(dto.About);
        employerProfile.IndustryId.Should().Be(dto.IndustryId);
        employerProfile.Id.Should().Be(Guid.Empty);
        employerProfile.UserId.Should().Be(Guid.Empty);
        employerProfile.Industry.Should().BeNull();
        employerProfile.User.Should().BeNull();
        employerProfile.StripeCustomerId.Should().BeNull();
    }

    [Fact]
    public void ShouldMapEmployerProfileDtoToEmployerProfile_WithNullOptionalFields()
    {
        // Arrange
        var dto = new EmployerProfileDto("Company Inc", null, null, false);

        // Act
        var employerProfile = _mapper.Map<EmployerProfile>(dto);

        // Assert
        employerProfile.Should().NotBeNull();
        employerProfile.CompanyName.Should().Be(dto.CompanyName);
        employerProfile.About.Should().BeNull();
        employerProfile.IndustryId.Should().BeNull();
        employerProfile.Id.Should().Be(Guid.Empty);
        employerProfile.UserId.Should().Be(Guid.Empty);
        employerProfile.Industry.Should().BeNull();
        employerProfile.User.Should().BeNull();
        employerProfile.StripeCustomerId.Should().BeNull();
    }
}