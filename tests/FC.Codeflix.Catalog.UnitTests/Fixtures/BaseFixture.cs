using Bogus;

namespace FC.Codeflix.Catalog.UnitTests.Fixtures;

public abstract class BaseFixture
{
    protected Faker Faker { get; set; } = new("pt_BR");
}