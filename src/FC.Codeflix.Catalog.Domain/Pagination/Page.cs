namespace FC.Codeflix.Catalog.Domain.Pagination;

public sealed record Page<TAggregate>(Metadata Meta, IReadOnlyList<TAggregate> Data)
{
    public Page(int currentPage, int perPage, long total, IReadOnlyList<TAggregate> data)
        : this(new Metadata(currentPage, perPage, total), data)
    {
    }

    public Page<TOutput> Map<TOutput>(Func<TAggregate, TOutput> mapper)
    {
        List<TOutput> aNewList = [];
        aNewList.AddRange(Data.Select(mapper));
        return new Page<TOutput>(Meta, aNewList);
    }
}