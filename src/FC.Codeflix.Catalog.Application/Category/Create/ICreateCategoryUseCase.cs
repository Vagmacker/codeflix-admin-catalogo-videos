using MediatR;

namespace FC.Codeflix.Catalog.Application.Category.Create;

public interface ICreateCategoryUseCase
    : IRequestHandler<CreateCategoryCommand, CategoryOutput>
{
}