using ProjectsService.Application.Mapping.CategoryMappingProfiles;
using ProjectsService.Application.UseCases.Commands.CategoryUseCases.UpdateCategory;

namespace ProjectsService.Tests.UnitTests.Tests.Mapping.ApplicationMapping.CategoryMappingProfiles;

public class UpdateCategoryCommandToCategoryTests
{
    private readonly IMapper _mapper;

    public UpdateCategoryCommandToCategoryTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UpdateCategoryCommandToCategory>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_ValidCommand_MapsToCategoryCorrectly()
    {
        // Arrange
        var command = new UpdateCategoryCommand(Guid.NewGuid(), "Updated Category");

        // Act
        var category = _mapper.Map<Category>(command);

        // Assert
        category.Should().NotBeNull();
        category.Id.Should().Be(command.Id);
        category.Name.Should().Be("Updated Category");
        category.NormalizedName.Should().Be("UPDATED_CATEGORY");
        category.Projects.Should().BeNull();
    }

    [Fact]
    public void Map_EmptyName_MapsToCategoryWithEmptyName()
    {
        // Arrange
        var command = new UpdateCategoryCommand(Guid.NewGuid(), "");

        // Act
        var category = _mapper.Map<Category>(command);

        // Assert
        category.Should().NotBeNull();
        category.Id.Should().Be(command.Id);
        category.Name.Should().BeEmpty();
        category.NormalizedName.Should().BeEmpty();
        category.Projects.Should().BeNull();
    }

    [Fact]
    public void Map_NameWithMultipleSpaces_MapsToNormalizedNameCorrectly()
    {
        // Arrange
        var command = new UpdateCategoryCommand(Guid.NewGuid(), "Updated   Multiple   Spaces");

        // Act
        var category = _mapper.Map<Category>(command);

        // Assert
        category.Should().NotBeNull();
        category.Id.Should().Be(command.Id);
        category.Name.Should().Be("Updated   Multiple   Spaces");
        category.NormalizedName.Should().Be("UPDATED___MULTIPLE___SPACES");
        category.Projects.Should().BeNull();
    }

    [Fact]
    public void Map_EmptyId_MapsToCategoryWithEmptyId()
    {
        // Arrange
        var command = new UpdateCategoryCommand(Guid.Empty, "Test Category");

        // Act
        var category = _mapper.Map<Category>(command);

        // Assert
        category.Should().NotBeNull();
        category.Id.Should().Be(Guid.Empty);
        category.Name.Should().Be("Test Category");
        category.NormalizedName.Should().Be("TEST_CATEGORY");
        category.Projects.Should().BeNull();
    }
}