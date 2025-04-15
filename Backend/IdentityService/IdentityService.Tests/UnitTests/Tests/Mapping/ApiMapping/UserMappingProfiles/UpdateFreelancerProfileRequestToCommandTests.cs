using IdentityService.API.Contracts.UserContracts;
using IdentityService.API.Mapping.UserMappingProfiles;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateFreelancerProfile;
using Microsoft.AspNetCore.Http;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.ApiMapping.UserMappingProfiles;

public class UpdateFreelancerProfileRequestToCommandTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IFormFile> _formFileMock;

    public UpdateFreelancerProfileRequestToCommandTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<UpdateFreelancerProfileRequestToCommand>());
        _mapper = configuration.CreateMapper();
        _formFileMock = new Mock<IFormFile>();
    }

    [Fact]
    public void ShouldMapUpdateFreelancerProfileRequest_WithImageFile()
    {
        // Arrange
        var profileDto = new FreelancerProfileDto("John", "Doe", "About me", [Guid.NewGuid()], true);
        var stream = new MemoryStream();
        _formFileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        _formFileMock.Setup(f => f.ContentType).Returns("image/jpeg");
        var request = new UpdateFreelancerProfileRequest(profileDto, _formFileMock.Object);

        // Act
        var command = _mapper.Map<UpdateFreelancerProfileCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.FreelancerProfile.Should().BeEquivalentTo(profileDto);
        command.FileStream.Should().BeSameAs(stream);
        command.ContentType.Should().Be("image/jpeg");
    }

    [Fact]
    public void ShouldMapUpdateFreelancerProfileRequest_WithoutImageFile()
    {
        // Arrange
        var profileDto = new FreelancerProfileDto("John", "Doe", "About me", [Guid.NewGuid()], true);
        var request = new UpdateFreelancerProfileRequest(profileDto, null);

        // Act
        var command = _mapper.Map<UpdateFreelancerProfileCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.FreelancerProfile.Should().BeEquivalentTo(profileDto);
        command.FileStream.Should().BeNull();
        command.ContentType.Should().BeNull();
    }
}