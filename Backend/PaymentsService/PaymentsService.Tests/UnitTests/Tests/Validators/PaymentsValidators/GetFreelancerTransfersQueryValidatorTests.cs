using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetFreelancerMyTransfers;
using PaymentsService.Application.Validators.PaymentsValidators;

namespace PaymentsService.Tests.UnitTests.Tests.Validators.PaymentsValidators;

public class GetFreelancerTransfersQueryValidatorTests
{
    private readonly GetFreelancerTransfersQueryValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_PassesValidation()
    {
        // Arrange
        var query = new GetFreelancerMyTransfersQuery(
            ProjectId: Guid.NewGuid(),
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_NullProjectId_PassesValidation()
    {
        // Arrange
        var query = new GetFreelancerMyTransfersQuery(
            ProjectId: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyProjectId_FailsWithInvalidGuidError()
    {
        // Arrange
        var query = new GetFreelancerMyTransfersQuery(
            ProjectId: Guid.Empty,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be("ProjectId");
        result.Errors[0].ErrorMessage.Should().Be("ProjectId must be a valid GUID or null.");
    }

    [Fact]
    public void Validate_PageNoLessThanOne_FailsWithPageNoError()
    {
        // Arrange
        var query = new GetFreelancerMyTransfersQuery(
            ProjectId: Guid.NewGuid(),
            PageNo: 0,
            PageSize: 10);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be("PageNo");
        result.Errors[0].ErrorMessage.Should().Be("Page number must be between 1 and 100_000.");
    }

    [Fact]
    public void Validate_PageNoGreaterThanMax_FailsWithPageNoError()
    {
        // Arrange
        var query = new GetFreelancerMyTransfersQuery(
            ProjectId: Guid.NewGuid(),
            PageNo: 100_001,
            PageSize: 10);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be("PageNo");
        result.Errors[0].ErrorMessage.Should().Be("Page number must be between 1 and 100_000.");
    }

    [Fact]
    public void Validate_PageSizeLessThanOne_FailsWithPageSizeError()
    {
        // Arrange
        var query = new GetFreelancerMyTransfersQuery(
            ProjectId: Guid.NewGuid(),
            PageNo: 1,
            PageSize: 0);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be("PageSize");
        result.Errors[0].ErrorMessage.Should().Be("Page size must be between 1 and 1000.");
    }

    [Fact]
    public void Validate_PageSizeGreaterThanMax_FailsWithPageSizeError()
    {
        // Arrange
        var query = new GetFreelancerMyTransfersQuery(
            ProjectId: Guid.NewGuid(),
            PageNo: 1,
            PageSize: 1001);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be("PageSize");
        result.Errors[0].ErrorMessage.Should().Be("Page size must be between 1 and 1000.");
    }

    [Fact]
    public void Validate_MultipleInvalidFields_ReturnsAllErrors()
    {
        // Arrange
        var query = new GetFreelancerMyTransfersQuery(
            ProjectId: Guid.Empty,
            PageNo: 0,
            PageSize: 1001);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId" && e.ErrorMessage == "ProjectId must be a valid GUID or null.");
        result.Errors.Should().Contain(e => e.PropertyName == "PageNo" && e.ErrorMessage == "Page number must be between 1 and 100_000.");
        result.Errors.Should().Contain(e => e.PropertyName == "PageSize" && e.ErrorMessage == "Page size must be between 1 and 1000.");
    }
}