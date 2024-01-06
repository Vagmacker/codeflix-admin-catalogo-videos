using FC.Codeflix.Catalog.Domain.Pagination;
using MediatR;

namespace FC.Codeflix.Catalog.Application.Category.Retrieve.List;

public interface IListCategoriesUseCase : IRequestHandler<ListCategoriesCommand, Page<ListCategoriesOutput>>;