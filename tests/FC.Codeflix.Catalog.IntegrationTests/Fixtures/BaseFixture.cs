using Bogus;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Infra.Data.EF;

namespace FC.Codeflix.Catalog.IntegrationTests.Fixtures;

public class BaseFixture
{
    protected Faker Faker { get; set; } = new("pt_BR");

    public CodeflixCatalogDbContext CreateDbContext()
        => new(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                .UseInMemoryDatabase("integration-tests")
                .Options
        );
}