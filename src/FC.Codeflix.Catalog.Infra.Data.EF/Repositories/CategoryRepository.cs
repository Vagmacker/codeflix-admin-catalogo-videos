using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Domain.Category;
using FC.Codeflix.Catalog.Domain.Pagination;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

public class CategoryRepository(CodeflixCatalogDbContext context) : ICategoryRepository
{
    private DbSet<Category> Categories => context.Set<Category>();

    public async Task<Page<Category>> GetAll(SearchQuery input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        var query = Categories.AsNoTracking();
        query = AddOrderToQuery(query, input.Sort, input.Direction);

        if (!string.IsNullOrWhiteSpace(input.Terms))
            query = query.Where(category => category.Name.Contains(input.Terms));

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync(cancellationToken);

        return new Page<Category>(input.Page, input.PerPage, total, items);
    }

    public async Task<Category> Get(Guid id, CancellationToken cancellationToken)
    {
        var category = await Categories.AsNoTracking()
            .FirstOrDefaultAsync(it => it.Id == id, cancellationToken);

        NotFoundException.ThrowIfNull(category, $"Category '${id}' not found.");

        return category;
    }

    public async Task Insert(Category category, CancellationToken cancellationToken)
        => await Categories.AddAsync(category, cancellationToken);

    public Task Delete(Category category, CancellationToken _)
        => Task.FromResult(Categories.Remove(category));

    public Task Update(Category category, CancellationToken _)
        => Task.FromResult(Categories.Update(category));

    private static IQueryable<Category> AddOrderToQuery(
        IQueryable<Category> query,
        string orderProperty,
        SearchOrder sort
    )
    {
        var orderedQuery = (orderProperty.ToLower(), sort) switch
        {
            ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name).ThenBy(x => x.Id),
            ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name).ThenBy(x => x.Id)
        };

        return orderedQuery;
    }
}