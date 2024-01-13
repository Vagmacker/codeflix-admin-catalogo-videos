namespace FC.Codeflix.Catalog.IntegrationTests.Fixtures;

[CollectionDefinition(nameof(CategoryFixture))]
public class CategoryRepositoryFixtureCollection
    : ICollectionFixture<CategoryFixture>
{
}

public class CategoryFixture : BaseFixture
{
}