using FC.Codeflix.Catalog.Domain.Category;
using FC.Codeflix.Catalog.Domain.Pagination;

namespace FC.Codeflix.Catalog.Application.Category.Retrieve.List;

public sealed class ListCategoriesUseCase(ICategoryRepository categoryRepository) : IListCategoriesUseCase
{
    public async Task<Page<ListCategoriesOutput>> Handle(ListCategoriesCommand aCommand,
        CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAll(
            new SearchQuery(
                aCommand.Page,
                aCommand.PerPage,
                aCommand.Terms,
                aCommand.Sort,
                aCommand.Direction
            ),
            cancellationToken
        );

        return categories
            .Map(ListCategoriesOutput.From);
    }
}