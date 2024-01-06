using FluentValidation;

namespace FC.Codeflix.Catalog.Application.Category.Retrieve.Get;

public class GetCategoryCommandValidator : AbstractValidator<GetCategoryCommand>
{
    public GetCategoryCommandValidator()
        => RuleFor(it => it.Id).NotEmpty();
}