using Bogus;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Infra.Data.EF;

namespace FC.Codeflix.Catalog.IntegrationTests.Fixtures;

public class BaseFixture
{
    protected Faker Faker { get; set; } = new("pt_BR");

    public CodeflixCatalogDbContext CreateDbContext(bool isEnsureDeleted = false)
    {
        var context = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                .UseInMemoryDatabase("integration-tests")
                .Options
        );

        if (isEnsureDeleted is false)
            context.Database.EnsureDeleted();

        return context;
    }
}