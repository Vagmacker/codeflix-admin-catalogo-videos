using MediatR;

namespace FC.Codeflix.Catalog.Application.Category.Create;

public sealed record CreateCategoryCommand(string Name, string Description, bool Active) : IRequest<CategoryOutput>
{
    public static CreateCategoryCommand With(string aName, string aDescription, bool isActive = true)
        => new(aName, aDescription, isActive);
}