using FC.Codeflix.Catalog.Domain.Category;

namespace FC.Codeflix.Catalog.Application.Category.Delete;

public sealed class DeleteCategoryUseCase(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : IDeleteCategoryUseCase
{
    public async Task Handle(DeleteCategoryCommand aCommand, CancellationToken aCancellationToken)
    {
        var aCategory = await categoryRepository.Get(aCommand.Id, aCancellationToken);

        await categoryRepository.Delete(aCategory, aCancellationToken);
        await unitOfWork.Commit(aCancellationToken);
    }
}