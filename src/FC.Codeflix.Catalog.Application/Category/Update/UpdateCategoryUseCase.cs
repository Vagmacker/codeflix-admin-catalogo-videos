using FC.Codeflix.Catalog.Domain.Category;

namespace FC.Codeflix.Catalog.Application.Category.Update;

public sealed class UpdateCategoryUseCase(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : IUpdateCategoryUseCase
{
    public async Task<CategoryOutput> Handle(UpdateCategoryCommand aCommand, CancellationToken aCancellationToken)
    {
        var isActive = aCommand.Active;
        var aName = aCommand.Name;
        var aDescription = aCommand.Description;

        var aCategory = await categoryRepository.Get(aCommand.Id, aCancellationToken);
        aCategory.Update(aName, aDescription, isActive);

        await categoryRepository.Update(aCategory, aCancellationToken);
        await unitOfWork.Commit(aCancellationToken);

        return CategoryOutput.From(aCategory);
    }
}