using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Category.Category;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Category;

public class CategoryRepositoryTest
{
    [Fact(DisplayName = nameof(GivenAValidCategory_WhenCallsInsert_ShouldReturnANewCategory))]
    [Trait("Integration/Infra.Data", "CategoryRepository")]
    public async Task GivenAValidCategory_WhenCallsInsert_ShouldReturnANewCategory()
    {
        // Given
        var dbContext = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                .UseInMemoryDatabase("integration-tests")
                .Options
        );

        const string expectedName = "Film";
        const bool expectedIsActive = true;
        const string expectedDescription = "Some description";

        var category = CategoryEntity.NewCategory(expectedName, expectedDescription, expectedIsActive);
        var categoryRepository = new CategoryRepository(dbContext);

        // When
        await categoryRepository.Insert(category, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actualCategory = await dbContext.Categories.FindAsync(category.Id);

        // Then
        actualCategory.Should().NotBeNull();
        actualCategory!.Name.Should().Be(expectedName);
        actualCategory.Id.Should().Be(category.Id);
        actualCategory.DeletedAt.Should().BeNull();
        actualCategory.IsActive.Should().Be(expectedIsActive);
        actualCategory.CreatedAt.Should().Be(category.CreatedAt);
        actualCategory.UpdatedAt.Should().Be(category.UpdatedAt);
        actualCategory.Description.Should().Be(expectedDescription);
    }

    [Fact(DisplayName = nameof(GivenAPrePersistedCategoryAndValidCategoryId_WhenCallsGet_ShouldReturnCategory))]
    [Trait("Integration/Infra.Data", "CategoryRepository")]
    public async void GivenAPrePersistedCategoryAndValidCategoryId_WhenCallsGet_ShouldReturnCategory()
    {
        // Given
        var dbContext = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                .UseInMemoryDatabase("integration-tests")
                .Options
        );
        var categoryRepository = new CategoryRepository(dbContext);

        const string expectedName = "Film";
        const bool expectedIsActive = true;
        const string expectedDescription = "Some description";

        var category = CategoryEntity.NewCategory(expectedName, expectedDescription, expectedIsActive);

        await dbContext.AddAsync(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // When
        var actualCategory = await categoryRepository.Get(category.Id, CancellationToken.None);

        // Then
        actualCategory.Should().NotBeNull();
        actualCategory.Id.Should().Be(category.Id);
        actualCategory.DeletedAt.Should().BeNull();
        actualCategory.Name.Should().Be(expectedName);
        actualCategory.IsActive.Should().Be(expectedIsActive);
        actualCategory.CreatedAt.Should().Be(category.CreatedAt);
        actualCategory.UpdatedAt.Should().Be(category.UpdatedAt);
        actualCategory.Description.Should().Be(expectedDescription);
    }

    [Fact(DisplayName = nameof(GivenAPrePersistedCategoryAndValidCategoryId_WhenTryToDeleteIt_ShouldDeleteCategory))]
    [Trait("Integration/Infra.Data", "CategoryRepository")]
    public async Task GivenAPrePersistedCategoryAndValidCategoryId_WhenTryToDeleteIt_ShouldDeleteCategory()
    {
        // Given
        var dbContext = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                .UseInMemoryDatabase("integration-tests")
                .Options
        );
        var categoryRepository = new CategoryRepository(dbContext);

        const string expectedName = "Film";
        const bool expectedIsActive = true;
        const string expectedDescription = "Some description";

        var category = CategoryEntity.NewCategory(expectedName, expectedDescription, expectedIsActive);

        dbContext.Categories.Count().Should().Be(0);

        await dbContext.AddAsync(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        dbContext.Categories.Count().Should().Be(1);

        // When
        await categoryRepository.Delete(category, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        // Then
        dbContext.Categories.Count().Should().Be(0);
    }
}