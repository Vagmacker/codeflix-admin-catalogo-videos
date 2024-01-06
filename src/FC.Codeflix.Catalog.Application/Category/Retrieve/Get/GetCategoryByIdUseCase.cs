using FC.Codeflix.Catalog.Domain.Category;

namespace FC.Codeflix.Catalog.Application.Category.Retrieve.Get;

public sealed class GetCategoryByIdUseCase(ICategoryRepository categoryRepository) : IGetCategoryByIdUseCase
{
    public async Task<CategoryOutput> Handle(GetCategoryCommand aCommand, CancellationToken aCancellationToken)
    {
        var aCategory = await categoryRepository.Get(aCommand.Id, aCancellationToken);
        return CategoryOutput.From(aCategory);
    }
}