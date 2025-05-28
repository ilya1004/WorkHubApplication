using IdentityService.BLL.Mapping.UserMappingProfiles;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.BllValidators.UserMappingProfiles;

public class AppUserToEmployerUserDtoTests
{
    private readonly IMapper _mapper;

    public AppUserToEmployerUserDtoTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<AppUserToEmployerUserDto>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapAppUserToEmployerUserDto()
    {
        // Arrange
        var appUser = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = "company",
            Email = "company@example.com",
            RegisteredAt = DateTime.UtcNow,
            ImageUrl = "https://image.url",
            EmployerProfile = new EmployerProfile
            {
                CompanyName = "Company Inc",
                About = "About company",
                StripeCustomerId = "cust_123",
                Industry = new EmployerIndustry
                {
                    Id = Guid.NewGuid(),
                    Name = "Technology"
                }
            },
            Role = new IdentityRole<Guid> { Name = "Employer" }
        };

        // Act
        var dto = _mapper.Map<EmployerUserDto>(appUser);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(appUser.Id.ToString());
        dto.UserName.Should().Be(appUser.UserName);
        dto.CompanyName.Should().Be(appUser.EmployerProfile.CompanyName);
        dto.About.Should().Be(appUser.EmployerProfile.About);
        dto.Email.Should().Be(appUser.Email);
        dto.RegisteredAt.Should().Be(appUser.RegisteredAt);
        dto.StripeCustomerId.Should().Be(appUser.EmployerProfile.StripeCustomerId);
        dto.Industry.Should().BeEquivalentTo(new EmployerIndustryDto(appUser.EmployerProfile.Industry.Id.ToString(), appUser.EmployerProfile.Industry.Name));
        dto.ImageUrl.Should().Be(appUser.ImageUrl);
        dto.RoleName.Should().Be(appUser.Role.Name);
    }

    [Fact]
    public void ShouldMapAppUserToEmployerUserDto_WithNullOptionalFields()
    {
        // Arrange
        var appUser = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = "company",
            Email = "company@example.com",
            RegisteredAt = DateTime.UtcNow,
            ImageUrl = null,
            EmployerProfile = new EmployerProfile
            {
                CompanyName = "Company Inc",
                About = null,
                StripeCustomerId = null,
                Industry = null
            },
            Role = new IdentityRole<Guid> { Name = "Employer" }
        };

        // Act
        var dto = _mapper.Map<EmployerUserDto>(appUser);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(appUser.Id.ToString());
        dto.UserName.Should().Be(appUser.UserName);
        dto.CompanyName.Should().Be(appUser.EmployerProfile.CompanyName);
        dto.About.Should().BeNull();
        dto.Email.Should().Be(appUser.Email);
        dto.RegisteredAt.Should().Be(appUser.RegisteredAt);
        dto.StripeCustomerId.Should().BeNull();
        dto.Industry.Should().BeNull();
        dto.ImageUrl.Should().BeNull();
        dto.RoleName.Should().Be(appUser.Role.Name);
    }
}