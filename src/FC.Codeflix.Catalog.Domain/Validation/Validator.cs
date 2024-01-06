namespace FC.Codeflix.Catalog.Domain.Validation;

public abstract class Validator
{
    private readonly ValidationHandler _handler;

    protected Validator(ValidationHandler aHandler)
        => _handler = aHandler;

    public abstract void Validate();

    protected ValidationHandler ValidationHandler()
        => _handler;
}