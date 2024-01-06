using CategoryEntity = FC.Codeflix.Catalog.Domain.Category.Category;

namespace FC.Codeflix.Catalog.Application.Category;

public sealed record CategoryOutput(Guid Id, string Name, string Description, bool IsActive, DateTime CreatedAt)
{
    public static CategoryOutput From(CategoryEntity aCategory)
        => new(aCategory.Id, aCategory.Name, aCategory.Description!, aCategory.IsActive, aCategory.CreatedAt);
}