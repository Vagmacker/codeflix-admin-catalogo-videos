using MediatR;

namespace FC.Codeflix.Catalog.Application.Category.Delete;

public sealed record DeleteCategoryCommand(Guid Id) : IRequest;