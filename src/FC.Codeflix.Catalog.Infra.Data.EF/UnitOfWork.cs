using FC.Codeflix.Catalog.Application;

namespace FC.Codeflix.Catalog.Infra.Data.EF;

public class UnitOfWork(CodeflixCatalogDbContext context)
    : IUnitOfWork
{
    public async Task Commit(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task Rollback(CancellationToken cancellationToken)
        => Task.CompletedTask;
}