namespace FC.Codeflix.Catalog.Api.Category.Models;

public sealed record UpdateCategoryRequest(string Name, string Description, bool IsActive);