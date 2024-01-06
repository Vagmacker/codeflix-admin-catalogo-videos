namespace FC.Codeflix.Catalog.Api.Category.Models;

public sealed record CreateCategoryRequest(string Name, string Description, bool IsActive);