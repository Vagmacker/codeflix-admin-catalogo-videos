using MediatR;

namespace FC.Codeflix.Catalog.Application.Category.Update;

public interface IUpdateCategoryUseCase : IRequestHandler<UpdateCategoryCommand, CategoryOutput>;