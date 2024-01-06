namespace FC.Codeflix.Catalog.Application;

public interface IUnitOfWork
{
    public Task Commit(CancellationToken cancellationToken);
    public Task Rollback(CancellationToken cancellationToken);
}