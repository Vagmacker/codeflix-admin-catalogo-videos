using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.UnitTests.Fixtures;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Category.Category;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Category;

[Collection(nameof(CategoryFixture))]
public class CategoryTest(CategoryFixture fixture)
{
    [Fact(DisplayName = nameof(GivenAValidParams_WhenCallNewCategory_ThenInstantiateACategory))]
    [Trait("Domain", "Category Aggregate")]
    public void GivenAValidParams_WhenCallNewCategory_ThenInstantiateACategory()
    {
        // Given
        const bool expectedIsActive = true;
        var expectedName = fixture.Name();
        var expectedDescription = fixture.Description();

        // When
        var aCategory = CategoryEntity.NewCategory(expectedName, expectedDescription, expectedIsActive);

        // Then
        aCategory.Should().NotBeNull();
        aCategory.Id.Should().NotBeEmpty();
        aCategory.IsActive.Should().BeTrue();
        aCategory.DeletedAt.Should().BeNull();
        aCategory.Name.Should().Be(expectedName);
        aCategory.Description.Should().Be(expectedDescription);
        aCategory.CreatedAt.Should().NotBeSameDateAs(default);
        aCategory.UpdatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(GivenAnInvalidName_WhenCallNewCategory_ThenReceiveError))]
    [Trait("Domain", "Category Aggregate")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void GivenAnInvalidName_WhenCallNewCategory_ThenReceiveError(string? anInvalidName)
    {
        // Given
        const bool expectedIsActive = true;
        var expectedDescription = fixture.Description();

        // When
        var exception = Assert.Throws<NotificationException>(
            () => CategoryEntity.NewCategory(anInvalidName!, expectedDescription, expectedIsActive)
        );

        const string expectedErrorMessage = "Name should not be null or empty";

        // Then 
        exception.Should().NotBeNull();
        exception.GetErrors().Should().ContainSingle();
        exception.GetErrors()[0].Message.Should().Be(expectedErrorMessage);
    }

    [Theory(DisplayName =
        nameof(GivenAnInvalidNameLengthLessThan3_WhenCallNewCategoryAndValidate_ThenShouldReceiveError))]
    [Trait("Domain", "Category Aggregate")]
    [MemberData(nameof(GetNamesWithLessThan3Characters), parameters: 6)]
    public void GivenAnInvalidNameLengthLessThan3_WhenCallNewCategoryAndValidate_ThenShouldReceiveError(
        string anInvalidName)
    {
        // Given
        const bool expectedIsActive = true;
        var expectedDescription = fixture.Description();

        // When
        var exception = Assert.Throws<NotificationException>(
            () => CategoryEntity.NewCategory(anInvalidName, expectedDescription, expectedIsActive)
        );

        const string expectedErrorMessage = "Name should be between 3 and 255 characters";

        // Then 
        exception.Should().NotBeNull();
        exception.GetErrors().Should().ContainSingle();
        exception.GetErrors()[0].Message.Should().Be(expectedErrorMessage);
    }

    [Fact(DisplayName =
        nameof(GivenAnInvalidNameLengthMoreThan255_WhenCallNewCategoryAndValidate_ThenShouldReceiveError))]
    [Trait("Domain", "Category Aggregate")]
    public void GivenAnInvalidNameLengthMoreThan255_WhenCallNewCategoryAndValidate_ThenShouldReceiveError()
    {
        // Given
        const bool expectedIsActive = true;
        var expectedDescription = fixture.Description();
        var expectedName = string.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());

        // When
        var exception = Assert.Throws<NotificationException>(
            () => CategoryEntity.NewCategory(expectedName, expectedDescription, expectedIsActive)
        );

        const string expectedErrorMessage = "Name should be between 3 and 255 characters";

        // Then 
        exception.Should().NotBeNull();
        exception.GetErrors().Should().ContainSingle();
        exception.GetErrors()[0].Message.Should().Be(expectedErrorMessage);
    }

    [Fact(DisplayName = nameof(GivenAnInvalidDescription_WhenCallNewCategoryAndValidate_ThenShouldReceiveOK))]
    [Trait("Domain", "Category Aggregate")]
    public void GivenAnInvalidDescription_WhenCallNewCategoryAndValidate_ThenShouldReceiveOK()
    {
        // Given
        const bool expectedIsActive = true;
        var expectedName = fixture.Name();

        // When
        var exception = Assert.Throws<NotificationException>(
            () => CategoryEntity.NewCategory(expectedName, null, expectedIsActive)
        );

        const string expectedErrorMessage = "Description should not be null";

        // Then 
        exception.Should().NotBeNull();
        exception.GetErrors().Should().ContainSingle();
        exception.GetErrors()[0].Message.Should().Be(expectedErrorMessage);
    }

    [Fact(DisplayName =
        nameof(GivenAnInvalidDescriptionLengthMoreThan4_000_WhenCallNewCategoryAndValidate_ThenShouldReceiveError))]
    [Trait("Domain", "Category Aggregate")]
    public void GivenAnInvalidDescriptionLengthMoreThan4_000_WhenCallNewCategoryAndValidate_ThenShouldReceiveError()
    {
        // Given
        const bool expectedIsActive = true;
        var expectedName = fixture.Name();
        var expectedDescription = string.Join(null, Enumerable.Range(1, 4_001).Select(_ => "a").ToArray());

        // When
        var exception = Assert.Throws<NotificationException>(
            () => CategoryEntity.NewCategory(expectedName, expectedDescription, expectedIsActive)
        );

        const string expectedErrorMessage = "Description should be between 1 and 4000 characters";

        // Then 
        exception.Should().NotBeNull();
        exception.GetErrors().Should().ContainSingle();
        exception.GetErrors()[0].Message.Should().Be(expectedErrorMessage);
    }

    [Fact(DisplayName = nameof(GivenAValidFalseIsActive_WhenCallNewCategoryAndValidate_ThenShouldReceiveOK))]
    [Trait("Domain", "Category Aggregate")]
    public void GivenAValidFalseIsActive_WhenCallNewCategoryAndValidate_ThenShouldReceiveOK()
    {
        // Given
        const bool expectedIsActive = false;
        var expectedName = fixture.Name();
        var expectedDescription = fixture.Description();

        // When
        var aCategory = CategoryEntity.NewCategory(expectedName, expectedDescription, expectedIsActive);

        // Then
        aCategory.Should().NotBeNull();
        aCategory.Id.Should().NotBeEmpty();
        aCategory.IsActive.Should().BeFalse();
        aCategory.Name.Should().Be(expectedName);
        aCategory.Description.Should().Be(expectedDescription);
        aCategory.DeletedAt.Should().NotBeSameDateAs(default);
        aCategory.CreatedAt.Should().NotBeSameDateAs(default);
        aCategory.UpdatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(GivenAValidActiveCategory_WhenCallDeactivate_ThenReturnCategoryInactivated))]
    [Trait("Domain", "Category Aggregate")]
    public void GivenAValidActiveCategory_WhenCallDeactivate_ThenReturnCategoryInactivated()
    {
        // Given
        var expectedName = fixture.Name();
        var expectedDescription = fixture.Description();

        var aCategory = CategoryEntity.NewCategory(expectedName, expectedDescription, true);

        var createdAt = aCategory.CreatedAt;
        var updatedAt = aCategory.UpdatedAt;

        aCategory.IsActive.Should().BeTrue();
        aCategory.DeletedAt.Should().BeNull();

        // When
        aCategory.Deactivate();

        // Then
        aCategory.Should().NotBeNull();
        aCategory.Id.Should().NotBeEmpty();
        aCategory.IsActive.Should().BeFalse();
        aCategory.Name.Should().Be(expectedName);
        aCategory.DeletedAt.Should().NotBeNull();
        aCategory.UpdatedAt.Should().BeAfter(updatedAt);
        aCategory.CreatedAt.Should().BeSameDateAs(createdAt);
        aCategory.Description.Should().Be(expectedDescription);
        aCategory.UpdatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(GivenAValidDeactivateCategory_WhenCallActivate_ThenReturnCategoryActivated))]
    [Trait("Domain", "Category Aggregate")]
    public void GivenAValidDeactivateCategory_WhenCallActivate_ThenReturnCategoryActivated()
    {
        // Given
        var expectedName = fixture.Name();
        var expectedDescription = fixture.Description();

        var aCategory = CategoryEntity.NewCategory(expectedName, expectedDescription, false);

        var createdAt = aCategory.CreatedAt;
        var updatedAt = aCategory.UpdatedAt;

        aCategory.IsActive.Should().BeFalse();
        aCategory.DeletedAt.Should().NotBeNull();

        // When
        aCategory.Activate();

        // Then
        aCategory.Should().NotBeNull();
        aCategory.Id.Should().NotBeEmpty();
        aCategory.IsActive.Should().BeTrue();
        aCategory.DeletedAt.Should().BeNull();
        aCategory.Name.Should().Be(expectedName);
        aCategory.UpdatedAt.Should().BeAfter(updatedAt);
        aCategory.CreatedAt.Should().BeSameDateAs(createdAt);
        aCategory.Description.Should().Be(expectedDescription);
        aCategory.UpdatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(GivenAValidCategory_WhenCallUpdate_ThenReturnCategoryUpdated))]
    [Trait("Domain", "Category Aggregate")]
    public void GivenAValidCategory_WhenCallUpdate_ThenReturnCategoryUpdated()
    {
        // Given
        const bool expectedIsActive = true;
        var expectedName = fixture.Name();
        var expectedDescription = fixture.Description();

        var aCategory = CategoryEntity.NewCategory("Film", expectedDescription, expectedIsActive);

        var createdAt = aCategory.CreatedAt;
        var updatedAt = aCategory.UpdatedAt;

        // When
        aCategory.Update(expectedName, expectedDescription, expectedIsActive);

        // Then
        aCategory.Should().NotBeNull();
        aCategory.Id.Should().NotBeEmpty();
        aCategory.IsActive.Should().BeTrue();
        aCategory.DeletedAt.Should().BeNull();
        aCategory.Name.Should().Be(expectedName);
        aCategory.UpdatedAt.Should().BeAfter(updatedAt);
        aCategory.CreatedAt.Should().BeSameDateAs(createdAt);
        aCategory.Description.Should().Be(expectedDescription);
        aCategory.UpdatedAt.Should().NotBeSameDateAs(default);
    }

    public static IEnumerable<object[]> GetNamesWithLessThan3Characters(int numberOfInteractions = 6)
    {
        var fixture = new CategoryFixture();

        for (var i = 0; i < numberOfInteractions; i++)
        {
            var isOdd = i % 2 == 1;
            yield return new object[]
            {
                fixture.Name()[..(isOdd ? 1 : 2)]
            };
        }
    }
}