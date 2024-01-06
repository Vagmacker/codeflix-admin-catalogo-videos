using CategoryEntity = FC.Codeflix.Catalog.Domain.Category.Category;

namespace FC.Codeflix.Catalog.UnitTests.Fixtures;

public class CategoryFixture : BaseFixture
{
    public CategoryFixture()
        : base()
    {
    }

    public string Name()
    {
        var categoryName = "";

        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];

        if (categoryName.Length > 255)
        {
            categoryName = categoryName[..255];
        }

        return categoryName;
    }

    public string Description()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();

        if (categoryDescription.Length > 4_000)
        {
            categoryDescription = categoryDescription[..4_000];
        }

        return categoryDescription;
    }

    public CategoryEntity Movies()
    {
        return CategoryEntity.NewCategory("Movies", "Some description", true);
    }
}

[CollectionDefinition(nameof(CategoryFixture))]
public class CategoryFixtureCollection : ICollectionFixture<CategoryFixture>;