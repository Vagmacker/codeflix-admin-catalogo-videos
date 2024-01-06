using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Pagination;

namespace FC.Codeflix.Catalog.Domain.Category;

public interface ICategoryRepository : IGenericRepository<Category>
{
    public Task<Page<Category>> GetAll(SearchQuery query, CancellationToken cancellationToken);
}