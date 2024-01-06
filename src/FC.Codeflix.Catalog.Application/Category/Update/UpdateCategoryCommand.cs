using MediatR;

namespace FC.Codeflix.Catalog.Application.Category.Update;

public sealed record UpdateCategoryCommand(Guid Id, string Name, string Description, bool Active)
    : IRequest<CategoryOutput>
{
    public static UpdateCategoryCommand With(Guid anId, string aName, string aDescription, bool isActive)
        => new(anId, aName, aDescription, isActive);
}