using CategoryEntity = FC.Codeflix.Catalog.Domain.Category.Category;

namespace FC.Codeflix.Catalog.Application.Category.Retrieve.List;

public sealed record ListCategoriesOutput(Guid Id, string Name, string Description, bool IsActive, DateTime CreatedAt)
{
    public static ListCategoriesOutput From(CategoryEntity aCategory)
        => new(
            aCategory.Id,
            aCategory.Name,
            aCategory.Description,
            aCategory.IsActive,
            aCategory.CreatedAt
        );
}