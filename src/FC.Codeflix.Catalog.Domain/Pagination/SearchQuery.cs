namespace FC.Codeflix.Catalog.Domain.Pagination;

public class SearchQuery(int page, int perPage, string terms, string sort, SearchOrder direction)
{
    public int Page { get; private set; } = page;

    public int PerPage { get; private set; } = perPage;

    public string Terms { get; private set; } = terms;

    public string Sort { get; private set; } = sort;

    public SearchOrder Direction { get; private set; } = direction;
}