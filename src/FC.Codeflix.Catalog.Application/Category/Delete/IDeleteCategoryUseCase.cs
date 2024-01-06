using MediatR;

namespace FC.Codeflix.Catalog.Application.Category.Delete;

public interface IDeleteCategoryUseCase : IRequestHandler<DeleteCategoryCommand>;