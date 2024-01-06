using MediatR;

namespace FC.Codeflix.Catalog.Application.Category.Retrieve.Get;

public sealed record GetCategoryCommand(Guid Id) : IRequest<CategoryOutput>;