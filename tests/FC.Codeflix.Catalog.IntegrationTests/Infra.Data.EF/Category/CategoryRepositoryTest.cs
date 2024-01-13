using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Pagination;
using FC.Codeflix.Catalog.IntegrationTests.Fixtures;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Category.Category;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Category;

[Collection(nameof(CategoryFixture))]
public class CategoryRepositoryTest(CategoryFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(GivenAValidCategory_WhenCallsInsert_ShouldReturnANewCategory))]
    [Trait("Integration/Infra.Data", "CategoryRepository")]
    public async Task GivenAValidCategory_WhenCallsInsert_ShouldReturnANewCategory()
    {
        // Given
        var dbContext = fixture.CreateDbContext();

        const string expectedName = "Film";
        const bool expectedIsActive = true;
        const string expectedDescription = "Some description";

        var category = CategoryEntity.NewCategory(expectedName, expectedDescription, expectedIsActive);
        var categoryRepository = new CategoryRepository(dbContext);

        dbContext.Categories.Count().Should().Be(0);

        // When
        await categoryRepository.Insert(category, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        dbContext.Categories.Count().Should().Be(1);
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
        var dbContext = fixture.CreateDbContext();
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
        var dbContext = fixture.CreateDbContext();
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

    [Fact(DisplayName = nameof(GivenEmptyCategories_WhenCallsGetAll_ShouldReturnEmpty))]
    [Trait("Integration/Infra.Data", "CategoryRepository")]
    public async Task GivenEmptyCategories_WhenCallsGetAll_ShouldReturnEmpty()
    {
        // Given
        var dbContext = fixture.CreateDbContext();
        var categoryRepository = new CategoryRepository(dbContext);

        const int expectedPage = 0;
        const int expectedTotal = 0;
        const int expectedPerPage = 10;
        const string expectedTerms = "";
        const string expectedSort = "name";
        const SearchOrder expectedDirection = SearchOrder.Asc;

        var aQuery =
            new SearchQuery(expectedPage, expectedPerPage, expectedTerms, expectedSort, expectedDirection);

        // When
        var output = await categoryRepository.GetAll(aQuery, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Total.Should().Be(expectedTotal);
        output.Data.Should().HaveCount(expectedTotal);
        output.Meta.PerPage.Should().Be(aQuery.PerPage);
        output.Meta.CurrentPage.Should().Be(aQuery.Page);
    }

    [Theory(DisplayName = nameof(GivenAValidTerm_WhenCallsGetAll_ShouldReturnFiltered))]
    [Trait("Integration/Infra.Data", "CategoryRepository")]
    [InlineData("Act", 1, 10, 1, 1, "Action")]
    [InlineData("Hor", 1, 10, 1, 1, "Horror")]
    public async Task GivenAValidTerm_WhenCallsGetAll_ShouldReturnFiltered(
        string expectedTerms,
        int expectedPage,
        int expectedPerPage,
        int expectedItemsCount,
        int expectedTotal,
        string expectedName
    )
    {
        // Given
        var dbContext = fixture.CreateDbContext();
        var categoryRepository = new CategoryRepository(dbContext);

        await dbContext.AddRangeAsync(new List<CategoryEntity>()
        {
            CategoryEntity.NewCategory("Action", "Some description", true),
            CategoryEntity.NewCategory("Horror", "Some description", true)
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        dbContext.Categories.Count().Should().Be(2);

        const string expectedSort = "name";
        const SearchOrder expectedDirection = SearchOrder.Asc;

        var aQuery =
            new SearchQuery(expectedPage, expectedPerPage, expectedTerms, expectedSort, expectedDirection);

        // When
        var output = await categoryRepository.GetAll(aQuery, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Total.Should().Be(expectedTotal);
        output.Data[0].Name.Should().Be(expectedName);
        output.Meta.PerPage.Should().Be(expectedPerPage);
        output.Meta.CurrentPage.Should().Be(expectedPage);
        output.Data.Should().HaveCount(expectedItemsCount);
    }

    [Theory(DisplayName = nameof(GivenAValidSortAndDirection_WhenCallsGetAll_ShouldReturnSorted))]
    [Trait("Integration/Infra.Data", "CategoryRepository")]
    [InlineData("name", "asc", 1, 10, 2, 2, "Action")]
    [InlineData("name", "desc", 1, 10, 2, 2, "Horror")]
    public async Task GivenAValidSortAndDirection_WhenCallsGetAll_ShouldReturnSorted(
        string expectedSort,
        string expectedDirection,
        int expectedPage,
        int expectedPerPage,
        int expectedItemsCount,
        long expectedTotal,
        string expectedName
    )
    {
        // Given
        var dbContext = fixture.CreateDbContext();
        var categoryRepository = new CategoryRepository(dbContext);

        await dbContext.AddRangeAsync(new List<CategoryEntity>()
        {
            CategoryEntity.NewCategory("Action", "Some description", true),
            CategoryEntity.NewCategory("Horror", "Some description", true)
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        dbContext.Categories.Count().Should().Be(2);

        var direction = expectedDirection.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var aQuery = new SearchQuery(expectedPage, expectedPerPage, "", expectedSort, direction);

        // When 
        var output = await categoryRepository.GetAll(aQuery, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Total.Should().Be(expectedTotal);
        output.Data[0].Name.Should().Be(expectedName);
        output.Meta.PerPage.Should().Be(expectedPerPage);
        output.Meta.CurrentPage.Should().Be(expectedPage);
        output.Data.Should().HaveCount(expectedItemsCount);
    }

    [Theory(DisplayName = nameof(GivenAValidPagination_WhenCallsGetAll_ShouldReturnPaginated))]
    [Trait("Integration/Infra.Data", "CategoryRepository")]
    [InlineData(1, 2, 2, 3)]
    [InlineData(2, 2, 1, 3)]
    public async Task GivenAValidPagination_WhenCallsGetAll_ShouldReturnPaginated(
        int expectedPage,
        int expectedPerPage,
        int expectedItemsCount,
        long expectedTotal
    )
    {
        // Given
        var dbContext = fixture.CreateDbContext();
        var categoryRepository = new CategoryRepository(dbContext);
        
        await dbContext.AddRangeAsync(GetCategories());
        await dbContext.SaveChangesAsync(CancellationToken.None);
        
        const string expectedTerms = "";
        const string expectedSort = "name";
        const SearchOrder expectedDirection = SearchOrder.Asc;
        
        var aQuery =
            new SearchQuery(expectedPage, expectedPerPage, expectedTerms, expectedSort, expectedDirection);
        
        // When 
        var output = await categoryRepository.GetAll(aQuery, CancellationToken.None);
        
        // Then
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Total.Should().Be(expectedTotal);
        // output.Data[0].Name.Should().Be(expectedName);
        output.Meta.PerPage.Should().Be(expectedPerPage);
        output.Meta.CurrentPage.Should().Be(expectedPage);
        output.Data.Should().HaveCount(expectedItemsCount);
    }

    private static IEnumerable<CategoryEntity> GetCategories()
    {
        return new List<CategoryEntity>()
        {
            CategoryEntity.NewCategory("Action", "Some description", true),
            CategoryEntity.NewCategory("Horror", "Some description", true),
            CategoryEntity.NewCategory("Sci-fi", "Some description", true)
        };
    }

    public void Dispose()
        => fixture.CreateDbContext().Database.EnsureDeleted();
}