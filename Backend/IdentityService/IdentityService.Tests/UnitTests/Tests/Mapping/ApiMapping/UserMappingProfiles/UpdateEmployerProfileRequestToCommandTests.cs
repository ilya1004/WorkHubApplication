using IdentityService.API.Contracts.UserContracts;
using IdentityService.API.Mapping.UserMappingProfiles;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateEmployerProfile;
using Microsoft.AspNetCore.Http;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.ApiMapping.UserMappingProfiles;

public class UpdateEmployerProfileRequestToCommandTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IFormFile> _formFileMock;

    public UpdateEmployerProfileRequestToCommandTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<UpdateEmployerProfileRequestToCommand>());
        _mapper = configuration.CreateMapper();
        _formFileMock = new Mock<IFormFile>();
    }

    [Fact]
    public void ShouldMapUpdateEmployerProfileRequest_WithImageFile()
    {
        // Arrange
        var profileDto = new EmployerProfileDto("Company Inc", "About company", Guid.NewGuid(), true);
        var stream = new MemoryStream();
        _formFileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        _formFileMock.Setup(f => f.ContentType).Returns("image/jpeg");
        var request = new UpdateEmployerProfileRequest(profileDto, _formFileMock.Object);

        // Act
        var command = _mapper.Map<UpdateEmployerProfileCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.EmployerProfile.Should().BeEquivalentTo(profileDto);
        command.FileStream.Should().BeSameAs(stream);
        command.ContentType.Should().Be("image/jpeg");
    }

    [Fact]
    public void ShouldMapUpdateEmployerProfileRequest_WithoutImageFile()
    {
        // Arrange
        var profileDto = new EmployerProfileDto("Company Inc", "About company", Guid.NewGuid(), true);
        var request = new UpdateEmployerProfileRequest(profileDto, null);

        // Act
        var command = _mapper.Map<UpdateEmployerProfileCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.EmployerProfile.Should().BeEquivalentTo(profileDto);
        command.FileStream.Should().BeNull();
        command.ContentType.Should().BeNull();
    }
}