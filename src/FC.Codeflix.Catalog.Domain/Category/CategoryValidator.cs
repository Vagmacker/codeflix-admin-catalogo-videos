using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Category;

public class CategoryValidator(Category category, ValidationHandler handler) : Validator(handler)
{
    private const int MinLength = 3;
    private const int MaxLength = 255;

    public override void Validate()
    {
        CheckNameConstraints();
        CheckDescriptionConstraints();
    }

    private void CheckNameConstraints()
    {
        var name = category.Name;

        if (string.IsNullOrWhiteSpace(name))
        {
            handler.Append(new Error("Name should not be null or empty"));
            return;
        }

        var length = name.Trim().Length;
        if (length is < MinLength or > MaxLength)
        {
            handler.Append(new Error("Name should be between 3 and 255 characters"));
        }
    }

    private void CheckDescriptionConstraints()
    {
        var description = category.Description;
        if (description is null)
        {
            handler.Append(new Error("Description should not be null"));
            return;
        }

        if (description?.Length > 4_000)
        {
            handler.Append(new Error("Description should be between 1 and 4000 characters"));
        }
    }
}