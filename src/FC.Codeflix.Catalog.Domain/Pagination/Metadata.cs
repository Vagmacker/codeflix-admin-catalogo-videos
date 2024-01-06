namespace FC.Codeflix.Catalog.Domain.Pagination;

public sealed record Metadata(int CurrentPage, int PerPage, long Total);