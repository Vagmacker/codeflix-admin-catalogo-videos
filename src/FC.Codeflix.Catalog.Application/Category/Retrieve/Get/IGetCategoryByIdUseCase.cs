using MediatR;

namespace FC.Codeflix.Catalog.Application.Category.Retrieve.Get;

public interface IGetCategoryByIdUseCase : IRequestHandler<GetCategoryCommand, CategoryOutput>
{
}