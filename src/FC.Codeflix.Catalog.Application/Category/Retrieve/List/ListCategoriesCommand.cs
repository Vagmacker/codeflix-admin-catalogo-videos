using MediatR;
using FC.Codeflix.Catalog.Domain.Pagination;

namespace FC.Codeflix.Catalog.Application.Category.Retrieve.List;

public sealed class ListCategoriesCommand(
    int page = 1,
    int perPage = 10,
    string terms = "",
    string sort = "",
    SearchOrder direction = SearchOrder.Asc)
    : SearchQuery(page, perPage, terms, sort, direction), IRequest<Page<ListCategoriesOutput>>
{
    public ListCategoriesCommand()
        : this(1, 10, "", "", SearchOrder.Asc)
    {
    }
}