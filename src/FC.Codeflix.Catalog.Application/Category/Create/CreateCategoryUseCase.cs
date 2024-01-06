using FC.Codeflix.Catalog.Domain.Category;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Category.Category;

namespace FC.Codeflix.Catalog.Application.Category.Create;

public sealed class CreateCategoryUseCase(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : ICreateCategoryUseCase
{
    public async Task<CategoryOutput> Handle(CreateCategoryCommand aCommand, CancellationToken cancellationToken)
    {
        var isActive = aCommand.Active;
        var aName = aCommand.Name;
        var aDescription = aCommand.Description;

        var aCategory = CategoryEntity.NewCategory(aName, aDescription, isActive);

        await categoryRepository.Insert(aCategory, cancellationToken);
        await unitOfWork.Commit(cancellationToken);

        return CategoryOutput.From(aCategory);
    }
}