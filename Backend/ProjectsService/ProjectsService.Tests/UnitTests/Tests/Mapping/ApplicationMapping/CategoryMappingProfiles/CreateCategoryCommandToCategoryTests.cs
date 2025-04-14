using ProjectsService.Application.Mapping.CategoryMappingProfiles;
using ProjectsService.Application.UseCases.Commands.CategoryUseCases.CreateCategory;

namespace ProjectsService.Tests.UnitTests.Tests.Mapping.ApplicationMapping.CategoryMappingProfiles;

public class CreateCategoryCommandToCategoryTests
{
    private readonly IMapper _mapper;

    public CreateCategoryCommandToCategoryTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateCategoryCommandToCategory>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_ValidCommand_MapsToCategoryCorrectly()
    {
        // Arrange
        var command = new CreateCategoryCommand("Test Category");

        // Act
        var category = _mapper.Map<Category>(command);

        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be("Test Category");
        category.NormalizedName.Should().Be("TEST_CATEGORY");
        category.Id.Should().Be(Guid.Empty);
        category.Projects.Should().BeNull();
    }

    [Fact]
    public void Map_EmptyName_MapsToCategoryWithEmptyName()
    {
        // Arrange
        var command = new CreateCategoryCommand("");

        // Act
        var category = _mapper.Map<Category>(command);

        // Assert
        category.Should().NotBeNull();
        category.Name.Should().BeEmpty();
        category.NormalizedName.Should().BeEmpty();
        category.Id.Should().Be(Guid.Empty);
        category.Projects.Should().BeNull();
    }

    [Fact]
    public void Map_NameWithMultipleSpaces_MapsToNormalizedNameCorrectly()
    {
        // Arrange
        var command = new CreateCategoryCommand("Test   Multiple   Spaces");

        // Act
        var category = _mapper.Map<Category>(command);

        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be("Test   Multiple   Spaces");
        category.NormalizedName.Should().Be("TEST___MULTIPLE___SPACES");
        category.Id.Should().Be(Guid.Empty);
        category.Projects.Should().BeNull();
    }
}